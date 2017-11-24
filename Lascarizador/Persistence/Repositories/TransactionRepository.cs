using Lascarizador.Core.Models;
using Lascarizador.Core.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Lascarizador.Persistence.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(LascarizadorDbContext context) : base(context)
        {
        }

        public IEnumerable<Transaction> GetTransactions(int pageindex, int pagesize = 10)
        {
            return LascarizadorDbContext.Transactions
                .OrderBy(t => t.CreationTimestamp)
                .Skip((pageindex - 1) * pagesize)
                .Take(pagesize)
                .ToList();
        }

        public IEnumerable<Transaction> GetTransactionsFromCard(int cardId)
        {
            return LascarizadorDbContext.Transactions
                .Where(t => t.CardId == cardId)
                .ToList();
        }

        public IEnumerable<Transaction> GetTransactionsWithAllRelations()
        {
            var transaction = LascarizadorDbContext.Transactions
                .OrderBy(t => t.CreationTimestamp)
                .ToList();

            foreach (var t in transaction)
            {
                var transactionType = LascarizadorDbContext.TransactionTypes.Find(t.TransactionTypeId);
                t.TransactionType = new TransactionType
                {
                    Id = t.TransactionTypeId,
                    Type = transactionType.Type
                };

                var card = LascarizadorDbContext.Cards.Find(t.CardId);
                var cardBrand = LascarizadorDbContext.CardBrands.Find(card.CardBrandId);
                var cardtype = LascarizadorDbContext.CardTypes.Find(card.CardTypeId);
                var client = LascarizadorDbContext.Clients.Find(card.ClientId);

                t.Card = new Card
                {
                    Id = t.CardId,
                    CardHolderName = card.CardHolderName,
                    Number = card.Number,
                    Cvv = card.Cvv,
                    ExpirationDate = card.ExpirationDate,
                    HasPassword = card.HasPassword,
                    HashPassword = card.HashPassword,
                    SaltPassword = card.SaltPassword,
                    IsBlocked = card.IsBlocked,
                    CardBrand = new CardBrand
                    {
                        Id = card.CardBrandId,
                        Name = cardBrand.Name
                    },
                    CardType = new CardType
                    {
                        Id = card.CardTypeId,
                        Name = cardtype.Name
                    },
                    Client = new Client
                    {
                        Id = card.ClientId,
                        CPF =  client.CPF,
                        CreditLimit = client.CreditLimit,
                        Email = client.Email,
                        Name = client.Name,
                        Saldo = client.Saldo
                    }
                };
            };

            return (transaction);
        }

        public Transaction GetTransactionWithAllRelations(int transactionId)
        {
            var transaction = LascarizadorDbContext.Transactions
                .SingleOrDefault(t => t.Id == transactionId);

            if (transaction == null)
                return (transaction);

            var transactionType = LascarizadorDbContext.TransactionTypes.Find(transaction.TransactionTypeId);
            transaction.TransactionType = new TransactionType
            {
                Id = transaction.TransactionTypeId,
                Type = transactionType.Type
            };

            var card = LascarizadorDbContext.Cards.Find(transaction.CardId);
            var cardBrand = LascarizadorDbContext.CardBrands.Find(card.CardBrandId);
            var cardtype = LascarizadorDbContext.CardTypes.Find(card.CardTypeId);
            var client = LascarizadorDbContext.Clients.Find(card.ClientId);

            transaction.Card = new Card
            {
                Id = transaction.CardId,
                CardHolderName = card.CardHolderName,
                Number = card.Number,
                Cvv = card.Cvv,
                ExpirationDate = card.ExpirationDate,
                HasPassword = card.HasPassword,
                HashPassword = card.HashPassword,
                SaltPassword = card.SaltPassword,
                IsBlocked = card.IsBlocked,
                CardBrand = new CardBrand
                {
                    Id = card.CardBrandId,
                    Name = cardBrand.Name
                },
                CardType = new CardType
                {
                    Id = card.CardTypeId,
                    Name = cardtype.Name
                },
                Client = new Client
                {
                    Id = card.ClientId,
                    CPF = client.CPF,
                    CreditLimit = client.CreditLimit,
                    Email = client.Email,
                    Name = client.Name,
                    Saldo = client.Saldo
                }
            };

            return (transaction);
        }

        public IEnumerable<Transaction> GetTransactionsFromCardWithCardAndTransactionType(int cardId)
        {
            return LascarizadorDbContext.Transactions
                .Where(t => t.CardId == cardId)
                .Include(t => t.Card)
                .Include(t => t.TransactionType)
                .OrderByDescending(t => t.CreationTimestamp)
                .ToList();
        }

        public int GetTransactionFromTransactionLog (int transactionLogId)
        {
            var transaction = LascarizadorDbContext.Transactions
                .SingleOrDefault(t => t.TransactionLogId == transactionLogId);

            if (transaction == null)
                return 0;

            return (transaction.Id);
        }

        public LascarizadorDbContext LascarizadorDbContext
        {
            get { return Context as LascarizadorDbContext; }
        }
    }
}