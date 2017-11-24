using Lascarizador.Core;
using Lascarizador.Core.Models;
using Lascarizador.Persistence;
using Lascarizador.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Lascarizador.Dtos;


namespace Lascarizador.Controllers
{
    public class TransactionsController : Controller
    {
        //___/ Controller API para os dados da transação \____________

        private readonly IUnitOfWork _unitOfWork;
        private readonly LascarizadorDbContext _context;

        public TransactionsController()
        {
            _context = new LascarizadorDbContext();
            _unitOfWork = new UnitOfWork(_context);
        }

        // Ação de Nova Transação
        public ActionResult New()
        {
            //Pega todos os Transactiontypes(tipos de transações)
            var transactionType = _unitOfWork.TransactionTypes.GetAll();
            //Pega todos os CardBrands(bandeiras de cartões)
            var cardBrand = _unitOfWork.CardBrands.GetAll();
            
            //Cria nova instância vazia de TransactionEditViewModel com as listas de TransactionTypes e CardBrands
            var viewModel = new TransactionEditViewModel
            {
                TransactionTypes = transactionType,
                CardBrands = cardBrand
            };

            //Retorna a TransactionViewModel para a view TransactionForm(nova transação)
            return View("TransactionForm", viewModel);
        }

        // Ação de Nova Transação com página consumindo API
        public ActionResult NewApi()
        {
            //Pega todos os Transactiontypes(tipos de transações)
            var transactionType = _unitOfWork.TransactionTypes.GetAll();
            //Pega todos os CardBrands(bandeiras de cartões)
            var cardBrand = _unitOfWork.CardBrands.GetAll();

            //Cria nova instância vazia de TransactionEditViewModel com as listas de TransactionTypes e CardBrands
            var viewModel = new TransactionEditViewModel
            {
                TransactionTypes = transactionType,
                CardBrands = cardBrand
            };

            //Retorna a TransactionViewModel para a view TransactionForm(nova transação)
            return View("TransactionApiForm", viewModel);
        }


        // Ação de Listar Todas as Transações       
        public ActionResult Index(int cardId)
        {
            //Busca pelo cartão com Id igual ao passado como parâmetro
            var card = _unitOfWork.Cards.SingleOrDefault(c => c.Id == cardId);

            //Caso não encontre o cartão retorna Not Found
            if (card == null)
                return HttpNotFound("Cartão não encontrado");

            var cardBrand = _unitOfWork.CardBrands.SingleOrDefault(c => c.Id == card.CardBrandId);
            //Caso não encontre o cartão retorna Not Found
            if (cardBrand == null)
                return HttpNotFound("Bandeira não encontrada");
            
            //Vincula CardBrand ao cartão
            card.CardBrand = cardBrand;

            //Busca pelo cliente com Id encontrado no cartão
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.Id == card.ClientId);

            //Caso não encontre o cliente retorna Not Found
            if (client == null)
                return HttpNotFound("Cliente não encontrado");

            //Busca pelas transações com Id encontrado no cartão
            var transactions = _unitOfWork.Transactions.GetTransactionsFromCardWithCardAndTransactionType(card.Id);

            //Cria nova instância de TransactionViewModel vinculado ao cliente encontrado, com o cartão do cliente 
            //  e com as transações
            var viewModel = new TransactionViewModel
            {
                Client = client,
                Card = card,
                Transactions = transactions
            };

            //Retorna a TransactionViewModel para a view Index (listagem de transações)
            return View("Index", viewModel);
        }

