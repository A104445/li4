using System.ComponentModel.DataAnnotations.Schema;

namespace MontagemPro.Models
{
    public class ProductComponent
    {
        [ForeignKey("ProductsId")]
        public int ProductsId { get; set; }
        public Products Produto { get; set; }

        [ForeignKey("ComponentsId")]
        public int ComponentsId { get; set; }
        public Components Componente { get; set; }

        // Propriedade adicional para armazenar a quantidade
        public int Quantidade { get; set; }

        public ProductComponent()
        {
        }
    }
}
