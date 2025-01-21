using System.ComponentModel.DataAnnotations.Schema;

namespace li4.Models
{
    public class OrderProduct
    {

        [ForeignKey("ProductsId")]
        public int ProductsId { get; set; }
        public Products Produto { get; set; }

        [ForeignKey("OrdersId")]
        public int OrdersId { get; set; }
        public Orders Orders { get; set; }

        // Propriedade adicional para armazenar a quantidade
        public int Quantidade { get; set; }

        public OrderProduct()
        {
            
        }
    }
}
