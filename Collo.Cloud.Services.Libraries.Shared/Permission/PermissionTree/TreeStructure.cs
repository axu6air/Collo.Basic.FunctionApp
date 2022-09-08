namespace Collo.Cloud.Services.Libraries.Shared.Permission.PermissionTree
{
    public class TreeStructure : ITreeStructure
    {
        public List<(string assignment, List<char> permisions)> AssignmentsAndPermissionsStrings { get; set; }
        public Node Root { get; set; }
        string assignments = string.Empty;

        public TreeStructure(List<(string assignment, List<char> permissions)> assignmentsAndPermissions, string data)
        {
            AssignmentsAndPermissionsStrings = assignmentsAndPermissions;
            Root = new Node(data);
        }

        public async Task Build()
        {
            foreach (var assignmentString in AssignmentsAndPermissionsStrings)
            {
                string[] assignments = assignmentString.assignment.Split('/');
                var queue = new Queue<string>(assignments);
                await BuildTree(queue, Root, assignmentString.permisions);
            }
        }

        private async Task BuildTree(Queue<string> queue, Node node, List<char> permissions)
        {
            if (queue.Any())
            {
                var data = queue.Dequeue();
                Node newNode = new(data);

                node = node.Add(newNode);
                await BuildTree(queue, node, permissions);
            }
            else
                node.AddPermission(permissions);

        }

        #region Search
        //public async Task<(bool flag, string any, string assignments)> Search(Node node, string url, int index = 0)
        //{
        //    var splits = url.Split('/');
        //    if (splits.Length == index)
        //        flag = true;

        //    if ((node.Children.Count == 0 || node.Children == null))
        //        return (flag, any, assignments.Length > 0 ? assignments.Remove(assignments.Length - 1) : null);


        //    for (int i = 0; i < node.Children.Count; i++)
        //    {
        //        if (index == splits.Length)
        //            flag = true;
        //        else
        //        {
        //            if (node.Children[i].Data == "any")
        //            {
        //                assignments += node.Children[i].Data + '/';
        //                index = splits.Length - 1;
        //                any = node.Data;
        //                return await Search(node.Children[i], url, index + 1);
        //                //flag = true;
        //            }
        //            else if (splits[index] == node.Children[i].Data && any == null)
        //            {
        //                assignments += node.Children[i].Data + '/';
        //                return await Search(node.Children[i], url, index + 1);
        //            }
        //        }

        //    }

        //    return (flag, any, assignments.Length > 0 ? assignments.Remove(assignments.Length - 1) : null);

        //}
        #endregion

        public (List<string> childrenIds, bool isAny) GetChildrenList(string assignmentString)
        {
            string[] assignments = assignmentString.Split('/');
            var queue = new Queue<string>(assignments);
            var node = Root;

            if (Root != null)
            {
                List<string> childrenIds = new();
                return GetChildrenList(queue, node, childrenIds);
            }

            return (new List<string>(), false);
        }

        private (List<string> childrenIds, bool isAny) GetChildrenList(Queue<string> queue, Node? node, List<string> childrenIds)
        {

            if (node == null)
                return (new List<string>(), false);

            else if (queue.Any())
            {
                var data = queue.Dequeue();
                var result = node.SearchAssignment(data);

                if (result.isAny && result.node.Permissions.Contains('l'))
                    return (childrenIds, true);

                node = result.node;
                return GetChildrenList(queue, node, childrenIds);
            }
            else if (node.Children.Any(x => x.Data.ToLower() == "any"))
            {
                var child = node.Children.First(x => x.Data.ToLower() == "any");

                if (child.Permissions.Contains('l'))
                    return (childrenIds, true);

            }

            var childrenIdList = node.Children.
                                 Where(x => x.Permissions.Contains('l'))
                                .Select(x => x.Data)?.ToList() ?? new List<string>();

            childrenIds.AddRange(childrenIdList);
            return (childrenIds, false);
        }

        public async Task<(bool hasPermission, string assignment)> SearchUrlInAssignments(string url)
        {
            var splits = url.Split('/');

            Queue<string> queue = new Queue<string>(splits);
            var node = Root;

            if (node != null)
            {
                var assignmentSearch = SearchAssignment(queue, node);

                return await Task.FromResult(assignmentSearch);
            }

            return (false, string.Empty);
        }

        private (bool hasPermission, string assignment) SearchAssignment(Queue<string> queue, Node? node, bool hasPermission = false)
        {
            if (node == null && queue.Any() == true)
                return (false, string.Empty);
            else if (queue.Any())
            {
                var data = queue.Dequeue();

                if (node.Children.Count > 0 && node.Children.Any(x => x.Data.ToLower() == "any"))
                {
                    var childNode = node.Children.FirstOrDefault(x => x.Data.ToLower() == "any");
                    assignments += childNode.Data + '/';
                    hasPermission = true;
                    return SearchAssignment(queue, childNode, hasPermission);
                }
                else if (node.Children.Count > 0 && node.Children.Any(x => x.Data == data))
                {
                    var childNode = (node.Children.FirstOrDefault(x => x.Data == data));
                    assignments += childNode.Data + '/';
                    hasPermission = true;
                    return SearchAssignment(queue, childNode, hasPermission);
                }
                else if (node.Children.Count > 0 && node.Children.Any(x => x.Data != data))
                {
                    hasPermission = false;
                    assignments = string.Empty;
                }
                else if (node.Children.Count == 0)
                {
                    hasPermission = false;
                    assignments = string.Empty;
                }
            }

            var assignment = assignments.Length > 0 ? assignments.Remove(assignments.Length - 1) : string.Empty;
            return (hasPermission, assignment);
        }
    }
}
