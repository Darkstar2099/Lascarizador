using System.Collections.Generic;
using Lascarizador.Core.Models;

namespace Lascarizador.ViewModels
{
    public class TransactionLogViewModel
    {
        public TransactionLog TransactionLog { get; set; }
        public IEnumerable<ErrorLog> ErrorsLog { get; set; }
    }
}