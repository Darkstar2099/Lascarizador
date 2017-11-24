using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lascarizador.Core.Models
{
    public class TransactionLog
    {
        //Identificação do registro no log
        public int Id { get; set; }

        //Tipo de transação solicitada
        public string Transaction_type { get; set; }

        //Bandeira do Cartão
        public string Card_brand { get; set; }

        //Quatro(4) primeiros números do cartão
        public string Card_number_first { get; set; }

        //Quatro(4) últimos números do cartão
        public string Card_number_last { get; set; }

        //Nome do portador do cartão
        public string Card_holder_name { get; set; }

        //Valor da transação
        public int Amount { get; set; }

        //Quantidade de parcelas (somente válido para transações do tipo parcelado (ex.: crédito_parcelado)
        public int Installments { get; set; }

        //Data e hora da criação do registro de log da transação
        public DateTime Creation_timestamp { get; set; }

        //Código de status da transação
        public string Status_code { get; set; }

        //Razão do código de status caso ele seja negativo
        public string Status_reason { get; set; }

        //Lista de Erros (em casos diferentes de status aproved e se solicitado)
        public IEnumerable<ErrorLog> ErrorsLog { get; set; }

    }
}