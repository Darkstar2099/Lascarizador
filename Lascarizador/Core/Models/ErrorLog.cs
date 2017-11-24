using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lascarizador.Core.Models
{
    public class ErrorLog
    {
        [Required]
        public int Id { get; set; }

        public int Error_code { get; set; }

        public string Error_message { get; set; }

        [Required]
        public int TransactionLogId { get; set; }

        [ForeignKey("TransactionLogId")]
        public TransactionLog transactionLog { get; set; }
    }
}