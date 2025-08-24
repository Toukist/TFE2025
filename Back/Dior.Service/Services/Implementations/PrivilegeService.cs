using AutoMapper;
using Dior.Database.Services.Interfaces;
using Dior.Library.BO.UserInterface;
using Microsoft.EntityFrameworkCore;

namespace Dior.Database.Services.Implementations
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        public PrivilegeService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PrivilegeDto>> GetAllAsync()
        {
            var privileges = await _context.Privileges.ToListAsync();
            return _mapper.Map<IEnumerable<PrivilegeDto>>(privileges);
        }

        public async Task<PrivilegeDto> GetByIdAsync(int id)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            return privilege != null ? _mapper.Map<PrivilegeDto>(privilege) : null;
        }

        public async Task<PrivilegeDto> CreateAsync(CreatePrivilegeDto createPrivilegeDto)
        {
            var privilege = _mapper.Map<Privilege>(createPrivilegeDto);

            _context.Privileges.Add(privilege);
            await _context.SaveChangesAsync();

            return _mapper.Map<PrivilegeDto>(privilege);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePrivilegeDto updatePrivilegeDto)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege == null)
                return false;

            _mapper.Map(updatePrivilegeDto, privilege);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
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