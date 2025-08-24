using AutoMapper;
using Dior.Database.Services.Interfaces;
using Dior.Library.BO.UserInterface;
using Microsoft.EntityFrameworkCore;

namespace Dior.Database.Services.Implementations
{
    public class RoleDefinitionPrivilegeService : IRoleDefinitionPrivilegeService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        public RoleDefinitionPrivilegeService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDefinitionPrivilegeDto>> GetAllAsync()
        {
            var roleDefPrivileges = await _context.RoleDefinitionPrivileges.ToListAsync();
            return _mapper.Map<IEnumerable<RoleDefinitionPrivilegeDto>>(roleDefPrivileges);
        }

        public async Task<RoleDefinitionPrivilegeDto> GetByIdAsync(int id)
        {
            var roleDefPrivilege = await _context.RoleDefinitionPrivileges.FindAsync(id);
            return roleDefPrivilege != null ? _mapper.Map<RoleDefinitionPrivilegeDto>(roleDefPrivilege) : null;
        }

        public async Task<IEnumerable<RoleDefinitionPrivilegeDto>> GetByRoleDefinitionIdAsync(int roleDefinitionId)
        {
            var roleDefPrivileges = await _context.RoleDefinitionPrivileges
                .Where(rdp => rdp.RoleDefinitionId == roleDefinitionId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoleDefinitionPrivilegeDto>>(roleDefPrivileges);
        }

        public async Task<RoleDefinitionPrivilegeDto> CreateAsync(CreateRoleDefinitionPrivilegeDto createDto)
        {
            var roleDefPrivilege = _mapper.Map<RoleDefinitionPrivilege>(createDto);
            roleDefPrivilege.CreatedAt = DateTime.UtcNow;

            _context.RoleDefinitionPrivileges.Add(roleDefPrivilege);
            await _context.SaveChangesAsync();

            return _mapper.Map<RoleDefinitionPrivilegeDto>(roleDefPrivilege);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRoleDefinitionPrivilegeDto updateDto)
        {
            var roleDefPrivilege = await _context.RoleDefinitionPrivileges.FindAsync(id);
            if (roleDefPrivilege == null)
                return false;

            _mapper.Map(updateDto, roleDefPrivilege);
            roleDefPrivilege.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var roleDefPrivilege = await _context.RoleDefinitionPrivileges.FindAsync(id);
            if (roleDefPrivilege == null)
                return false;

            _context.RoleDefinitionPrivileges.Remove(roleDefPrivilege);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}