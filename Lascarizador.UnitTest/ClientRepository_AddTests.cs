using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lascarizador;
using Lascarizador.Core;
using Lascarizador.Persistence;
using Lascarizador.Core.Models;
using Lascarizador.Dtos;
using System.Data.SqlClient;

namespace Lascarizador.UnitTest
{
    [TestClass]
    public class ClientRepository_AddTests
    {
        private IUnitOfWork _unitOfWork;
        private LascarizadorDbContext _context;
        private SqlConnectionStringBuilder sqlBuilder;
        private SqlConnection sqlConnection;
        private Client client;
        private string clientCPF;

        [TestInitialize]
        public void Init()
        {
            sqlConnection = new SqlConnection();
            var connectionString = @"data source=(LocalDb)\MSSQLLocalDB;initial catalog=Lascarizador.LascarizadorDbContext;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            sqlBuilder = new SqlConnectionStringBuilder(connectionString);

            sqlConnection.ConnectionString = sqlBuilder.ConnectionString;
            _context = new LascarizadorDbContext(sqlConnection);
            _unitOfWork = new UnitOfWork(_context);
            clientCPF = "77788899900";
        }

        [TestMethod]
        // Verifica se o cliente é criado sem problemas na Base de Dados
        public void Add_ClienteSemProblemas_AchaClienteNoBD()
        {
            //Arrange
            client = new Client
            {
                CPF = clientCPF,
                CreditLimit = 1,
                Email = "pedrita.flinstone@gmail.com",
                Name = "Pedrita Flinstone",
                Saldo = 0
            };

            //Act
            _unitOfWork.Clients.Add(client);
            _unitOfWork.Complete();

            var clientInDb = _unitOfWork.Clients.SingleOrDefault(c => c.CPF == client.CPF);

            //Assert
            // Cliente foi achado na base de dados?
            Assert.IsTrue(clientInDb != null);
            // Cliente achado na base de dados, é o mesmo que foi criado?
            Assert.AreSame(client, clientInDb);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var cleanupClient = _unitOfWork.Clients.SingleOrDefault(c => c.CPF == clientCPF);
            if (cleanupClient != null)
            {
                _unitOfWork.Clients.Remove(cleanupClient);
                _unitOfWork.Complete();
            }
        }
    }
}
