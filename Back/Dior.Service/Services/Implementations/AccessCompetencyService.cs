using AutoMapper;
using Dior.Data.Services.Interfaces;
using Dior.Database.Services.Interfaces;
using Dior.Library.BO.UserInterface;
using Microsoft.EntityFrameworkCore;

namespace Dior.Database.Services.Implementations
{
    /// <summary>
    /// Service de gestion des AccessCompetency
    /// </summary>
    public class AccessCompetencyService : IAccessCompetencyService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>Constructor</summary>
        public AccessCompetencyService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>Retourne toutes les compétences d'accès</summary>
        public async Task<IEnumerable<AccessCompetencyDto>> GetAllAsync()
        {
            var competencies = await _context.AccessCompetencies.ToListAsync();
            return _mapper.Map<IEnumerable<AccessCompetencyDto>>(competencies);
        }

        /// <summary>Retourne une compétence par id</summary>
        public async Task<AccessCompetencyDto> GetByIdAsync(long id)
        {
            var competency = await _context.AccessCompetencies.FindAsync(id);
            return competency != null ? _mapper.Map<AccessCompetencyDto>(competency) : null;
        }

<<<<<<< Updated upstream
        
=======
        /// <summary>Retourne les enfants d'une compétence</summary>
        public async Task<IEnumerable<AccessCompetencyDto>> GetChildrenAsync(long parentId)
        {
            var children = await _context.AccessCompetencies
                .Where(ac => ac.ParentId == parentId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AccessCompetencyDto>>(children);
        }
>>>>>>> Stashed changes

        /// <summary>Crée une compétence</summary>
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

        /// <summary>Met à jour une compétence</summary>
        public async Task<bool> UpdateAsync(long id, UpdateAccessCompetencyDto updateDto)
        {
            var competency = await _context.AccessCompetencies.FindAsync(id);
            if (competency == null)
                return false;

            _mapper.Map(updateDto, competency);
            competency.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Supprime une compétence</summary>
        public async Task<bool> DeleteAsync(long id)
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