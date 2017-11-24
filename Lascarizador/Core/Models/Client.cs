using System.ComponentModel.DataAnnotations;

namespace Lascarizador.Core.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(11)]
        [RegularExpression(@"[0-9]{11}", ErrorMessage = "O CPF deve ser composto de 11 números.")]
        public string CPF { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Limite de Crédito")]
        public decimal CreditLimit { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        public decimal Saldo { get; set; }
    }
}