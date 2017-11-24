using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lascarizador;
using Lascarizador.Core;
using Lascarizador.Persistence;
using Lascarizador.Core.Models;
using Lascarizador.Dtos;
using System;
using System.Data.SqlClient;

namespace Lascarizador.UnitTest
{
    [TestClass]
    public class ValidadorCampoInvalidoTests
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

            cardPassword = "123456";
            securedPassword = new SecuredPassword(cardPassword);

            card = new Card
            {
                CardHolderName = "FREDERICO FLINSTONE",
                CardBrandId = 1,
                CardTypeId = 2,
                ClientId = 1,
                Cvv = 111,
                ExpirationDate = Convert.ToDateTime("01/01/2020"),
                HasPassword = true,
                IsBlocked = false,
                Number = "1111222233334444",
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
                card_brand = "bedrock_visa",
                card_holder_name = "FREDERICO FLINSTONE",
                card_number = "1111222233334444",
                cvv = "111",
                expiration_month = 01,
                expiration_year = 2020,
                installments = 0,
                password = "123456",
                transaction_type = "credito",
                show_errors = true
            };

        }

        [TestMethod]
        public void ProcessTransaction_TransactionTypeInvalido_RetornaStatusReasonCampoInvalido()
        {
            //Arrange
            // Muda Tipo de Transação para um valor inválido
            inputTransaction.transaction_type = "drebito";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 401);
        }

        [TestMethod]
        public void ProcessTransaction_CardBrandInvalido_RetornaStatusReasonCampoInvalido()
        {
            //Arrange
            // Muda Bandeira de Cartão para um valor inválido
            inputTransaction.card_brand = "bedrock_citi";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 402);
        }

        [TestMethod]
        public void ProcessTransaction_CardNumberNaoNumerico_RetornaStatusReasonCampoInvalido()
        {
            //Arrange
            // Muda Número do Cartão para um valor inválido
            inputTransaction.card_number = "A111222233334444";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 403);
        }

        [TestMethod]
        public void ProcessTransaction_CardNumberMenorQue12_RetornaStatusReasonCampoInvalido()
        {
            //Arrange
            // Muda Número do Cartão para um valor inválido
            inputTransaction.card_number = "11112222333";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 404);
        }

        [TestMethod]
        public void ProcessTransaction_CardNumberMaiorQue16_RetornaStatusReasonCampoInvalido()
        {
            //Arrange
            // Muda Número do Cartão para um valor inválido
            inputTransaction.card_number = "11112222333344445";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 404);
        }

        [TestMethod]
        public void ProcessTransaction_CvvNaoNumerico_RetornaStatusReasonCampoInvalido()
        {
            //Arrange
            // Muda Cvv para um valor inválido
            inputTransaction.cvv = "12A";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 405);
        }

        [TestMethod]
        public void ProcessTransaction_CvvMenorQue3_RetornaStatusReasonCampoInvalido()
        {
            //Arrange
            // Muda Cvv para um valor inválido
            inputTransaction.cvv = "12";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 406);
        }

        [TestMethod]
        public void ProcessTransaction_CvvMaiorQue3_RetornaStatusReasonCampoInvalido()
        {
            //Arrange
            // Muda Cvv para um valor inválido
            inputTransaction.cvv = "1234";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 406);
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
