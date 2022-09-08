using Collo.Cloud.Services.Libraries.Shared.Authorization;
using Collo.Cloud.Services.Libraries.Shared.Extensions;
using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Collo.Cloud.Services.Libraries.Shared.Infrastructures.Constants;
using Collo.Cloud.Services.Libraries.Shared.Permission.Enums;
using Collo.Cloud.Services.Libraries.Shared.Permission.Interfaces;
using Collo.Cloud.Services.Libraries.Shared.Permission.PermissionTree;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net;
using System.Reflection;
using System.Security.Claims;

namespace Collo.Cloud.Services.Libraries.Shared.Middlewares
{
    public class AuthorizationMiddleware : IFunctionsWorkerMiddleware
    {
        private const string ScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";
        private readonly IColloPermissionService _colloPermissionService;
        private readonly TreeStructureFactory _treeStructureFactory;
        private ITreeStructure _treeStructure;

        public AuthorizationMiddleware(IColloPermissionService colloPermissionService,
            TreeStructureFactory treeStructureFactory)
        {
            _colloPermissionService = colloPermissionService;
            _treeStructureFactory = treeStructureFactory;
        }

        public async Task Invoke(
            FunctionContext context,
            FunctionExecutionDelegate next)
        {
            var (organizationId, userId) = TokenHelper.GetOrganiationIdAndUserId(context);
            var (routeName, roleName) = GetRouteAndRole(context.GetTargetFunctionMethod());
            var url = UrlMaker.GetUrlForPermission(context.GetHttpRequestData(), routeName, roleName);

            var assignment = await _colloPermissionService.GetAsync(organizationId, userId);
            var roleAndAssignments = await _colloPermissionService.GetRolesAndPermissions(assignment);
            var assignments = await _colloPermissionService.GetAssigmnmentsAndPermissions(assignment);
            var rolesAndPermission = await _colloPermissionService.GetRoles(assignment);

            _treeStructure = _treeStructureFactory.GetAssignments(assignments, AssignmentConstants.ROOT);

            var assignmentSearch = await _treeStructure.SearchUrlInAssignments(url);

            if (!assignmentSearch.hasPermission)
            {
                context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
                return;
            }

            if (roleName.EndsWith("read") || roleName.EndsWith("update") || roleName.EndsWith("delete"))
            {
                var permissions = GetPermissions(roleAndAssignments, assignmentSearch, rolesAndPermission);
                if (!CheckPermissions(permissions, roleName))
                {
                    context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
                    return;
                }
            }
            else if (roleName.EndsWith("create"))
            {
                var permissions = GetPermissions(roleAndAssignments, assignmentSearch, rolesAndPermission);
                if (!CheckPermissions(permissions, roleName))
                {
                    context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
                    return;
                }
            }
            else if (roleName.EndsWith("list"))
            {
                var childrenIds = _treeStructure.GetChildrenList(url);

                context.Items["permittedIds"] = childrenIds.childrenIds;
                context.Items["isAnyPermitted"] = childrenIds.isAny;
            }

            await next(context);

            #region commented code
            //if (assignmentSearch.any != null)
            //{
            //    var role = roleandAssignments.FirstOrDefault(x => x.services.Contains(assignmentSearch.assignments)).role;
            //    var permissions = getRoles.FirstOrDefault(x => x.role == role).permission.ToCharArray();
            //    if (!CheckPermissions(permissions, routeAndRole.roleName))
            //    {
            //        context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
            //        return;
            //    }
            //}

            //if (principalFeature.Principal.HasClaim(c => c.Type == ScopeClaimType && c.Value == "access.api"))
            //{
            //    var targetMethod = context.GetTargetFunctionMethod();
            //    var (scopes, roles, permissions) = GetAcceptedScopesAndUserRoles(targetMethod);
            //    if (scopes.Count != 0 && roles.Count != 0 && permissions.Count != 0)
            //    {
            //        var isPermitted = await _colloPermissionService.HasPermissionAsync(roles, scopes, permissions);
            //        if (!isPermitted)
            //        {
            //            context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
            //            return;
            //        }
            //    }

            //    /*
            //     *   inject permissionService verify with [role,scopes as service(for now)]
            //     *   other wise
            //     *   context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
            //     *   return;
            //     */
            //}
            //else
            //{
            //    context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
            //    return;
            //}
            #endregion
        }

