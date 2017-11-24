using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lascarizador.Core.Models
{
    public class Card
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [RegularExpression(@"^[A-ZÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+$", ErrorMessage = "O campo Nome do Portador do Cartão deve conter somente caracteres maiúsculos e válidos")]
        [Display(Name = "Nome do Portador do Cartão")]
        public string CardHolderName { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{3}", ErrorMessage ="O campo cvv deve ter 3 números.")]
        [Display(Name = "cvv")]
        public int Cvv { get; set; }

        [Required]
        [MinLength(12, ErrorMessage = "O campo {0} deve ter no mínimo 12 números.")]
        [MaxLength(16, ErrorMessage = "O campo {0} deve ter no máximo 16 números.")]
        [DataType(DataType.CreditCard)]
        [Display(Name = "Número do Cartão")]
        public string Number { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Expiração")]
        public DateTime ExpirationDate { get; set; }

        [ForeignKey("CardBrandId")]
        public CardBrand CardBrand { get; set; }

        [Required]
        [Display(Name = "Bandeira")]
        public byte CardBrandId { get; set; }

        [MinLength(4, ErrorMessage = "O campo {0} deve ter no mínimo 4 números.")]
        [MaxLength(6, ErrorMessage = "O campo {0} deve ter no máximo 6 números.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        public byte[] HashPassword { get; set; }

        public byte[] SaltPassword { get; set; }

        [ForeignKey("CardTypeId")]
        public CardType CardType { get; set; }

        [Required]       
        [Display(Name = "Tipo")]
        public byte CardTypeId { get; set; }

        public bool HasPassword { get; set; }

        [Required]
        public bool IsBlocked { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        [Required]
        public int ClientId { get; set; }
    }
}