using Lascarizador.Core.Models;
using Lascarizador.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Lascarizador.Persistence.Repositories
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(LascarizadorDbContext context) : base(context)
        {
        }

        public IEnumerable<Client> GetClients(int pageindex, int pagesize = 10)
        {
            return LascarizadorDbContext.Clients
                .OrderBy(c => c.Name)
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