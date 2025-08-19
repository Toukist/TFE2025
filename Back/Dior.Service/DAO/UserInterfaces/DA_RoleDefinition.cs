using Dior.Library.Service.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO.UserInterfaces
{
    public class DA_RoleDefinition : IDA_RoleDefinition
    {
        private readonly string _connectionString;

        public DA_RoleDefinition(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey);
        }

        public long Add(RoleDefinition roleDefinition, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Add", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_Name", roleDefinition.Name);
            cmd.Parameters.AddWithValue("@PR_Description", roleDefinition.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PR_ParentRoleId", roleDefinition.ParentRoleId.HasValue ? roleDefinition.ParentRoleId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PR_IsActive", roleDefinition.IsActive);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy);

            var outParam = new SqlParameter("@PR_ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();

                return outParam.Value != DBNull.Value ? Convert.ToInt32(outParam.Value) : -1;
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de l'ajout de la définition de rôle : " + ex.Message, ex);
            }
        }

        public void Set(RoleDefinition roleDefinition, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Set", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", roleDefinition.Id);
            cmd.Parameters.AddWithValue("@PR_Name", roleDefinition.Name);
            cmd.Parameters.AddWithValue("@PR_Description", roleDefinition.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PR_ParentRoleId", roleDefinition.ParentRoleId.HasValue ? roleDefinition.ParentRoleId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PR_IsActive", roleDefinition.IsActive);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la mise à jour de la définition de rôle : " + ex.Message, ex);
            }
        }

        public void Del(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Del", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la suppression de la définition de rôle : " + ex.Message, ex);
            }
        }

        // Méthode Get avec un objet RoleDefinition (existante)
        public RoleDefinition Get(RoleDefinition roleDefinition)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Get", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", roleDefinition.Id);

            try
            {
                conn.Open();
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new RoleDefinition
                    {
                        Id = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("Id"))),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        ParentRoleId = reader.IsDBNull(reader.GetOrdinal("ParentRoleId")) ? null : (int?)Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("ParentRoleId"))),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                        LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                        LastEditBy = reader.IsDBNull(reader.GetOrdinal("LastEditBy")) ? null : reader.GetString(reader.GetOrdinal("LastEditBy"))
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la récupération de la définition de rôle : " + ex.Message, ex);
            }
        }

        // Méthode Get avec un ID long
        public RoleDefinition Get(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Get", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);

            try
            {
                conn.Open();
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new RoleDefinition
                    {
                        Id = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("Id"))),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        ParentRoleId = reader.IsDBNull(reader.GetOrdinal("ParentRoleId")) ? null : (int?)Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("ParentRoleId"))),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                        LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                        LastEditBy = reader.IsDBNull(reader.GetOrdinal("LastEditBy")) ? null : reader.GetString(reader.GetOrdinal("LastEditBy"))
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la récupération de la définition de rôle : " + ex.Message, ex);
            }
        }

        // Méthode Get avec un ID int (pour satisfaire l'interface Dior.Library.Interfaces.UserInterface.Services.IDA_RoleDefinition)
        public RoleDefinition Get(int id)
        {
            // Réutilise la méthode Get(long id) en convertissant l'ID int en long
            return Get((long)id);
        }

        public List<RoleDefinition> GetList()
        {
            var roleDefinitions = new List<RoleDefinition>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_GetList", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            try
            {
                conn.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var roleDefinition = new RoleDefinition
                    {
                        Id = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("Id"))),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        ParentRoleId = reader.IsDBNull(reader.GetOrdinal("ParentRoleId")) ? null : (int?)Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("ParentRoleId"))),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                        LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                        LastEditBy = reader.IsDBNull(reader.GetOrdinal("LastEditBy")) ? null : reader.GetString(reader.GetOrdinal("LastEditBy"))
                    };

                    roleDefinitions.Add(roleDefinition);
                }

                return roleDefinitions;
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la récupération de la liste des définitions de rôle : " + ex.Message, ex);
            }
        }
    }
}