using NuGet.Protocol.Core.Types;

namespace MontagemPro.Models
{
    public class Products
    {
        public int ProductsId { get; set; }

        public float Preco { get; set; }

        public string Nome { get; set; }

        public ICollection<ProductComponent>? ProdutoComponentes { get; set; }

        public ICollection<OrderProduct>? EncomendaProduto { get; set; }

        public Products()
        {
        }
    }
}
