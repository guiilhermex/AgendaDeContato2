using AgendaContato.Domain.Models;
using AgendaContato.Services;
using AgendaContato.ViewModels.Usuario;
using AgendaContatos.Data;
using Blog.Extensions;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaContato.ViewModels;

namespace AgendaContato.Controllers
{
    [Route("Usuario")]
    public class UsuarioController : Controller
    {

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult LoginView()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet("Cadastro")]
        public IActionResult CadastroView()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ListarUsuarios([FromServices] AppDbContext context)
        {
            try
            {
                var usuarios = await context.Usuarios.AsNoTracking().ToListAsync();
                return Ok(new ResultViewModel<List<Usuario>>(usuarios));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Usuario>>("05X01 - falha interna no servidor"));
            }
        }
        [AllowAnonymous]
        [HttpPost("Cadastro")]
        public async Task<IActionResult> CriarUsuario(
        [FromBody] UsuarioCreateViewModel model,
        [FromServices] AppDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Usuario>(ModelState.GetErrors()));

            try
            {
                if (await context.Usuarios.AnyAsync(x => x.Email == model.Email))
                    return Conflict(new ResultViewModel<string>("E-mail já cadastrado"));

                var senhaHash = BCrypt.Net.BCrypt.HashPassword(model.Senha);

                var usuario = new Usuario
                {
                    Nome = model.Nome,
                    Email = model.Email,
                    SenhaHash = senhaHash,
                    Perfil = Domain.Enums.PerfilUsuario.Usuario,
                    Roles = new List<Usuario.UsuarioRole>
            {
                new Usuario.UsuarioRole { Slug = "Usuario" }
            }
                };

                await context.Usuarios.AddAsync(usuario);
                await context.SaveChangesAsync();

                return Created($"/v1/usuario/{usuario.IdUsuario}", new ResultViewModel<Usuario>(usuario));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Usuario>("05X02 - não foi possível incluir o usuário"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Usuario>("05X03 - falha interna no servidor"));
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(
        [FromBody] UsuarioLoginViewModel model,
        [FromServices] AppDbContext context,
        [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Usuario>(ModelState.GetErrors()));

            try
            {
                var usuario = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email == model.Email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Senha, usuario.SenhaHash))
                    return Unauthorized(new ResultViewModel<string>("usuário ou senha inválidos"));

                var token = tokenService.GenerateToken(usuario);
                return Ok(new { token, nome = usuario.Nome });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>($"Erro interno: {ex.GetType().Name} - {ex.Message}"));
            }
        }

        [AllowAnonymous]
        [HttpPost("EsqueciSenha")]
        public async Task<IActionResult> EsqueciSenha(
            [FromBody] UsuarioEsqueciSenhaViewModel model,
            [FromServices] AppDbContext context)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(new ResultViewModel<string>("E-mail inválido"));

            var usuario = await context.Usuarios.FirstOrDefaultAsync(x => x.Email == model.Email);

            if (usuario == null)
                return Ok(new ResultViewModel<string>("Se o e-mail existir, enviaremos instruções"));

            usuario.ResetPasswordToken = Guid.NewGuid().ToString("N");
            usuario.ResetPasswordTokenExpiration = DateTime.UtcNow.AddMinutes(30);

            context.Usuarios.Update(usuario);
            await context.SaveChangesAsync();

            //troca isso por envio de e-mail
            return Ok(new
            {
                message = "Token gerado com sucesso",
                token = usuario.ResetPasswordToken
            });
        }
        [AllowAnonymous]
        [HttpPost("ResetarSenha")]
        public async Task<IActionResult> ResetarSenha(
            [FromBody] UsuarioResetSenhaViewModel model,
            [FromServices] AppDbContext context)
        {
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Token) ||
                string.IsNullOrWhiteSpace(model.NovaSenha))
            {
                return BadRequest(new ResultViewModel<string>("Dados inválidos"));
            }

            var usuario = await context.Usuarios.FirstOrDefaultAsync(x =>
                x.Email == model.Email &&
                x.ResetPasswordToken == model.Token &&
                x.ResetPasswordTokenExpiration > DateTime.UtcNow);

            if (usuario == null)
                return BadRequest(new ResultViewModel<string>("Token inválido ou expirado"));

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.NovaSenha);
            usuario.ResetPasswordToken = null;
            usuario.ResetPasswordTokenExpiration = null;

            context.Usuarios.Update(usuario);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<string>("Senha redefinida com sucesso"));
        }

    }
}
