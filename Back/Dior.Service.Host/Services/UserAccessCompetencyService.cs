namespace Dior.Service.Host.Services
{
    using Dior.Library.Entities;
    using Dior.Library.Interfaces.UserInterface.Services;
    using Dior.Service.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserAccessCompetencyService : IUserAccessCompetencyService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<UserAccessCompetencyService> _logger;

        public UserAccessCompetencyService(DiorDbContext context, ILogger<UserAccessCompetencyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int Add(UserAccessCompetencyDto item, string editBy)
        {
            var entity = new UserAccessCompetency
            {
                UserId = item.UserId,
                AccessCompetencyId = item.AccessCompetencyId,
                LastEditBy = editBy,
                LastEditAt = DateTime.UtcNow
            };

            _context.UserAccessCompetencies.Add(entity);
            _context.SaveChanges();

            return entity.Id;
        }

        public void Del(int id)
        {
            var entity = _context.UserAccessCompetencies.FirstOrDefault(e => e.Id == id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"UserAccessCompetency avec Id={id} non trouvé.");
            }

            _context.UserAccessCompetencies.Remove(entity);
            _context.SaveChanges();
        }

        public UserAccessCompetencyDto? Get(int id)
        {
            var entity = _context.UserAccessCompetencies
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == id);

            if (entity == null)
                return null;

            return new UserAccessCompetencyDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                AccessCompetencyId = entity.AccessCompetencyId
            };
        }

        public List<UserAccessCompetencyDto> GetList()
        {
            var entities = _context.UserAccessCompetencies.AsNoTracking().ToList();
            var dtos = new List<UserAccessCompetencyDto>();

            foreach (var entity in entities)
            {
                dtos.Add(new UserAccessCompetencyDto
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    AccessCompetencyId = entity.AccessCompetencyId
                });
            }

            return dtos;
        }

        public List<UserAccessCompetencyDto> GetListByUserId(int userId)
        {
            var entities = _context.UserAccessCompetencies
                .AsNoTracking()
                .Where(uac => uac.UserId == userId)
                .ToList();

            var dtos = new List<UserAccessCompetencyDto>();

            foreach (var entity in entities)
            {
                dtos.Add(new UserAccessCompetencyDto
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    AccessCompetencyId = entity.AccessCompetencyId
                });
            }

            return dtos;
        }

        public bool HasAccessCompetency(int userId, string competencyName)
        {
            return _context.UserAccessCompetencies
                .Include(uac => uac.AccessCompetency)
                .Any(uac => uac.UserId == userId && uac.AccessCompetency.Name == competencyName);
        }

        public void Set(UserAccessCompetencyDto item, string editBy)
        {
            var entity = _context.UserAccessCompetencies.FirstOrDefault(uac => uac.Id == item.Id);
            if (entity != null)
            {
                entity.UserId = item.UserId;
                entity.AccessCompetencyId = item.AccessCompetencyId;
                entity.LastEditBy = editBy;
                entity.LastEditAt = DateTime.UtcNow;

                _context.SaveChanges();
            }
            else
            {
                throw new KeyNotFoundException($"UserAccessCompetency avec Id={item.Id} non trouvé.");
            }
        }
    }
}
