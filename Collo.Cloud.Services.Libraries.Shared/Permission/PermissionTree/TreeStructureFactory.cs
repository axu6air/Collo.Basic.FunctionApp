namespace Collo.Cloud.Services.Libraries.Shared.Permission.PermissionTree
{
    public class TreeStructureFactory
    {
        public ITreeStructure GetAssignments(List<(string assignment, List<char> permisions)> assignments, string rootData)
        {
            if (assignments == null || rootData == null) return null;

            var tree = new TreeStructure(assignments, rootData);
            tree.Build().GetAwaiter().GetResult();

            return tree;
        }
    }
}