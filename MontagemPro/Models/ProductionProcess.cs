using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MontagemPro.Models
{
    public class ProductionProcess
    {
        public int Id { get; set; }
        public int OrdersId { get; set; }

        [ForeignKey("OrdersId")]
        public Orders Order { get; set; }


        public string ProductName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O valor deve ser maior que 0.")]
        public int TotalStages { get; set; }
        public int CurrentStage { get; set; }
        public bool IsCompleted { get; set; }

        public ProductionProcess()
        {
            
        }
    }
}
