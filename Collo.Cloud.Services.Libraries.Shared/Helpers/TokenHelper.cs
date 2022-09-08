using Collo.Cloud.Services.Libraries.Shared.Commons;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    /// <summary>
    /// This is helper class is responsible decoding access token and set claims,requestser.
    /// </summary>
    public static class TokenHelper
    {
        public static RequestUser EmptyUser = new() { Id = string.Empty, OrgId = string.Empty, FirstName = string.Empty, LastName = string.Empty };
        public static IEnumerable<Claim> Claims { get; set; }

        /// <summary>
        /// Set request user from claims
        /// </summary>
        public static RequestUser AppUser
        {
            get
            {
                if (Claims == null) return EmptyUser;
                var claimItems = Claims.Select(s => new { key = s.Type, s.Value }).ToDictionary(t => t.key, t => t.Value);
                return new RequestUser
                {
                    //Id = claimItems["sub"],
                    OrgId = claimItems["extension_OrgId"]
                    //rest will adde if need
                };
            }
        }

        public static IEnumerable<Claim> DecodeToken(string token) => new JwtSecurityTokenHandler()
              .ReadJwtToken(token)
              .Claims;

        public static IEnumerable<Claim> DecodeToken(this HttpRequestData req)
        {
            var isAuthorizationHeaderFount = req.Headers.TryGetValues("Authorization", out IEnumerable<string> values);
            string token = default;

            if (isAuthorizationHeaderFount)
                token = values.First().Split(' ')[1];

            return DecodeToken(token);
        }

        public static string GetOrganizationIdFromToken(IEnumerable<Claim> token)
        {
            if (token == null) return null;

            return token.FirstOrDefault(x => x.Type == "extension_OrgId").Value;
        }

        public static string GetUserIdFromToken(IEnumerable<Claim> token)
        {
            if (token == null) return null;

            return token.FirstOrDefault(x => x.Type == "extension_UserId").Value;
        }

        /// <summary>
        /// Get token when request comes and set claims
        /// </summary>
        /// <param name="token"></param>
        public static void ConfigureToken(string token)
        {
            Claims = DecodeToken(token);
        }

        public static (string organizationId, string userId) GetOrganiationIdAndUserId(FunctionContext context)
        {

            var principalFeature = context.Features.Get<JwtPrincipalFeature>();
            var decodeToken = DecodeToken(principalFeature.AccessToken);
            var orgId = GetOrganizationIdFromToken(decodeToken);
            var userId = GetUserIdFromToken(decodeToken);

            return (orgId, userId);
        }
    }
}
