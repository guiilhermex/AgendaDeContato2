using System.ComponentModel.DataAnnotations;

namespace AgendaContato.ViewModels.Usuario
{
    public class UsuarioEsqueceuSenhaViewModel
    {
        [Required(ErrorMessage = "O E-mail é obrigatório")]
        public string Email { get; set; }
    }
}
