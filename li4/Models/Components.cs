namespace li4.Models
{
    public class Components
    {
        public int ComponentsId { get; set; }

        public string Nome { get; set; }

        public int Quantidade { get; set; }

        // Relacionamento muitos-para-muitos
        public ICollection<ProductComponent>? ProdutoComponentes { get; set; }

        public Components()
        {
            
        }
    }
}
