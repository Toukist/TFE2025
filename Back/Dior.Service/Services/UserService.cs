using AutoMapper;
using BCrypt.Net;
using Dior.Database.DTOs.User;
using Dior.Library.DTOs;
using Dior.Library.Entities;
using Dior.Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Services;

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
        var users = await _context.Users
            .Include(u => u.Team)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Team)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
            .FirstOrDefaultAsync(u => u.Email == email);

        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        var user = _mapper.Map<User>(createUserDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        user.CreatedAt = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _mapper.Map(updateUserDto, user);
        user.LastEditAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AuthenticateAsync(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}