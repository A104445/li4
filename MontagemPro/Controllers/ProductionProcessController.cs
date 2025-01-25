using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MontagemPro.Data;
using MontagemPro.Models;
using static Azure.Core.HttpHeader;

namespace MontagemPro.Controllers
{
    public class ProductionProcessController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductionProcessController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostra a página com as 9 fases
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == "1194edf7-7dff-4d44-92e7-c762cfbd2908")// admin
            {
                var ordersAtrasadasAdmin = await _context.Orders
                .Where(o => o.Data <= DateTime.Now) // a data já chegou ou passou
                .ToListAsync();
                foreach (var order in ordersAtrasadasAdmin)
                {
                    bool jaExiste = await _context.ProductionProcess
                        .AnyAsync(pp => pp.OrdersId == order.OrdersId);

                    if (!jaExiste)
                    {
                        var produto = order.EncomendaProduto.FirstOrDefault().Produto;
                        var novoProcesso = new ProductionProcess
                        {
                            OrdersId = order.OrdersId,
                            Order = order,
                            ProductName = "Carro",
                            TotalStages = 9,
                            CurrentStage = 0,
                            IsCompleted = false
                        };
                        _context.ProductionProcess.Add(novoProcesso);
                    }
                }

                await _context.SaveChangesAsync();

                // 3. Agora busca a lista de processos de produção ainda não concluídos
                var processosEmAbertoAdmin = await _context.ProductionProcess
                    .Include(pp => pp.Order)
                    .Where(pp => !pp.IsCompleted)
                    .OrderBy(pp => pp.Order.Prioridade)
                    .ThenBy(pp => pp.Order.Data)
                    .ToListAsync();

                // 4. Retorna a lista para a View
                return View(processosEmAbertoAdmin);
            }

            //nao é admin


            // 1. Verifica quais orders devem entrar em produção
            var ordersAtrasadas = await _context.Orders
                .Where(o => o.Data <= DateTime.Now) // a data já chegou ou passou
                .ToListAsync();

            // 2. Para cada Order que ainda não está em ProductionProcess, cria um registro
            foreach (var order in ordersAtrasadas)
            {
                bool jaExiste = await _context.ProductionProcess
                    .AnyAsync(pp => pp.OrdersId == order.OrdersId);

                if (!jaExiste)
                {
                    ;
                    var novoProcesso = new ProductionProcess
                    {
                        OrdersId = order.OrdersId,
                        Order = order,
                        ProductName = "Carro", // Ajuste conforme sua lógica
                        TotalStages = 9,
                        CurrentStage = 0,
                        IsCompleted = false
                    };
                    _context.ProductionProcess.Add(novoProcesso);
                }
            }

            // Salva as inclusões
            await _context.SaveChangesAsync();

            // 3. Agora busca a lista de processos de produção ainda não concluídos daquele user
            var processosEmAberto = await _context.ProductionProcess
                .Include(pp => pp.Order)
                .Where(pp => !pp.IsCompleted)
                .Where(pp => pp.Order.AccountsId == userId)
                .OrderByDescending(pp => pp.Order.Prioridade)
                .ThenBy(pp => pp.Order.Data)
                .ToListAsync();

            // 4. Retorna a lista para a View
            return View(processosEmAberto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var process = await _context.ProductionProcess
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (process == null)
            {
                return NotFound();
            }

            return View(process);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AdvanceStage(int id)
        {
            var componentes = _context.Components.ToList();

            var nomes = new List<string>();

            foreach (var comp in componentes)
            {
                nomes.Add(comp.Nome);

            }

            var process = await _context.ProductionProcess
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id);

            if (process == null) return NotFound();

            process.CurrentStage++;
            if (process.CurrentStage == 1)
            {
                if (process.Order.Quantidade > 0)
                {
                    
                    foreach (var comp in componentes)
                    {
                        comp.Quantidade--;
                        if (comp.Quantidade == -1)
                        {
                            return Json(new
                            {
                                success = false,
                                message = $"Componente {comp.Nome} em falta"
                            });
                        }
                    }

                }
                if (process.Order.Quantidade == 0)
                {
                    process.CurrentStage = process.TotalStages;
                    process.IsCompleted = true;
                    process.Order.Completada = true;
                }
            }
            if (process.CurrentStage > process.TotalStages & process.Order.Quantidade > 0)
            {
                process.Order.Quantidade--;
                process.CurrentStage = 0;
            }


            _context.Update(process);
            await _context.SaveChangesAsync();

            return Json(new
            {
                Names = nomes,
                currentStage = process.CurrentStage,
                isCompleted = process.IsCompleted,
                orderQuantity = process.Order.Quantidade
            });
        }


    }
}
