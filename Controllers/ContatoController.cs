using AgendaContato.Domain.Models;
using AgendaContato.ViewModels.Contato;
using AgendaContatos.Data;
using Blog.Extensions;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaContato.Controllers
{
    [Authorize]
    [Route("Contato")]
    public class ContatoController : ControllerBase
    {
        private int? ObterUsuarioLogadoId()
        {
            var idClaim = User.FindFirst("IdUsuario")?.Value;
            return int.TryParse(idClaim, out var idUsuario) ? idUsuario : null;
        }

        /* ===============================
           LISTAR CONTATOS
        ================================ */
        [HttpGet]
        public async Task<IActionResult> GetContatos(
            [FromServices] AppDbContext context)
        {
            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var contatos = await context.Contatos
                .AsNoTracking()
                .Where(x => x.IdUsuario == userId)
                .OrderBy(x => x.NomeContato)
                .Select(x => new
                {
                    x.IdContato,
                    x.NomeContato,
                    x.Email,
                    x.Telefone,
                    Grupos = x.GrupoContatos
                        .Select(gc => gc.Grupo.NomeGrupo)
                        .ToList()
                })
                .ToListAsync();

            return Ok(new ResultViewModel<object>(contatos));
        }

        /* ===============================
           OBTER CONTATO POR ID
        ================================ */
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetContatoPorId(
            int id,
            [FromServices] AppDbContext context)
        {
            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var contato = await context.Contatos
                .AsNoTracking()
                .Where(x => x.IdContato == id && x.IdUsuario == userId)
                .Select(x => new
                {
                    x.IdContato,
                    x.NomeContato,
                    x.Email,
                    x.Telefone,
                    Grupos = x.GrupoContatos
                        .Select(gc => gc.Grupo.NomeGrupo)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (contato == null)
                return NotFound(new ResultViewModel<string>("Contato não encontrado"));

            return Ok(new ResultViewModel<object>(contato));
        }

        /* ===============================
           CRIAR CONTATO
        ================================ */
        [HttpPost("Criar")]
        public async Task<IActionResult> CriarContato(
            [FromBody] EditorContatoViewModel model,
            [FromServices] AppDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var contato = new Contato
            {
                NomeContato = model.NomeContato.Trim(),
                Telefone = model.Telefone,
                Email = model.Email,
                IdUsuario = userId.Value
            };

            context.Contatos.Add(contato);
            await context.SaveChangesAsync();

            await VincularGruposAsync(
                contato.IdContato,
                model.Grupos,
                userId.Value,
                context
            );

            return Ok(new ResultViewModel<string>("Contato criado com sucesso"));
        }

        /* ===============================
           EDITAR CONTATO
        ================================ */
        [HttpPut("Editar/{id:int}")]
        public async Task<IActionResult> AlterarContato(
            int id,
            [FromBody] EditorContatoViewModel model,
            [FromServices] AppDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var contato = await context.Contatos
                .Include(x => x.GrupoContatos)
                .FirstOrDefaultAsync(x =>
                    x.IdContato == id &&
                    x.IdUsuario == userId);

            if (contato == null)
                return NotFound(new ResultViewModel<string>("Contato não encontrado"));

            contato.NomeContato = model.NomeContato.Trim();
            contato.Telefone = model.Telefone;
            contato.Email = model.Email;

            // Remove somente os vínculos
            context.GrupoContatos.RemoveRange(contato.GrupoContatos);
            await context.SaveChangesAsync();

            await VincularGruposAsync(
                contato.IdContato,
                model.Grupos,
                userId.Value,
                context
            );

            return Ok(new ResultViewModel<string>("Contato atualizado com sucesso"));
        }

        /* ===============================
           EXCLUIR CONTATO
        ================================ */
        [HttpDelete("Deletar/{id:int}")]
        public async Task<IActionResult> DeletarContato(
            int id,
            [FromServices] AppDbContext context)
        {
            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var contato = await context.Contatos
                .Include(x => x.GrupoContatos)
                .FirstOrDefaultAsync(x =>
                    x.IdContato == id &&
                    x.IdUsuario == userId);

            if (contato == null)
                return NotFound(new ResultViewModel<string>("Contato não encontrado"));

            context.GrupoContatos.RemoveRange(contato.GrupoContatos);
            context.Contatos.Remove(contato);

            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<string>("Contato excluído com sucesso"));
        }

        /* ===============================
           MÉTODO PRIVADO — VINCULAR GRUPOS
        ================================ */
        private async Task VincularGruposAsync(
            int idContato,
            List<string>? grupos,
            int userId,
            AppDbContext context)
        {
            if (grupos == null || !grupos.Any())
                return;

            foreach (var nomeGrupo in grupos
                .Select(g => g.Trim())
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct())
            {
                var grupo = await context.Grupos
                    .FirstOrDefaultAsync(g =>
                        g.UsuarioId == userId &&
                        g.NomeGrupo == nomeGrupo);

                if (grupo == null)
                {
                    grupo = new Grupo
                    {
                        NomeGrupo = nomeGrupo,
                        UsuarioId = userId
                    };

                    context.Grupos.Add(grupo);
                    await context.SaveChangesAsync();
                }

                context.GrupoContatos.Add(new GrupoContato
                {
                    IdContato = idContato,
                    IdGrupo = grupo.IdGrupo
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
