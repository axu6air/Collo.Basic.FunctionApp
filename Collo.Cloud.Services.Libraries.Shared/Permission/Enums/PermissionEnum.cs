using System.Runtime.Serialization;

namespace Collo.Cloud.Services.Libraries.Shared.Permission.Enums
{
    public enum PermissionEnum
    {
        [EnumMember(Value = "0")]
        None = 0,
        [EnumMember(Value = "1")]
        Read = 1,
        [EnumMember(Value = "2")]
        Create = 2,
        [EnumMember(Value = "3")]
        Delete = 3,
        [EnumMember(Value = "4")]
        Update = 4,
        [EnumMember(Value = "5")]
        List = 5
    }
}
