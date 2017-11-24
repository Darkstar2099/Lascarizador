using Lascarizador.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lascarizador.Core.Repositories
{
    public interface ITransactionLogRepository : IRepository<TransactionLog>
    {
        IEnumerable<TransactionLog> GetAllWithErrors();
        IEnumerable<TransactionLog> GetTransactionLogWithTransactions();
        IEnumerable<TransactionLog> GetAllOrderByCreationTsDesc();
        TransactionLog GetTransactionLogFromTransaction(int transactionId);

    }
}
