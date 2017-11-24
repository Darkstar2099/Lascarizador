using System.ComponentModel.DataAnnotations;

namespace Lascarizador.Core.Models
{
    public class CardType
    {
        public byte Id { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Exige Senha")]
        public bool PasswordAvailable { get; set; }
    }
    
}