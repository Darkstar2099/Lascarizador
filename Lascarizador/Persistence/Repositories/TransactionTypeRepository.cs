using Lascarizador.Core.Models;
using Lascarizador.Core.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Lascarizador.Persistence.Repositories
{
    public class TransactionTypeRepository : Repository<TransactionType>, ITransactionTypeRepository
    {
        public TransactionTypeRepository(LascarizadorDbContext context) : base(context)
        {
        }

        public IEnumerable<TransactionType> GetTransactionTypes(int pageindex, int pagesize)
        {
            return LascarizadorDbContext.TransactionTypes
                .OrderBy(t => t.Type)
                .Skip((pageindex - 1) * pagesize)
                .Take(pagesize)
                .ToList();
        }

        public LascarizadorDbContext LascarizadorDbContext
        {
            get { return Context as LascarizadorDbContext; }
        }
    }
}