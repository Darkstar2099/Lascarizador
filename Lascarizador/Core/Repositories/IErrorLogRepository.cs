using Lascarizador.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lascarizador.Core.Repositories
{
    public interface IErrorLogRepository : IRepository<ErrorLog>
    {
        IEnumerable<ErrorLog> GetAllErrorsFromTransactionLog(int transactionLogId);

    }

}
