using AutoMapper;

namespace Collo.Cloud.Services.Libraries.Shared.Permission.Mappers
{
    public class PermissionMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomPermissionMapper>();
            });
            return config.CreateMapper();
        });

        public static IMapper Mapper => Lazy.Value;
    }
}
