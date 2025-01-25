using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

using MontagemPro.Data;

namespace MontagemPro.Models
{
    public class ProductionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProductionBackgroundService> _logger;

        public ProductionBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<ProductionBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Loop infinito, até encerrar
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var ordersParaProduzir = await db.Orders
                            .Where(o => o.Data <= DateTime.Now && !o.Completada)
                            .OrderBy(o => o.Prioridade)
                            .ThenBy(o => o.Data)
                            .ToListAsync(stoppingToken);

                        var componentes = db.Components.ToList();

                        foreach (var order in ordersParaProduzir)
                        {
                            foreach (var comp in componentes)
                            {
                                if(comp.Quantidade == 0)
                                {
                                    throw new InvalidOperationException("Componente em falta");
                                }
                            }

                            var process = await db.ProductionProcess
                                .FirstOrDefaultAsync(p => p.OrdersId == order.OrdersId, stoppingToken);

                            if (process != null && !process.IsCompleted)
                            {

                                process.CurrentStage++;
                                if (process.CurrentStage == 1)
                                {
                                    if (process.Order.Quantidade > 0)
                                    {

                                        foreach (var comp in componentes)
                                        {
                                            comp.Quantidade--;
                                            if (comp.Quantidade == 0)
                                            {
                                                throw new InvalidOperationException("Componente em falta");
                                            }
                                        }

                                    }
                                    if (process.Order.Quantidade == 0)
                                    {
                                        process.CurrentStage = process.TotalStages;
                                        process.IsCompleted = true;
                                        process.Order.Completada = true;
                                        continue;
                                    }
                                }
                                if (process.CurrentStage > process.TotalStages & process.Order.Quantidade > 0)
                                {
                                    process.Order.Quantidade--;
                                    process.CurrentStage = 0;
                                }

                                db.Update(process);
                                db.Update(order);
                            }
                        }

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no serviço de produção");
                }

                // Espera 1 minuto (por exemplo) antes de checar novamente
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
