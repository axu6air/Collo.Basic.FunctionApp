using AutoMapper;
using Collo.Cloud.Services.Libraries.Shared.Permission.Models;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Assignment;

namespace Collo.Cloud.Services.Libraries.Shared.Permission.Mappers
{
    internal class CustomPermissionMapper : Profile
    {
        public CustomPermissionMapper()
        {
            CreateMap<PermissionRequest, Assignment>();
            //CreateMap<PermissionRequest, Assignment>()
            //.ForMember(dest => dest.Id, src => src.MapFrom(s => PartitionKeyHelper.GenerateId("::", new string[] { "orgId", "userId" })));
            CreateMap<Assignment, PermissionResponse>().ReverseMap();
        }
    }
}
