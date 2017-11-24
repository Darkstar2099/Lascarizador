using System;
using System.Collections.Generic;
using System.Linq;
using Lascarizador.Dtos;
using Lascarizador.Core.Models;
using Lascarizador.Persistence;

namespace Lascarizador.Core
{
    //Aqui juntaremos o processo de validação da do Controller da MVC e da API.

    public class Validador
    {
        // Classe com o método ProcessTransaction que valida se uma transação é válida e pode ser aceita.

        private bool bolShowErrors;
        private bool cardHasPassword = false;
        private bool passwordLengthOk = false;
        private IUnitOfWork _unitOfWork;

        //A transação é presumida recusada inicialmente       
        public string statusCode = Constantes.scRecusada;
        public string statusReason = Constantes.srErroInesperado;

        public decimal transAmountInDecimal = 0m;

        public List<Error> errors;
        public TransactionType transactionType = new TransactionType();
        public CardBrand cardBrand = new CardBrand();
        public Card card = new Card();
        public Client clientInDB = new Client();


        public Validador(IUnitOfWork unitOfWork, bool showErrors)
        {
            _unitOfWork = unitOfWork;
            bolShowErrors = showErrors;
            errors = new List<Error>();
        }

        public void ProcessTransaction(TransactionApiInputDto inputTransaction)
        {
            //___/ Verifica campos requeridos - Erros 300 \_____________________

            var transactioTypeIsNull = String.IsNullOrEmpty(inputTransaction.transaction_type);
            var cardBrandIsNull = String.IsNullOrEmpty(inputTransaction.card_brand);
            var cardHolderNameIsNull = String.IsNullOrEmpty(inputTransaction.card_holder_name);
            var cardNumberIsNull = String.IsNullOrEmpty(inputTransaction.card_number);
            var cvvIsNull = String.IsNullOrEmpty(inputTransaction.cvv);

            var passwordIsNull = String.IsNullOrEmpty(inputTransaction.password);

            statusReason = Constantes.srCampoRequerido;

            if (transactioTypeIsNull)
                errors.Add(new Error(301, "O tipo de transação não foi informado."));
            if (cardBrandIsNull)
                errors.Add(new Error(302, "A bandeira do cartão não foi informada."));
            if (cardHolderNameIsNull)
                errors.Add(new Error(303, "O nome do portador do cartão não foi informado."));
            if (cardNumberIsNull)
                errors.Add(new Error(304, "O número do cartão não foi informado."));
            if (cvvIsNull)
                errors.Add(new Error(305, "O cvv do cartão não foi informado."));


            //___/ Verifica campos inválidos - Erros 400 \_____________________

            // Se nenhum erro foi encontrado continua a validação
            if (errors.Count == 0)
            {
                statusReason = Constantes.srCampoInvalido;

                var transactionTypeApiName = inputTransaction.transaction_type;
                transactionType = _unitOfWork.TransactionTypes.SingleOrDefault(t => t.ApiName == transactionTypeApiName);
                // Validando TransactionType (Tipo da Transação)
                //Se não encontrar o tipo da transação...
                if (transactionType == null)
                    errors.Add(new Error(401, "O tipo de transação informado não é válido."));

                var cardBrandApiName = inputTransaction.card_brand;
                cardBrand = _unitOfWork.CardBrands.SingleOrDefault(t => t.ApiName == cardBrandApiName);
                // Validando CardBrand (bandeira do cartão)
                if (cardBrand == null)
                    errors.Add(new Error(402, "A bandeira de cartão informada não é válida."));

                var cardNumberIsDigit = inputTransaction.card_number.All(char.IsDigit);
                //Verifica se todos os caracteres do campo card_number são números           
                if (!cardNumberIsDigit)
                    errors.Add(new Error(403, "O número do cartão informado não é válido."));

                var cardNumberLength = inputTransaction.card_number.Length;
                //Verifica se o tamanho do campo card_number está correto           
                if (cardNumberLength < 12 || cardNumberLength > 16)
                    errors.Add(new Error(404, "O número do cartão informado deve ter de 12 a 16 números."));

                var cvvIsDigit = inputTransaction.cvv.All(char.IsDigit);
                //Verifica se todos os caracteres do campo card_number são números           
                if (!cvvIsDigit)
                    errors.Add(new Error(405, "O cvv do cartão informado não é válido."));

                var cvvLength = inputTransaction.cvv.Length;
                //Verifica se o tamanho do campo cvv está correto           
                if (cvvLength != 3)
                    errors.Add(new Error(406, "O cvv do cartão informado deve ter 3 números."));

            }

            // Se nenhum erro foi encontrado continua a validação 
            if (errors.Count == 0)
            {
                //Valor Inválido (Mínimo de 10 centavos)
                if (inputTransaction.amount < 10)
                {
                    statusReason = Constantes.srValorInvalido;
                    errors.Add(new Error(407, "O valor da transação deve ser maior ou igual a 10 centavos."));
                }
            }

            // Se nenhum erro foi encontrado continua a validação 
            if (errors.Count == 0)
            {
                // Validação do valor das parcelas
                if (transactionType.InstallmentsAvailable == true)
                {
                    // Parcelas fora do intervalo de [1,12]
                    if (inputTransaction.installments < 1 || inputTransaction.installments > 12)
                    {
                        statusReason = Constantes.srCampoInvalido;
                        errors.Add(new Error(408, "O campo número de parcelas deve ser um número entre 1 e 12."));
                    }
                }
                else
                {
                    // Parcela diferente de 0 para Transações que não são parceladas
                    if (inputTransaction.installments != 0)
                    {
                        statusReason = Constantes.srCampoInvalido;
                        errors.Add(new Error(409, "O campo número de parcelas deve ser 0 para transações que não podem ser parceladas."));
                    }
                }
            }

            // ***ATENÇÃO *** O Erro 410 - O valor informado não é válido., está sendo usando na classe TransactionController

            //___/ Validando parâmetros do cartão - Erros 500 \____________________

            // Os próximos erros geram status_reason = cartao_invalido

            // Se nenhum erro foi encontrado continua a validação
            if (errors.Count == 0)
            {
                // Procura pelo cartão com a Bandeira informada e o Número
                card = _unitOfWork.Cards.SingleOrDefault(c => c.CardBrand.Id == cardBrand.Id && c.Number == inputTransaction.card_number);
                //Se não encontrar o cartão...
                if (card == null)
                {
                    statusReason = Constantes.srCartaoInvalido;
                    errors.Add(new Error(501, "O cartão informado não foi encontrado."));
                }
            }

            // Se nenhum erro foi encontrado continua a validação 
            if (errors.Count == 0)
            {
                statusReason = Constantes.srCartaoInvalido;

                //Validando CardHolderName
                if (inputTransaction.card_holder_name != card.CardHolderName)
                    errors.Add(new Error(502, "Nome do portador do cartão não está correto."));

                //Validando CVV
                if (int.Parse(inputTransaction.cvv) != card.Cvv)
                    errors.Add(new Error(503, "O campo cvv não está correto."));

                //Validando Mês e Ano de Expiração do cartão
                if (inputTransaction.expiration_month != card.ExpirationDate.Month || inputTransaction.expiration_year != card.ExpirationDate.Year)
                    errors.Add(new Error(504, "O mês e/ou ano de expiração não estão corretos."));
            }

            // Se nenhum erro foi encontrado continua a validação
            if (errors.Count == 0)
            {
                cardHasPassword = card.HasPassword;

                // Se o cartão possui senha e a senha não foi informada
                if (cardHasPassword && passwordIsNull)
                {
                    statusReason = Constantes.srSenhaRequerida;
                    errors.Add(new Error(505, "A senha é requerida para esse tipo de cartão e não foi informada."));
                }
            }

            // Se nenhum erro foi encontrado continua a validação 
            if (errors.Count == 0)
            {
                var passwordLength = inputTransaction.password.Length;
                passwordLengthOk = (passwordLength >= 4 && passwordLength <= 6);

                // Se o cartão possui senha e uma senha foi informada, verifica se o tamanho da senha está incorreto
                //  (Senha deve ter entre 4 e 6 dígitos)
                if (cardHasPassword && !passwordIsNull && !passwordLengthOk)
                {
                    statusReason = Constantes.srErroTamanhoSenha;
                    errors.Add(new Error(506, "A senha deve ter no mínimo 4 e no máximo 6 caracteres."));
                }
            }

            // Se nenhum erro foi encontrado continua a validação 
            if (errors.Count == 0)
            {
                var securedPassword = new SecuredPassword(card.HashPassword, card.SaltPassword);
                // Verifica se a senha está correta
                if (card.HasPassword && !passwordIsNull && passwordLengthOk)
                    if (!securedPassword.Verify(inputTransaction.password))
                    {
                        statusReason = Constantes.srSenhaInvalida;
                        errors.Add(new Error(507, "Senha inválida."));
                    }
            }

            // Se nenhum erro foi encontrado continua a validação 
            var now = DateTime.Now;

            if (errors.Count == 0)
            {
                //Validando se o cartão está expirado
                if (card.ExpirationDate < now)
                {
                    statusReason = Constantes.srCartaoExpirado;
                    errors.Add(new Error(508, "Cartão informado está expirado."));
                }
            }

            // Se nenhum erro foi encontrado continua a validação 
            if (errors.Count == 0)
            {
                //Validando Cartão Bloqueado
                if (card.IsBlocked)
                {
                    statusReason = Constantes.srCartaoBloqueado;
                    errors.Add(new Error(509, "O cartão informado se encontra bloqueado."));
                }
            }


            //___/ Validando parâmetros do cliente - Erros 600 \__________________

            // Se nenhum erro foi encontrado continua a validação 
            if (errors.Count == 0)
            {
                //Achando o Cliente do cartão.
                clientInDB = _unitOfWork.Clients.SingleOrDefault(c => c.Id == card.ClientId);

                //Se não encontrar o cliente do cartão...
                if (clientInDB == null)
                {
                    statusReason = Constantes.srClienteNaoEncontrado;
                    errors.Add(new Error(600, "Cliente não encontrado para esse cartão."));
                }
            }

            // Se nenhum erro foi encontrado continua a validação
            if (errors.Count == 0)
            {
                transAmountInDecimal = Convert.ToDecimal(inputTransaction.amount / 100);
                decimal creditLimit = clientInDB.CreditLimit;
                decimal saldo = clientInDB.Saldo;

                //Validando Saldo Insuficiente
                if ((creditLimit + saldo) < transAmountInDecimal)
                {
                    // Se o Limite de Crédito do cliente menos o saldo atual dele
                    //   for menor que o valor da transação então gera erro.
                    statusReason = Constantes.srSaldoInsuficiente;
                    errors.Add(new Error(601, "Saldo Insuficiente para realizar a transação."));
                }
            }

            // *** FIM DAS VALIDAÇÕES ***
            // Se nenhum erro foi encontrado retorna transação validada com sucesso
            if (errors.Count == 0)
            {
                statusCode = Constantes.scAprovada;
                statusReason = Constantes.srSucesso;
            };

        }
    }
}