using AutoMapper;
<<<<<<< Updated upstream
using Dior.Data.Services.Interfaces;
using Dior.Database.Services.Interfaces;
using Dior.Library.BO.UserInterface;
=======
using Dior.Service.Host.Services;
using Dior.Library.DTO.Role;
>>>>>>> Stashed changes
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Services.Implementations
{
    /// <summary>
    /// Service pour la gestion des privilèges
    /// </summary>
    public class PrivilegeService : IPrivilegeService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>Constructor</summary>
        public PrivilegeService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>Retourne tous les privilèges</summary>
        public async Task<IEnumerable<PrivilegeDto>> GetAllAsync()
        {
            var privileges = await _context.Privileges.ToListAsync();
            return _mapper.Map<IEnumerable<PrivilegeDto>>(privileges);
        }

        /// <summary>Retourne un privilège par id</summary>
        public async Task<PrivilegeDto?> GetByIdAsync(long id)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            return privilege != null ? _mapper.Map<PrivilegeDto>(privilege) : null;
        }

        /// <summary>Crée un privilège</summary>
        public async Task<PrivilegeDto> CreateAsync(CreatePrivilegeDto createPrivilegeDto)
        {
<<<<<<< Updated upstream
            // Correction : utiliser le bon type d'entité pour l'ajout dans le DbSet
=======
            // Correction : Mapper vers Dior.Library.Entities.Privilege
>>>>>>> Stashed changes
            var privilege = _mapper.Map<Dior.Library.Entities.Privilege>(createPrivilegeDto);

            _context.Privileges.Add(privilege);
            await _context.SaveChangesAsync();

            return _mapper.Map<PrivilegeDto>(privilege);
        }

        /// <summary>Met à jour un privilège</summary>
        public async Task<bool> UpdateAsync(long id, UpdatePrivilegeDto updatePrivilegeDto)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege == null)
                return false;

            _mapper.Map(updatePrivilegeDto, privilege);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Supprime un privilège</summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege == null)
                return false;

            _context.Privileges.Remove(privilege);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}