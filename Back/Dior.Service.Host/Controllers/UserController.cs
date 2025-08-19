// UserController.cs - Code corrigé
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // Remplacez le type de retour de MapToFullDto par UserFullDto
        private static UserFullDto MapToFullDto(object bo)
        {
            var team = TryGetObj(bo, "Team");
            var access = TryGetObj(bo, "Access");

            int? badgePhysicalNumber = null;
            var badgeValue = TryGetProp(access, "BadgePhysicalNumber");
            if (badgeValue != null && int.TryParse(badgeValue.ToString(), out int badgeInt))
            {
                badgePhysicalNumber = badgeInt;
            }

            // Correction : instanciation d'un UserFullDto au lieu d'un UserDto
            return new UserFullDto
            {
                Id = ConvertToLong(TryGetId(bo)),
                IsActive = TryGetBool(bo, "IsActive"),
                Username = TryGetProp(bo, "Username") ?? string.Empty,  
                FirstName = TryGetProp(bo, "FirstName") ?? string.Empty,
                LastName = TryGetProp(bo, "LastName") ?? string.Empty,
                Email = TryGetProp(bo, "Email") ?? string.Empty,
                Phone = TryGetProp(bo, "Phone"),
                TeamId = ConvertToInt(TryGetId(team)),
                TeamName = TryGetProp(team, "Name") ?? string.Empty,
                BadgePhysicalNumber = badgePhysicalNumber,
                // Ajoutez ici l'initialisation de Roles si nécessaire
                // Roles = ... (exemple : TryGetRoles(bo))
            };
        }

        // Méthodes utilitaires (si elles n'existent pas déjà)
        private static object TryGetObj(object source, string propertyName)
        {
            try
            {
                var property = source?.GetType().GetProperty(propertyName);
                return property?.GetValue(source);
            }
            catch
            {
                return null;
            }
        }

        private static string TryGetProp(object source, string propertyName)
        {
            try
            {
                var property = source?.GetType().GetProperty(propertyName);
                return property?.GetValue(source)?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static bool TryGetBool(object source, string propertyName)
        {
            try
            {
                var property = source?.GetType().GetProperty(propertyName);
                var value = property?.GetValue(source);
                if (value is bool boolValue)
                    return boolValue;
                if (bool.TryParse(value?.ToString(), out bool result))
                    return result;
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static object TryGetId(object source)
        {
            try
            {
                var property = source?.GetType().GetProperty("Id") ??
                              source?.GetType().GetProperty("ID");
                return property?.GetValue(source);
            }
            catch
            {
                return null;
            }
        }

        private static long ConvertToLong(object value)
        {
            try
            {
                if (value == null) return 0;
                if (long.TryParse(value.ToString(), out long result))
                    return result;
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private static int ConvertToInt(object value)
        {
            try
            {
                if (value == null) return 0;
                if (int.TryParse(value.ToString(), out int result))
                    return result;
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        // NOUVELLE méthode pour gérer les int? correctement
        private static int? TryParseNullableInt(object value)
        {
            if (value == null) return null;

            string stringValue = value.ToString();
            if (string.IsNullOrWhiteSpace(stringValue)) return null;

            if (int.TryParse(stringValue, out int result))
                return result;

            return null;
        }
    }

    // DTO classes (si elles n'existent pas déjà)
    public class UserDto
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public string Username { get; set; }
        public string? UserName { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int? BadgePhysicalNumber { get; set; }
    }

    public class UserFullDto : UserDto
    {
        // Ajoutez ici les propriétés supplémentaires si nécessaire
        public List<string> Roles { get; set; } = new List<string>();
    }
}