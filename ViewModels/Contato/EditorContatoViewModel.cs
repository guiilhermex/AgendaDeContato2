using System.ComponentModel.DataAnnotations;

namespace AgendaContato.ViewModels.Contato
{
    public class EditorContatoViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Este campo deve conter entre 2 a 100 caracteres")]
        public string NomeContato { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [Phone(ErrorMessage = "Número de telefone inválido")]
        public string Telefone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "Este campo deve conter entre 5 a 80 caracteres")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O id é obrigatório")]
        public int IdUsuario { get; set; } 
    }
}
