using System.ComponentModel.DataAnnotations;

namespace MontagemPro.Models
{
    public class Components
    {
        public int ComponentsId { get; set; }

        public string Nome { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O valor deve ser maior que 0.")]
        public int Quantidade { get; set; }

        // Relacionamento muitos-para-muitos
        public ICollection<ProductComponent>? ProdutoComponentes { get; set; }

        public Components()
        {
            
        }
    }
}
