using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MontagemPro.Models
{
    public class OrderProduct
    {
        public int OrderProductId { get; set; }

        [ForeignKey("ProductsId")]
        public int ProductsId { get; set; }
        public Products? Produto { get; set; }

        [ForeignKey("OrdersId")]
        public int OrdersId { get; set; }
        public Orders? Orders { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O valor deve ser maior que 0.")]
        public int Quantidade { get; set; }

        public OrderProduct()
        {
            
        }
    }
}
