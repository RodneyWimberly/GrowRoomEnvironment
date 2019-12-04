using AutoMapper;
using GrowRoomEnvironment.DataAccess.Core;
using GrowRoomEnvironment.DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace GrowRoomEnvironment.Web.ViewModels
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                   .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserViewModel, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<ApplicationUser, UserEditViewModel>()
                .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserEditViewModel, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<ApplicationUser, UserPatchViewModel>()
                .ReverseMap();

            CreateMap<ApplicationRole, RoleViewModel>()
                .ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
                .ForMember(d => d.UsersCount, map => map.MapFrom(s => s.Users != null ? s.Users.Count : 0))
                .ReverseMap();
            CreateMap<RoleViewModel, ApplicationRole>()
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<IdentityRoleClaim<string>, ClaimViewModel>()
                .ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
                .ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
                .ReverseMap();

            CreateMap<ApplicationPermission, PermissionViewModel>()
                .ReverseMap();

            CreateMap<IdentityRoleClaim<string>, PermissionViewModel>()
                .ConvertUsing(s => (PermissionViewModel)ApplicationPermissions.GetPermissionByValue(s.ClaimValue));

            CreateMap<ExtendedLog, ExtendedLogViewModel>();
            CreateMap<ExtendedLogViewModel, ExtendedLog>()
                .ForMember(e => e.Browser, map => map.Ignore())
                .ForMember(e => e.Host, map => map.Ignore())
                .ForMember(e => e.Path, map => map.Ignore())
                .ForMember(e => e.User, map => map.Ignore());

            CreateMap<ActionDevice, ActionDeviceViewModel>()
                .ReverseMap();

            CreateMap<Event, EventViewModel>()
                .ReverseMap();

            CreateMap<EventCondition, EventConditionViewModel>()
                .ReverseMap();

            CreateMap<DataPoint, DataPointViewModel>()
                .ReverseMap();

            CreateMap<Notification, NotificationViewModel>()
               .ReverseMap();
        }
    }
}
