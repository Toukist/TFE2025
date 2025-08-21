using AutoMapper;
using Dior.Database.Data;
using Dior.Database.DTOs.User;
using Dior.Database.Entities;
using Dior.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Database.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        public UserService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            user.CreatedAt = DateTime.UtcNow;
            
            // Hash the password before saving
            if (!string.IsNullOrEmpty(createUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _mapper.Map(updateUserDto, user);
            user.LastEditAt = DateTime.UtcNow;
            
            // Only update password if provided
            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}