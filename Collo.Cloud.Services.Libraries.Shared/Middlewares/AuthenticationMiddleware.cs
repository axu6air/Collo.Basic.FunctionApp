using Collo.Cloud.Services.Libraries.Shared.Commons;
using Collo.Cloud.Services.Libraries.Shared.Extensions;
using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace Collo.Cloud.Services.Libraries.Shared.Middlewares
{
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly JwtSecurityTokenHandler _tokenValidator;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        public AuthenticationMiddleware(IConfiguration configuration)
        {
            var authority = Environment.GetEnvironmentVariable("AUTHENTICATION_AUTHORITY_KV");
            var audience = Environment.GetEnvironmentVariable("AUTHENTICATION_CLIENT_ID_KV");
            var userFlow = Environment.GetEnvironmentVariable("AUTHENTICATION_B2C_USER_FLOW_KV");

            _tokenValidator = new JwtSecurityTokenHandler();

            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{authority}/.well-known/openid-configuration?p={userFlow}",
                new OpenIdConnectConfigurationRetriever());

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = audience,
            };
        }

        public async Task Invoke(
            FunctionContext context,
            FunctionExecutionDelegate next)
        {
            if (!TryGetTokenFromHeaders(context, out var token))
            {
                // Unable to get token from headers
                context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                return;
            }

            if (!_tokenValidator.CanReadToken(token))
            {
                // Token is malformed
                context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                return;
            }

            // Get OpenID Connect metadata
            //var validationParameters = _tokenValidationParameters.Clone();
            var openIdConfig = await _configurationManager.GetConfigurationAsync(default);
            _tokenValidationParameters.ValidateIssuer = true;
            _tokenValidationParameters.ValidIssuer = openIdConfig.Issuer;
            _tokenValidationParameters.IssuerSigningKeys = openIdConfig.SigningKeys;
            //_tokenValidationParameters.ValidateAudience = true;
            //_tokenValidationParameters.ValidateIssuerSigningKey = true;

            try
            {
                // Validate token
                var principal = _tokenValidator.ValidateToken(
                        token, _tokenValidationParameters, out _);

                /*
                 * if we need to set custom claim.
                 */
                //principal.Identities.FirstOrDefault().AddClaim(new Claim(ClaimTypes.Name, "subrata roy"));

                /*
                 * Set principal + token in Features collection
                 * They can be accessed from here later in the call chain
                 */
                context.Features.Set(new JwtPrincipalFeature(principal, token));
                TokenHelper.ConfigureToken(token);
                await next(context);
            }
            catch (SecurityTokenException ex)
            {
                // Token is not valid (expired etc.)
                context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                return;
            }
        }

        private static bool TryGetTokenFromHeaders(FunctionContext context, out string token)
        {
            token = null;
            // HTTP headers are in the binding context as a JSON object
            // The first checks ensure that we have the JSON string
            if (!context.BindingContext.BindingData.TryGetValue("Headers", out var headersObj))
            {
                return false;
            }

            if (headersObj is not string headersStr)
            {
                return false;
            }

            // Deserialize headers from JSON
            var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersStr);
            var normalizedKeyHeaders = headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value);
            if (!normalizedKeyHeaders.TryGetValue("authorization", out var authHeaderValue))
            {
                // No Authorization header present
                return false;
            }

            if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // Scheme is not Bearer
                return false;
            }

            token = authHeaderValue.Substring("Bearer ".Length).Trim();
            return true;
        }
    }
}
