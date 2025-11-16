using AgendaContato.Domain.Models;
using AgendaContato.ViewModels.Grupo;
using AgendaContatos.Data;
using Blog.Extensions;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AgendaContato.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[Controller]")]
    public class GrupoController : ControllerBase
    {
        private int? ObterUsuarioLogadoId()
        {
            var idClaim = User.FindFirst("IdUsuario")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return null;

            if (int.TryParse(idClaim, out var idUsuario))
                return idUsuario;

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> ListarGrupos(
            [FromServices] AppDbContext context,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = ObterUsuarioLogadoId();
                if (userId is null)
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var total = await context.Grupos
                    .Where(x => x.UsuarioId == userId)
                    .CountAsync();

                var grupos = await context.Grupos
                    .AsNoTracking()
                    .Where(x => x.UsuarioId == userId)
                    .OrderBy(x => x.NomeGrupo)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new
                {
                    TotalCount = total,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = grupos
                };

                return Ok(new ResultViewModel<object>(result));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Grupo>>("05X01 - falha interna no servidor"));
            }
        }

        [HttpGet("Listar/{id:int}")]
        public async Task<IActionResult> ListarGrupoId(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            try
            {
                var userId = ObterUsuarioLogadoId();
                if (userId is null)
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var grupo = await context.Grupos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdGrupo == id && x.UsuarioId == userId);

                if (grupo == null)
                    return NotFound(new ResultViewModel<Grupo>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Grupo>(grupo));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Grupo>("05X02 - falha interna no servidor"));
            }
        }

        [HttpPost("Criar")]
        public async Task<IActionResult> CriarGrupos(
            [FromBody] EditorGrupoViewModel model,
            [FromServices] AppDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Grupo>(ModelState.GetErrors()));

            try
            {
                var userId = ObterUsuarioLogadoId();
                if (userId is null)
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var grupo = new Grupo
                {
                    NomeGrupo = model.NomeGrupo,
                    UsuarioId = userId.Value
                };

                await context.Grupos.AddAsync(grupo);
                await context.SaveChangesAsync();

                return Created($"/v1/Grupo/{grupo.IdGrupo}", new ResultViewModel<Grupo>(grupo));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Grupo>("05X03 - não foi possível criar o grupo"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Grupo>("05X04 - falha interna no servidor"));
            }
        }

        [HttpPut("Editar/{id:int}")]
        public async Task<IActionResult> EditarGrupos(
            [FromRoute] int id,
            [FromBody] EditorGrupoViewModel model,
            [FromServices] AppDbContext context)
        {
            try
            {
                var userId = ObterUsuarioLogadoId();
                if (userId is null)
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var grupo = await context.Grupos
                    .FirstOrDefaultAsync(x => x.IdGrupo == id && x.UsuarioId == userId);

                if (grupo == null)
                    return NotFound(new ResultViewModel<Grupo>("Conteúdo não encontrado"));

                grupo.NomeGrupo = model.NomeGrupo;

                context.Grupos.Update(grupo);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Grupo>(grupo));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Grupo>("05X05 - não foi possível alterar o grupo"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Grupo>("05X06 - falha interna no servidor"));
            }
        }

        [HttpDelete("Deletar/{id:int}")]
        public async Task<IActionResult> ExcluirGrupos(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            try
            {
                var userId = ObterUsuarioLogadoId();
                if (userId is null)
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var grupo = await context.Grupos
                    .FirstOrDefaultAsync(x => x.IdGrupo == id && x.UsuarioId == userId);

                if (grupo == null)
                    return NotFound(new ResultViewModel<Grupo>("Conteúdo não encontrado"));

                context.Grupos.Remove(grupo);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Grupo>(grupo));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Grupo>("05X07 - não foi possível excluir grupo"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Grupo>("05X08 - falha interna no servidor"));
            }
        }
    }
}
