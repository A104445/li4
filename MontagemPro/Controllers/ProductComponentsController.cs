using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MontagemPro.Data;
using MontagemPro.Models;

namespace MontagemPro.Controllers
{
    public class ProductComponentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductComponentsController> _logger;

        public ProductComponentsController(ApplicationDbContext context, ILogger<ProductComponentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ProductComponents
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductComponents.Include(p => p.Componente).Include(p => p.Produto);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductComponents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComponent = await _context.ProductComponents
                .Include(p => p.Componente)
                .Include(p => p.Produto)
                .FirstOrDefaultAsync(m => m.ProductComponentId == id);
            if (productComponent == null)
            {
                return NotFound();
            }

            return View(productComponent);
        }

        // GET: ProductComponents/Create
        public IActionResult Create()
        {
            ViewData["ComponentsId"] = new SelectList(_context.Components, "ComponentsId", "Nome");
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "Nome");
            return View();
        }

        // POST: ProductComponents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductComponentId,ProductsId,ComponentsId,Quantidade")] ProductComponent productComponent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productComponent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ComponentsId"] = new SelectList(_context.Components, "ComponentsId", "Nome", productComponent.ComponentsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "Nome", productComponent.ProductsId);
            return View(productComponent);
        }

        // GET: ProductComponents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComponent = await _context.ProductComponents.FindAsync(id);
            if (productComponent == null)
            {
                return NotFound();
            }
            ViewData["ComponentsId"] = new SelectList(_context.Components, "ComponentsId", "Nome", productComponent.ComponentsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "Nome", productComponent.ProductsId);
            return View(productComponent);
        }

        // POST: ProductComponents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductComponentId,ProductsId,ComponentsId,Quantidade")] ProductComponent productComponent)
        {
            if (id != productComponent.ProductComponentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productComponent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductComponentExists(productComponent.ProductComponentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ComponentsId"] = new SelectList(_context.Components, "ComponentsId", "ComponentsId", productComponent.ComponentsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId", productComponent.ProductsId);
            return View(productComponent);
        }

        // GET: ProductComponents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogInformation($"Validation error: (id == null)");
                return NotFound();
            }

            var productComponent = await _context.ProductComponents
                .Include(p => p.Componente)
                .Include(p => p.Produto)
                .FirstOrDefaultAsync(m => m.ProductComponentId == id);
            if (productComponent == null)
            {
                _logger.LogInformation($"Validation error: (productComponent == null)");
                return NotFound();
            }

            return View(productComponent);
        }

        // POST: ProductComponents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation($"DeleteConfirmed called with id: {id}");

            var productComponent = await _context.ProductComponents.FindAsync(id);
            if (productComponent != null)
            {
                _logger.LogInformation($"Deleting ProductComponent with Id: {id}");
                _context.ProductComponents.Remove(productComponent);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation($"ProductComponent with Id: {id} not found.");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductComponentExists(int id)
        {
            return _context.ProductComponents.Any(e => e.ProductComponentId == id);
        }
    }
}
