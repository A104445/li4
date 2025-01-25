using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MontagemPro.Models
{
    public class ProductComponent
    {
        [Key]
        public int ProductComponentId { get; set; }

        [ForeignKey("ProductsId")]
        public int ProductsId { get; set; }
        public Products? Produto { get; set; }

        [ForeignKey("ComponentsId")]
        public int ComponentsId { get; set; }
        public Components? Componente { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O valor deve ser maior que 0.")]
        public int Quantidade { get; set; }

        public ProductComponent()
        {
        }

    }
}
