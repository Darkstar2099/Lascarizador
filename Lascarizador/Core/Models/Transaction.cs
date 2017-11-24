using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lascarizador.Core.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [ForeignKey("TransactionTypeId")]
        public TransactionType TransactionType { get; set; }

        [Required]
        public byte TransactionTypeId { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

        [Required]
        public int CardId { get; set; }

        [Required]
        [Display(Name = "Valor")]
        public decimal Amount { get; set; }

        [Display(Name ="Número de Parcelas")]
        public int Installments { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreationTimestamp { get; set; }

        [Required]
        public int TransactionLogId { get; set; }

        [ForeignKey("TransactionLogId")]
        public TransactionLog TransactionLog { get; set; }

    }
}