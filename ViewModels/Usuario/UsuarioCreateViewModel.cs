using System.ComponentModel.DataAnnotations;

namespace AgendaContato.ViewModels.Usuario
{
    public class UsuarioCreateViewModel
    {
        [Required(ErrorMessage ="O Nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage ="O E-mail é obrigatório")]
        [EmailAddress(ErrorMessage ="E-mail inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage ="A senha é obrigatória"), MinLength(6)]
        public string Senha { get; set; }
    }
}
