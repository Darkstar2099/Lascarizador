using Lascarizador.Core.Models;
using Lascarizador.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Lascarizador.Persistence.Repositories
{
    public class CardTypeRepository : Repository<CardType>, ICardTypeRepository
    {
        public CardTypeRepository(LascarizadorDbContext context) : base(context)
        {
        }

        public IEnumerable<CardType> GetCardTypes()
        {
            return LascarizadorDbContext.CardTypes
                .OrderBy(c => c.Name)
                .ToList();
        }

        public LascarizadorDbContext LascarizadorDbContext
        {
            get { return Context as LascarizadorDbContext; }
        }
    }
}