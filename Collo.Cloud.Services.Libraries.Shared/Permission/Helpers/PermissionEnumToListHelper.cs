using Collo.Cloud.Services.Libraries.Shared.Permission.Enums;

namespace Collo.Cloud.Services.Libraries.Shared.Permission.Helpers
{
    public class EnumPair
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
    public static class PermissionEnumToListHelper
    {
        public static List<EnumPair> Convert()
        {
            var qatype = typeof(PermissionEnum);
            var names = qatype.GetEnumNames();
            var values = qatype.GetEnumValues().Cast<int>().ToList();
            var nameValues = names.Select(n =>
                    qatype.GetMember(n)[0]
                        .CustomAttributes.First()
                        .NamedArguments[0].TypedValue
                        .Value)
                .ToList();
            var valuesList = names.Select((n, index) =>
                    new EnumPair { Key = n, Value = values[index] })
                .ToList();
            var nameValuesList = names.Select((n, index) =>
                    new { key = n, value = nameValues[index] })
                .ToList();
            return valuesList;
        }
    }
}
