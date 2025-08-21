using AutoMapper;
using Dior.Database.Data;
using Dior.Database.DTOs.UserAccessCompetency;
using Dior.Database.Entities;
using Dior.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Database.Services.Implementations
{
    public class UserAccessCompetencyService : IUserAccessCompetencyService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        public UserAccessCompetencyService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserAccessCompetencyDto>> GetAllAsync()
        {
            var userAccessCompetencies = await _context.UserAccessCompetencies.ToListAsync();
            return _mapper.Map<IEnumerable<UserAccessCompetencyDto>>(userAccessCompetencies);
        }

        public async Task<UserAccessCompetencyDto> GetByIdAsync(int id)
        {
            var userAccessCompetency = await _context.UserAccessCompetencies.FindAsync(id);
            return userAccessCompetency != null ? _mapper.Map<UserAccessCompetencyDto>(userAccessCompetency) : null;
        }

        public async Task<IEnumerable<UserAccessCompetencyDto>> GetByUserIdAsync(int userId)
        {
            var userAccessCompetencies = await _context.UserAccessCompetencies
                .Where(uac => uac.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserAccessCompetencyDto>>(userAccessCompetencies);
        }

        public async Task<UserAccessCompetencyDto> CreateAsync(CreateUserAccessCompetencyDto createDto)
        {
            var userAccessCompetency = _mapper.Map<UserAccessCompetency>(createDto);
            userAccessCompetency.CreatedAt = DateTime.UtcNow;

            _context.UserAccessCompetencies.Add(userAccessCompetency);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserAccessCompetencyDto>(userAccessCompetency);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserAccessCompetencyDto updateDto)
        {
            var userAccessCompetency = await _context.UserAccessCompetencies.FindAsync(id);
            if (userAccessCompetency == null)
                return false;

            _mapper.Map(updateDto, userAccessCompetency);
            userAccessCompetency.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var userAccessCompetency = await _context.UserAccessCompetencies.FindAsync(id);
            if (userAccessCompetency == null)
                return false;

            _context.UserAccessCompetencies.Remove(userAccessCompetency);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}