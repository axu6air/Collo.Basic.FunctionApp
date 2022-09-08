using AutoMapper;
using Collo.Cloud.IAM.Api.Models.Assignment;
using Collo.Cloud.IAM.Api.Models.Organization;
using Collo.Cloud.IAM.Api.Models.Request;
using Collo.Cloud.IAM.Api.Models.Response;
using Collo.Cloud.IAM.Api.Models.User;
using Collo.Cloud.Services.Libraries.Shared.Permission.Models;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Instrument;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using UpdateRoleRequestModel = Collo.Cloud.IAM.Api.Models.Request.UpdateRoleRequestModel;

namespace Collo.Cloud.IAM.Api.Mappers
{
    public class IamMapper : Profile
    {
        public IamMapper()
        {
            #region Organizations
            CreateMap<OrganizationSetting, Setting>().ReverseMap();
            CreateMap<CreateOrgRequestModel, Organization>();
            CreateMap<Organization, CreateOrgRequestModel>();

            CreateMap<Organization, CreateOrgResponeModel>();
            CreateMap<UpdateOrgRequestModel, Organization>();
            CreateMap<Organization, UpdateOrgResponseModel>();
            CreateMap<Organization, GetOrgResponseModel>();
            #endregion

            #region Roles
            CreateMap<Organization, RoleResponseModel>();
            CreateMap<CreateRoleRequestModel, Role>();
            CreateMap<Organization, RoleResponseModel>();
            CreateMap<Role, GetRoleResponseModel>();
            CreateMap<UpdateRoleRequestModel, Role>();
            CreateMap<Role, UpdateRoleResponseModel>();
            CreateMap<Role, RoleResponseModel>();
            CreateMap<Role, UpdateRoleResponseModel>();
            #endregion

            #region Users

            CreateMap<User, CreateUserRequestModel>().ReverseMap();
            CreateMap<User, CreateLocalUserRequestModel>().ReverseMap();
            CreateMap<User, GetUserResponseModel>().ReverseMap();

            #endregion

            #region Permission
            CreateMap<CreateAssignmentRequestModel, PermissionRequest>();
            CreateMap<PermissionResponse, CreateUpdateAssignmentResponseModel>();
            CreateMap<AssignmentRequestModel, AssignmentModel>().ReverseMap();
            CreateMap<RolePermissionModel, PermissionModel>();
            #endregion

            #region Instrument
            CreateMap<CreateInstrumentRequestModel, Instrument>();
            CreateMap<Instrument, InstrumentResponseModel>();
            CreateMap<AllocateInstrumentRequestModel, AllocatedInstrument>().ReverseMap();
            CreateMap<Instrument, AllocatedInstrument>();
            CreateMap<Instrument, AllocateInstrumentResponseModel>();
            CreateMap<UpdateInstrumentRequestModel, Instrument>();
            #endregion

            #region Site
            CreateMap<Site, SiteResponseModel>();
            CreateMap<CreateSiteRequestModel, Site>();
            CreateMap<UpdateSiteRequestModel, Site>();
            CreateMap<Site, UpdateSiteResponseModel>();
            #endregion

            #region Gateway
            CreateMap<Gateway, GatewayResponseModel>();
            CreateMap<CreateGatewayRequestModel, Gateway>();
            CreateMap<UpdateGatewayRequestModel, Gateway>();
            CreateMap<Gateway, UpdateGatewayResponseModel>();
            #endregion
        }

    }

    public static class CustomMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<IamMapper>();
            });
            return config.CreateMapper();
        });

        public static IMapper Mapper => Lazy.Value;
    }
}
