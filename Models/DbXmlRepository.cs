using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Npgsql;

namespace InsiderCareers.Models
{
    public class DbXmlRepository : IXmlRepository
    {
        private readonly string _connectionString;

        public DbXmlRepository(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTable();
        }

        private void EnsureTable()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "CREATE TABLE IF NOT EXISTS \"DataProtectionKeysXml\" (\"Id\" SERIAL PRIMARY KEY, \"Xml\" TEXT NOT NULL)",
                conn);
            cmd.ExecuteNonQuery();
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var elements = new List<XElement>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT \"Xml\" FROM \"DataProtectionKeysXml\"", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                elements.Add(XElement.Parse(reader.GetString(0)));
            }
            return elements;
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO \"DataProtectionKeysXml\" (\"Xml\") VALUES (@xml)", conn);
            cmd.Parameters.AddWithValue("xml", element.ToString());
            cmd.ExecuteNonQuery();
        }
    }
}