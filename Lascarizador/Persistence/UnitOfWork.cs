using Lascarizador.Core;
using Lascarizador.Core.Repositories;
using Lascarizador.Persistence.Repositories;

namespace Lascarizador.Persistence
{
    //Implementação da Interface IUnitOfWork(unidade de trabalho) do padrão Repository(Repositório)

    public class UnitOfWork : IUnitOfWork
    {
        private readonly LascarizadorDbContext _context;

        public UnitOfWork(LascarizadorDbContext context)
        {
            _context = context;
            Clients = new ClientRepository(_context);
            CardBrands = new CardBrandRepository(_context);
            CardTypes = new CardTypeRepository(_context);
            Cards = new CardRepository(_context);
            Transactions = new TransactionRepository(_context);
            TransactionTypes = new TransactionTypeRepository(_context);
            TransactionLogs = new TransactionLogRepository(_context);
            ErrorLogs = new ErrorLogRepository(_context);
        }

        public IClientRepository Clients { get; private set; }
        public ICardRepository Cards { get; private set; }
        public ICardBrandRepository CardBrands { get; private set; }
        public ICardTypeRepository CardTypes { get; private set; }
        public ITransactionRepository Transactions { get; private set; }
        public ITransactionTypeRepository TransactionTypes { get; private set; }
        public ITransactionLogRepository TransactionLogs { get; private set; }
        public IErrorLogRepository ErrorLogs { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}