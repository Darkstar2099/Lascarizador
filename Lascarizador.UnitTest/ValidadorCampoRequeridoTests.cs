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
    public class ValidadorCampoRequeridoTests
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
        public void ProcessTransaction_TransactioTypeNaoInformado_RetornaStatusReasonCampoRequerido()
        {
            //Arrange
            // Limpa Tipo de Transação
            inputTransaction.transaction_type = "";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoRequerido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 301);
        }

        [TestMethod]
        public void ProcessTransaction_CardBrandNaoInformado_RetornaStatusReasonCampoRequerido()
        {
            //Arrange
            // Limpa Bandeira do Cartão
            inputTransaction.card_brand = "";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoRequerido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 302);

        }

        [TestMethod]
        public void ProcessTransaction_CardHolderNameNaoInformado_RetornaStatusReasonCampoRequerido()
        {
            //Arrange
            // Limpa Nome do Portador
            inputTransaction.card_holder_name = "";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoRequerido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 303);
        }

        [TestMethod]
        public void ProcessTransaction_CardNumberNaoInformado_RetornaStatusReasonCampoRequerido()
        {
            //Arrange
            // Limpa Número do Cartão
            inputTransaction.card_number = "";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoRequerido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 304);
        }

        [TestMethod]
        public void ProcessTransaction_CvvNaoInformado_RetornaStatusReasonCampoRequerido()
        {
            //Arrange
            // Limpa Cvv
            inputTransaction.cvv = "";

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srCampoRequerido);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 305);
        }

        //Como testar o erro 600 (Cliente não encontrado)???

        [TestMethod]
        public void ProcessTransaction_SaldoInsuficiente_RetornaStatusReasonSaldoInsuficiente()
        {
            //Arrange
            // Muda Valor para um número maior que o Limite de Crédito do Cliente
            inputTransaction.amount = 300100;

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scRecusada);
            Assert.AreSame(statusReason, Constantes.srSaldoInsuficiente);
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(errors[0].error_code == 601);
        }

        //Refazer o teste do saldo apos uma transação passar
        // Testar um valor = Limite de Crédito + Saldo(que é negativo) + 1.
        //Exemplo valor = 13000,00 + (-200,00) + 1 = 12801,00

        [TestMethod]
        public void ProcessTransaction_TransactioOK_RetornaStatusReasonSucesso()
        {
            //Arrange
            // Usa a transação definida na inicialização sem alterá-la

            //Act
            validador.ProcessTransaction(inputTransaction);
            var statusCode = validador.statusCode;
            var statusReason = validador.statusReason;
            var errors = validador.errors;

            //Assert
            Assert.AreSame(statusCode, Constantes.scAprovada);
            Assert.AreSame(statusReason, Constantes.srSucesso);
            Assert.IsTrue(errors.Count == 0);

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
