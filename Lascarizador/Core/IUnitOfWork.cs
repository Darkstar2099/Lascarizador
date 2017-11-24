using Lascarizador.Core.Repositories;
using System;

namespace Lascarizador.Core
{
    // Interface IUnitOfWork(unidade de trabalho) do padrão Repository(Repositório).
    public interface IUnitOfWork : IDisposable
    {
        IClientRepository Clients { get; }
        ICardRepository Cards { get; }
        ICardBrandRepository CardBrands { get; }
        ICardTypeRepository CardTypes { get; }
        ITransactionRepository Transactions { get; }
        ITransactionTypeRepository TransactionTypes { get; }
        ITransactionLogRepository TransactionLogs { get; }
        IErrorLogRepository ErrorLogs { get; }
        int Complete();
    }
}
