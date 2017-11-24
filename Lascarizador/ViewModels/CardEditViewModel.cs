using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lascarizador.Core.Models;

namespace Lascarizador.ViewModels
{
    public class CardEditViewModel
    {
        public string ClientName { get; set; }

        public IEnumerable<CardBrand> CardBrands { get; set; }

        public IEnumerable<CardType> CardTypes { get; set; }

        public Card Card { get; set; }

        [Required]
        [Display(Name = "Mês")]
        [Range(1, 12)]
        public byte ExpirationMonth { get; set; }

        [Required]
        [Display(Name = "Ano")]
        [Range(2017, 2100)]
        public int ExpirationYear { get; set; }

    }



}