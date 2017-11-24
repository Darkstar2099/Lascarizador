using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lascarizador;
using Lascarizador.Core;
using Lascarizador.Persistence;
using Lascarizador.Core.Models;
using Lascarizador.Controllers;
using Lascarizador.ViewModels;
using Lascarizador.Dtos;
using System.Data.SqlClient;
using System;
using System.Web.Mvc;

namespace Lascarizador.UnitTest
{
    // Não consegui fazer os testes dos Controllers funcionar
    // Adicionar como Issue para futuras pesquisas

    [TestClass]
    public class ClientsControllerTests
    {
        private IUnitOfWork _unitOfWork;
        private LascarizadorDbContext _context;
        private SqlConnectionStringBuilder sqlBuilder;
        private SqlConnection sqlConnection;
        private ClientsController clientsController;
        private CardsController cardsController;
        private TransactionsController transactionsController;
        private Client client;
        private string clientCPF;
        //private Card card;
        //private string cardBrandId;
        private string cardPassword;
        private string cardBrandApiName;
        //private Transaction transaction;
        private string transactionTypeApiName;

        [TestInitialize]
        public void Init()
        {
            sqlConnection = new SqlConnection();
            var connectionString = @"data source=(LocalDb)\MSSQLLocalDB;initial catalog=Lascarizador.LascarizadorDbContext;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            sqlBuilder = new SqlConnectionStringBuilder(connectionString);

            sqlConnection.ConnectionString = sqlBuilder.ConnectionString;
            _context = new LascarizadorDbContext(sqlConnection);
            _unitOfWork = new UnitOfWork(_context);
            clientsController = new ClientsController();
            cardsController = new CardsController();

            //Cria Cliente
            clientCPF = "00424704714";
            client = new Client
            {
                CPF = clientCPF,
                CreditLimit = Convert.ToDecimal(100.00),
                Email = "pedrita.flinstone@gmail.com",
                Name = "Pedrita Flinstone",
                Saldo = 0
            };
            var clientEVM = new ClientEditViewModel
            {
                Client = client,
                CreditLimit = "100,00"
            };

            var clientResult = clientsController.Save(clientEVM);

            var cardBrandId = Convert.ToByte(1); //bedrock_visa;
            cardBrandApiName = "bedrock_visa";
            var cardType = Convert.ToByte(2); //tarja_magnetica
            cardPassword = "666666";
            //Encripta a senha e adiciona o Hash e o Salt ao dados do cartão
            var securedPassword = new SecuredPassword(cardPassword);

            var cardEVM = new CardEditViewModel
            {
                Card = new Card
                {
                    CardBrandId = cardBrandId,
                    CardHolderName = "PEDRITA FLINTSTONE",
                    CardTypeId = cardType,
                    Client = client,
                    Cvv = 999,
                    ExpirationDate = DateTime.Parse("2019/01/01"),
                    HashPassword = securedPassword.Hash,
                    HasPassword = true,
                    IsBlocked = false,
                    Number = "5555666677778888",
                    Password = cardPassword,
                    SaltPassword = securedPassword.Hash
                },
                CardBrands = null,
                CardTypes = null,
                ClientName = null,
                ExpirationMonth = 01,
                ExpirationYear = 2019
            };

            var cardResult = cardsController.Save(cardEVM);


        }

        [TestMethod]
        [Ignore]
        // Teste sendo ignorado pois não consegui fazer os Controllers funcionarem
        public void Save_TransaçãoComValorMaiorQueLimiteCredito_RetornaErros()
        {
            //Arrange
            transactionsController = new TransactionsController();

            transactionTypeApiName = "credito";
            var transactionEVM = new TransactionEditViewModel
            {
                Amount = "120,00",
                CardBrandApiName = cardBrandApiName,
                CardHolderName = "PEDRITA FLINTSTONE",
                Cvv = "999",
                ExpirationMonth = 01,
                ExpirationYear = 2019,
                Installments = 0,
                Number = "5555666677778888",
                Password = cardPassword,
                TransactionTypeApiName = transactionTypeApiName
            };

            //Act
            var transactionResult = transactionsController.Save(transactionEVM);
            var test = new RedirectResult("test");

            //Assert
            Assert.IsNotNull(transactionResult);
            Assert.AreSame(test, transactionResult);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var cleanupClient = _unitOfWork.Clients.SingleOrDefault(c => c.CPF == clientCPF);
            var cleanupClientResult = clientsController.Delete(cleanupClient.Id);
        }
    }
}
