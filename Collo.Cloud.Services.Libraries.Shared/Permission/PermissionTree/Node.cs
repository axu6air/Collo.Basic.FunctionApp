namespace Collo.Cloud.Services.Libraries.Shared.Permission.PermissionTree
{
    public class Node
    {
        public string Data; //data for storage
        public List<Node> Children = new();//array will keep children
        public Node parent;//parent to start the tree
        public List<char> Permissions = new();

        private bool HasChildren
        {
            get { return Children.Count > 0; }
        }

        public Node(string data)
        {
            Data = data;
        }

        public Node Add(Node node)
        {
            var parentNode = this;

            if (parentNode.Children.Any(x => x.Data == node.Data))
            {
                node = parentNode.Children.Single(x => x.Data == node.Data);
                return node;
            }

            if (parentNode.Children.Any(x => x.Data != node.Data))
            {
                node.parent = parentNode;
                parentNode.Children.Add(node);

                //  parentNode.Children.ForEach(x => Console.Write(x.Data + ", "));

            }
            else if (parentNode.Children.Count == 0)
            {
                node.parent = parentNode;
                parentNode.Children.Add(node);

                // parentNode.Children.ForEach(x => Console.Write(x.Data + ", "));

            }

            return node;
        }

        public Node AddPermission(List<char> permissions)
        {
            Permissions = permissions;
            return this;
        }

        public (Node node, bool isAny) SearchAssignment(string value)
        {
            if (this.HasChildren && Children.Any(x => x.Data.ToLower() == "any"))
                return (Children.First(x => x.Data.ToLower() == "any"), true);
            else if (Data == value)
                return (this, false);
            else if (this.HasChildren && Children.Any((x) => x.Data == value))
                return (Children.First(x => x.Data == value), false);

            return (null, false);
        }

    }
}
