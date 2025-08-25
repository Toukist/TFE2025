using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Entities;
using Dior.Library.DTO.Access.Dior.Data.DTO.UserAccessCompetency;

namespace Dior.Data.Services.Implementations
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

        public List<UserAccessCompetencyDto> GetList()
        {
            throw new NotImplementedException();
        }

        public UserAccessCompetencyDto? Get(int id)
        {
            throw new NotImplementedException();
        }

        public List<UserAccessCompetencyDto> GetListByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public int Add(UserAccessCompetencyDto item, string editBy)
        {
            throw new NotImplementedException();
        }

        public void Set(UserAccessCompetencyDto item, string editBy)
        {
            throw new NotImplementedException();
        }

        public void Del(int id)
        {
            throw new NotImplementedException();
        }

        public bool HasAccessCompetency(int userId, string competencyName)
        {
            throw new NotImplementedException();
        }
    }
}