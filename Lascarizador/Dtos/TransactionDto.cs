using System;

namespace Lascarizador.Dtos
{
    public class TransactionDto
    {
        //Valor da transação
        public int amount { get; set; }

        //Cartão da transação
        public CardDto card { get; set; }

        //Data e Hora da criação da Transação
        public DateTime creation_timestamp { get; set; }

        //Número identificador da transação
        public int id { get; set; }

        //Quantidade de parcelas(somente válido para transações do tipo parcelado (ex.: crédito_parcelado)
        public int installments { get; set; }

        //Número identificador da requisição da transação
        public int transaction_log_id { get; set; }

        //Tipo da transação
        public TransactionTypeDto transaction_type { get; set; }
    }
}