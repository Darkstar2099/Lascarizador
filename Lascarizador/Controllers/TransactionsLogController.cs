using Lascarizador.Core;
using Lascarizador.Core.Models;
using Lascarizador.Persistence;
using Lascarizador.ViewModels;
using System.Web.Mvc;

namespace Lascarizador.Controllers
{
    //___/ Controller API para os dados do log de requisição de transação \____________

    public class TransactionsLogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LascarizadorDbContext _context;

        public TransactionsLogController()
        {
            _context = new LascarizadorDbContext();
            _unitOfWork = new UnitOfWork(_context);
        }

        // Ação de listar todo o Log de Transações
        //[HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        // Ação de detalhes de um cliente específico
        public ActionResult Detail(int transactionLogId)
        {
            var transactionLogViewModel = new TransactionLogViewModel();
            //Buscar pelo registro no log de transações cujo Id tenha sido fornecido como parâmetro
            var transactionLog = _unitOfWork.TransactionLogs.SingleOrDefault(t => t.Id == transactionLogId);

            //Caso não encontre o cliente específico retorna Not Found
            if (transactionLog == null)
                return HttpNotFound();

            transactionLogViewModel.TransactionLog = transactionLog;

            var errorLog = _unitOfWork.ErrorLogs.GetAllErrorsFromTransactionLog(transactionLogId);
            transactionLogViewModel.ErrorsLog = errorLog;
            
            //Retorna o cliente específico para a view Details(detalhes do cliente)
            return View(transactionLogViewModel);
        }

    }
}