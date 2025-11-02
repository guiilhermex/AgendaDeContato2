using System.ComponentModel.DataAnnotations;

namespace AgendaContato.ViewModels.Grupo
{
    public class EditorGrupoViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Este campo deve conter entre 10 a 50 caracteres")]
        public string NomeGrupo { get; set; }
    }
}
