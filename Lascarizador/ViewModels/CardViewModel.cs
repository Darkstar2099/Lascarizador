using System.Collections.Generic;
using Lascarizador.Core.Models;

namespace Lascarizador.ViewModels
{
    public class CardViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<Card> Cards { get; set; }
    }
}