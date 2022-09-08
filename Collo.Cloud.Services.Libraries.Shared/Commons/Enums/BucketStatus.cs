using System.Runtime.Serialization;

namespace Collo.Cloud.Services.Libraries.Shared.Commons.Enums
{
    public enum BucketStatus
    {
        [EnumMember(Value = "Update")]
        Update,

        [EnumMember(Value = "Late")]
        Late,

        [EnumMember(Value = "Ready")]
        Ready
    }
}
