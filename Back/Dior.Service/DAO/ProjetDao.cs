using Dior.Library.BO;
using Dior.Library.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO
{
    public class ProjetDao : IProjetDao
    {
        private readonly string _connectionString;

        public ProjetDao(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"] ?? "DIOR_DB";
            _connectionString = configuration.GetConnectionString(activeDbKey)
                ?? throw new InvalidOperationException($"Connection string '{activeDbKey}' not found");
        }

        public List<Projet> GetAll()
        {
            var projets = new List<Projet>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                SELECT Id, Nom, Description, DateDebut, DateFin, TeamId, 
                       CreatedAt, CreatedBy, LastEditAt, LastEditBy 
                FROM Projet 
                ORDER BY CreatedAt DESC", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                projets.Add(MapFromReader(reader));
            }

            return projets;
        }

        public Projet? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                SELECT Id, Nom, Description, DateDebut, DateFin, TeamId, 
                       CreatedAt, CreatedBy, LastEditAt, LastEditBy 
                FROM Projet 
                WHERE Id = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();

            return reader.Read() ? MapFromReader(reader) : null;
        }

        public List<Projet> GetByTeamId(int teamId)
        {
            var projets = new List<Projet>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                SELECT Id, Nom, Description, DateDebut, DateFin, TeamId, 
                       CreatedAt, CreatedBy, LastEditAt, LastEditBy 
                FROM Projet 
                WHERE TeamId = @TeamId 
                ORDER BY CreatedAt DESC", conn);

            cmd.Parameters.AddWithValue("@TeamId", teamId);
            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                projets.Add(MapFromReader(reader));
            }

            return projets;
        }

        public int Create(Projet projet)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO Projet (Nom, Description, DateDebut, DateFin, TeamId, CreatedAt, CreatedBy)
                OUTPUT INSERTED.Id
                VALUES (@Nom, @Description, @DateDebut, @DateFin, @TeamId, @CreatedAt, @CreatedBy)", conn);

            cmd.Parameters.AddWithValue("@Nom", projet.Nom);
            cmd.Parameters.AddWithValue("@Description", projet.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DateDebut", projet.DateDebut ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DateFin", projet.DateFin ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TeamId", projet.TeamId);
            cmd.Parameters.AddWithValue("@CreatedAt", projet.CreatedAt);
            cmd.Parameters.AddWithValue("@CreatedBy", projet.CreatedBy);

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Update(Projet projet)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                UPDATE Projet 
                SET Nom = @Nom, 
                    Description = @Description, 
                    DateDebut = @DateDebut, 
                    DateFin = @DateFin, 
                    TeamId = @TeamId, 
                    LastEditAt = @LastEditAt, 
                    LastEditBy = @LastEditBy
                WHERE Id = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", projet.Id);
            cmd.Parameters.AddWithValue("@Nom", projet.Nom);
            cmd.Parameters.AddWithValue("@Description", projet.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DateDebut", projet.DateDebut ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DateFin", projet.DateFin ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TeamId", projet.TeamId);
            cmd.Parameters.AddWithValue("@LastEditAt", projet.LastEditAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditBy", projet.LastEditBy ?? (object)DBNull.Value);

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM Projet WHERE Id = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Projet MapFromReader(SqlDataReader reader)
        {
            return new Projet
            {
                Id = reader.GetInt32("Id"),
                Nom = reader.GetString("Nom"),
                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                DateDebut = reader.IsDBNull("DateDebut") ? null : reader.GetDateTime("DateDebut"),
                DateFin = reader.IsDBNull("DateFin") ? null : reader.GetDateTime("DateFin"),
                TeamId = reader.GetInt32("TeamId"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                CreatedBy = reader.GetString("CreatedBy"),
                LastEditAt = reader.IsDBNull("LastEditAt") ? null : reader.GetDateTime("LastEditAt"),
                LastEditBy = reader.IsDBNull("LastEditBy") ? null : reader.GetString("LastEditBy")
            };
        }
    }
}