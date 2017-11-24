using System.ComponentModel.DataAnnotations;

namespace Lascarizador.Core.Models
{
    public class TransactionType
    {
        public byte Id { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public string Type { get; set; }

        [Required]
        [Display(Name ="Parcelas permitidas?")]
        public bool InstallmentsAvailable { get; set; }

        [Required]
        [Display(Name = "Nome do Tipo para API")]
        public string ApiName { get; set; }
    }

}