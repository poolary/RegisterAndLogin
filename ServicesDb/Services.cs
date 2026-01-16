using LoggAutorz.DataBase;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace LoggAutorz.ServicesDb
{
    public class ServicesToApiHttp
    {
        private readonly AppDbContext _appDbContext;

        public ServicesToApiHttp(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
    public class JwtToken
    {
        private readonly string _secret;
        public JwtToken(string secret)
        {
            _secret = secret;
        }
        public string GenerateToken(string userName, string[] roles)
        {
            var key = System.Text.Encoding.ASCII.GetBytes(_secret);
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, userName));
            foreach (var role in roles)
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}