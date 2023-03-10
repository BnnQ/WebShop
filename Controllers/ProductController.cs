using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Homework.Data;
using Homework.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Homework.Models;
using AutoMapper;
using Homework.Services.Abstractions;
using Homework.Services;
using Homework.Models.Product;

namespace Homework.Controllers
{
    [Authorize(policy: "AdminOnly")]
    public class ProductController : Controller
    {
        private readonly ShopContext context;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment environment;

        public ProductController(ShopContext context, IMapper mapper, IWebHostEnvironment environment)
        {
            this.context = context;
            this.mapper = mapper;
            this.environment = environment;
        }

        // GET: Product
        public async Task<IActionResult> List()
        {
            var shopContext = context.Products.Include(p => p.Category).Include(p => p.Manufacturer);
            return View(await shopContext.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (context.Products?.Any() is false)
            {
                return NotFound();
            }

            var product = await context.Products!
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(context.Categories, "Id", "Name");
            ViewData["ManufacturerId"] = new SelectList(context.Manufacturers, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreationDto productCreationDto, IFormFileCollection? images, [FromServices] IFormImageProcessor imageProcessor)
        {
            Product product = mapper.Map<Product>(productCreationDto);
            if (ModelState.IsValid)
            {
                TrySaveProductImages(product, images, imageProcessor);

                context.Add(product);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            ViewData["CategoryId"] = new SelectList(context.Categories, "Id", "Name", product.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(context.Manufacturers, "Id", "Name", product.ManufacturerId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (context.Products?.Any() is false)
            {
                return NotFound();
            }

            var product = await context.Products!.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(context.Categories, "Id", "Name", product.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(context.Manufacturers, "Id", "Name", product.ManufacturerId);
            return View(mapper.Map<Product, ProductEditingDto>(product));
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditingDto productEditingDto, IFormFileCollection? images, [FromServices] BackSlashFilePathNormalizer pathNormalizer, [FromServices] IFormImageProcessor imageProcessor)
        {
            if (ModelState.IsValid)
            {
                Product? product = await context.Products.Where(product => product.Id == productEditingDto.Id)
                                                         .Include(product => product.AssociatedBanners)
                                                         .Include(product => product.Images)
                                                         .FirstOrDefaultAsync();
                if (product is null)
                {
                    return NotFound();
                }
                productEditingDto.AssociatedBanners = product.AssociatedBanners;
                productEditingDto.Images = product.Images;
                mapper.Map(productEditingDto, product);

                if (images?.Any() is true)
                {
                    TryDeleteProductImages(product, pathNormalizer);
                    TrySaveProductImages(product, images, imageProcessor);
                }

                try
                {
                    context.Update(product);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(productEditingDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToList();
            }
            ViewData["CategoryId"] = new SelectList(context.Categories, "Id", "Name", productEditingDto.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(context.Manufacturers, "Id", "Name", productEditingDto.ManufacturerId);
            return View(productEditingDto);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Products == null)
            {
                return NotFound();
            }

            var product = await context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, [FromServices] BackSlashFilePathNormalizer pathNormalizer)
        {
            if (context.Products is null)
            {
                return Problem("Entity set 'ShopContext.Products' is null.");
            }
            var product = await context.Products.Where(product => product.Id == id)
                                                .Include(product => product.Images)
                                                .FirstOrDefaultAsync();
            if (product is not null)
            {
                TryDeleteProductImages(product, pathNormalizer);
                context.Products.Remove(product);
            }

            await context.SaveChangesAsync();
            return RedirectToList();
        }

        #region Utils
        private bool ProductExists(int id)
        {
            return context.Products.Any(e => e.Id == id);
        }

        private IActionResult RedirectToList()
        {
            return RedirectToAction(nameof(List));
        }

        private void TrySaveProductImages(Product product, IFormFileCollection? images, IFormImageProcessor imageProcessor)
        {
            product.Images = new List<ProductImage>();
            if (images?.Any() is true)
            {
                foreach (IFormFile image in images)
                {
                    product.Images.Add(new ProductImage()
                    {
                        FilePath = imageProcessor.Process(image)
                    });
                }
            }
            else
            {
                product.Images.Add(new ProductImage());
            }
        }

        private void TryDeleteProductImages(Product product, BackSlashFilePathNormalizer pathNormalizer)
        {
            if (product.Images?.Any() is true)
            {
                for (int i = product.Images.Count - 1; i >= 0; i--)
                {
                    DeleteProductImage(product.Images[i].FilePath, pathNormalizer);
                    product.Images.RemoveAt(i);
                }
            }
        }

        private void DeleteProductImage(string relativePath, BackSlashFilePathNormalizer pathNormalizer)
        {
            System.IO.File.Delete(Path.Combine(environment.WebRootPath, pathNormalizer.Normalize(relativePath)));
        }
        #endregion
    }
}
