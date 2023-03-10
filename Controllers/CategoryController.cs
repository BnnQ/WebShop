using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Homework.Data;
using Homework.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Homework.Models.Category;
using AutoMapper;

namespace Homework.Controllers
{
    [Authorize(policy: "AdminOnly")]
    public class CategoryController : Controller
    {
        private readonly ShopContext context;
        private readonly IMapper mapper;

        public CategoryController(ShopContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        // GET: Category
        public async Task<IActionResult> List()
        {
            var shopContext = context.Categories.Include(c => c.ParentCategory);
            return View(await shopContext.ToListAsync());
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.Categories == null)
            {
                return NotFound();
            }

            var category = await context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewData["ParentCategoryId"] = new SelectList(context.Categories, "Id", "Name");
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreationDto categoryCreationDto)
        {
            if (ModelState.IsValid)
            {
                Category category = mapper.Map<CategoryCreationDto, Category>(categoryCreationDto);
                context.Add(category);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            ViewData["ParentCategoryId"] = new SelectList(context.Categories, "Id", "Name", categoryCreationDto.ParentCategoryId);
            return View(categoryCreationDto);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Categories == null)
            {
                return NotFound();
            }

            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = new SelectList(context.Categories.Where(category => category.Id != id), "Id", "Name", category.ParentCategoryId);
            return View(mapper.Map<Category, CategoryEditingDto>(category));
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryEditingDto categoryEditingDto)
        {
            if (ModelState.IsValid)
            {
                Category? category = await context.Categories.Where(category => category.Id == categoryEditingDto.Id)
                                                             .Include(category => category.Products)
                                                             .SingleOrDefaultAsync();
                if (category is null)
                {
                    return NotFound();
                }

                categoryEditingDto.Products = category.Products;
                mapper.Map(categoryEditingDto, category);
                try
                {
                    context.Update(category);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            ViewData["ParentCategoryId"] = new SelectList(context.Categories, "Id", "Name", categoryEditingDto.ParentCategoryId);
            return View(categoryEditingDto);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Categories == null)
            {
                return NotFound();
            }

            var category = await context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Categories == null)
            {
                return Problem("Entity set 'ShopContext.Categories'  is null.");
            }
            var category = await context.Categories.FindAsync(id);
            if (category != null)
            {
                context.Categories.Remove(category);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private bool CategoryExists(int id)
        {
          return context.Categories.Any(e => e.Id == id);
        }
    }
}
