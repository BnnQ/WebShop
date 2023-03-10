using Homework.Data;
using Homework.Data.Entities;
using Homework.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework.Controllers
{
    public class ShopController : Controller
    {
        private readonly ShopContext shopContext;

        public ShopController(ShopContext shopContext)
        {
            this.shopContext = shopContext;
        }

        public async Task<IActionResult> List(string? categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return NotFound();

            Category? category = (await shopContext.Categories.Where(category => category.Name.ToLower().Equals(categoryName.ToLower()))
                .Include(category => category.Products!)
                    .ThenInclude(product => product.Manufacturer!)
                    .ThenInclude(manufacturer => manufacturer.Products!)
                    .ThenInclude(product => product.Images)
                .ToListAsync()).FirstOrDefault();

            if (category is null)
                return NotFound();

            return View(category);
        }

        public async Task<IActionResult> Home()
        {
            if (User.Identity?.IsAuthenticated is true && User.IsInRole("Admin"))
            {
                return RedirectToAction(controllerName: "Admin", actionName: "Home");
            }

            return View(await shopContext.Banners.ToListAsync());
        }

        public async Task<IActionResult> Details(int productId)
        {
            Product? product = await shopContext.Products.Where(product => product.Id == productId)
                                                         .Include(product => product.Manufacturer)
                                                         .Include(product => product.Images)
                                                         .Include(product => product.Category)
                                                         .ThenInclude(product => product.ParentCategory)
                                                         .SingleOrDefaultAsync();

            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            IEnumerable<Product> products = new List<Product>();
            if (!string.IsNullOrWhiteSpace(query))
            {
                if (int.TryParse(query, out int id))
                {
                    products = await shopContext.Products.Where(product => product.Id == id)
                                                         .Include(product => product.Images)
                                                         .Include(product => product.Manufacturer)
                                                         .Include(product => product.Category)
                                                         .ToListAsync();
                }
                else
                {
                    string lowercaseQuery = query.ToLower();
                    products = await shopContext.Products.Where(product =>
                                                                               product.Title.ToLower().Contains(lowercaseQuery) ||
                                                                               product.Manufacturer.Name.ToLower().Contains(lowercaseQuery) ||
                                                                               product.Category.Name.ToLower().Contains(lowercaseQuery) ||
                                                                               product.Category.UnitName.ToLower().Contains(lowercaseQuery))
                                                                              .Include(product => product.Images)
                                                                              .Include(product => product.Manufacturer)
                                                                              .Include(product => product.Category)
                                                                              .ToListAsync();
                }

            }
            
            return View(new SearchViewModel() { Products = products, Query = query });
        }
    }
}