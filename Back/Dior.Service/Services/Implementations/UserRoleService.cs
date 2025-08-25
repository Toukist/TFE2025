using AutoMapper;
using Dior.Database.Data;
using Dior.Data.DTOs.UserRole;
using Dior.Data.Entities;
using Dior.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Database.Services.Implementations
{
    public class UserRoleService : IUserRoleService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        public UserRoleService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserRoleDto>> GetAllAsync()
        {
            var userRoles = await _context.UserRoles.ToListAsync();
            return _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);
        }

        public async Task<UserRoleDto> GetByIdAsync(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            return userRole != null ? _mapper.Map<UserRoleDto>(userRole) : null;
        }

        public async Task<IEnumerable<UserRoleDto>> GetByUserIdAsync(int userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);
        }

        public async Task<UserRoleDto> CreateAsync(CreateUserRoleDto createDto)
        {
            var userRole = _mapper.Map<UserRole>(createDto);
            userRole.CreatedAt = DateTime.UtcNow;

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserRoleDto>(userRole);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserRoleDto updateDto)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
                return false;

            _mapper.Map(updateDto, userRole);
            userRole.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
                return false;

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}