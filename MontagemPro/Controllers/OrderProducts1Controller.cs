using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MontagemPro.Data;
using MontagemPro.Migrations;
using MontagemPro.Models;

namespace MontagemPro.Controllers
{
    public class OrderProducts1Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderProducts1Controller> _logger;
        private static int? _lastOrderId;

        public OrderProducts1Controller(ApplicationDbContext context, ILogger<OrderProducts1Controller> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: OrderProducts1
        [Authorize]
        public async Task<IActionResult> Index(int? id)
        {
            // Atualiza _lastOrderId se um ID for passado, ou utiliza o último armazenado
            int ordersId = id ?? _lastOrderId ?? 0;

            if (ordersId == 0)
            {
                return BadRequest("Nenhum OrdersId válido foi fornecido.");
            }

            // Atualiza o último ID utilizado
            _lastOrderId = ordersId;

            var model = await _context.OrderProducts
                .Include(o => o.Produto)
                .Include(o => o.Orders)
                .Where(o => o.OrdersId == _lastOrderId)
                .ToListAsync();

            float totalPrice = model.Sum(item => item.Quantidade * item.Produto.Preco);
           

            // Passa o total para a View
            ViewBag.TotalPrice = totalPrice;

            // Consulta filtrando pelo OrdersId
            var applicationDbContext = _context.OrderProducts
                .Include(o => o.Orders)
                .Include(o => o.Produto)
                .Where(o => o.OrdersId == ordersId);

            return View(await applicationDbContext.ToListAsync());

        }

        // GET: OrderProducts1/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts
                .Include(o => o.Orders)
                .Include(o => o.Produto)
                .FirstOrDefaultAsync(m => m.OrderProductId == id);
            if (orderProduct == null)
            {
                return NotFound();
            }
            

            return View(orderProduct);
        }

        // GET: OrderProducts1/Create
        [Authorize]
        public IActionResult Create(int? ordersId)
        {
            // Se um ordersId for fornecido, atualize _lastOrderId
            if (ordersId.HasValue)
            {
                _lastOrderId = ordersId.Value;
                _logger.LogInformation($"Received new OrderId: ID={ordersId.Value}");
            }
            else if (_lastOrderId.HasValue)
            {
                _logger.LogInformation($"Using last stored OrderId: ID={_lastOrderId.Value}");
            }
            else
            {
                // Nenhum ordersId foi passado e nenhum valor armazenado está disponível
                _logger.LogError("No OrderId provided and no last OrderId stored.");
                return BadRequest("OrderId is required.");
            }

            ViewBag.OrdersId = _lastOrderId;
            ViewData["Products"] = new SelectList(_context.Products, "ProductsId", "Nome");
            return View();
        }

        // POST: OrderProducts1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderProductId,ProductsId,Quantidade,OrdersId")] OrderProduct orderProduct)
        {


            _logger.LogInformation($"Received OrderId: ID={orderProduct.OrdersId}");

            // Obtenha a lista de OrderProducts para o último OrdersId
            var model = await _context.OrderProducts
                .Include(o => o.Produto)
                .Include(o => o.Orders)
                .Where(o => o.OrdersId == _lastOrderId)
                .ToListAsync();

            // Verifique se já existe um ProductsId igual
            bool exists = model.Any(o => o.ProductsId == orderProduct.ProductsId);

            if (exists)
            {
                // Mensagem de erro
                ModelState.AddModelError("", "Este produto já foi adicionado a este pedido.");
                ModelState.Remove("");
                ViewData["Products"] = new SelectList(_context.Products, "ProductsId", "Nome", orderProduct.ProductsId);

                // Retorne a View diretamente
                return RedirectToAction(nameof(Create));
            }

            // Certifique-se de que o estado é válido antes de criar a entrada
            if (ModelState.IsValid)
            {
                _context.Add(orderProduct);
                await _context.SaveChangesAsync();

                // Atualize o último OrderId utilizado após o POST
                _lastOrderId = orderProduct.OrdersId;

                return RedirectToAction("Index", "OrderProducts1", new { ordersId = _lastOrderId });
            }

            // Caso contrário, retorne a View para correção
            ViewData["Products"] = new SelectList(_context.Products, "ProductsId", "Nome", orderProduct.ProductsId);
            return RedirectToAction(nameof(Create));
        }

        // GET: OrderProducts1/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }
            ViewData["OrdersId"] = new SelectList(_context.Orders, "OrdersId", "OrdersId", orderProduct.OrdersId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId", orderProduct.ProductsId);
            return View(orderProduct);
        }

        // POST: OrderProducts1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderProductId,ProductsId,OrdersId,Quantidade")] OrderProduct orderProduct)
        {
            if (id != orderProduct.OrderProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderProductExists(orderProduct.OrderProductId))
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
            ViewData["OrdersId"] = new SelectList(_context.Orders, "OrdersId", "OrdersId", orderProduct.OrdersId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId", orderProduct.ProductsId);
            return View(orderProduct);
        }

        // GET: OrderProducts1/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts
                .Include(o => o.Orders)
                .Include(o => o.Produto)
                .FirstOrDefaultAsync(m => m.OrderProductId == id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return View(orderProduct);
        }

        // POST: OrderProducts1/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderProduct = await _context.OrderProducts.FindAsync(id);
            if (orderProduct != null)
            {
                _context.OrderProducts.Remove(orderProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // Confirma a Order e conclui a encomenda
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            // Procura a ordem pelo ID
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Altera a variável Confirmada para true
            order.Confirmada = true;

            var model = await _context.OrderProducts
                .Include(o => o.Produto)
                .Include(o => o.Orders)
                .Where(o => o.OrdersId == _lastOrderId)
                .ToListAsync();

            float totalPrice = model.Sum(item => item.Quantidade * item.Produto.Preco);

            order.PreçoTotal = totalPrice;

            int quantidade = model.Sum(item => item.Quantidade);

            order.Quantidade = quantidade;

            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();

            // Redireciona de volta para a página Index
            return RedirectToAction("Index","Orders");
        }

        private bool OrderProductExists(int id)
        {
            return _context.OrderProducts.Any(e => e.OrderProductId == id);
        }
    }
}
