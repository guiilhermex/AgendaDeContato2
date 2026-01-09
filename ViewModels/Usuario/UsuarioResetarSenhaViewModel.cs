using System.ComponentModel.DataAnnotations;

namespace AgendaContato.ViewModels.Usuario
{
    public class UsuarioResetarSenhaViewModel
    {
        public string Senha { get; set; }
        public string ConfirmarSenha { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
