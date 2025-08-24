using Dior.Library.BO;
using Dior.Library.DAO;
using System.Data;
using Microsoft.Data.SqlClient; // Remplacement de System.Data.SqlClient

namespace Dior.Service.DAO
{
    public class ContractDao : IContractDao
    {
        private readonly string _connectionString;

        public ContractDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<ContractBO> GetAll()
        {
            var contracts = new List<ContractBO>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM [CONTRACT]", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contracts.Add(Map(reader));
            }
            return contracts;
        }

        public List<ContractBO> GetByUserId(int userId)
        {
            var contracts = new List<ContractBO>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM [CONTRACT] WHERE UserId = @userId", conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contracts.Add(Map(reader));
            }
            return contracts;
        }

        public ContractBO? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM [CONTRACT] WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return Map(reader);
            return null;
        }

        public void Create(ContractBO contract)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"INSERT INTO [CONTRACT] (UserId, FileName, FileUrl, UploadedAt, UploadedBy) VALUES (@UserId, @FileName, @FileUrl, @UploadedAt, @UploadedBy)", conn);
            cmd.Parameters.AddWithValue("@UserId", contract.UserId);
            cmd.Parameters.AddWithValue("@FileName", contract.FileName);
            cmd.Parameters.AddWithValue("@FileUrl", contract.FileUrl);
            cmd.Parameters.AddWithValue("@UploadedAt", contract.UploadedAt);
            cmd.Parameters.AddWithValue("@UploadedBy", contract.UploadedBy);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM [CONTRACT] WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private ContractBO Map(IDataRecord record)
        {
            return new ContractBO
            {
                Id = record.GetInt32(record.GetOrdinal("Id")),
                UserId = record.GetInt32(record.GetOrdinal("UserId")),
                FileName = record.GetString(record.GetOrdinal("FileName")),
                FileUrl = record.GetString(record.GetOrdinal("FileUrl")),
                UploadedAt = record.GetDateTime(record.GetOrdinal("UploadedAt")),
                UploadedBy = record.GetString(record.GetOrdinal("UploadedBy"))
            };
        }
    }
}