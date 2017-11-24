using Lascarizador.Core.Models;
using Lascarizador.Core.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Lascarizador.Persistence.Repositories
{
    public class CardBrandRepository : Repository<CardBrand>, ICardBrandRepository
    {
        public CardBrandRepository(LascarizadorDbContext context) : base(context)
        {
        }

        public IEnumerable<CardBrand> GetCardBrands()
        {
            return LascarizadorDbContext.CardBrands
                .OrderBy(c => c.Id)
                .ToList();
        }

        public LascarizadorDbContext LascarizadorDbContext
        {
            get { return Context as LascarizadorDbContext; }
        }
    }
}