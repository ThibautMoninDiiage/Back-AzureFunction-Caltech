using Microsoft.IdentityModel.Tokens;
using SecurityServer.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SecurityServer.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJwtService _jwtService;

        public AuthenticationService(IJwtService jwtService)
        {
             _jwtService = jwtService;
        }

        public bool VerifyToken(string token)
        {
            if(token == null)
                return false;
            else
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                    return false;
                else
                {
                    X509Certificate2 certificate = _jwtService.LoadCertificate();

                    RSA test = certificate.GetRSAPrivateKey();
                    RsaSecurityKey securityKey = new RsaSecurityKey(test);

                    JwtSecurityToken jwtJeton = handler.ReadToken(token) as JwtSecurityToken;

                    TokenValidationParameters validationParameters = new TokenValidationParameters()
                    {
                        RequireExpirationTime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = securityKey
                    };

                    ClaimsPrincipal verify = handler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                    if (verify != null)
                        return true;
                    else
                        return false;
                }
            }
        }
    }
}
