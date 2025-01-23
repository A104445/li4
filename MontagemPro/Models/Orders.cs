using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MontagemPro.Models
{
    public class Orders
    {
        public int OrdersId { get; set; }

        public DateTime Data { get; set; }

        public int Prioridade { get; set; }

        public double PreçoTotal { get; set; }

        public string AccountsId { get; set; }

        [ForeignKey("AccountsId")]
        public IdentityUser? User { get; set; } // '?' Torna opcional

        public ICollection<OrderProduct>? EncomendaProduto { get; set; }

        public Orders()
        {
        }


    }
}
