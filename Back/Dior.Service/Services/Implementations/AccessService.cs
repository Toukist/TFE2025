using AutoMapper;
using Dior.Database.Data;
using Dior.Database.DTOs.Access;
using Dior.Database.Entities;
using Dior.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Database.Services.Implementations
{
    public class AccessService : IAccessService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        public AccessService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccessDto>> GetAllAsync()
        {
            var accesses = await _context.Accesses.ToListAsync();
            return _mapper.Map<IEnumerable<AccessDto>>(accesses);
        }

        public async Task<AccessDto> GetByIdAsync(int id)
        {
            var access = await _context.Accesses.FindAsync(id);
            return access != null ? _mapper.Map<AccessDto>(access) : null;
        }

        public async Task<AccessDto> CreateAsync(CreateAccessDto createAccessDto)
        {
            var access = _mapper.Map<Access>(createAccessDto);
            access.CreatedAt = DateTime.UtcNow;

            _context.Accesses.Add(access);
            await _context.SaveChangesAsync();

            return _mapper.Map<AccessDto>(access);
        }

        public async Task<bool> UpdateAsync(int id, UpdateAccessDto updateAccessDto)
        {
            var access = await _context.Accesses.FindAsync(id);
            if (access == null)
                return false;

            _mapper.Map(updateAccessDto, access);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var access = await _context.Accesses.FindAsync(id);
            if (access == null)
                return false;

            _context.Accesses.Remove(access);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}