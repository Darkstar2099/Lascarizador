using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lascarizador.Dtos
{

    public class ErrorDto
    {
        //Identificação numérica do erro
        public int error_code { get; set; }

        //Mensagem do erro
        public string error_message { get; set; }

        public ErrorDto(int code, string message)
        {
            this.error_code = code;
            this.error_message = message;
        }
    }


}
