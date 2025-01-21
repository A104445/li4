using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MontagemPro.Data;
using MontagemPro.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MontagemPro.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;


        public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Orders
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Obtém o ID do usuário logado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Filtra as Orders pelo UserId
            var applicationDbContext = _context.Orders
                .Include(o => o.User) // Inclui os dados do usuário relacionado
                .Where(o => o.AccountsId == userId); // Filtra pelo usuário logado

            // Retorna apenas as Orders associadas ao usuário logado
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrdersId == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrdersId,Data,Total,Prioridade")] Orders orders)
        {
            // get user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            orders.AccountsId = userId;

            // Limpar os erros de validação para AccountsId e validar novamente
            ModelState.Remove(nameof(orders.AccountsId));


            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogInformation($"Validation error: {error.ErrorMessage}");
                }
            }

            return View(orders);
        }

        // GET: Orders/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            ViewData["AccountsId"] = new SelectList(_context.Users, "Id", "Id", orders.AccountsId);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrdersId,Data,Total,Prioridade")] Orders orders)
        {
            if (id != orders.OrdersId)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(orders.AccountsId));

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing order
                    var existingOrder = await _context.Orders.FirstOrDefaultAsync(o => o.OrdersId == id);
                    if (existingOrder == null)
                    {
                        _logger.LogError($"Order not found in the database: {id}");
                        return NotFound();
                    }

                    _logger.LogInformation($"Received Order: ID={orders.OrdersId}, Data={orders.Data}, Total={orders.Total}, Prioridade={orders.Prioridade}");


                    // Update fields explicitly
                    existingOrder.Data = orders.Data;
                    existingOrder.Total = orders.Total;
                    existingOrder.Prioridade = orders.Prioridade;

                    // Save changes
                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Changes successfully saved to the database.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error saving changes: {ex.Message}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving changes: {ex.Message}");
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError($"Validation error: {error.ErrorMessage}");
                }
            }

            return View(orders);
        }

        // GET: Orders/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrdersId == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrdersId == id);
        }
    }
}
