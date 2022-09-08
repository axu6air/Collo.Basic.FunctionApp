using Collo.Cloud.Services.Libraries.Shared.Infrastructures.Constants;
using Collo.Cloud.Services.Libraries.Shared.Permission.Models;

namespace Collo.Cloud.Services.Libraries.Shared.Validations
{
    public class AssignmentValidation
    {
        public static bool CheckAssignment(string assignment, AssignmentModel model)
        {
            var stringArray = assignment.Split(AssignmentConstants.ROLE_ASSIGNMENT_SEPARATOR);

            if (!stringArray[0].Contains(model.RoleName))
                return false;

            if (!stringArray[1].StartsWith(model.ServiceName))
                return false;

            if (model.Organization != null)
            {
                var organizationString = AssignmentConstants.ORGANIZATIONS;

                if (model.Organization.AccessAny == true)
                    organizationString += '/' + AssignmentConstants.ANY;
                else if (model.Organization.Id != null)
                    organizationString += '/' + model.Organization.Id;

                if (!stringArray[1].Contains(organizationString))
                    return false;
            }

            if (model.User != null)
            {
                var userString = AssignmentConstants.USERS;

                if (model.User.AccessAny == true)
                    userString += '/' + AssignmentConstants.ANY;
                else
                    userString += '/' + model.User.Id;

                if (!stringArray[1].Contains(userString) || model.ServiceName != AssignmentConstants.IAM)
                    return false;

            }

            if (model.Site != null)
            {
                var siteString = AssignmentConstants.SITES;

                if (model.Site.AccessAny == true)
                    siteString += '/' + AssignmentConstants.ANY;
                else
                    siteString += '/' + model.Site.Id;

                if (!stringArray[1].Contains(siteString) || model.ServiceName != AssignmentConstants.DM)
                    return false;

            }

            if (model.Gateway != null)
            {
                var gatewayString = AssignmentConstants.GATEWAYS;

                if (model.Gateway.AccessAny == true)
                    gatewayString += '/' + AssignmentConstants.ANY;
                else
                    gatewayString += '/' + model.Gateway.Id;

                if (!stringArray[1].Contains(gatewayString) || model.ServiceName != AssignmentConstants.DM)
                    return false;
            }

            if (model.Instrument != null)
            {
                var instrumentString = AssignmentConstants.ALLOCATEDINSTRUMENTS;

                if (model.Instrument.AccessAny == true)
                    instrumentString += '/' + AssignmentConstants.ANY;
                else
                    instrumentString += '/' + model.Instrument.Id;

                if (!stringArray[1].Contains(instrumentString) || model.ServiceName != AssignmentConstants.DM)
                    return false;
            }

            return true;
        }
    }
}