        #region AuthorizePrincipal
        private static bool AuthorizePrincipal(FunctionContext context, ClaimsPrincipal principal)
        {
            // This authorization implementation was made
            // for Azure AD. Your identity provider might differ.

            if (principal.HasClaim(c => c.Type == ScopeClaimType))
            {
                // Request made with delegated permissions, check scopes and user roles
                //  return AuthorizeDelegatedPermissions(context, principal);
                return AuthorizeApplicationPermissions(context, principal);
            }

            // Request made with application permissions, check app roles
            return AuthorizeApplicationPermissions(context, principal);
        }
        #endregion

        #region AuthorizeDelegatedPermissions
        //private static bool AuthorizeDelegatedPermissions(FunctionContext context, ClaimsPrincipal principal)
        //{
        //    var targetMethod = context.GetTargetFunctionMethod();

        //    var (acceptedScopes, acceptedUserRoles) = GetAcceptedScopesAndUserRoles(targetMethod);

        //    var userRoles = principal.FindAll(ClaimTypes.Role);
        //    //var userHasAcceptedRole = userRoles.Any(ur => acceptedUserRoles.Contains(ur.Value));

        //    // Scopes are stored in a single claim, space-separated
        //    var callerScopes = (principal.FindFirst(ScopeClaimType)?.Value ?? "")
        //        .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //    var callerHasAcceptedScope = callerScopes.Any(cs => acceptedScopes.Contains(cs));

        //    // This app requires both a scope and user role
        //    // when called with scopes, so we check both
        //    return /*userHasAcceptedRole &&*/ callerHasAcceptedScope;
        //}
        #endregion

        #region AuthorizeApplicationPermissions
        private static bool AuthorizeApplicationPermissions(FunctionContext context, ClaimsPrincipal principal)
        {
            var targetMethod = context.GetTargetFunctionMethod();

            var acceptedPermissions = GetAcceptedPermissions(targetMethod);
            var appPermissions = principal.FindAll("permissions");
            var appHasAcceptedRole = appPermissions.Any(ur =>
            {

                var hasValue = Enum.TryParse(typeof(PermissionEnum), ur.Value, out var result);
                if (hasValue)
                    return acceptedPermissions.Contains((PermissionEnum)Enum.Parse(typeof(PermissionEnum), ur.Value));
                return false;
            });
            return appHasAcceptedRole;
        }
        #endregion

        private bool CheckPermissions(string permissions, string routeRole)
        {
            if (permissions.Contains('c') && routeRole.ToLower() == "create")
                return true;
            else if (permissions.Contains('r') && routeRole.ToLower() == "read")
                return true;
            else if (permissions.Contains('d') && routeRole.ToLower() == "delete")
                return true;
            else if (permissions.Contains('u') && routeRole.ToLower() == "update")
                return true;
            else if (permissions.Contains('l') && routeRole.ToLower() == "list")
                return true;

            return false;
        }

        private static string GetPermissions(
            List<(string role, string services)> roleAndAssignments,
            (bool hasPermission, string assignments) assignmentSearch,
            List<(string role, string permission)> rolesAndPermission)
        {
            var role = roleAndAssignments
                .FirstOrDefault(
                    x => x.services
                        .Equals(assignmentSearch.assignments, StringComparison.OrdinalIgnoreCase)).role
                ?? string.Empty;
            var permissions = rolesAndPermission.FirstOrDefault(x => x.role == role)
                .permission?.ToString() ?? null;
            return permissions;
        }

