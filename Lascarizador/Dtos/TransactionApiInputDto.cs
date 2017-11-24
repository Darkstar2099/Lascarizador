namespace Lascarizador.Dtos

{
    public class TransactionApiInputDto
    {
        //Tipo de transação solicitada (valores possíveis encontrados na classe TransactionType)
        public string transaction_type { get; set; }

        //Bandeiras de cartão informada (valores possíveis encontrados na classe CardBrand)
        public string card_brand { get; set; }

        //Número do cartão informado
        public string card_number { get; set; }

        //Nome do portador do cartão informado
        public string card_holder_name { get; set; }

        //Mês de expiração do cartão informado
        public byte expiration_month { get; set; }

        //Ano de expiração do cartão informado
        public int expiration_year { get; set; }

        //Cvv do cartão informado
        public string cvv { get; set; }

        //Valor da transação solicitada
        public int amount { get; set; }

        //Quantidade de parcelas (campo necessário para transações que tenham o atributo TransactionType(tipo de transação) com o campo InstallmentsAvailable como verdadeiro, Ex.: crédito_parcelado)
        public int installments { get; set; }

        //Senha do cartão informado (requerida para cartões que tenham o atributo HasPassword(possui senha) como verdadeiro)
        public string password { get; set; }

        //mostra os erros encontrados, popula a lista de erros
        public bool show_errors { get; set; }
    }
}