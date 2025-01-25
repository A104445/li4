using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MontagemPro.Models;

namespace MontagemPro.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MontagemPro.Models.Orders> Orders { get; set; } = default!;
        public DbSet<MontagemPro.Models.Components> Components { get; set; } = default!;
        public DbSet<MontagemPro.Models.Products> Products { get; set; } = default!;
        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<ProductComponent> ProductComponents { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configurar os relacionamentos
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Produto)
                .WithMany(p => p.EncomendaProduto)
                .HasForeignKey(op => op.ProductsId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Orders)
                .WithMany(o => o.EncomendaProduto)
                .HasForeignKey(op => op.OrdersId);

            // Configurar os relacionamentos
            modelBuilder.Entity<ProductComponent>()
                .HasOne(pc => pc.Produto)
                .WithMany(p => p.ProdutoComponentes)
                .HasForeignKey(pc => pc.ProductsId);

            modelBuilder.Entity<ProductComponent>()
                .HasOne(pc => pc.Componente)
                .WithMany(c => c.ProdutoComponentes)
                .HasForeignKey(pc => pc.ComponentsId);
        }
        public DbSet<MontagemPro.Models.ProductionProcess> ProductionProcess { get; set; } = default!;
    }
}
