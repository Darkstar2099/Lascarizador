using System.Collections.Generic;
using Lascarizador.Core.Models;

namespace Lascarizador.ViewModels
{
    public class TransactionViewModel
    {
        public Client Client { get; set; }
        public Card Card { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}