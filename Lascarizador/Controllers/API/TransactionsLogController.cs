using Lascarizador.Core;
using Lascarizador.Dtos;
using Lascarizador.Persistence;
using System.Collections.Generic;
using System.Web.Http;

namespace Lascarizador.Controllers.API
{
    //___/ Controller API para os dados do Log de Requisição de Transações \____________

    public class TransactionsLogController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LascarizadorDbContext _context;

        public TransactionsLogController()
        {
            _context = new LascarizadorDbContext();
            _unitOfWork = new UnitOfWork(_context);
        }

        // GET /api/transactionslog
        [HttpGet]
        public IEnumerable<TransactionApiOutputDto> GetTransactionsLog()
        {
            var transactionLog = _unitOfWork.TransactionLogs.GetAllOrderByCreationTsDesc();

            List<TransactionApiOutputDto> _transactionsApiOutputDto = new List<TransactionApiOutputDto>();

            foreach (var l in transactionLog)
            {
                var transactionApiOutputDto = new TransactionApiOutputDto
                {
                    amount = l.Amount,
                    card_brand = l.Card_brand,
                    card_holder_name = l.Card_holder_name,
                    card_number_first = l.Card_number_first,
                    card_number_last = l.Card_number_last,
                    creation_timestamp = l.Creation_timestamp,
                    installments = l.Installments,
                    status_code = l.Status_code,
                    status_reason = l.Status_reason,
                    transaction_id = l.Id,
                    transaction_type = l.Transaction_type,
                };

                //List<ErrorDto> _errorsDto = new List<ErrorDto>();
                //foreach ( var e in l.Errors)
                //{
                //    var errorDto = new ErrorDto (e.error_code, e.error_message);
                //    _errorsDto.Add(errorDto);
                //}

                _transactionsApiOutputDto.Add(transactionApiOutputDto);

            }

            return (_transactionsApiOutputDto);
        }

        // GET /api/transactionslog/{transactionLogId}
        [HttpGet]
        public IEnumerable<TransactionApiOutputDto> GetTransactionsLog(int transactionLogId)
        {
            var transactionLog = _unitOfWork.TransactionLogs.Find(t => t.Id == transactionLogId);

            List<TransactionApiOutputDto> _transactionsApiOutputDto = new List<TransactionApiOutputDto>();

            foreach (var l in transactionLog)
            {
                var transactionApiOutputDto = new TransactionApiOutputDto
                {
                    amount = l.Amount,
                    card_brand = l.Card_brand,
                    card_holder_name = l.Card_holder_name,
                    card_number_first = l.Card_number_first,
                    card_number_last = l.Card_number_last,
                    creation_timestamp = l.Creation_timestamp,
                    installments = l.Installments,
                    status_code = l.Status_code,
                    status_reason = l.Status_reason,
                    transaction_id = l.Id,
                    transaction_type = l.Transaction_type,
                };

                //List<ErrorDto> _errorsDto = new List<ErrorDto>();
                //foreach ( var e in l.Errors)
                //{
                //    var errorDto = new ErrorDto (e.error_code, e.error_message);
                //    _errorsDto.Add(errorDto);
                //}

                _transactionsApiOutputDto.Add(transactionApiOutputDto);

            }

            return (_transactionsApiOutputDto);
        }

    }
}
