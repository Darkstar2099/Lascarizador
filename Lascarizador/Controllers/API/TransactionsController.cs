using Lascarizador.Core;
using Lascarizador.Core.Models;
using Lascarizador.Dtos;
using Lascarizador.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lascarizador.Controllers.API
{
    public class TransactionsController : ApiController
    {
        //___/ Controller API para os dados da transação \____________

        private readonly IUnitOfWork _unitOfWork;
        private readonly LascarizadorDbContext _context;

        public TransactionsController()
        {
            _context = new LascarizadorDbContext();
            _unitOfWork = new UnitOfWork(_context);
        }

        // GET /api/transactions
        [HttpGet]
        public IHttpActionResult GetTransactions()
        {
            //Pega todas as transações do banco de dados
            var transaction = _unitOfWork.Transactions.GetTransactionsWithAllRelations();

            //Cria uma List de TransactionDto
            List<TransactionDto> _transactionDto = new List<TransactionDto>();

            //Para cada transação encontrada no banco de dados...
            foreach (var t in transaction)
            {
                var cardBrand = _unitOfWork.CardBrands.Get(t.Card.CardBrand.Id);
                var transactionType = _unitOfWork.TransactionTypes.Get(t.TransactionType.Id);
                var firstDigits = t.Card.Number.Substring(1, 4);
                var lastDigits = t.Card.Number.Substring(t.Card.Number.Length - 4, 4);

                //Cria uma transactionDto com os dados da transação recuperada do BD
                var transactionDto = new TransactionDto
                {
                    //Converte o valor para Int32 sem virgula para evitar problemas com casas decimais
                    amount = Convert.ToInt32(t.Amount * 100),
                    card = new CardDto
                    {
                        card_brand = new CardBrandDto
                        {
                            id = t.Card.CardBrandId,
                            name = cardBrand.ApiName
                        },
                        client = new ClientDto
                        {
                            cpf = t.Card.Client.CPF,
                            email = t.Card.Client.Email,
                            name = t.Card.Client.Name,
                        },
                        expiration_month = t.Card.ExpirationDate.Month,
                        expiration_year = t.Card.ExpirationDate.Year,
                        first_digits = firstDigits,
                        last_digits = lastDigits,
                    },
                    creation_timestamp = t.CreationTimestamp,
                    id = t.Id,
                    installments = t.Installments,
                    transaction_log_id = t.TransactionLogId,
                    transaction_type = new TransactionTypeDto
                    {
                        id = t.TransactionTypeId,
                        type = t.TransactionType.ApiName
                    }
                };

                //Adiciona transação à Lista de Transações
                _transactionDto.Add(transactionDto);

            }
            //Retorna o IEnumerable<TransactionDto> preenchido com os dados da transação (Status=200)
            return Ok(_transactionDto);
        }

        // GET /api/transactions/{id}
        [HttpGet]
        public IHttpActionResult GetTransaction(int id)
        {
            //Pega a transação especificada pelo Id recebido como parâmetro
            var transaction = _unitOfWork.Transactions.GetTransactionWithAllRelations(id);

            if (transaction == null)
                return NotFound();

            var cardBrand = _unitOfWork.CardBrands.Get(transaction.Card.CardBrand.Id);
            var transactionType = _unitOfWork.TransactionTypes.Get(transaction.TransactionType.Id);
            var firstDigits = transaction.Card.Number.Substring(1, 4);
            var lastDigits = transaction.Card.Number.Substring(transaction.Card.Number.Length - 4, 4);

            //Cria uma transactionDto com os dados da transação recuperada do BD
            var transactionDto = new TransactionDto
            {
                //Converte o valor para Int32 sem virgula para evitar problemas com casas decimais
                amount = Convert.ToInt32(transaction.Amount * 100),
                //Cria um CardDto e vincula à transactionDto
                card = new CardDto
                {
                    //Cria um cardBrandDto vinculado ao cardDto
                    card_brand = new CardBrandDto
                    {
                        id = transaction.Card.CardBrand.Id,
                        name = cardBrand.ApiName
                    },

                    //Cria um clientDto vinculado ao cardDto
                    client = new ClientDto
                    {
                        cpf = transaction.Card.Client.CPF,
                        name = transaction.Card.Client.Name,
                        email = transaction.Card.Client.Email,
                    },

                    expiration_month = transaction.Card.ExpirationDate.Month,
                    expiration_year = transaction.Card.ExpirationDate.Year,
                    first_digits = firstDigits,
                    last_digits = lastDigits
                },

                creation_timestamp = transaction.CreationTimestamp,
                id = transaction.Id,
                installments = transaction.Installments,
                transaction_log_id = transaction.TransactionLogId,

                //Cria um transactionTypeDto vinculado à transactionDto
                transaction_type = new TransactionTypeDto
                {
                    id = transaction.TransactionTypeId,
                    type = transactionType.ApiName
                }
            };

            return Ok(transactionDto);

        }

        // POST /api/transactions
        [HttpPost]
        public IHttpActionResult CreateTransaction(TransactionApiInputDto transactionApiInputDto)
        {
            //Coloca valor do parâmetro show_errors numa var booleana para uso futuro
            var showErros = transactionApiInputDto.show_errors;

            //Cria lista de erros encontrados durante a validação
            List<Error> errors = new List<Error>();

            var transactionType = new TransactionType();
            var cardBrand = new CardBrand();
            var card = new Card();
            var clientInDB = new Client();

            decimal transAmountInDecimal = 0m;

            //A transação é presumida recusada inicialmente
            var statusCode = Constantes.scRecusada;
            var statusReason = Constantes.srErroInesperado;

            if (!ModelState.IsValid)
            {
                foreach (var e in ModelState.SelectMany(keyValuePair => keyValuePair.Value.Errors))
                    errors.Add(new Error(500, "Erro Inesperado (" + e.ErrorMessage + ")"));
                    //errors.Add(new Error(500, "Erro Inesperado."));
            }

            if (errors.Count == 0)
            {
                var validador = new Validador(_unitOfWork, showErros);
                validador.ProcessTransaction(transactionApiInputDto);

                statusCode = validador.statusCode;
                statusReason = validador.statusReason;

                transAmountInDecimal = validador.transAmountInDecimal;

                transactionType = validador.transactionType;
                cardBrand = validador.cardBrand;
                card = validador.card;
                clientInDB = validador.clientInDB;

                errors = validador.errors;
            };

            // *** FIM DA VALIDAÇÂO ***


            //___/ Cria Dto para saida da API \_____________________

            var transactionApiOutputDto = new TransactionApiOutputDto();

            transactionApiOutputDto.amount = transactionApiInputDto.amount;
            transactionApiOutputDto.card_brand = transactionApiInputDto.card_brand;
            transactionApiOutputDto.card_holder_name = transactionApiInputDto.card_holder_name;

            var cardNumberFirst = "";
            var cardNumberLast = "";
            if (!String.IsNullOrEmpty(transactionApiInputDto.card_number))
            {
                cardNumberFirst = transactionApiInputDto.card_number.Substring(1, 4);
                if (transactionApiInputDto.card_number.Length >= 4)
                    cardNumberLast = transactionApiInputDto.card_number.Substring(transactionApiInputDto.card_number.Length - 4, 4);
            }

            transactionApiOutputDto.card_number_first = cardNumberFirst;
            transactionApiOutputDto.card_number_last = cardNumberLast;

            transactionApiOutputDto.installments = transactionApiInputDto.installments;
            transactionApiOutputDto.transaction_type = transactionApiInputDto.transaction_type;
            transactionApiOutputDto.creation_timestamp = DateTime.Now;

            transactionApiOutputDto.status_code = statusCode;
            transactionApiOutputDto.status_reason = statusReason;

            //Gera registro da transação(transaction log) solicitada para ser salva na base de dados
            var transactionLog = new TransactionLog
            {
                Amount = transactionApiInputDto.amount,
                Card_brand = transactionApiInputDto.card_brand,
                Card_holder_name = transactionApiInputDto.card_holder_name,
                Card_number_first = cardNumberFirst,
                Card_number_last = cardNumberLast,
                Installments = transactionApiInputDto.installments,
                Transaction_type = transactionApiInputDto.transaction_type,
                Status_code = statusCode,
                Status_reason = statusReason,
                Creation_timestamp = DateTime.Now,
            };

            //Adiciona registro de TransactionLog a base de dados
            _unitOfWork.TransactionLogs.Add(transactionLog);

            //Caso algum erro tenha sido encontrado...
            if (errors.Count > 0)
            {

                if (showErros)
                {
                    //Passa todos os erros em errors para errorsDto
                    List<ErrorDto> errorsDto = new List<ErrorDto>();
                    if (errors.Count > 0)
                    {
                        foreach (var e in errors)
                        {
                            var errorDto = new ErrorDto(e.error_code, e.error_message);
                            errorsDto.Add(errorDto);
                            var errorLog = new ErrorLog
                            {
                                TransactionLogId = transactionLog.Id,
                                Error_code = e.error_code,
                                Error_message = e.error_message
                            };
                            _unitOfWork.ErrorLogs.Add(errorLog);
                        }
                    }

                    //Adiciona erros à TransactionApiOutputDto
                    transactionApiOutputDto.errors = errorsDto;
                }

                //Atualiza a base de dados
                _unitOfWork.Complete();

                //Adiciona registro de TransactionLog a Dto de saida
                transactionApiOutputDto.transaction_log_id = transactionLog.Id;

                //Retorna TransactionApiPoutputDto como JSon
                return Json(transactionApiOutputDto);
            }

        
            // Validação Terminada com Sucesso \__________________

            // Criar a transação vinculada ao cartão e coloca a data de criação
            var transaction = new Transaction {
                Amount = transAmountInDecimal,
                Card = card,
                TransactionType = transactionType,
                Installments = transactionApiInputDto.installments,
                CreationTimestamp = DateTime.Now,
                TransactionLogId = transactionLog.Id
            };

            //Adiciona a transação à base de dados
            _unitOfWork.Transactions.Add(transaction);

            //Atualiza o saldo do cliente com o valor da transação
            clientInDB.Saldo = clientInDB.Saldo - transAmountInDecimal;

            //Atualiza a base de dados
            _unitOfWork.Complete();

            //Adiciona registro de TransactionLog a Dto de saida
            transactionApiOutputDto.transaction_log_id = transactionLog.Id;

            //Atualiza a TransactionApiOutputDto com a Id da transação
            transactionApiOutputDto.transaction_id = transaction.Id;

            //Retorna TransactionApiOutputDto como Json e disponibiliza o link para a transação criada
            return Created(new Uri(Request.RequestUri + "/" + transaction.Id), transactionApiOutputDto);

        }


        //PUT /api/transaction/[id]
        //[HttpPut]
        //public IHttpActionResult UpdateTransaction(int id, TransactionDto transactionDto)
        //{
            //Retorna avisando que o método não foi implementado ou que não está disponível
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}

        //DELETE /api/transaction/[id]
        //[HttpDelete]
        //public IHttpActionResult DeleteTransaction()
        //public void DeleteTransaction()
        //{
        //    //Retorna avisando que o método não foi implementado ou que não está disponível
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}

    }
}