        #region GetAcceptedScopesAndUserRoles
        private static (List<string> scopes, List<string> userRoles, List<PermissionEnum> permissions) GetAcceptedScopesAndUserRoles(MethodInfo targetMethod)
        {
            var attributes = GetCustomAttributesOnClassAndMethod<AuthorizeAttribute>(targetMethod);
            // If scopes A and B are allowed at class level,
            // and scope A is allowed at method level,
            // then only scope A can be allowed.
            // This finds those common scopes and
            // user roles on the attributes.
            var scopes = attributes
                .Skip(1)
                .Select(a => a.Scopes)
                .Aggregate(attributes.FirstOrDefault()?.Scopes ?? Enumerable.Empty<string>(), (result, acceptedScopes) =>
                {
                    return result.Intersect(acceptedScopes);
                })
                .ToList();
            var userRoles = attributes
                .Skip(1)
                .Select(a => a.AppRoles)
                .Aggregate(attributes.FirstOrDefault()?.AppRoles ?? Enumerable.Empty<string>(), (result, acceptedRoles) =>
                {
                    return result.Intersect(acceptedRoles);
                })
                .ToList();

            var permissions = attributes
               .Skip(1)
               .Select(a => a.Permissions)
               .Aggregate(attributes.FirstOrDefault()?.Permissions ?? Enumerable.Empty<PermissionEnum>(), (result, acceptedPermissions) =>
               {
                   return result.Intersect(acceptedPermissions);
               })
               .ToList();
            return (scopes, userRoles, permissions);
        }
        #endregion

        private static List<string> GetAcceptedAppRoles(MethodInfo targetMethod)
        {
            var attributes = GetCustomAttributesOnClassAndMethod<AuthorizeAttribute>(targetMethod);
            // Same as above for scopes and user roles,
            // only allow app roles that are common in
            // class and method level attributes.
            return attributes
                .Skip(1)
                .Select(a => a.AppRoles)
                .Aggregate(attributes.FirstOrDefault()?.UserRoles ?? Enumerable.Empty<string>(), (result, acceptedRoles) =>
                {
                    return result.Intersect(acceptedRoles);
                })
                .ToList();
        }

        private static (string routeName, string roleName) GetRouteAndRole(MemberInfo targetMethod)
        {
            var attribute = targetMethod.GetCustomAttribute<RouteMapperAttribute>();

            return (attribute.RouteName, attribute.RoleName);
        }

        private static List<PermissionEnum> GetAcceptedPermissions(MethodInfo targetMethod)
        {
            var attributes = GetCustomAttributesOnMethod<AuthorizeAttribute>(targetMethod);
            // Same as above for scopes and user roles,
            // only allow app roles that are common in
            // class and method level attributes.
            return attributes
                .Skip(1)
                .Select(a => a.Permissions)
                .Aggregate(attributes.FirstOrDefault()?.Permissions ?? Enumerable.Empty<PermissionEnum>(), (result, acceptedPermissions) =>
                {
                    return result.Intersect(acceptedPermissions);
                })
                .ToList();
        }

        private static List<T> GetCustomAttributesOnClassAndMethod<T>(MethodInfo targetMethod)
            where T : Attribute
        {
            var methodAttributes = targetMethod.GetCustomAttributes<T>();
            var classAttributes = targetMethod.DeclaringType.GetCustomAttributes<T>();
            return methodAttributes.Concat(classAttributes).ToList();
        }

        private static List<T> GetCustomAttributesOnClass<T>(MethodInfo targetMethod)
          where T : Attribute
        {
            var classAttributes = targetMethod.DeclaringType.GetCustomAttributes<T>();
            return classAttributes.ToList();
        }

        private static List<T> GetCustomAttributesOnMethod<T>(MethodInfo targetMethod)
        where T : Attribute
        {
            var methodAttributes = targetMethod.GetCustomAttributes<T>();
            return methodAttributes.ToList();
        }
    }
}