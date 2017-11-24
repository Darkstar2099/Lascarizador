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
                ModelState.AddModelError("", "408 - O valor informado não é válido.");

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
                TransactionTypeId = transactionType.Id,
                CreationTimestamp = DateTime.Now,
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
/*

            //Validando TransactionType (Tipo da Transação)
            //var transTypeFound = _unitOfWork.TransactionTypes.SingleOrDefault(t => t.Id == transEditView.TransactionTypeId);
            //Como já pegamos as TransactionTypes vamos procurar o tipo de transação recebido na TransactionEditViewModel
            bool transTypeFound = false;
            foreach (var type in transactionTypes)
                if (transEditView.TransactionTypeId == type.Id)
                    transTypeFound = true;
            //Se não encontrar a bandeira de cartão...
            if (!transTypeFound)
                //Gera um erro no ModelState
                ModelState.AddModelError("", "501 - Tipo de Transação não encontrada.");

            //Validando CardBrand(Bandeira)
            //var cardBrandFound = _unitOfWork.CardBrands.SingleOrDefault(t => t.Id == transEditView.CardBrandId);
            //Como já pegamos as CardBrands vamos procurar a bandeira de cartão recebida na TransactionEditViewModel
            bool cardBrandFound = false;
            foreach (var brand in cardBrands)
                if (transEditView.CardBrandId == brand.Id)
                    cardBrandFound = true;
            //Se não encontrar a bandeira de cartão...
            if (!cardBrandFound)
                //Gera um erro no ModelState
                ModelState.AddModelError("", "502 - Bandeira de Cartão não encontrada.");

            //Valida Número do Cartão
            if (String.IsNullOrEmpty(transEditView.Number))
            {
                //Gera um erro no ModelState
                ModelState.AddModelError("", "503 - Número do Cartão vazio ou não informado.");
            }
            else
            {
                //Verifica se o tamanho do número está incorreto (Número deve ter entre 12 e 16 dígitos)           
                if (transEditView.Number.Length < 12 || transEditView.Number.Length > 16)
                {
                    //Gera um erro no ModelState
                    ModelState.AddModelError("", "504 - Número do Cartão deve ter de 12 a 16 números.");
                }
                else
                {
                    //Verifica se todos os caracteres de string Número são números           
                    if (!transEditView.Number.All(char.IsDigit))
                        //Gera um erro no ModelState
                        ModelState.AddModelError("", "505 - Número do Cartão deve conter somente números.");
                }

            }

            //Se algum erro foi gerado até aqui...
            if (!ModelState.IsValid)
                //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
                return View("TransactionForm", transEditView);


            //Procura pelo cartão com a Bandeira informada e o Número
            var card = _unitOfWork.Cards.SingleOrDefault(c => c.CardBrandId == transEditView.CardBrandId && c.Number == transEditView.Number);

            //Se não encontrar o cartão...
            if (card == null)
                //Gera um erro no ModelState
                ModelState.AddModelError("", "506 - Não foi encontrado o cartão informado");

            //Se algum erro foi gerado até aqui...
            if (!ModelState.IsValid)
                //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
                return View("TransactionForm", transEditView);


            // Validando Parâmetros Primários do Cartão \____________________

            //Validando CardHolderName
            if (transEditView.CardHolderName != card.CardHolderName)
                //Gera um erro no ModelState
                ModelState.AddModelError("", "507 - Nome do Proprietário do Cartão não está correto.");

            //Validando CVV
            if (int.Parse(transEditView.Cvv) != card.Cvv)
                //Gera um erro no ModelState
                ModelState.AddModelError("", "508 - O campo cvv não está correto.");

            //Validando Mês e Ano de Expiração do cartão
            if (transEditView.ExpirationMonth != card.ExpirationDate.Month || transEditView.ExpirationYear != card.ExpirationDate.Year)
                //Gera um erro no ModelState
                ModelState.AddModelError("", "509 - Mês e Ano de Expiração estão incorretos.");

            //Se o cartão possui senha...
            if (card.HasPassword) //Se o cartão possui senha então...
            {
                // Verifica se a senha não foi informada
                if (String.IsNullOrEmpty(transEditView.Password))
                {
                    //Gera um erro no ModelState
                    ModelState.AddModelError("", "510 - A Senha é requerida para esse tipo de cartão e não foi informada.");
                }
                else //Se uma senha foi informada...
                {
                    //Verifica se o tamanho da senha está incorreto (Senha deve ter entre 4 e 6 dígitos)           
                    if (transEditView.Password.Length < 4 || transEditView.Password.Length > 6)
                    {
                        //Gera um erro no ModelState
                        ModelState.AddModelError("", "511 - A Senha deve ter no mínimo 4 e no máximo 6 caracteres.");
                    }
                    else //Se o tamanho da senha estiver correto...
                    {
                        //Verificar se a senha está correta.
                        var securedPassword = new SecuredPassword(card.HashPassword, card.SaltPassword);
                        if (!securedPassword.Verify(transEditView.Password))
                            //Gera um erro no ModelState
                            ModelState.AddModelError("", "512 - Senha Inválida.");
                    }
                }

            };

            //Se algum erro foi gerado até aqui...
            if (!ModelState.IsValid)
                //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
                return View("TransactionForm", transEditView);


            // Validando Parâmetros Secundários do Cartão \____________________

            //Validando se o cartão já está expirado
            var now = DateTime.Now;
            if (card.ExpirationDate < now)
            //if (((transEditView.ExpirationYear * 100) + transEditView.ExpirationMonth) < ((now.Year * 100) + now.Month))
                //Gera um erro no ModelState
                ModelState.AddModelError("", "513 - Cartão informado está expirado.");

            //Validar se o Amount só tem números
            //???

            //Valor Inválido (Mínimo de 10 centavos)
            decimal transAmountDecimal = Convert.ToDecimal(transEditView.Amount);
            decimal minAmount = 0.10m;
            if (transAmountDecimal < minAmount)
                ModelState.AddModelError("", "514 - Valor da transação deve ser maior ou igual a 10 centavos.");

            //Se algum erro foi gerado até aqui...
            if (!ModelState.IsValid)
                //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
                return View("TransactionForm", transEditView);


            // Validando Parâmetros do Cliente \__________________

            //Achando o Cliente do cartão.
            var clientInDB = _unitOfWork.Clients.SingleOrDefault(c => c.Id == card.ClientId);

            //Se não encontrar o cliente do cartão...
            if (clientInDB == null)
                //Gera um erro no ModelState
                ModelState.AddModelError("", "600 - Cliente não encontrado para esse cartão.");

            //Validando Saldo Insuficiente
            // Se o Limite de Crédito do cliente menos o saldo altual dele forem menores que o valor da transação
            // então gera erro.
            decimal creditLimit = clientInDB.CreditLimit;
            decimal saldo = clientInDB.Saldo;
            //Se o saldo não for suficiente...
            if ((creditLimit - saldo) < Convert.ToDecimal(transEditView.Amount))
                //Gera um erro no ModelState
                ModelState.AddModelError("", "601 - Saldo Insuficiente para realizar a transação.");

            //Validando Cartão Bloqueado
            if (card.IsBlocked)
                //Gera um erro no ModelState
                ModelState.AddModelError("", "602 - Esse Cartão se encontra bloqueado.");

            //Se algum erro foi gerado até aqui...
            if (!ModelState.IsValid)
                //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
                return View("TransactionForm", transEditView);
*/
/*

var transactionType = new TransactionType();
var cardBrand = new CardBrand();
var card = new Card();
string transactionTypeApiName = "";
bool cardHasPassword = false;
bool passwordLengthOk = false;
decimal transAmountInDecimal = 0m;
var clientInDB = new Client();

//var transactioTypeIsNull = String.IsNullOrEmpty(transEditView.TransactionTypes);
//var cardBrandIsNull = String.IsNullOrEmpty(transactionApiInputDto.card_brand);
var cardHolderNameIsNull = String.IsNullOrEmpty(transEditView.CardHolderName);
var cardNumberIsNull = String.IsNullOrEmpty(transEditView.Number);
var cvvIsNull = String.IsNullOrEmpty(transEditView.Cvv);
var amountIsNull = String.IsNullOrEmpty(transEditView.Amount);

var passwordIsNull = String.IsNullOrEmpty(transEditView.Password);

//if (transactioTypeIsNull)
//    errors.Add(new ErrorDto("", "301 - O tipo de transação não foi informado."));
//if (cardBrandIsNull)
//    errors.Add(new ErrorDto("", "302 - A bandeira do cartão não foi informada."));
if (cardHolderNameIsNull)
    ModelState.AddModelError("", "303 - O nome do portador do cartão não foi informado.");
if (cardNumberIsNull)
    ModelState.AddModelError("", "304 - O número do cartão não foi informado.");
if (cvvIsNull)
    ModelState.AddModelError("", "305 - O cvv do cartão não foi informado.");
if (amountIsNull)
    ModelState.AddModelError("", "306 - O valor não foi informado.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);


//___/ Verifica campos inválidos - Erros 400 \_____________________

//Validando TransactionType (Tipo da Transação)
//var transTypeFound = _unitOfWork.TransactionTypes.SingleOrDefault(t => t.Id == transEditView.TransactionTypeId);
//Como já pegamos as TransactionTypes vamos procurar o tipo de transação recebido na TransactionEditViewModel
bool transTypeFound = false;
foreach (var type in transactionTypes)
    if (transEditView.TransactionTypeId == type.Id)
    {
        transTypeFound = true;
        transactionTypeApiName = type.ApiName;
    }
//Se não encontrar o tipo de transação...
if (!transTypeFound)
    //Gera um erro no ModelState
    ModelState.AddModelError("", "401 - Tipo de Transação não encontrada.");

//Validando CardBrand(Bandeira)
//var cardBrandFound = _unitOfWork.CardBrands.SingleOrDefault(t => t.Id == transEditView.CardBrandId);
//Como já pegamos as CardBrands vamos procurar a bandeira de cartão recebida na TransactionEditViewModel
bool cardBrandFound = false;
foreach (var brand in cardBrands)
    if (transEditView.CardBrandId == brand.Id)
        cardBrandFound = true;
//Se não encontrar a bandeira de cartão...
if (!cardBrandFound)
    //Gera um erro no ModelState
    ModelState.AddModelError("", "402 - Bandeira de Cartão não encontrada.");

var cardNumberIsDigit = transEditView.Number.All(char.IsDigit);
//Verifica se todos os caracteres do campo card_number são números           
if (!cardNumberIsDigit)
    ModelState.AddModelError("", "403 - O número do cartão informado não é válido.");

var cardNumberLength = transEditView.Number.Length;
//Verifica se o tamanho do campo card_number está correto           
if (cardNumberLength < 12 || cardNumberLength > 16)
    ModelState.AddModelError("", "404 - O número do cartão deve ter de 12 a 16 números.");

var cvvIsDigit = transEditView.Cvv.All(char.IsDigit);
//Verifica se todos os caracteres do campo cvv são números           
if (!cvvIsDigit)
    ModelState.AddModelError("", "405 - O cvv do cartão informado não é válido.");

//var tempAmount = transEditView.Amount.Replace(",", "");
//var amountIsDigit = tempAmount.All(char.IsDigit);
//Verifica se todos os caracteres do campo card_number são números           
//if (!amountIsDigit)
//    ModelState.AddModelError("", "407 - O valor informado não é válido.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

//Valor Inválido (Mínimo de 10 centavos)
decimal transAmountDecimal = Convert.ToDecimal(transEditView.Amount);
decimal minAmount = 0.10m;
if (transAmountDecimal < minAmount)
    ModelState.AddModelError("", "406 - O valor da transação deve ser maior ou igual a 10 centavos.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);


//___/ Validando parâmetros do cartão - Erros 500 \____________________

// Procura pelo cartão com a Bandeira informada e o Número
card = _unitOfWork.Cards.SingleOrDefault(c => c.CardBrand.Id == transEditView.CardBrandId && c.Number == transEditView.Number);
//Se não encontrar o cartão...
if (card == null)
    ModelState.AddModelError("", "501 - O cartão informado não foi encontrado.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

//Validando CardHolderName
if (transEditView.CardHolderName != card.CardHolderName)
    ModelState.AddModelError("", "502 - Nome do portador do cartão não está correto.");

//Validando CVV
if (int.Parse(transEditView.Cvv) != card.Cvv)
    ModelState.AddModelError("", "503 - O campo cvv não está correto.");

//Validando Mês e Ano de Expiração do cartão
if (transEditView.ExpirationMonth != card.ExpirationDate.Month || transEditView.ExpirationYear != card.ExpirationDate.Year)
    ModelState.AddModelError("", "504 - O mês e/ou ano de expiração não estão corretos.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

cardHasPassword = card.HasPassword;
// Se o cartão possui senha e a senha não foi informada
if (cardHasPassword && passwordIsNull)
    ModelState.AddModelError("", "505 - A senha é requerida para esse tipo de cartão e não foi informada.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

var passwordLength = transEditView.Password. Length;
passwordLengthOk = (passwordLength >= 4 && passwordLength <= 6);
// Se o cartão possui senha e uma senha foi informada, verifica se o tamanho da senha está incorreto
//  (Senha deve ter entre 4 e 6 dígitos)
if (cardHasPassword && !passwordIsNull && !passwordLengthOk)
    ModelState.AddModelError("", "506 - A senha deve ter no mínimo 4 e no máximo 6 caracteres.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

var securedPassword = new SecuredPassword(card.HashPassword, card.SaltPassword);
    // Verifica se a senha está correta
    if (card.HasPassword && !passwordIsNull && passwordLengthOk)
        if (!securedPassword.Verify(transEditView.Password))
            ModelState.AddModelError("", "507 - Senha inválida.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

// Se nenhum erro foi encontrado continua a validação 
var now = DateTime.Now;
//Validando se o cartão está expirado
if (card.ExpirationDate < now)
    ModelState.AddModelError("", "508 - Cartão informado está expirado.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

//Validando Cartão Bloqueado
if (card.IsBlocked)
        ModelState.AddModelError("", "509 - O cartão informado se encontra bloqueado.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);


//___/ Validando parâmetros do cliente - Erros 600 \__________________

//Achando o Cliente do cartão.
clientInDB = _unitOfWork.Clients.SingleOrDefault(c => c.Id == card.ClientId);
//Se não encontrar o cliente do cartão...
if (clientInDB == null)
    ModelState.AddModelError("", "600 - Cliente não encontrado para esse cartão.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

transAmountInDecimal = Convert.ToDecimal(transEditView.Amount);
decimal creditLimit = clientInDB.CreditLimit;
decimal saldo = clientInDB.Saldo;

//Validando Saldo Insuficiente
if ((creditLimit - saldo) < transAmountInDecimal)
// Se o Limite de Crédito do cliente menos o saldo atual dele
//   for menor que o valor da transação então gera erro.
    ModelState.AddModelError("", "601 - Saldo Insuficiente para realizar a transação.");

//Se algum erro foi gerado até aqui...
if (!ModelState.IsValid)
    //Retorna a TransactionEditViewModel para a view TransactionForm(nova transação)
    return View("TransactionForm", transEditView);

*/

