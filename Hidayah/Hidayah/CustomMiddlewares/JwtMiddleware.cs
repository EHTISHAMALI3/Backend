using Hidayah.Controllers;
using Hidayah.Domain.Models;
using Hidayah.Infrastrcture.Repositriy;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hidayah.CustomMiddlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                // First, try to validate the token (including its lifetime)
                if (IsTokenExpired(token))
                {
                    // If the token is expired, do not extend or validate further.
                    // Optionally, you can set a flag in the context or return an error here.
                    // For now, simply continue without extending.
                    // Return a 401 Unauthorized response if token is expired.
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token expired");
                    return;

                }
                else
                {
                    // Token is valid, so check if it is nearing expiration.
                    ExtendTokenExpiration(context, token);
                }
            }

            await _next(context);
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true, // Validate lifetime here
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _configuration["Jwt:ValidIssuer"],
                    ValidAudience = _configuration["Jwt:ValidAudience"]
                };

                // If token is invalid or expired, an exception will be thrown.
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                // Token is expired
                return true;
            }
            catch
            {
                // Other validation errors (could also log if necessary)
                return true;
            }
        }

        private void ExtendTokenExpiration(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
                // Validate token but ignore lifetime for our purpose here
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _configuration["Jwt:ValidIssuer"],
                    ValidAudience = _configuration["Jwt:ValidAudience"]
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                // Check if token is nearing expiration (e.g., less than 5 minutes remaining)
                if (jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(2))
                {
                    var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                    // Here, assume that AuthRepositoryImpl (or similar) has a method to generate token.
                    // Make sure to pass required dependencies (e.g., DB context) if needed.
                    var newToken = new UserRepositoryImpl(null, null, _configuration,null,null).GenerateToken(new User { Email = email });

                    // Add the new token to the response headers.
                    context.Response.Headers["Authorization"] = "Bearer " + newToken;
                }
            }
            catch
            {
                // Optionally log or handle exceptions
            }
        }
    }
}