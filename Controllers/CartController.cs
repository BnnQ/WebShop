using System.Security.Claims;
using Homework.Data;
using Homework.Data.Entities;
using Homework.Models.Domain;
using Homework.Utils.Enums;
using Homework.Utils.Extensions;
using Homework.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Homework.Filters;
using Homework.Services.Abstractions;
using Homework.ViewModels;
using Microsoft.AspNetCore.Identity;
using Razor.Templating.Core;

namespace Homework.Controllers;

[RetrieveModelErrorsFromRedirector]
public class CartController : Controller
{
    private readonly ShopContext shopContext;
    private readonly UserManager<User> userManager;

    public CartController(ShopContext shopContext, UserManager<User> userManager)
    {
        this.shopContext = shopContext;
        this.userManager = userManager;
    }

    // GET
    public IActionResult List(Cart cart, string? returnUrl)
    {
        return View(new ListViewModel
            { Cart = cart, ReturnUrl = !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : "/" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(Cart cart, int productId, string? returnUrl)
    {
        if (shopContext.Products?.Any() is not true)
        {
            return NotFound();
        }

        var product = await shopContext.Products.Where(item => item.Id == productId)
            .Include(product => product.Images)
            .Include(product => product.Manufacturer)
            .Include(product => product.Category)
            .FirstOrDefaultAsync();
        if (product is null)
        {
            return NotFound();
        }
        
        cart.AddItem(product);
        HttpContext.Session.SaveCart(cart);
        return RedirectToList(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromCart(Cart cart, int productId, string? returnUrl)
    {
        if (shopContext.Products?.Any() is not true)
        {
            return NotFound();
        }

        var product = await shopContext.Products.FindAsync(productId);
        if (product is null)
        {
            return NotFound();
        }
        
        cart.RemoveItem(product);
        HttpContext.Session.SaveCart(cart);
        return RedirectToList(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(Cart cart, int productId, QuantityActions action)
    {
        if (shopContext.Products?.Any() is not true)
        {
            return NotFound();
        }

        var product = await shopContext.Products.FindAsync(productId);
        if (product is null)
        {
            return NotFound();
        }
        
        var cartItem = cart.GetItemOrDefault(product);
        if (cartItem is null)
        {
            return Json(new { success = false });
        }

        switch (action)
        {
            case QuantityActions.Add:
                cart.AddItemCount(cartItem, 1);
                break;
            case QuantityActions.Subtract:
                cart.SubtractItemCount(cartItem, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(action),
                    actualValue: action,
                    message: "QuantityAction does not exist");
        }

        HttpContext.Session.SaveCart(cart);
        return Json(new { success = true });
    }

    [Authorize(policy: "Authenticated")]
    [HttpPost]
    public async Task<IActionResult> ConfirmPurchase(Cart cart, string returnUrl, [FromServices] IEmailSender emailSender)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        if (!cart.Items.Any())
            return NotFound();

        var totalPrice =
            cart.Items.Aggregate(seed: 0.0, (accumulator, item) => accumulator + item.Product.Price * item.Count);

        if (user.Balance < totalPrice)
        {
            return BadRequest(error: "Your balance is not enough money to make a purchase.");
        }

        if (shopContext.Products?.Any() is not true)
        {
            return Problem();
        }

        var generatedConfirmationCode = Guid.NewGuid().ToString();
        HttpContext.Session.SetValue(key: "confirmationCode", value: generatedConfirmationCode);

        var htmlMessageViewModel = new OrderConfirmationViewModel(cart, generatedConfirmationCode, HttpContext.GetSiteBaseUrl());
        var htmlMessage = await RazorTemplateEngine.RenderAsync(viewName: "_OrderConfirmation", viewModel: htmlMessageViewModel);
        await emailSender.SendEmailAsync(name: user.UserName, emailAddress: user.Email, subject: "Order confirmation", htmlMessage);
        return RedirectToList(returnUrl);
    }

    [Authorize(policy: "Authenticated")]
    [HttpGet]
    public async Task<IActionResult> PurchaseConfirmed(Cart cart, string? code)
    {
        var session = HttpContext.Session;
        if (string.IsNullOrWhiteSpace(code))
        {
            session.SetValue(key: "confirmationCode", value: string.Empty);
            return RedirectToList(returnUrl: null);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        if (!cart.Items.Any())
        {
            return NotFound();
        }

        var confirmationCode = session.GetValueOrDefault<string>(key: "confirmationCode");
        if (!code.Equals(confirmationCode))
        {
            return BadRequest(error: "Confirmation code does not match, or your order has been canceled.");
        }
        session.SetValue(key: "confirmationCode", value: string.Empty);

        var totalPrice =
            cart.Items.Aggregate(seed: 0.0, (accumulator, item) => accumulator + item.Product.Price * item.Count);

        if (user.Balance < totalPrice)
        {
            return BadRequest(error: "Your balance is not enough money to make a purchase.");
        }

        if (shopContext.Products?.Any() is not true)
        {
            return Problem();
        }
        
        user.Balance -= totalPrice;
        foreach (var item in cart.Items)
        {
            var product = await shopContext.Products.FindAsync(item.Product.Id);
            if (product is null)
            {
                return Problem();
            }

            product.Count -= item.Count;
            shopContext.Products.Update(product);
        }

        await userManager.UpdateAsync(user);
        cart.Clear();
        HttpContext.Session.SaveCart(cart);

        return RedirectToList(returnUrl: null);
    }
    
    #region Utils
    private IActionResult RedirectToList(string? returnUrl)
    {
        return RedirectToAction(nameof(List), routeValues: new { returnUrl });
    }

    #endregion
}