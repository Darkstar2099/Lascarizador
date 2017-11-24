namespace Lascarizador.Core
{
    public class Constantes
    {
        // Constantes utilizadas para retorno na API

        // Constantes de Status Code (sc)
        public static string scAprovada = "aprovada";
        public static string scRecusada = "recusada";

        // Constantes de Status Reason (sr)
        public static string srErroInesperado = "erro_inesperado";
        public static string srCampoRequerido = "campo_requerido";
        public static string srCampoInvalido = "campo_inválido";
        public static string srValorInvalido = "valor_inválido";
        public static string srCartaoInvalido = "cartão_inválido";
        public static string srSenhaRequerida = "senha_requerida";
        public static string srErroTamanhoSenha = "erro_tamanho_senha";
        public static string srSenhaInvalida = "senha_inválida";
        public static string srCartaoExpirado = "cartão_expirado";
        public static string srCartaoBloqueado = "cartão_bloqueado";
        public static string srClienteNaoEncontrado = "cliente_não_encontrado";
        public static string srSaldoInsuficiente = "saldo_insuficiente";
        public static string srSucesso = "sucesso";

    }
}