using Homework.Data;
using Homework.Data.Entities;
using Homework.Filters;
using Homework.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Homework.Controllers;

[RetrieveModelErrorsFromRedirector]
public class ShopController : Controller
{
    private readonly ShopContext shopContext;

    public ShopController(ShopContext shopContext)
    {
        this.shopContext = shopContext;
    }

    public async Task<IActionResult> List(string? categoryName, int page = 1, int pageSize = 8)
    {
        if (shopContext.Products?.Any() is not true)
            return NotFound();
        
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            var products = shopContext.Products.Include(product => product.Images).Include(product => product.Manufacturer).Include(product => product.Category);
            return View(new ListViewModel(category: null, await products.ToPagedListAsync(page, pageSize), await products.CountAsync()));
        }
        else
        {
            if (shopContext.Categories?.Any() is not true)
                return NotFound();

            var category = await shopContext.Categories.FirstOrDefaultAsync(item =>
                item.Name.ToLower().Equals(categoryName.ToLower()));

            if (category is null)
                return NotFound();

            var products = shopContext.Products.Where(product => product.CategoryId == category.Id)
                .Include(product => product.Images)
                .Include(product => product.Manufacturer);
            
            return View(new ListViewModel(category: null, await products.ToPagedListAsync(page, pageSize), await products.CountAsync()));
        }
    }

    public async Task<IActionResult> Details(int productId)
    {
        if (shopContext.Products?.Any() is not true)
        {
            return NotFound();
        }
            
        var product = await shopContext.Products.Where(product => product.Id == productId)
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
        if (shopContext.Products?.Any() is true)
        {
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
        }

        return View(new SearchViewModel() { Products = products, Query = query });
    }
}