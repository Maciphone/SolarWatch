using System.Security.Claims;

namespace SolarWatch.Service.Authentication;

public interface ITokenValidator
{
    
    
    ClaimsPrincipal GetPrincipalFromToken(string token);
}