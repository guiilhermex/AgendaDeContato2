using AgendaContato.Domain.Models;
using AgendaContato.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AgendaContato.Services
{
    public class TokenService
    {
        private readonly string _jwtKey;

        public TokenService(string jwtKey)
        {
            _jwtKey = jwtKey;
        }

        public string GenerateToken(Usuario usuario)
        {
            var claims = usuario.GetClaims().ToList();

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "AgendaContato",
                audience: "AgendaContato",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
