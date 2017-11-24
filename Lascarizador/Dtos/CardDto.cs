using System.ComponentModel.DataAnnotations.Schema;

namespace Lascarizador.Dtos
{
    public class CardDto
    {
        //Bandeira do cartão
        [ForeignKey("CardBrandId")]
        public CardBrandDto card_brand { get; set; }

        //Cliente do cartão
        [ForeignKey("ClientId")]
        public ClientDto client { get; set; }

        //Mês de expiração do cartão
        public int expiration_month { get; set; }

        //Ano de expiração do cartão
        public int expiration_year { get; set; }

        //Primeiros dígitos do cartão
        public string first_digits { get; set; }

        //Últimos dígitos do cartão
        public string last_digits { get; set; }
    }
}