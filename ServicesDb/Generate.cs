using LoggAutorz.Repositorie;
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
        public string? Generate(RegisterUserDTO? user)
        {
            if (user == null)
                return null;

            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Password))
                return null;
            var handler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(_privateKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>{
            new Claim(ClaimTypes.Name, user.Name), 
            new Claim(ClaimTypes.Role, "Employee"),
            new Claim(ClaimTypes.Role, "AdminMax"), // Esta é a linha mágica
            new Claim(ClaimTypes.Role, "Admin"),    // Você pode adicionar múltiplas roles
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.Add(new Claim(ClaimTypes.Role, "AdminMax"));
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
