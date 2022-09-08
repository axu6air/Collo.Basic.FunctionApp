namespace Collo.Cloud.Services.Libraries.Shared.Permission.PermissionTree
{
    public interface ITreeStructure
    {
        List<(string assignment, List<char> permisions)> AssignmentsAndPermissionsStrings { get; set; }
        Node Root { get; set; }
        //Dictionary<string, string> KeyValuePairs { get; set; }


        Task Build();
        // (List<string> childrenIds, bool isAny) GetChildrenList(string assignmentString);
        //   Task<(bool flag, string any, string assignments)> Search(Node node, string url, int index = 0);
        Task<(bool hasPermission, string assignment)> SearchUrlInAssignments(string url);
        (List<string> childrenIds, bool isAny) GetChildrenList(string assignmentString);
        //Task<(List<string> ids, bool isAny)> GetChildData(Node node, string url, int index = 0);
    }
}
