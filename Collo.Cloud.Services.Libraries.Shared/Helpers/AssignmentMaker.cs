using Collo.Cloud.Services.Libraries.Shared.Infrastructures.Constants;
using Collo.Cloud.Services.Libraries.Shared.Infrastructures.Enums;
using Collo.Cloud.Services.Libraries.Shared.Permission.Models;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using System.Text;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    public class AssignmentMaker
    {
        /// <summary>
        /// Create Assignment string for user
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public static string GetAssignmentString(AssignmentModel requestModel)
        {
            StringBuilder assignment = new StringBuilder();

            if (requestModel == null || requestModel.Organization == null || requestModel.ServiceName == null || requestModel.RoleName == null)
                return null;
            else
                assignment.Append(requestModel.RoleName);

            if (requestModel.ServiceName.ToLower() == ServiceEnum.iam.ToString())
            {
                GetIamAssignmentString(assignment, requestModel);
            }
            else if (requestModel.ServiceName.ToLower() == ServiceEnum.dm.ToString())
            {
                return GetDmAssignmentString(assignment, requestModel);
            }

            return assignment.ToString();
        }

        private static string GetIamAssignmentString(StringBuilder assignment, AssignmentModel requestModel)
        {
            assignment.Append(AssignmentConstants.ROLE_ASSIGNMENT_SEPARATOR);
            assignment.Append(requestModel.ServiceName);

            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
            assignment.Append(AssignmentConstants.ORGANIZATIONS);
            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);

            if (requestModel.Organization.AccessAny)
            {
                assignment.Append(AssignmentConstants.ANY);
                return assignment.ToString();
            }
            else if (requestModel.Organization.Id != null)
                assignment.Append(requestModel.Organization.Id);

            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);

            if (requestModel.User != null && requestModel.ServiceName == AssignmentConstants.IAM)
            {
                assignment.Append(AssignmentConstants.USERS);
                assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);

                if (requestModel.User.AccessAny)
                {
                    //assignment += AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR + AssignmentConstants.ANY;
                    //return assignment;

                    assignment.Append(AssignmentConstants.ANY);
                    return assignment.ToString();
                }
                else if (requestModel.User.Id != null)
                {
                    //assignment += AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR + requestModel.User.Id;
                    //return assignment;
                    assignment.Append(requestModel.User.Id);
                    return assignment.ToString();

                }
            }

            return assignment.ToString();
        }

        private static string GetDmAssignmentString(StringBuilder assignment, AssignmentModel requestModel)
        {
            assignment.Append(AssignmentConstants.ROLE_ASSIGNMENT_SEPARATOR);
            assignment.Append(requestModel.ServiceName);

            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
            assignment.Append(AssignmentConstants.ORGANIZATIONS);
            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);

            if (requestModel.Organization.AccessAny)
            {
                assignment.Append(AssignmentConstants.ANY);
                return assignment.ToString();
            }
            else if (requestModel.Organization.Id != null)
                assignment.Append(requestModel.Organization.Id);

            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);

            if (requestModel.ServiceName.ToLower() == ServiceEnum.dm.ToString())
            {
                if (requestModel.Site != null && requestModel.ServiceName == AssignmentConstants.DM)
                {
                    assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                    assignment.Append(AssignmentConstants.SITES);
                    if (requestModel.Site.AccessAny == true)
                    {
                        assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                        assignment.Append(AssignmentConstants.ANY);
                        return assignment.ToString();
                    }
                    else if (requestModel.Site.Id != null)
                    {
                        assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                        assignment.Append(requestModel.Site.Id);
                    }

                    if (requestModel.Gateway != null)
                    {
                        assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                        assignment.Append(AssignmentConstants.GATEWAYS);
                        if (requestModel.Gateway.AccessAny == true)
                        {
                            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                            assignment.Append(AssignmentConstants.ANY);
                            return assignment.ToString();
                        }
                        else if (requestModel.Gateway.Id != null)
                        {
                            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                            assignment.Append(requestModel.Gateway.Id);
                        }

                        if (requestModel.Instrument != null)
                        {
                            assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                            assignment.Append(AssignmentConstants.ALLOCATEDINSTRUMENTS);

                            if (requestModel.Instrument.AccessAny == true)
                            {
                                assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                                assignment.Append(AssignmentConstants.ANY);
                                return assignment.ToString();

                            }
                            else if (requestModel.Instrument.Id != null)
                            {
                                assignment.Append(AssignmentConstants.ASSIGNMENT_TYPE_SEPARATOR);
                                assignment.Append(requestModel.Instrument.Id);

                            }
                        }
                    }
                }
            }

            return assignment.ToString();
        }

        public static List<string> GetRolePermission(List<Role> roles)
        {

            if (roles == null)
                return null;

            List<string> rolePermissions = new();

            foreach (var role in roles)
            {
                string rolePermission = role.Id + AssignmentConstants.ROLE_PERMISSION_SEPARATOR + role.Permissions.ToString();
                rolePermissions.Add(rolePermission);
            }

            return rolePermissions.Count > 0 ? rolePermissions : null;
        }
    }
}
