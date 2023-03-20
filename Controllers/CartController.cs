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
using Microsoft.AspNetCore.Identity;

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
                throw new ArgumentOutOfRangeException(nameof(action), action, "QuantityAction does not exist");
        }

        HttpContext.Session.SaveCart(cart);
        return Json(new { success = true });
    }

    [Authorize(policy: "Authenticated")]
    [HttpPost]
    public async Task<IActionResult> ConfirmPurchase(Cart cart, string returnUrl)
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
            cart.Items.Aggregate(0.0, (accumulator, item) => accumulator + item.Product.Price * item.Count);

        if (user.Balance < totalPrice)
        {
            return BadRequest("Your balance is not enough money to make a purchase.");
        }

        if (shopContext.Products?.Any() is not true)
        {
            return Problem();
        }

        //*code for placing an order and creating a delivery request*

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

        return RedirectToList(returnUrl);
    }

    #region Utils
    private IActionResult RedirectToList(string? returnUrl)
    {
        return RedirectToAction(nameof(List), routeValues: new { returnUrl });
    }

    #endregion
}