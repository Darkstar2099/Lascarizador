using Lascarizador.Core;
using Lascarizador.Core.Models;
using Lascarizador.Persistence;
using Lascarizador.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Lascarizador.Controllers
{
    //___/ Controller da Classe Cliente \__________

    public class ClientsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LascarizadorDbContext _context;

        public ClientsController()
        {
            _context = new LascarizadorDbContext();
            _unitOfWork = new UnitOfWork(_context);
        }

        // Ação de Novo Cliente
        public ActionResult New()
        {
            //Cria nova instância vazia de ClientViewModel
            var editViewModel = new ClientEditViewModel
            {
                Client = new Client()
            };

            //Retorna a ClientViewModel para a view ClienteForm(novo cliente ou edição de cliente)
            return View("ClientForm", editViewModel);
        }

        // Ação de listar todos os clientes
        public ViewResult Index()
        {
            //Retorna todos os clientes da base de dados
            var client = _unitOfWork.Clients.GetAll();

            //Retorna a lista para a view Index (listagem de clientes)
            return View(client);
        }

        // Ação de detalhes de um cliente específico
        public ActionResult Details(int clientId)
        {
            //Buscar pelo cliente cujo Id tenha sido fornecido como parâmetro
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.Id == clientId);

            //Caso não encontre o cliente específico retorna Not Found
            if (client == null)
                return HttpNotFound();

            //Retorna o cliente específico para a view Details(detalhes do cliente)
            return View(client);
        }

        // Ação de Salvar dados de Cliente (novo ou edição)
        public ActionResult Save(ClientEditViewModel clientEditViewModel)
        {
            //Caso exista algum problema, cria uma nova instância da ClientEditViewModel para retornar os dados para a view ClientForm
            var editViewModel = new ClientEditViewModel
            {
                Client = clientEditViewModel.Client,
                CreditLimit = clientEditViewModel.CreditLimit
            };

            //Se o ModelState for válido
            if (!ModelState.IsValid)
            {
                //Retorna a ClienteEditViewModel para a view ClientForm(novo ou edição de cliente)
                return View("ClientForm", editViewModel);
            }

            //Verifica se o CPF é um número válido
            if (!ValidaCPF.IsCpf(clientEditViewModel.Client.CPF))
            {
                //Gera um erro no ModelState
                ModelState.AddModelError("", "O número de CPF não é válido");
                //Retorna a ClienteEditViewModel para a view ClientForm(novo ou edição de cliente)
                return View("ClientForm", editViewModel);
            }

            //Verifica CreditLimit está no formato numérico
            string tempCreditLimit;
            tempCreditLimit = clientEditViewModel.CreditLimit.Trim();
            tempCreditLimit = clientEditViewModel.CreditLimit.Replace(",", "");
            if (!tempCreditLimit.All(char.IsDigit))
            {
                //Gera um erro no ModelState
                ModelState.AddModelError("", "O Campo Limite de Crédito apresenta caracteres inválidos");
                //Retorna a ClienteEditViewModel para a view ClientForm(novo ou edição de cliente)
                return View("ClientForm", editViewModel);
            }

            decimal dec;
            bool isDecimal = decimal.TryParse(clientEditViewModel.CreditLimit, out dec);
            if (!isDecimal)
            {
                //Gera um erro no ModelState
                ModelState.AddModelError("", "O Campo Limite de Crédito apresenta caracteres inválidos");
                //Retorna a ClienteEditViewModel para a view ClientForm(novo ou edição de cliente)
                return View("ClientForm", editViewModel);
            }

            var decCreditLimit = decimal.Parse(clientEditViewModel.CreditLimit);

            //Verifica se os dados vem de um novo cliente ou se é uma edição de um cliente existente
            if (clientEditViewModel.Client.Id == 0)
            {
                // Atualiza o Limiti de Crédito com o valor do tipo decimal
                clientEditViewModel.Client.CreditLimit = decCreditLimit;
                //Caso seja um novo cliente adiciona ele a base de dados
                _unitOfWork.Clients.Add(clientEditViewModel.Client);
            }
            else
            {
                //Caso seja um cliente existente sobrepõe os campos novos com os antigos.
                var clientInDb = _unitOfWork.Clients.Get(clientEditViewModel.Client.Id);
                clientInDb.Name = clientEditViewModel.Client.Name;
                clientInDb.CPF = clientEditViewModel.Client.CPF;
                clientInDb.CreditLimit = decCreditLimit;
                clientInDb.Email = clientEditViewModel.Client.Email;
            }

            _unitOfWork.Complete();

            //Redireciona para a view Index (listagem de clientes)
            return RedirectToAction("Index", "Clients");
        }

        // Ação de edição de um cliente específico
        public ActionResult Edit(int clientId)
        {
            //Busca pelo cliente cujo Id tenha sido fornecido como parâmetro
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.Id == clientId);

            //Caso não encontre o cliente específico retorna Not Found
            if (client == null)
                return HttpNotFound();

            //Caso seja encontrado cria uma nova instância de ClientViewModel populada com os dados do cliente.
            var editViewModel = new ClientEditViewModel
            {
                Client = client,
                CreditLimit = Convert.ToString(client.CreditLimit)
            };

            //Retorna a ClientViewModel para a view ClienteForm(novo cliente ou edição de cliente)
            return View("ClientForm", editViewModel);
        }

        // Ação de remoção de um cliente específico
        public ActionResult Delete(int clientId)
        {
            //Busca pelo cliente com o Id igual ao passado como parâmetro
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.Id == clientId);

            if (client != null)
            {
                //Busca pelos cartões do cliente
                var cards = _unitOfWork.Cards.GetCardsFromClient(clientId);

                //Caso encontre algum cartão
                if (cards.Count() > 0)
                {

                    foreach (var c in cards)
                    {
                        //Recupera todas as transações de cada cartão
                        var transactions = _unitOfWork.Transactions.GetTransactionsFromCard(c.Id);

                        //Caso encontre alguma transação
                        if (transactions.Count() > 0)
                        {
                            foreach (var t in transactions)
                            {
                                //Busca pelos registros no Log de requisição de transação
                                var transactionLog = _unitOfWork.TransactionLogs.GetTransactionLogFromTransaction(t.Id);

                                //Remove os registros do Log de Requisições de Transação
                                _unitOfWork.TransactionLogs.Remove(transactionLog);
                            }

                            //Remove as transações do cartão
                            _unitOfWork.Transactions.RemoveRange(transactions);
                        }

                    }

                    //Remove o cartão do banco de dados
                    _unitOfWork.Cards.RemoveRange(cards);
                }

            }

            //Remove o cliente
            _unitOfWork.Clients.Remove(client);

            _unitOfWork.Complete();

            return RedirectToAction("Index");
        }
    }

}