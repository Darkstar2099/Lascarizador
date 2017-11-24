using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lascarizador.Core;
using Lascarizador.Core.Models;
using Lascarizador.Persistence;
using Lascarizador.Dtos;
using System;
using System.Data.SqlClient;

namespace Lascarizador.UnitTest
{
    [TestClass]
    public class ValidadorCartaoBloqueado
    {
        private IUnitOfWork _unitOfWork;
        private LascarizadorDbContext _context;
        private Validador validador;
        private SqlConnectionStringBuilder sqlBuilder;
        private SqlConnection sqlConnection;
        private Card card;
        private int cardId;
        private string cardPassword;
        private SecuredPassword securedPassword;
        private TransactionApiInputDto inputTransaction;

        [TestInitialize]
        public void Init()
        {
            sqlConnection = new SqlConnection();
            var connectionString = @"data source=(LocalDb)\MSSQLLocalDB;initial catalog=Lascarizador.LascarizadorDbContext;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            sqlBuilder = new SqlConnectionStringBuilder(connectionString);

            sqlConnection.ConnectionString = sqlBuilder.ConnectionString;
            _context = new LascarizadorDbContext(sqlConnection);
            _unitOfWork = new UnitOfWork(_context);
            validador = new Validador(_unitOfWork, true);

            cardPassword = "222333";
            securedPassword = new SecuredPassword(cardPassword);

            card = new Card
            {
                CardHolderName = "FREDERICO FLINSTONE",
                CardBrandId = 2,
                CardTypeId = 2,
                ClientId = 1,
                Cvv = 222,
                ExpirationDate = Convert.ToDateTime("01/01/2019"),
                HasPassword = true,
                IsBlocked = true,
                Number = "5555666677778888",
                Password = cardPassword,
                HashPassword = securedPassword.Hash,
                SaltPassword = securedPassword.Salt
            };

            //Act
            _unitOfWork.Cards.Add(card);
            // Guarda o identificador do cartão;
            _unitOfWork.Complete();
            cardId = card.Id;

            // Cria um input padrão sem problemas para uso futuro
            inputTransaction = new TransactionApiInputDto
            {
                amount = 10000,
                card_brand = "bedrock_master",
                card_holder_name = "FREDERICO FLINSTONE",
                card_number = "5555666677778888",
                cvv = "222",
                expiration_month = 01,
                expiration_year = 2019,
                installments = 0,
                password = cardPassword,
                transaction_type = "credito",
                show_errors = true
            };

        }

        [TestMethod]
        public void ProcessTransaction_CardBloqueado_RetornaStatusReasonCartaoBloqueado()
        {
            //Arrange
            // Usar a transação criada na inicialização sem alterações

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCartaoBloqueado);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 509);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var cleanupCard = _unitOfWork.Cards.SingleOrDefault(c => c.Id == cardId);
            if (cleanupCard != null)
            {
                _unitOfWork.Cards.Remove(cleanupCard);
                _unitOfWork.Complete();
            }
        }
    }
}
