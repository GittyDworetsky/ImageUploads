using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework4_17.Data
{
    public class ImagesDb
    {

        private string _connectionString;

        public ImagesDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddImage(string imagePath, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = "INSERT INTO Images (ImagePath, Password, Views) VALUES (@imagePath, @password, @views); SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@imagePath", imagePath);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@views", 0);

                     
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public Image GetImageById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM  Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            
            var reader = cmd.ExecuteReader();
            reader.Read();

            return new Image
            {
                Id = (int)reader["id"],
                ImagePath = (string)reader["imagePath"],
                Password = (string)reader["password"],
                Views = (int)reader["views"]

            };
      
        }
    }
}