        public ActionResult Save(TransactionEditViewModel transEditView)
        {
            //Coloca valor do parâmetro show_errors numa var booleana para uso futuro
            var showErros = true;

            //Cria lista de erros encontrados durante a validação
            List<Error> errors = new List<Error>();

            TransactionApiInputDto inputTransaction;
            var transactionType = new TransactionType();
            var cardBrand = new CardBrand();
            var card = new Card();
            var clientInDB = new Client();

            decimal transAmountInDecimal = 0m;

            //A transação é presumida recusada inicialmente
            var statusCode = Constantes.scRecusada;
            var statusReason = Constantes.srErroInesperado;

            //Pega todos os Transactiontypes(tipos de transações)
            var transactionTypes = _unitOfWork.TransactionTypes.GetAll();
            //Pega todos os CardBrands(bandeiras de cartões)
            var cardBrands = _unitOfWork.CardBrands.GetAll();
            //Atribuindo os TransactionTypes e os CardBrands a viewmodel caso tenhamos que voltar para a view
            transEditView.TransactionTypes = transactionTypes;
            transEditView.CardBrands = cardBrands;

            //Se algum erro foi gerado até aqui...
            if (!ModelState.IsValid)
                //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
                return View("TransactionForm", transEditView);

            //Esse teste é executado somente aqui e antes de chamar o validador (não é executado no validador)
            var tempAmount = transEditView.Amount.Replace(",", "");
            var amountIsDigit = tempAmount.All(char.IsDigit);
            //Verifica se todos os caracteres do campo card_number são números           
            if (!amountIsDigit)
                ModelState.AddModelError("", "410 - O valor informado não é válido.");

            if (!ModelState.IsValid)
                //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
                return View("TransactionForm", transEditView);

            var dblAmount = Convert.ToDouble(transEditView.Amount);
            var intAmount = Convert.ToInt32(dblAmount*100);
            //Move os dados da Edit View para a Dto.
            inputTransaction = new TransactionApiInputDto
            {
                amount = intAmount,
                card_brand = transEditView.CardBrandApiName,
                card_holder_name = transEditView.CardHolderName,
                card_number = transEditView.Number,
                cvv = transEditView.Cvv,
                expiration_month = transEditView.ExpirationMonth,
                expiration_year = transEditView.ExpirationYear,
                installments = transEditView.Installments,
                password = transEditView.Password,
                show_errors = showErros,
                transaction_type = transEditView.TransactionTypeApiName,                
            };

            var validador = new Validador(_unitOfWork, showErros);
            validador.ProcessTransaction(inputTransaction);

            statusCode = validador.statusCode;
            statusReason = validador.statusReason;

            transAmountInDecimal = validador.transAmountInDecimal;

            transactionType = validador.transactionType;
            cardBrand = validador.cardBrand;
            card = validador.card;
            clientInDB = validador.clientInDB;

            errors = validador.errors;
            
            // *** FIM DAS VALIDAÇÕES ***

            //Adiciona todos os error do validador ao ModelState
            if (errors.Count > 0)
                foreach (var e in errors)
                    ModelState.AddModelError("", e.error_code + " - " + e.error_message);

            if (!ModelState.IsValid)
                //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
                return View("TransactionForm", transEditView);


            // Validação Terminada com Sucesso \__________________

            var cardNumberFirst = "";
            var cardNumberLast = "";
            if (!String.IsNullOrEmpty(transEditView.Number))
            {
                cardNumberFirst = transEditView.Number.Substring(1, 4);
                if (transEditView.Number.Length >= 4)
                    cardNumberLast = transEditView.Number.Substring(transEditView.Number.Length - 4, 4);
            }

            //Gera registro da transação(transaction log) solicitada para ser salva na base de dados
            var transactionLog = new TransactionLog
            {
                Amount = Convert.ToInt32(transAmountInDecimal * 100),
                Card_brand = card.CardBrand.ApiName,
                Card_holder_name = transEditView.CardHolderName,
                Card_number_first = cardNumberFirst,
                Card_number_last = cardNumberLast,
                Installments = transEditView.Installments,
                Transaction_type = transEditView.TransactionTypeApiName,
                Status_code = statusCode,
                Status_reason = statusReason,
                Creation_timestamp = DateTime.Now,
                //Errors = null
            };

            //Adiciona registro no log de transações ao banco de dados
            _unitOfWork.TransactionLogs.Add(transactionLog);

            if (errors.Count > 0)
            {
                foreach (var e in errors)
                {
                    var errorLog = new ErrorLog
                    {
                        TransactionLogId = transactionLog.Id,
                        Error_code = e.error_code,
                        Error_message = e.error_message
                    };
                    _unitOfWork.ErrorLogs.Add(errorLog);
                }
            };

            //Criar a transação vinculada ao cartão e coloca a data de criação
            var transaction = new Transaction {
                Amount = transAmountInDecimal,
                CardId = card.Id,
                CreationTimestamp = DateTime.Now,
                Installments = transEditView.Installments,
                TransactionTypeId = transactionType.Id,                
                TransactionLogId = transactionLog.Id                
            };

            //Adiciona a transação ao banco de dados
            _unitOfWork.Transactions.Add(transaction);

            //Atualiza o saldo do cliente com o valor da transação.
            clientInDB.Saldo = clientInDB.Saldo - transaction.Amount;

            _unitOfWork.Complete();

            //Retorna a TransactionEditViewModel para a view Index(listagem de transações)
            return RedirectToAction("Index", new { cardId = transaction.CardId });
        }

    }
}