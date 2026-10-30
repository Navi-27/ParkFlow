using MySqlConnector;
using ParkFlow.Contracts;
using ParkFlow.models;
using ParkFlow.Persistence;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ParkFlow.Infraestructure
{
    internal class RepositoryUsuarios : IRepository<Usuario, int>
    {
        private const string Columnas = "id_usuario, usuario, password, rol";

        // Convierte una fila de la tabla usuario en un Usuario
        private Usuario Map(MySqlDataReader reader)
        {
            Usuario usuario = Usuario.Create();

            usuario.Id = reader.GetInt32(reader.GetOrdinal("id_usuario"));
            usuario.NombreUsuario = reader.GetString(reader.GetOrdinal("usuario"));
            usuario.Password = reader.GetString(reader.GetOrdinal("password"));
            usuario.Rol = reader.GetString(reader.GetOrdinal("rol"));

            return usuario;
        }

        // Hash SHA-256 en mayusculas: igual al UPPER(SHA2(clave, 256)) de los datos de prueba
        public static string HashPassword(string clave)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
            return Convert.ToHexString(bytes);
        }

        // Add y Update guardan entity.Password tal cual: debe llegar ya con el hash
        public void Add(Usuario entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"INSERT INTO usuario
                (
                    usuario,
                    password,
                    rol
                )
                VALUES
                (
                    @usuario,
                    @password,
                    @rol
                );";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@usuario", entity.NombreUsuario);
                    cmd.Parameters.AddWithValue("@password", entity.Password);
                    cmd.Parameters.AddWithValue("@rol", entity.Rol);

                    cmd.ExecuteNonQuery();

                    entity.Id = Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public void Update(Usuario entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"UPDATE usuario
                              SET
                               usuario = @usuario,
                               password = @password,
                               rol = @rol
                              WHERE id_usuario = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@usuario", entity.NombreUsuario);
                    cmd.Parameters.AddWithValue("@password", entity.Password);
                    cmd.Parameters.AddWithValue("@rol", entity.Rol);
                    cmd.Parameters.AddWithValue("@id", entity.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "DELETE FROM usuario WHERE id_usuario = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool Exist(int id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM usuario WHERE id_usuario = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public IEnumerable<Usuario> GetAll()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM usuario";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(Map(reader));
                        }
                    }
                }
            }
            return usuarios;
        }

        public Usuario? GetById(int id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM usuario WHERE id_usuario = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Map(reader);
                        }
                    }
                }
            }
            return null;
        }

        public Usuario? GetByNombreUsuario(string nombreUsuario)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM usuario WHERE usuario = @usuario;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@usuario", nombreUsuario);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Map(reader);
                        }
                    }
                }
            }
            return null;
        }

        // Login: devuelve el usuario si el nombre y la clave coinciden, o null si no
        public Usuario? Autenticar(string nombreUsuario, string clave)
        {
            Usuario? usuario = GetByNombreUsuario(nombreUsuario);

            if (usuario != null && usuario.Password == HashPassword(clave))
            {
                return usuario;
            }
            return null;
        }
    }
}