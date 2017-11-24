using System;
using System.ComponentModel.DataAnnotations;

namespace Lascarizador.Core.Models
{
    public class CardBrand
    {
        public byte Id { get; set; }

        [Required]
        [Display(Name = "Bandeira")]
        public string Name { get; set; }

        [Required]
        [Display (Name = "Nome da Bandeira para API")]
        public string ApiName { get; set; }
    }
}