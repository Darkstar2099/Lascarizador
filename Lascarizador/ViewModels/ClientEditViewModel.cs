using Lascarizador.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Lascarizador.ViewModels
{
    public class ClientEditViewModel
    {
        public Client Client { get; set; }

        [Required]
        [Display(Name = "Limite de Crédito")]
        public string CreditLimit { get; set; }
    }
}