using Microsoft.IdentityModel.Tokens;
using QuizEngineBE.DTO;
using QuizEngineBE.Interfaces;
using QuizEngineBE.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuizEngineBE.Services
{
    public class SecurityService(IConfiguration config) : ISecurityService
    {
        private readonly string _secretKey = config["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT secret key not found in configuration.");
        private readonly string _issuer = config["Jwt:Issuer"] ?? "QuizEngineBE";
        private readonly string _audience = config["Jwt:Audience"] ?? "QuizEngineClient";
        

        public string GenerateJwtToken(string username, string role="1", int expiryMinutes = 600)
        {
            //chiave simmetrica per firmare il Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //claims = informazioni contenute nel Token
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID univoco del Token
        };

           
            var token = new JwtSecurityToken(
                issuer: _issuer,         // chi emette il Token
                audience: _audience,   // chi riceverà il Token
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

           return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string EncryptSHA256xBase64(string word)
        {
            return Convert.ToBase64String(
                   System.Security.Cryptography.SHA256.HashData(
                   System.Text.Encoding.UTF8.GetBytes(word)));

        }
        
        public string GenerateSalt(int length = 16)
        {
            
            byte[] saltBytes = new byte[length];
            
            using var randomNumberGen = RandomNumberGenerator.Create();
            
            randomNumberGen.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes); 
        }

        public (bool Success, string Message) ValidateJwtTokenForUser(string username,string token)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(username))
                return (false, "Token o username non forniti");
            token = GetBearerToken(token)??"";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Controllo username
                var tokenUsername = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (tokenUsername != username)
                {
                    Log.Warning("username non corrispondente :  " + username + " ha provato a fingersi " + tokenUsername);
                    return (false, "Username non corrispondente al Token");

                }
                
                return (true, "Token valido");
            }
            catch (SecurityTokenExpiredException ex)
            {
                Log.Warning("token scaduto "+ ex.Message);
                return (false, "Token scaduto");
            }
            catch (Exception ex)
            {
                Log.Error("token non valido "+ ex.Message);
                return (false, "Token non valido");
            }
        }
        
        public string? GetBearerToken(string authHeader)
        {
            if (string.IsNullOrWhiteSpace(authHeader))
                return null;

            authHeader = authHeader.TrimStart(); // rimuove solo spazi iniziali
            return authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader[7..].Trim()
                : authHeader.Trim();
        }

    }
}
