using System.Security.Claims;
using AgendaContato.Domain.Models;

namespace AgendaContato.Extensions
{
    public static class RoleClaimsExtensions
    {
        public static IEnumerable<Claim> GetClaims(this Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new(ClaimTypes.Name, usuario.Nome),
                new(ClaimTypes.Email, usuario.Email),
                new(ClaimTypes.Role, usuario.Perfil.ToString()),
            };

            if (usuario.Roles != null && usuario.Roles.Any())
            {
                claims.AddRange(
                    usuario.Roles.Select(r => new Claim(ClaimTypes.Role, r.Slug))
                );
            }

            return claims;
        }
    }
}
