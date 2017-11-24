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
    public class ClientRepository_EditRemoveTests
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

            client = new Client
            {
                CPF = clientCPF,
                CreditLimit = 1,
                Email = "pedrita.flinstone@gmail.com",
                Name = "Pedrita Flinstone",
                Saldo = 0
            };

            _unitOfWork.Clients.Add(client);
            _unitOfWork.Complete();

        }

        [TestMethod]
        // Verifica se o campo de e-mail é editado corretamente
        public void Edit_TrocaEmail_EmailÉTrocadoNoBD()
        {
            //Arrange
            var newEmail = "pedrita.f.rubble@gmail.com";
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.CPF == clientCPF);

            //Act
            if (client != null)
                client.Email = newEmail;
            _unitOfWork.Complete();

            var clientInDb = _unitOfWork.Clients.SingleOrDefault(c => c.CPF == client.CPF);

            //Assert
            // Achou o client na base de dados?
            Assert.IsTrue(clientInDb != null);
            // O campo email está com o novo valor?
            Assert.AreSame(newEmail, clientInDb.Email);
        }

        [TestMethod]
        // Verifica se um cliente é apagado da base de dados
        public void Remove_ApagaCliente_ClienteNãoEncotradoNoBD()
        {
            //Arrange
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.CPF == clientCPF);

            //Act
            _unitOfWork.Clients.Remove(client);
            _unitOfWork.Complete();

            var clientInDb = _unitOfWork.Clients.SingleOrDefault(c => c.CPF == client.CPF);

            //Assert
            // O cliente foi encontrado na base de dados?
            Assert.IsTrue(client != null);
            // O cliente não foi encontrado na base de dados após ser apagado?
            Assert.IsTrue(clientInDb == null);
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
