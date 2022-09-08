using Collo.Cloud.Services.Libraries.Shared.Helpers;
using System.Security.Claims;

namespace Collo.Cloud.Services.Libraries.Shared.Permission.Helpers
{
    public static class PartitionKeyHelper
    {
        private static string GetClaimValue(this IEnumerable<Claim> claims, string key) => claims
            .FirstOrDefault(f => f.Type == key)?
            .Value;

        public static string GenerateId(this IEnumerable<Claim> claims, string formator, params string[] keys)
        {
            var id = string.Empty;
            int part = 1;
            foreach (var key in keys)
                id += $"{claims.GetClaimValue(key)}{formator}";
            //need to be clear about part;
            return $"{id}{part}";
        }

        /*
         * Example
         * formatort->::
         * keys->"{orgId,userId}"
         */

        public static string GenerateId(string token, string formator, params string[] keys)
        {
            var id = string.Empty;
            int part = 1;
            var claims = TokenHelper.DecodeToken(token);

            foreach (var key in keys)
                id += $"{claims.GetClaimValue(key)}{formator}";

            //need to be cleared about part;
            return $"{id}{part}";
        }

        public static string GenerateId(string formator, params string[] keys)
        {
            var id = string.Empty;
            int part = 1;
            var claims = TokenHelper.Claims;

            foreach (var key in keys)
                id += $"{claims.GetClaimValue(key)}{formator}";

            //need to be cleared about part;
            return $"{id}{part}";
        }

        public static string GenerateIdFromOrgId(string formator, string userId)
        {
            int part = 1;
            var claims = TokenHelper.Claims;

            //need to be cleared about part;
            return $"{claims.GetClaimValue("extension_OrgId")}{formator}{userId}{formator}{part}";
        }

        public static string GetOrgIdFromClaim()
        {
            var claims = TokenHelper.Claims;

            return $"{claims.GetClaimValue("extension_OrgId")}";
        }
    }
}
