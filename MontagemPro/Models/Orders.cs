using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MontagemPro.Models
{
    public class Orders
    {
        public int OrdersId { get; set; }

        public DateTime Data { get; set; }

        [Range(0, 5, ErrorMessage = "O valor deve ser entre 0 e 5.")]
        public int Prioridade { get; set; }

        public double PreçoTotal { get; set; }
        public bool Confirmada { get; set; }

        public string AccountsId { get; set; }

        public int Quantidade { get; set; }

        public bool Completada { get; set; }


        [ForeignKey("AccountsId")]
        public IdentityUser? User { get; set; } // '?' Torna opcional

        public ICollection<OrderProduct>? EncomendaProduto { get; set; }

        public Orders()
        {
        }


    }
}
