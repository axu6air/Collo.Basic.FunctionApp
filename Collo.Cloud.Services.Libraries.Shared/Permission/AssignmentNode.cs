using Collo.Cloud.Services.Libraries.Shared.Permission.Enums;

namespace Collo.Cloud.Services.Libraries.Shared.Permission
{
    public class AssignmentNode
    {
        public Dictionary<string, AssignmentNode> children = new Dictionary<string, AssignmentNode>();
        public bool endOfAssign;
        public int level = -1;
        public List<PermissionEnum> Permissions = new List<PermissionEnum>();
    }
}
