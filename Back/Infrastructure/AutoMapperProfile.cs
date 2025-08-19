using AutoMapper;
using Dior.Database.DTOs.Access;
using Dior.Database.DTOs.AccessCompetency;
using Dior.Database.DTOs.AuditLog;
using Dior.Database.DTOs.Privilege;
using Dior.Database.DTOs.RoleDefinition;
using Dior.Database.DTOs.RoleDefinitionPrivilege;
using Dior.Database.DTOs.User;
using Dior.Database.DTOs.UserAccess;
using Dior.Database.DTOs.UserAccessCompetency;
using Dior.Database.DTOs.UserRole;
using Dior.Database.Entities;

namespace Dior.Database.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Access
            CreateMap<Access, AccessDto>();
            CreateMap<CreateAccessDto, Access>();
            CreateMap<UpdateAccessDto, Access>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // RoleDefinition
            CreateMap<RoleDefinition, RoleDefinitionDto>();
            CreateMap<CreateRoleDefinitionDto, RoleDefinition>();
            CreateMap<UpdateRoleDefinitionDto, RoleDefinition>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Privilege
            CreateMap<Privilege, PrivilegeDto>();
            CreateMap<CreatePrivilegeDto, Privilege>();
            CreateMap<UpdatePrivilegeDto, Privilege>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // AccessCompetency
            CreateMap<AccessCompetency, AccessCompetencyDto>();
            CreateMap<CreateAccessCompetencyDto, AccessCompetency>();
            CreateMap<UpdateAccessCompetencyDto, AccessCompetency>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // UserRole
            CreateMap<UserRole, UserRoleDto>();
            CreateMap<CreateUserRoleDto, UserRole>();
            CreateMap<UpdateUserRoleDto, UserRole>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // UserAccessCompetency
            CreateMap<UserAccessCompetency, UserAccessCompetencyDto>();
            CreateMap<CreateUserAccessCompetencyDto, UserAccessCompetency>();
            CreateMap<UpdateUserAccessCompetencyDto, UserAccessCompetency>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // UserAccess
            CreateMap<UserAccess, UserAccessDto>();
            CreateMap<CreateUserAccessDto, UserAccess>();
            CreateMap<UpdateUserAccessDto, UserAccess>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // RoleDefinitionPrivilege
            CreateMap<RoleDefinitionPrivilege, RoleDefinitionPrivilegeDto>();
            CreateMap<CreateRoleDefinitionPrivilegeDto, RoleDefinitionPrivilege>();
            CreateMap<UpdateRoleDefinitionPrivilegeDto, RoleDefinitionPrivilege>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // AuditLog
            CreateMap<AuditLog, AuditLogDto>();
            CreateMap<CreateAuditLogDto, AuditLog>();
        }
    }
}