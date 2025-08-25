using AutoMapper;
using Dior.Data.Services.Interfaces;
using Dior.Database.Services.Interfaces;
using Dior.Library.BO.UserInterface;
using Microsoft.EntityFrameworkCore;

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

        

        public async Task<AccessCompetencyDto> CreateAsync(CreateAccessCompetencyDto createDto)
        {
            // Remplacez la ligne suivante :
            // var competency = _mapper.Map<AccessCompetency>(createDto);
            // par :
            var competency = _mapper.Map<Dior.Library.Entities.AccessCompetency>(createDto);
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

        public Task<IEnumerable<AccessCompetencyDto>> GetChildrenAsync(int parentId)
        {
            throw new NotImplementedException();
        }
    }
}