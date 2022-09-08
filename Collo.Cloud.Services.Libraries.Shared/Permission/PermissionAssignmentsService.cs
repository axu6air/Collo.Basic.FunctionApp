using Collo.Cloud.Services.Libraries.Shared.Permission.Enums;

namespace Collo.Cloud.Services.Libraries.Shared.Permission
{
    public class PermissionAssignmentsService
    {
        private string _wildCard;
        private char _separator;

        public AssignmentNode root;
        public List<(string Role, string Resource)> RoleResourcesList { get; set; }
        public List<string> Roles { get; set; }

        public PermissionAssignmentsService()
        {
            Roles = new List<string>();
            RoleResourcesList = new List<(string Role, string Resource)>();
        }

        //Configure("organizations", 1, "any", '/');
        public void Configure(string topResource, int topLevel, string wildCard, char separator)
        {
            _wildCard = wildCard;
            _separator = separator;

            root = new AssignmentNode();
            root.level = topLevel;
            root.children.Add(topResource, new AssignmentNode());

        }

        public void Insert(string resource)
        {
            AssignmentNode tempRoot = root;
            var resources = resource.Split(_separator);
            int total = resources.Count() - 1;
            for (int i = 0; i < resources.Count(); i++)
            {
                AssignmentNode newNode;
                if (tempRoot.children.Keys.Contains(resources[i]))
                    tempRoot = tempRoot.children[resources[i]];

                else
                {
                    newNode = new AssignmentNode();

                    if (total == i)
                        newNode.endOfAssign = true;

                    newNode.level = root.level + 1;
                    tempRoot.children.Add(resources[i], newNode);
                    tempRoot = newNode;
                }
            }
        }


        public AssignmentNode Search(string resource)
        {
            AssignmentNode tempRoot = root;
            var resources = resource.TrimStart(_separator)
                .Split(_separator);

            int total = resources.Count() - 1;

            for (int i = 0; i < resources.Count(); i++)
            {
                //wildCard
                if (tempRoot.children.Keys.Contains(_wildCard) && tempRoot.level > 1)
                    return GetPermission(resource, tempRoot);

                else if (tempRoot.children.Keys.Contains(resources[i]))
                {
                    tempRoot = tempRoot.children[resources[i]];
                    if (total == i)
                        return GetPermission(resource, tempRoot);
                }
                else
                    return null;
            }

            return null;
        }


        public bool SearchPrefix(string resource)
        {
            AssignmentNode tempRoot = root;
            var resources = resource.TrimStart(_separator).Split(_separator);

            for (int i = 0; i < resources.Count(); i++)
            {
                if (tempRoot.children.Keys.Contains(_wildCard) && tempRoot.level > 1)
                    return true;

                else if (tempRoot.children.Keys.Contains(resources[i]))
                    tempRoot = tempRoot.children[resources[i]];
                else
                    return false;
            }

            return true;
        }

        private AssignmentNode GetPermission(string resource, AssignmentNode node)
        {
            node.Permissions.Clear();
            var find = RoleResourcesList.FirstOrDefault(f => f.Resource == resource);

            if (find.Role == null) return node;

            foreach (var roleItem in Roles)
            {
                var data = roleItem.Split(":"); //->live/user:cl
                var role = data[0]; //->live/user
                var permission = data[1]; //->cl

                if (find.Role == role)
                {
                    foreach (char p in permission)
                        switch (p)
                        {
                            case 'c':
                                node.Permissions.Add(PermissionEnum.Create);
                                break;
                            case 'l':
                                node.Permissions.Add(PermissionEnum.List);
                                break;
                            case 'r':
                                node.Permissions.Add(PermissionEnum.Read);
                                break;
                            case 'u':
                                node.Permissions.Add(PermissionEnum.Update);
                                break;
                        }
                }
            }

            return node;
        }
    }
}
