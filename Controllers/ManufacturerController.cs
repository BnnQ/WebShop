using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Homework.Data;
using Homework.Data.Entities;
using Homework.Models.Manufacturer;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Homework.Controllers
{
    [Authorize(policy: "ProductManagement")]
    public class ManufacturerController : Controller
    {
        private readonly ShopContext context;
        private readonly IMapper mapper;

        public ManufacturerController(ShopContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        // GET: Manufacturer
        public async Task<IActionResult> List()
        {
              return View(context.Manufacturers?.Any() is true ? await context.Manufacturers.ToListAsync() : new List<Manufacturer>());
        }

        // GET: Manufacturer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.Manufacturers == null)
            {
                return NotFound();
            }

            var productManufacturer = await context.Manufacturers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productManufacturer == null)
            {
                return NotFound();
            }

            return View(productManufacturer);
        }

        // GET: Manufacturer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Manufacturer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManufacturerCreationDto manufacturerCreationDto)
        {
            if (ModelState.IsValid)
            {
                Manufacturer manufacturer = mapper.Map<ManufacturerCreationDto, Manufacturer>(manufacturerCreationDto);
                context.Add(manufacturer);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(manufacturerCreationDto);
        }

        // GET: Manufacturer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Manufacturers == null)
            {
                return NotFound();
            }

            var productManufacturer = await context.Manufacturers.FindAsync(id);
            if (productManufacturer == null)
            {
                return NotFound();
            }
            return View(productManufacturer);
        }

        // POST: Manufacturer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Manufacturer manufacturer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(manufacturer);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManufacturerExists(manufacturer.Id))
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
            return View(manufacturer);
        }

        // GET: Manufacturer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Manufacturers == null)
            {
                return NotFound();
            }

            var manufacturer = await context.Manufacturers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            return View(manufacturer);
        }

        // POST: Manufacturer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Manufacturers == null)
            {
                return Problem("Entity set 'ShopContext.Manufacturers'  is null.");
            }
            var manufacturer = await context.Manufacturers.FindAsync(id);
            if (manufacturer != null)
            {
                context.Manufacturers.Remove(manufacturer);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private bool ManufacturerExists(int id)
        {
          return context.Manufacturers?.Any(e => e.Id == id) is true;
        }
    }
}
