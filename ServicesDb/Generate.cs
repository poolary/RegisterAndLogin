using LoggAutorz.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoggAutorz.ServicesDb
{
    public class GenerateService
    {
        private readonly string? _privateKey;

        public GenerateService(IConfiguration configuration) => _privateKey = configuration["JwtSettings:PrivateKey"]!;
        public string Generate(UsersEntity? user)
        {
            var handler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(_privateKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>{ };

            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            claims.Add(new Claim(ClaimTypes.Role, "Émployee"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(3),
            };

            var token = handler.CreateToken(tokenDescriptor);
            var stringToken = handler.WriteToken(token);

            return stringToken;
        }
    }
}
