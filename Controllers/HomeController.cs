using Homework.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework.Controllers;

public class HomeController : Controller
{
    private readonly ShopContext shopContext;
    
    // GET
    public HomeController(ShopContext shopContext)
    {
        this.shopContext = shopContext;
    }

    public async Task<IActionResult> Home()
    {
        if (User.Identity?.IsAuthenticated is true && User.IsInRole("Admin"))
        {
            return RedirectToAction(controllerName: "Admin", actionName: "Home");
        }

        return View(await shopContext.Banners.ToListAsync());
    }
}