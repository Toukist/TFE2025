using Dior.Library.DTO;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;

namespace Dior.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationDao _dao;
        
        public NotificationService(INotificationDao dao)
        {
            _dao = dao;
        }

        // ===== MÉTHODES EXISTANTES (préservées pour compatibilité) =====
        public List<NotificationDto> GetByUserId(int userId) => _dao.GetByUserId(userId);
        public void MarkAsRead(int id) => _dao.MarkAsRead(id);
        public void Delete(int id) => _dao.Delete(id);
        public int Add(NotificationDto dto, string createdBy) => _dao.Add(dto, createdBy);

        public void Add(NotificationDto notificationDto)
        {
            Add(notificationDto, "System");
        }

        // ===== NOUVELLES MÉTHODES ASYNC POUR ANGULAR =====

        /// <summary>
        /// Récupère toutes les notifications d'un utilisateur
        /// </summary>
        public async Task<List<NotificationDto>> GetByUserIdAsync(long userId)
        {
            return await Task.Run(() => GetByUserId((int)userId));
        }

        /// <summary>
        /// Récupère les notifications non lues d'un utilisateur
        /// </summary>
        public async Task<List<NotificationDto>> GetUnreadByUserIdAsync(long userId)
        {
            return await Task.Run(() => 
                GetByUserId((int)userId).Where(n => !n.IsRead).ToList());
        }

        /// <summary>
        /// Marque une notification comme lue
        /// </summary>
        public async Task<bool> MarkAsReadAsync(int id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    MarkAsRead(id);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Marque toutes les notifications d'un utilisateur comme lues
        /// </summary>
        public async Task<bool> MarkAllAsReadAsync(long userId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var unreadNotifications = GetByUserId((int)userId)
                        .Where(n => !n.IsRead).ToList();
                    
                    foreach (var notification in unreadNotifications)
                    {
                        MarkAsRead(notification.Id);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Crée une nouvelle notification
        /// </summary>
        public async Task<NotificationDto> CreateAsync(CreateNotificationRequest request, string createdBy = "System")
        {
            return await Task.Run(() =>
            {
                var dto = new NotificationDto
                {
                    UserId = request.UserId, // long -> long maintenant compatible
                    Type = request.Type,
                    Message = request.Message,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };

                var newId = Add(dto, createdBy);
                dto.Id = newId;
                return dto;
            });
        }

        /// <summary>
        /// Envoie des notifications en lot à plusieurs utilisateurs
        /// </summary>
        public async Task<bool> SendBulkNotificationsAsync(List<long> userIds, string message, string type, string createdBy = "System")
        {
            return await Task.Run(() =>
            {
                try
                {
                    foreach (var userId in userIds)
                    {
                        var dto = new NotificationDto
                        {
                            UserId = userId, // long -> long maintenant compatible
                            Type = type,
                            Message = message,
                            IsRead = false,
                            CreatedAt = DateTime.Now,
                            CreatedBy = createdBy
                        };

                        Add(dto, createdBy);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Supprime une notification de manière asynchrone
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Delete(id);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<NotificationDto>> GetAllNotificationsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<NotificationDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationDto> CreateAsync(CreateNotificationDto createDto)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}