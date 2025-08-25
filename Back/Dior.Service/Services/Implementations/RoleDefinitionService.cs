using AutoMapper;
using Dior.Database.Data;
using Dior.Data.DTOs.RoleDefinition;
using Dior.Data.Entities;
using Dior.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Database.Services.Implementations
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
            var role = _mapper.Map<RoleDefinition>(createRoleDefinitionDto);
            role.CreatedAt = DateTime.UtcNow;

            _context.RoleDefinitions.Add(role);
            await _context.SaveChangesAsync();

            return _mapper.Map<RoleDefinitionDto>(role);
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