using AgendaContato.Domain.Models;
using AgendaContato.ViewModels.Grupo;
using AgendaContatos.Data;
using Blog.Extensions;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaContato.Controllers
{
    [Authorize]
    [Route("Grupo")]
    public class GrupoController : ControllerBase
    {
        private int? ObterUsuarioLogadoId()
        {
            var idClaim = User.FindFirst("IdUsuario")?.Value;
            return int.TryParse(idClaim, out var idUsuario) ? idUsuario : null;
        }

        /* ===============================
           LISTAR GRUPOS
        ================================ */
        [HttpGet]
        public async Task<IActionResult> ListarGrupos(
            [FromServices] AppDbContext context,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? nome = null)
        {
            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized(new ResultViewModel<string>("Usuário não identificado"));

            var query = context.Grupos
                .AsNoTracking()
                .Where(x => x.UsuarioId == userId);

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(x => x.NomeGrupo.Contains(nome));

            var total = await query.CountAsync();

            var grupos = await query
                .OrderBy(x => x.NomeGrupo)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new
                {
                    g.IdGrupo,
                    g.NomeGrupo
                })
                .ToListAsync();

            return Ok(new ResultViewModel<object>(new
            {
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = grupos
            }));
        }

        /* ===============================
           OBTER GRUPO POR ID
        ================================ */
        [HttpGet("Listar/{id:int}")]
        public async Task<IActionResult> ListarGrupoId(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var grupo = await context.Grupos
                .AsNoTracking()
                .Where(x => x.IdGrupo == id && x.UsuarioId == userId)
                .Select(g => new
                {
                    g.IdGrupo,
                    g.NomeGrupo
                })
                .FirstOrDefaultAsync();

            if (grupo == null)
                return NotFound(new ResultViewModel<string>("Grupo não encontrado"));

            return Ok(new ResultViewModel<object>(grupo));
        }

        /* ===============================
           CRIAR GRUPO
        ================================ */
        [HttpPost("Criar")]
        public async Task<IActionResult> CriarGrupo(
            [FromBody] EditorGrupoViewModel model,
            [FromServices] AppDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var nomeNormalizado = model.NomeGrupo.Trim();

            var existe = await context.Grupos.AnyAsync(x =>
                x.UsuarioId == userId &&
                x.NomeGrupo == nomeNormalizado);

            if (existe)
                return BadRequest(new ResultViewModel<string>("Já existe um grupo com esse nome"));

            var grupo = new Grupo
            {
                NomeGrupo = nomeNormalizado,
                UsuarioId = userId.Value
            };

            context.Grupos.Add(grupo);
            await context.SaveChangesAsync();

            return Created("", new ResultViewModel<object>(new
            {
                grupo.IdGrupo,
                grupo.NomeGrupo
            }));
        }

        /* ===============================
           EDITAR GRUPO
        ================================ */
        [HttpPut("Editar/{id:int}")]
        public async Task<IActionResult> EditarGrupo(
            [FromRoute] int id,
            [FromBody] EditorGrupoViewModel model,
            [FromServices] AppDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var grupo = await context.Grupos
                .FirstOrDefaultAsync(x =>
                    x.IdGrupo == id &&
                    x.UsuarioId == userId);

            if (grupo == null)
                return NotFound(new ResultViewModel<string>("Grupo não encontrado"));

            var novoNome = model.NomeGrupo.Trim();

            var duplicado = await context.Grupos.AnyAsync(x =>
                x.UsuarioId == userId &&
                x.NomeGrupo == novoNome &&
                x.IdGrupo != id);

            if (duplicado)
                return BadRequest(new ResultViewModel<string>("Já existe outro grupo com esse nome"));

            grupo.NomeGrupo = novoNome;
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<object>(new
            {
                grupo.IdGrupo,
                grupo.NomeGrupo
            }));
        }

        /* ===============================
           EXCLUIR GRUPO (CORRETO)
        ================================ */
        [HttpDelete("Deletar/{id:int}")]
        public async Task<IActionResult> ExcluirGrupo(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            var userId = ObterUsuarioLogadoId();
            if (userId is null)
                return Unauthorized();

            var grupo = await context.Grupos
                .FirstOrDefaultAsync(x =>
                    x.IdGrupo == id &&
                    x.UsuarioId == userId);

            if (grupo == null)
                return NotFound(new ResultViewModel<string>("Grupo não encontrado"));

            var vinculos = await context.GrupoContatos
                .Where(x => x.IdGrupo == id)
                .ToListAsync();

            if (vinculos.Any())
                context.GrupoContatos.RemoveRange(vinculos);

            context.Grupos.Remove(grupo);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<string>("Grupo excluído com sucesso"));
        }
    }
}
