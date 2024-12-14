using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SolarWatch.Service.Authentication;

public class TokenValidator : ITokenValidator
{
    private readonly IConfiguration _configuration;

    public TokenValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var secret = _configuration["IssuerSigningKey"];
        var jwtStettings =_configuration.GetSection("JWTSettings"); //IConfigurationSection object - kulcsokra hivatkozva get value like a json
        var validIssuer = jwtStettings["ValidIssuer"];
        var validAudience = jwtStettings["ValidAudience"];
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = validIssuer, //az issuert is ellenörzöm
            ValidateAudience = false,
            
        };
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

            if (securityToken is JwtSecurityToken jwtSecurityToken && 
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch (Exception)
        {
            // Hiba esetén kezelheted a kivételt
            return null;
        }

        return null;
    }
       
    
}