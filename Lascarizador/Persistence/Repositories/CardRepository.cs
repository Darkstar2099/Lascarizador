using Lascarizador.Core.Models;
using Lascarizador.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Lascarizador.Persistence.Repositories
{
    public class CardRepository : Repository<Card>, ICardRepository
    {
        public CardRepository(LascarizadorDbContext context) : base(context)
        {
        }

        public IEnumerable<Card> GetCards(int pageindex, int pagesize = 10)
        {
            return LascarizadorDbContext.Cards
                .OrderBy(c => c.Number)
                .Skip((pageindex - 1) * pagesize)
                .Take(pagesize)
                .ToList();
        }

        public IEnumerable<Card> GetCardsWithClient(int pageindex, int pagesize = 10)
        {
            return LascarizadorDbContext.Cards
                .Include(c => c.Client)
                .OrderBy(c => c.Number)
                .Skip((pageindex - 1) * pagesize)
                .Take(pagesize)
                .ToList();
        }

        public IEnumerable<Card> GetCardsFromClient(int clientId)
        {
            return LascarizadorDbContext.Cards
                .Where(c => c.ClientId == clientId)
                .OrderBy(c => c.Number)
                .ToList();

        }

        public IEnumerable<Card> GetCardsFromClientWithBrandsAndTypes(int clientId)
        {
            return LascarizadorDbContext.Cards
                .Where(c => c.ClientId == clientId)
                .Include(c => c.CardBrand)
                .Include(c => c.CardType)
                .OrderBy(c => c.Number)
                .ToList();
        }

        public LascarizadorDbContext LascarizadorDbContext
        {
            get { return Context as LascarizadorDbContext; }
        }
    }
}