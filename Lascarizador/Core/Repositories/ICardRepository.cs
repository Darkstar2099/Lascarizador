using Lascarizador.Core.Models;
using System.Collections.Generic;

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
