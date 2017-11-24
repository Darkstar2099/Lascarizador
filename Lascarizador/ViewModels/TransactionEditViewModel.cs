using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lascarizador.Core.Models;

namespace Lascarizador.ViewModels
{
    public class TransactionEditViewModel
    {
        public int Id { get; set; }

        public IEnumerable<TransactionType> TransactionTypes { get; set; }

        public IEnumerable<CardBrand> CardBrands { get; set; }

        [Required]
        [Display(Name = "Tipo de Transação")]
        //public byte TransactionTypeId { get; set; }
        public string TransactionTypeApiName { get; set; }

        [Required]
        [Display(Name = "Bandeira")]
        //public byte CardBrandId { get; set; }
        public string CardBrandApiName { get; set; }

        [Required]
        [MinLength(12,ErrorMessage = "O campo {0} deve ter no mínimo 12 números.")]
        [MaxLength(16, ErrorMessage = "O campo {0} deve ter no máximo 16 números.")]
        [DataType(DataType.CreditCard)]
        [Display(Name = "Número do Cartão")]
        public string Number { get; set; }

        [Required]
        [MaxLength(255)]
        [RegularExpression(@"^[A-ZÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+$",ErrorMessage ="O campo {0} deve conter somente caracteres maiúsculos e válidos")]
        [Display(Name = "Nome do Portador do Cartão")]
        public string CardHolderName { get; set; }

        [Required]
        [Display(Name = "Mês Expiração")]
        [Range(1,12)]
        public byte ExpirationMonth { get; set; }

        [Required]
        [Display(Name = "Ano Expiração")]
        [Range(2017, 2100)]
        public int ExpirationYear { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(3)]
        [RegularExpression(@"[0-9]{3}", ErrorMessage = "O campo {0} deve ter 3 números.")]
        [Display(Name = "cvv")]
        public string Cvv { get; set; }

        [Required]
        [Display(Name = "Valor")]
        public string Amount { get; set; }

        [Display(Name = "Número de Parcelas")]
        public int Installments { get; set; }

        [MinLength(4, ErrorMessage = "O campo {0} deve ter no mínimo 4 números.")]
        [MaxLength(6,ErrorMessage ="O campo {0} deve ter no máximo 6 números.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }
}