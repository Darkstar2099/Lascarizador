using Lascarizador.Core.Models;
using Lascarizador.Core.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Lascarizador.Persistence.Repositories
{
    public class TransactionLogRepository : Repository<TransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(LascarizadorDbContext context) : base(context)
        {
        }

        public IEnumerable<TransactionLog> GetAllWithErrors()
        {
            var transactionsLog = LascarizadorDbContext.TransactionLogs.ToList();
            return transactionsLog;

        }

        public IEnumerable<TransactionLog> GetTransactionLogWithTransactions()
        {
            return LascarizadorDbContext.TransactionLogs.ToList();
        }

        public IEnumerable<TransactionLog> GetAllOrderByCreationTsDesc()
        {
            var transactionsLog = LascarizadorDbContext.TransactionLogs
                .OrderByDescending(t => t.Creation_timestamp)
                .ToList();
            return transactionsLog;
        }

        public TransactionLog GetTransactionLogFromTransaction(int transactionId)
        {
            TransactionLog transactionLog;

            var transaction = LascarizadorDbContext.Transactions.SingleOrDefault(t => t.Id == transactionId);

            if (transaction == null)
                return null;

            transactionLog = LascarizadorDbContext.TransactionLogs.SingleOrDefault(l => l.Id == transaction.TransactionLogId);

            if (transactionLog == null)
                return null;

            return transactionLog;

        }

        public LascarizadorDbContext LascarizadorDbContext
        {
            get { return Context as LascarizadorDbContext; }
        }
    }
}