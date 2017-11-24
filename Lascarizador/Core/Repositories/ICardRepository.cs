using Lascarizador.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lascarizador.Core.Repositories
{
    public interface ICardRepository : IRepository<Card>
    {
        IEnumerable<Card> GetCards(int pageindex, int pagesize);
        IEnumerable<Card> GetCardsWithClient(int pageindex, int pagesize);
        IEnumerable<Card> GetCardsFromClient(int clientId);
        IEnumerable<Card> GetCardsFromClientWithBrandsAndTypes(int clientId);
    }
}
