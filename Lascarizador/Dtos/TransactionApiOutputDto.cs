using System;
using System.Collections.Generic;

namespace Lascarizador.Dtos
{
    public class TransactionApiOutputDto
    {
        //Número identificador da transação
        public int transaction_id { get; set; }

        //Tipo de transação solicitada
        public string transaction_type { get; set; }

        //Bandeira do Cartão
        public string card_brand { get; set; }

        //Quatro(4) primeiros números do cartão
        public string card_number_first { get; set; }

        //Quatro(4) últimos números do cartão
        public string card_number_last { get; set; }
        
        //Nome do portador do cartão
        public string card_holder_name { get; set; }

        //Valor da transação
        public int amount { get; set; }

        //Quantidade de parcelas (somente válido para transações do tipo parcelado (ex.: crédito_parcelado)
        public int installments { get; set; }

        //Data e hora da criação da transação
        public DateTime creation_timestamp { get; set; }

        //Código de status da transação
        public string status_code { get; set; }

        //Razão do código de status caso ele seja negativo
        public string status_reason { get; set; }

        //Número identificador da requisição da transação
        public int transaction_log_id { get; set; }

        //Lista de Erros (em casos diferentes de status aproved e se solicitado)
        public IEnumerable<ErrorDto> errors { get; set; }
    }
}