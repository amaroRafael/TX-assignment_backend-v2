namespace TX.RMC.Api.Services;

using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TX.RMC.Api.Models;
using TX.RMC.Api.Utils;
using TX.RMC.BusinessLogic;

/// <summary>
/// Service for identity management.
/// </summary>
public class IdentityService(BusinessLogic.LoginService businessLogic)
{
    private readonly LoginService businessLogic = businessLogic;

    /// <summary>
    /// Login user.
    /// </summary>
    /// <returns>Authentication Response</returns>
    public async Task<AuthenticationResponse> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            string userId = await businessLogic.ValidateCredentialsAsync(username, password);

            AuthenticationResponse authenticationResult = !string.IsNullOrEmpty(userId) ? Authenticate(userId) : new AuthenticationResponse { Errors = ["Access not authorized!"] };

            return authenticationResult;
        }
        catch (Exception ex)
        {
            return new AuthenticationResponse { Errors = [ex.Message] };
        }

        static AuthenticationResponse Authenticate(string userId, string username)
        {
            // authentication successful so generate jwt token
            AuthenticationResponse authenticationResult = new AuthenticationResponse { TokenType = "bearer" };
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                string key = ApiSecurityHelper.OauthKey; // Secret key which will be used later during validation
                var issuer = $"{ApiSecurityHelper.Issuer}";  // this will be your site URL
                var audience = ApiSecurityHelper.Audience;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var listClaims = new List<Claim>(
                [
                    new Claim(ClaimTypes.Sid, userId),
                    new Claim(ClaimTypes.NameIdentifier, $"{username}"),
                    new Claim(JwtRegisteredClaimNames.Iss, issuer),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ]);

                DateTime expires = DateTime.UtcNow.AddMinutes(60);

                ClaimsIdentity subject = new ClaimsIdentity(listClaims);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = issuer,
                    IssuedAt = DateTime.UtcNow,
                    Subject = subject,
                    Expires = expires,
                    Audience = audience,
                    SigningCredentials = credentials
                };

                // Create Security Token object by giving required parameters
                var token = tokenHandler.CreateToken(tokenDescriptor);
                authenticationResult.AccessToken = tokenHandler.WriteToken(token);

                authenticationResult.ExpiresIn = Convert.ToInt32(Math.Truncate(expires.Subtract(DateTime.Now).TotalSeconds));

                authenticationResult.Success = true;
                return authenticationResult;
            }
            catch (Exception ex)
            {
                return new AuthenticationResponse { Errors = [ex.Message] };
            }
        }
    }
}
