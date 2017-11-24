using Lascarizador.Core.Models;
using Lascarizador.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lascarizador.Persistence.Repositories
{
    public class ErrorLogRepository : Repository<ErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(LascarizadorDbContext context) : base(context)
        {
        }
        public IEnumerable<ErrorLog> GetAllErrorsFromTransactionLog(int transactionLogId)
        {
            var errorLog = LascarizadorDbContext.ErrorLogs
                .Where(e => e.TransactionLogId == transactionLogId)
                .OrderBy(e => e.Id)
                .ToList();
            return errorLog;

        }

        public LascarizadorDbContext LascarizadorDbContext
        {
            get { return Context as LascarizadorDbContext; }
        }
    }

 
 
}
 
 