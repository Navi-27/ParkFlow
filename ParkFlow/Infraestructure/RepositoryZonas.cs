using MySqlConnector;
using ParkFlow.Contracts;
using ParkFlow.models;
using ParkFlow.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.Infraestructure
{
    internal class RepositoryZonas : IRepository<Zona, int>
    {

        private Zona Map(MySqlDataReader reader)
        {
            Zona zona = Zona.Create();

            zona.Id = reader.GetInt32(reader.GetOrdinal("id_zona"));
            zona.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
            zona.Ancho = reader.GetDouble(reader.GetOrdinal("ancho"));
            zona.Largo = reader.GetDouble(reader.GetOrdinal("largo"));

            return zona;

        }
        public void Add(Zona entity)
        {
            using (MySqlConnection con = new MySqlConnection())
            {
                con.Open();
                string sql = @"INSERT INTO zona
                (
                    nombre,
                    ancho,
                    largo
                )
                VALUES
                (
                    @nombre,
                    @ancho,
                    @largo
                );";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@nombre", entity.Nombre);
                    cmd.Parameters.AddWithValue("@ancho", entity.Ancho);
                    cmd.Parameters.AddWithValue("@largo", entity.Largo);

                    cmd.ExecuteNonQuery();

                    entity.Id = Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public void Update(Zona entity)
        {
            using(MySqlConnection con = new MySqlConnection())
            {
                con.Open();
                string sql = @"UPDATE zona
                              SET
                               nombre = @nombre,
                               ancho = @ancho,
                               largo = @largo
                              WHERE id_zona = @id;";

                using(MySqlCommand cmd = new MySqlCommand( sql, con))
                {
                    cmd.Parameters.AddWithValue("@nombre", entity.Nombre);
                    cmd.Parameters.AddWithValue("@ancho", entity.Ancho);
                    cmd.Parameters.AddWithValue("@largo", entity.Largo);
                    cmd.Parameters.AddWithValue("@id", entity.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (MySqlConnection con = new MySqlConnection()) 
            {
                con.Open();
                string sql = "DELETE FROM zona WHERE id_zona = @id; ";
               
                using(MySqlCommand cmd = new MySqlCommand(sql,con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool Exist(int id)
        {
            t using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM zona WHERE id_zona = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public IEnumerable<Zona> GetAll()
        {
            List<Zona> zonas = new List<Zona>();

            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT id_zona, nombre, ancho, largo FROM zona";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            zonas.Add(Map(reader));
                        }
                    }
                }
            }
            return zonas;
        }

        public Zona? GetById(int id)
        {

            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT id_zona, nombre, ancho, largo FROM zona WHERE id_zona = @id;";

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

        
    }
}
