using AutoMapper;
using Dior.Data.Services.Interfaces;
using Dior.Library.BO.UserInterface;
using Microsoft.EntityFrameworkCore;

namespace Dior.Data.Services.Implementations
{
    public class RoleDefinitionService : IRoleDefinitionService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        public RoleDefinitionService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDefinitionDto>> GetAllAsync()
        {
            var roles = await _context.RoleDefinitions.ToListAsync();
            return _mapper.Map<IEnumerable<RoleDefinitionDto>>(roles);
        }

        public async Task<RoleDefinitionDto> GetByIdAsync(int id)
        {
            var role = await _context.RoleDefinitions.FindAsync(id);
            return role != null ? _mapper.Map<RoleDefinitionDto>(role) : null;
        }

        public async Task<IEnumerable<RoleDefinitionDto>> GetChildRolesAsync(int parentId)
        {
            var childRoles = await _context.RoleDefinitions
                .Where(r => r.ParentRoleId == parentId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<RoleDefinitionDto>>(childRoles);
        }

        public async Task<RoleDefinitionDto> CreateAsync(CreateRoleDefinitionDto createRoleDefinitionDto)
        {
            // Correction : Utiliser le bon type d'entité pour l'ajout dans le DbSet
            var roleEntity = _mapper.Map<Dior.Library.Entities.RoleDefinition>(createRoleDefinitionDto);
            roleEntity.CreatedAt = DateTime.UtcNow;

            _context.RoleDefinitions.Add(roleEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<RoleDefinitionDto>(roleEntity);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRoleDefinitionDto updateRoleDefinitionDto)
        {
            var role = await _context.RoleDefinitions.FindAsync(id);
            if (role == null)
                return false;

            _mapper.Map(updateRoleDefinitionDto, role);
            role.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.RoleDefinitions.FindAsync(id);
            if (role == null)
                return false;

            _context.RoleDefinitions.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}