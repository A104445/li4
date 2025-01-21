using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using li4.Models;

namespace li4.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<li4.Models.Orders> Orders { get; set; } = default!;
        public DbSet<li4.Models.Components> Components { get; set; } = default!;
        public DbSet<li4.Models.Products> Products { get; set; } = default!;
        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<ProductComponent> ProductComponents { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurando a chave primária composta
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.ProductsId, op.OrdersId });

            // Configurando os relacionamentos
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Produto)
                .WithMany(p => p.EncomendaProduto)
                .HasForeignKey(op => op.ProductsId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Orders)
                .WithMany(o => o.EncomendaProduto)
                .HasForeignKey(op => op.OrdersId);

            // Configurando a chave primária composta
            modelBuilder.Entity<ProductComponent>()
                .HasKey(pc => new { pc.ProductsId, pc.ComponentsId });

            // Configurando os relacionamentos
            modelBuilder.Entity<ProductComponent>()
                .HasOne(pc => pc.Produto)
                .WithMany(p => p.ProdutoComponentes)
                .HasForeignKey(pc => pc.ProductsId);

            modelBuilder.Entity<ProductComponent>()
                .HasOne(pc => pc.Componente)
                .WithMany(c => c.ProdutoComponentes)
                .HasForeignKey(pc => pc.ComponentsId);
        }
    }
}
