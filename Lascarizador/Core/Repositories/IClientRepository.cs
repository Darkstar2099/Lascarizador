using Lascarizador.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lascarizador.Core.Repositories
{
    public interface IClientRepository : IRepository<Client>
    {
        IEnumerable<Client> GetClients(int pageindex, int pagesize);
    }
}
