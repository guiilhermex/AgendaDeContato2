using AgendaContato.Domain.Models;
using AgendaContato.ViewModels.Contato;
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
    [Route("v1/[controller]")]
    public class ContatoController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetContatos(
            [FromServices] AppDbContext context,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Pega o Id do usuário autenticado a partir do token JWT
                var userIdClaim = User.FindFirst("IdUsuario")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var userId = int.Parse(userIdClaim);

                var total = await context.Contatos
                    .Where(x => x.IdUsuario == userId)
                    .CountAsync();

                var contatos = await context.Contatos
                    .AsNoTracking()
                    .Where(x => x.IdUsuario == userId)
                    .OrderBy(x => x.NomeContato)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new
                {
                    TotalCount = total,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = contatos
                };

                return Ok(new ResultViewModel<object>(result));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Contato>("05X01 - falha interna no servidor"));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetIdContato(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            try
            {
                var userIdClaim = User.FindFirst("IdUsuario")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var userId = int.Parse(userIdClaim);

                var contato = await context.Contatos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdContato == id && x.IdUsuario == userId);

                if (contato == null)
                    return NotFound(new ResultViewModel<Contato>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Contato>(contato));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Contato>("05X02 - falha interna no servidor"));
            }
        }

        [HttpPost("Criar")]
        public async Task<IActionResult> CriarContato(
            [FromBody] EditorContatoViewModel model,
            [FromServices] AppDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Contato>(ModelState.GetErrors()));

            try
            {
                var userIdClaim = User.FindFirst("IdUsuario")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var userId = int.Parse(userIdClaim);

                var contato = new Contato
                {
                    NomeContato = model.NomeContato,
                    Telefone = model.Telefone,
                    Email = model.Email,
                    IdUsuario = userId 
                };

                await context.Contatos.AddAsync(contato);
                await context.SaveChangesAsync();

                return Created($"/v1/Contato/{contato.IdContato}", new ResultViewModel<Contato>(contato));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Contato>("05X03 - Não foi possível incluir o contato"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Contato>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpPut("Editar/{id:int}")]
        public async Task<IActionResult> AlterarContato(
            [FromRoute] int id,
            [FromBody] EditorContatoViewModel model,
            [FromServices] AppDbContext context)
        {
            try
            {
                var userIdClaim = User.FindFirst("IdUsuario")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var userId = int.Parse(userIdClaim);

                var contato = await context.Contatos
                    .FirstOrDefaultAsync(x => x.IdContato == id && x.IdUsuario == userId);

                if (contato == null)
                    return NotFound(new ResultViewModel<Contato>("Conteúdo não encontrado"));

                contato.NomeContato = model.NomeContato;
                contato.Telefone = model.Telefone;
                contato.Email = model.Email;

                context.Contatos.Update(contato);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Contato>(contato));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Contato>("05X05 - não foi possível alterar o contato"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Contato>("05X06 - falha interna no servidor"));
            }
        }

        [HttpDelete("Deletar/{id:int}")]
        public async Task<IActionResult> DeletarContato(
            [FromRoute] int id,
            [FromServices] AppDbContext context)
        {
            try
            {
                var userIdClaim = User.FindFirst("IdUsuario")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new ResultViewModel<string>("Token inválido ou usuário não identificado"));

                var userId = int.Parse(userIdClaim);

                var contato = await context.Contatos
                    .FirstOrDefaultAsync(x => x.IdContato == id && x.IdUsuario == userId);

                if (contato == null)
                    return NotFound(new ResultViewModel<Contato>("Conteúdo não encontrado"));

                context.Contatos.Remove(contato);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Contato>(contato));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Contato>("05X07 - não foi possível excluir contato"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Contato>("05X08 - falha interna no servidor"));
            }
        }
    }
}
