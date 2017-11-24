using Lascarizador.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lascarizador.Core.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        IEnumerable<Transaction> GetTransactions(int pageindex, int pagesize);
        IEnumerable<Transaction> GetTransactionsWithAllRelations();
        Transaction GetTransactionWithAllRelations(int transactionId);
        IEnumerable<Transaction> GetTransactionsFromCard(int cardId);
        IEnumerable<Transaction> GetTransactionsFromCardWithCardAndTransactionType(int cardId);


    }
}
