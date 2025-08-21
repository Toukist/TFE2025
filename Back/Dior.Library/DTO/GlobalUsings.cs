// Imports centralisés pour tous les DTOs du nouveau namespace
// Ce fichier permet d'importer facilement tous les DTOs nécessaires

// User DTOs  
global using UserDto = Dior.Library.DTO.User.UserDto;
global using CreateUserDto = Dior.Library.DTO.User.CreateUserDto;
global using UpdateUserDto = Dior.Library.DTO.User.UpdateUserDto;
global using UserFullDto = Dior.Library.DTO.User.UserFullDto;
global using UserSummaryDto = Dior.Library.DTO.User.UserSummaryDto;

// Auth DTOs
global using LoginRequestDto = Dior.Library.DTO.Auth.LoginRequestDto;
global using LoginResponseDto = Dior.Library.DTO.Auth.LoginResponseDto;
global using LoginResponseCompleteDto = Dior.Library.DTO.Auth.LoginResponseCompleteDto;

// Access DTOs
global using AccessDto = Dior.Library.DTO.Access.AccessDto;
global using CreateAccessDto = Dior.Library.DTO.Access.CreateAccessDto;
global using UpdateAccessDto = Dior.Library.DTO.Access.UpdateAccessDto;
global using AccessCompetencyDto = Dior.Library.DTO.Access.AccessCompetencyDto;
global using CreateAccessCompetencyDto = Dior.Library.DTO.Access.CreateAccessCompetencyDto;
global using UpdateAccessCompetencyDto = Dior.Library.DTO.Access.UpdateAccessCompetencyDto;
global using UserAccessDto = Dior.Library.DTO.Access.UserAccessDto;
global using CreateUserAccessDto = Dior.Library.DTO.Access.CreateUserAccessDto;
global using UpdateUserAccessDto = Dior.Library.DTO.Access.UpdateUserAccessDto;
global using UserAccessCompetencyDto = Dior.Library.DTO.Access.UserAccessCompetencyDto;
global using CreateUserAccessCompetencyDto = Dior.Library.DTO.Access.CreateUserAccessCompetencyDto;

// Role DTOs
global using RoleDefinitionDto = Dior.Library.DTO.Role.RoleDefinitionDto;
global using CreateRoleDefinitionDto = Dior.Library.DTO.Role.CreateRoleDefinitionDto;
global using UpdateRoleDefinitionDto = Dior.Library.DTO.Role.UpdateRoleDefinitionDto;
global using PrivilegeDto = Dior.Library.DTO.Role.PrivilegeDto;
global using CreatePrivilegeDto = Dior.Library.DTO.Role.CreatePrivilegeDto;
global using UpdatePrivilegeDto = Dior.Library.DTO.Role.UpdatePrivilegeDto;
global using UserRoleDto = Dior.Library.DTO.Role.UserRoleDto;
global using CreateUserRoleDto = Dior.Library.DTO.Role.CreateUserRoleDto;
global using UpdateUserRoleDto = Dior.Library.DTO.Role.UpdateUserRoleDto;
global using RoleDefinitionPrivilegeDto = Dior.Library.DTO.Role.RoleDefinitionPrivilegeDto;
global using CreateRoleDefinitionPrivilegeDto = Dior.Library.DTO.Role.CreateRoleDefinitionPrivilegeDto;
global using UpdateRoleDefinitionPrivilegeDto = Dior.Library.DTO.Role.UpdateRoleDefinitionPrivilegeDto;

// Team DTOs
global using TeamDto = Dior.Library.DTO.Team.TeamDto;
global using CreateTeamDto = Dior.Library.DTO.Team.CreateTeamDto;
global using UpdateTeamDto = Dior.Library.DTO.Team.UpdateTeamDto;

// Audit DTOs
global using AuditLogDto = Dior.Library.DTO.Audit.AuditLogDto;
global using CreateAuditLogDto = Dior.Library.DTO.Audit.CreateAuditLogDto;

// Task DTOs
global using TaskDto = Dior.Library.DTO.Task.TaskDto;
global using CreateTaskDto = Dior.Library.DTO.Task.CreateTaskDto;
global using UpdateTaskDto = Dior.Library.DTO.Task.UpdateTaskDto;
global using TaskFilterDto = Dior.Library.DTO.Task.TaskFilterDto;
global using TaskSummaryDto = Dior.Library.DTO.Task.TaskSummaryDto;

// Contract DTOs
global using ContractDto = Dior.Library.DTO.Contract.ContractDto;
global using CreateContractDto = Dior.Library.DTO.Contract.CreateContractDto;
global using UpdateContractDto = Dior.Library.DTO.Contract.UpdateContractDto;

// Payroll DTOs
global using PayslipDto = Dior.Library.DTO.Payroll.PayslipDto;
global using CreatePayslipDto = Dior.Library.DTO.Payroll.CreatePayslipDto;
global using UpdatePayslipDto = Dior.Library.DTO.Payroll.UpdatePayslipDto;

// Notification DTOs
global using NotificationDto = Dior.Library.DTO.Notification.NotificationDto;
global using CreateNotificationDto = Dior.Library.DTO.Notification.CreateNotificationDto;
global using CreateNotificationRequest = Dior.Library.DTO.Notification.CreateNotificationRequest;

// Message DTOs
global using MessageDto = Dior.Library.DTO.Message.MessageDto;
global using CreateMessageDto = Dior.Library.DTO.Message.CreateMessageDto;
global using CreateMessageRequest = Dior.Library.DTO.Message.CreateMessageRequest;

// Project DTOs
global using ProjetDto = Dior.Library.DTO.Project.ProjetDto;
global using CreateProjetDto = Dior.Library.DTO.Project.CreateProjetDto;
global using UpdateProjetDto = Dior.Library.DTO.Project.UpdateProjetDto;

// Compatibility aliases for old DTOs
global using UpdateTaskRequest = Dior.Library.DTO.Task.UpdateTaskDto;
global using UpdateContractRequest = Dior.Library.DTO.Contract.UpdateContractDto;