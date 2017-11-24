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
    public class ValidadorCartaoSenhaInvalidoTests
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
        public void ProcessTransaction_CardNaoEncontrado_RetornaStatusReasonCartaoInvalido()
        {
            //Arrange
            // Muda Bandeira e Número para valores válidos porém inexistente na base de dados
            inputTransaction.card_brand = "bedrock_master";
            inputTransaction.card_number = "1111222233334445";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCartaoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 501);
        }

        [TestMethod]
        public void ProcessTransaction_CardHolderNameInvalido_RetornaStatusReasonCartaoInvalido()
        {
            //Arrange
            // Muda Nome do Portador para valor diferente do cartão
            inputTransaction.card_holder_name = "FRED FLINSTONE";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCartaoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 502);
        }

        [TestMethod]
        public void ProcessTransaction_CvvInvalido_RetornaStatusReasonCartaoInvalido()
        {
            //Arrange
            // Muda Cvv para valor diferente do cartão
            inputTransaction.cvv = "222";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCartaoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 503);
        }

        [TestMethod]
        public void ProcessTransaction_ExpMonthInvalido_RetornaStatusReasonCartaoInvalido()
        {
            //Arrange
            // Muda Mês de Expiração para valor diferente do cartão
            inputTransaction.expiration_month = 02;

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCartaoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 504);
        }

        [TestMethod]
        public void ProcessTransaction_ExpYearInvalido_RetornaStatusReasonCartaoInvalido()
        {
            //Arrange
            // Muda Ano de Expiração para valor diferente do cartão
            inputTransaction.expiration_year = 2019;

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCartaoInvalido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 504);
        }

        [TestMethod]
        public void ProcessTransaction_CardExigeSenha_RetornaStatusReasonSenhaRequerida()
        {
            //Arrange
            // Limpa Senha
            inputTransaction.password = "";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.IsTrue(validador.card.HasPassword = true);
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srSenhaRequerida);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 505);
        }

        [TestMethod]
        public void ProcessTransaction_PasswordMenorQue4_RetornaStatusReasonErroTamanhoSenha()
        {
            //Arrange
            // Muda Senha para valor inválido
            inputTransaction.password = "123";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.IsTrue(validador.card.HasPassword = true);
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srErroTamanhoSenha);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 506);
        }

        [TestMethod]
        public void ProcessTransaction_PasswordMaiorQue6_RetornaStatusReasonErroTamanhoSenha()
        {
            //Arrange
            // Muda Senha para valor inválido
            inputTransaction.password = "1234567";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.IsTrue(validador.card.HasPassword = true);
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srErroTamanhoSenha);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 506);
        }

        [TestMethod]
        public void ProcessTransaction_PasswordInvalida_RetornaStatusReasonSenhaInvalida()
        {
            //Arrange
            // Muda Senha para valor válido porém diferente da que está no cartão
            inputTransaction.password = "666666";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.IsTrue(validador.card.HasPassword = true);
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srSenhaInvalida);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 507);
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
