using AutoMapper;
using Dior.Database.Data;
using Dior.Database.DTOs.AccessCompetency;
using Dior.Database.Entities;
using Dior.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Database.Services.Implementations
{
    public class AccessCompetencyService : IAccessCompetencyService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        public AccessCompetencyService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccessCompetencyDto>> GetAllAsync()
        {
            var competencies = await _context.AccessCompetencies.ToListAsync();
            return _mapper.Map<IEnumerable<AccessCompetencyDto>>(competencies);
        }

        public async Task<AccessCompetencyDto> GetByIdAsync(int id)
        {
            var competency = await _context.AccessCompetencies.FindAsync(id);
            return competency != null ? _mapper.Map<AccessCompetencyDto>(competency) : null;
        }

        public async Task<IEnumerable<AccessCompetencyDto>> GetChildrenAsync(int parentId)
        {
            var children = await _context.AccessCompetencies
                .Where(ac => ac.ParentId == parentId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AccessCompetencyDto>>(children);
        }

        public async Task<AccessCompetencyDto> CreateAsync(CreateAccessCompetencyDto createDto)
        {
            var competency = _mapper.Map<AccessCompetency>(createDto);
            competency.CreatedAt = DateTime.UtcNow;

            _context.AccessCompetencies.Add(competency);
            await _context.SaveChangesAsync();

            return _mapper.Map<AccessCompetencyDto>(competency);
        }

        public async Task<bool> UpdateAsync(int id, UpdateAccessCompetencyDto updateDto)
        {
            var competency = await _context.AccessCompetencies.FindAsync(id);
            if (competency == null)
                return false;

            _mapper.Map(updateDto, competency);
            competency.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var competency = await _context.AccessCompetencies.FindAsync(id);
            if (competency == null)
                return false;

            _context.AccessCompetencies.Remove(competency);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}