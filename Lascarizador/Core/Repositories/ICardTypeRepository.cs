using Lascarizador.Core.Models;
using System.Collections.Generic;

namespace Lascarizador.Core.Repositories
{
    public interface ICardTypeRepository : IRepository<CardType>
    {
        IEnumerable<CardType> GetCardTypes();
    }
}