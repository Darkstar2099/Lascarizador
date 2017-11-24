using Lascarizador.Core;
using Lascarizador.Core.Models;
using Lascarizador.Persistence;
using Lascarizador.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Lascarizador.Controllers
{
    //___/ Controller da Classe Card \__________

    public class CardsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LascarizadorDbContext _context;

        public CardsController()
        {
            _context = new LascarizadorDbContext();
            _unitOfWork = new UnitOfWork(_context);
        }

        // Ação de Novo Cartão para o Cliente de Id passado como parâmetro
        public ActionResult New(int clientId)
        {
            //Busca pelo cliente com Id igual ao passado como parâmetro
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.Id == clientId);

            //Caso não encontre o cliente retorna Not Found
            if (client == null)
                return HttpNotFound();

            //Pega todos os CardBrands(Bandeiras de Cartão)
            var cardbrand = _unitOfWork.CardBrands.GetAll();

            //Pega todos os CardTypes(Tipos de Cartão)
            var cardtype = _unitOfWork.CardTypes.GetAll();

            //Cria nova instância vazia de CardEditViewModel com um cartão vazio, vinculado ao cliente encontrado
            //  e com as listas de CardBrands e CardTypes
            var viewModel = new CardEditViewModel
            {
                ClientName = client.Name,
                CardBrands = cardbrand,
                CardTypes = cardtype,
                Card = new Card
                {
                    ClientId = client.Id
                }
                
            };

            //Retorna a CardEditViewModel para a view CardForm(novo cartão ou edição de cartão)
            return View("CardForm", viewModel);
        }

        // Ação de listar todos os cartões de um cliente específico
        public ViewResult Index(int clientId)
        {
            //Busca pelo cliente com Id igual ao passado como parâmetro
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.Id == clientId);

            //Caso não encontre o cliente retorna Not Found
            //if (client == null)
            //    return new HttpNotFoundResult("Client não encontrado na Base de dados");

            //Busca pelos cartões de um cliente específico e retorna
            //  com a lista de CardBrands(bandeiras de cartão) e CardTypes(tipos de cartão)
            var cards = _unitOfWork.Cards.GetCardsFromClientWithBrandsAndTypes(clientId);

            //Cria nova instância de CardEditViewModel vinculado ao cliente encontrado, com os cartões do cliente 
            //  e com as listas de CardBrands e CardTypes
            var viewModel = new CardViewModel
            {
                Client = client,
                Cards = cards
            };

            //Retorna a CardViewModel para a view Index (listagem de cartões)
            return View("Index", viewModel);
        }

        // Ação de detalhes de um cartão específico
        public ActionResult Details(int cardId)
        {
            //Busca pelo cartão com Id igual ao passado como parâmetro
            var card = _unitOfWork.Cards.SingleOrDefault(c => c.Id == cardId);
            //var card = _context.Cards.Include(c => c.Id).SingleOrDefault(c => c.Id == cardId);

            //Caso não encontre o cartão retorna Not Found
            if (card == null)
                return HttpNotFound();

            //Retorna o cartão para a view Details (detalhes do cartão)
            return View(card);

        }

        // Ação de salvar dados de cartão (novo ou edição)
        public ActionResult Save(CardEditViewModel cardEditViewModel)
        {

            //Busca pelo cardtype(tipo de cartão) do cartão recebido como parâmetro
            var _cardType = _unitOfWork.CardTypes.Get(cardEditViewModel.Card.CardTypeId);
            //Se o tipo do cartão não disponibiliza senha...
            if (!_cardType.PasswordAvailable)
            {
                //... e o cartão está com a opção HasPassword(tem senha) marcada como true
                if (cardEditViewModel.Card.HasPassword || String.IsNullOrEmpty(cardEditViewModel.Card.Password))
                    //Gera um erro no ModelState
                    ModelState.AddModelError("", "Esse tipo de cartão não permite Senha.");
            }
            else //Se o tipo do cartão disponibiliza senha...
            {
                //... e se o cartão está com opção HasPassword(tem senha) marcada como true...
                if (cardEditViewModel.Card.HasPassword)
                    //... e uma senha não foi informada ou uma senha vazia foi passada
                    if (String.IsNullOrEmpty(cardEditViewModel.Card.Password))
                        //Gera um erro no ModelState
                        ModelState.AddModelError("", "A senha é exigida para esse tipo de cartão e não foi informada.");
            }

            //Se o cartão está com opção HasPassword(tem senha) marcada como false...
            if (!cardEditViewModel.Card.HasPassword)
                //... e foi informada uma senha não vazia
                if (!(cardEditViewModel.Card.Password == null))
                    //Gera um erro no ModelState
                    ModelState.AddModelError("", "A senha não é exigida para esse tipo de cartão mas uma senha foi informada.");

            //Se ModelState não for válido
            if (!ModelState.IsValid)
            {
                //Pega todos os CardBrands(bandeiras de cartão)
                var cardBrand = _unitOfWork.CardBrands.GetAll();
                //Pega todos os Cardtypes(tipos de cartão)
                var cardType = _unitOfWork.CardTypes.GetAll();
                var client = _unitOfWork.Clients.Get(cardEditViewModel.Card.ClientId);
                //Caso exista algum problema, cria uma nova instância da CardEditViewModel com os CardBrands e os CardTypes 
                //  para retornar os dados para a view CardForm
                var editViewModel = new CardEditViewModel
                {
                    Card = cardEditViewModel.Card,
                    CardBrands = cardBrand,
                    CardTypes = cardType,
                    ClientName = client.Name,
                    ExpirationMonth = cardEditViewModel.ExpirationMonth,
                    ExpirationYear = cardEditViewModel.ExpirationYear
                };

                //Retorna a CardEditViewModel para a view CardForm (novo cartão ou edição de cartão)
                return View("CardForm", editViewModel);
            }

            //Se o cartão for um cartão novo...
            if (cardEditViewModel.Card.Id == 0)
            {
                //Encripta a senha e adiciona o Hash e o Salt ao dados do cartão
                var securedPassword = new SecuredPassword(cardEditViewModel.Card.Password);
                cardEditViewModel.Card.HashPassword = securedPassword.Hash;
                cardEditViewModel.Card.SaltPassword = securedPassword.Salt;

                //Acerta ExpirationDate
                cardEditViewModel.Card.ExpirationDate = Convert.ToDateTime("01/" + Convert.ToString(cardEditViewModel.ExpirationMonth) + "/" + Convert.ToString(cardEditViewModel.ExpirationYear));

                //Adiciona o cartão a base de dados
                _unitOfWork.Cards.Add(cardEditViewModel.Card);
            }
            //Se o cartão já existir e estiver sendo editado
            else
            {
                //Sobrepoe os dados novos aos antigos
                var cardInDb = _unitOfWork.Cards.Get(cardEditViewModel.Card.Id);
                cardInDb.CardBrandId = cardEditViewModel.Card.CardBrandId;
                cardInDb.CardHolderName = cardEditViewModel.Card.CardHolderName;
                cardInDb.CardTypeId = cardEditViewModel.Card.CardTypeId;
                cardInDb.Cvv = cardEditViewModel.Card.Cvv;
                cardInDb.ExpirationDate = Convert.ToDateTime("01/" + Convert.ToString(cardEditViewModel.ExpirationMonth) + "/" + Convert.ToString(cardEditViewModel.ExpirationYear));
               // cardInDb.ExpirationDate = cardEditViewModel.Card.ExpirationDate;
                cardInDb.HasPassword = cardEditViewModel.Card.HasPassword;
                cardInDb.IsBlocked = cardEditViewModel.Card.IsBlocked;
                cardInDb.Number = cardEditViewModel.Card.Number;
                var securedPassword = new SecuredPassword(cardEditViewModel.Card.Password);
                cardInDb.Password = cardEditViewModel.Card.Password;
                cardInDb.HashPassword = securedPassword.Hash;
                cardInDb.SaltPassword = securedPassword.Salt;
            }

            _unitOfWork.Complete();            

            //Redireciona para a view Index(listagem de cartões) passando o Id do Cliente
            return RedirectToAction("Index", new { clientId = cardEditViewModel.Card.ClientId });
        }

        // Ação de edição de um cartão específico
        public ActionResult Edit(int cardId)
        {
            //Busca pelo cartão com Id igual ao passado como parâmetro
            var card = _unitOfWork.Cards.SingleOrDefault(c => c.Id == cardId);

            //Caso não encontre o cartão retorna Not Found
            if (card == null)
                return HttpNotFound();

            //Busca pelo cliente com Id igual ao do cartão encontrado
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.Id == card.ClientId);

            //Caso não encontre o cliente retorna Not Found
            if (client == null)
                return HttpNotFound();

            //Busca todos os cardbrands (bandeiras de cartão)
            var cardbrand = _unitOfWork.CardBrands.GetAll();

            //Busca todos os cardtypes (tipos de cartão)
            var cardtype = _unitOfWork.CardTypes.GetAll();

            //Cria nova instância de CardEditViewModel com o cartão do cliente encontrado 
            //  e com as listas de CardBrands e CardTypes
            var viewModel = new CardEditViewModel()
            {
                ClientName = client.Name,
                CardTypes = cardtype,
                CardBrands = cardbrand,
                Card = card
            };

            //Retorna a CardEditViewModel para a view CardForm (novo cartão ou edição de cartão)
            return View("CardForm", viewModel);
        }

        // Ação de Apagar um cartão específico
        public ActionResult Delete (int cardId)
        {
            //Busca pelo cartão com Id igual ao passado como parâmetro
            var card = _unitOfWork.Cards.SingleOrDefault(c => c.Id == cardId);

            //Caso não encontre o cartão retorna Not Found
            if (card == null)
                return HttpNotFound();

            //Guarda Id do Clinte para posterior envio a view de Index(listagem de cartões)
            int clientId = card.ClientId;

            //Recupera todas as transações do cartão
            var transactions = _unitOfWork.Transactions.GetTransactionsFromCard(cardId);

            if (transactions.Count() > 0)
            {
                foreach (var t in transactions)
                {
                    //Busca pelos registros no Log de requisição de transação
                    var transactionLog = _unitOfWork.TransactionLogs.GetTransactionLogFromTransaction (t.Id);

                    //Remove os registros do Log de Requisições de Transação
                    _unitOfWork.TransactionLogs.Remove(transactionLog);
                }

                //Remove as transações do cartão
                _unitOfWork.Transactions.RemoveRange(transactions);
            }

            //Remove o cartão do banco de dados
            _unitOfWork.Cards.Remove(card);

            _unitOfWork.Complete();

            //Redireciona para a view Index(listagem de cartões) com o Id do cliente em questão
            return RedirectToAction("Index", new { clientId = clientId });

        }
    }
}