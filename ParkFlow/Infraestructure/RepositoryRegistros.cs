using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using ParkFlow.Contracts;
using ParkFlow.models;
using ParkFlow.Persistence;

namespace ParkFlow.Infraestructure
{
    internal class RepositoryRegistros
    {
        private const string Columnas =
            "id_registro, persona_visitante, persona_visitada, vivienda_id_vivienda, fecha_ingreso, fecha_salida";

        // Convierte una fila de la base en una Estancia
        private Registro Map(MySqlDataReader reader)
        {
            Registro registro = Registro.Create();
            registro.Id = reader.GetInt32(reader.GetOrdinal("id_estancia"));
            registro.PersonaVisitante = reader.GetString(reader.GetOrdinal("persona_visitante"));
            registro.PersonaVisitada = reader.GetString(reader.GetOrdinal("persona_visitada"));
            registro.IdVivienda = reader.GetInt32(reader.GetOrdinal("vivienda_id_vivienda"));
            registro.FechaIngreso = reader.GetDateTime(reader.GetOrdinal("fecha_ingreso"));

            // Esta columna puede ser NULL en la base

            int posSalida = reader.GetOrdinal("fecha_salida");
            registro.FechaSalida = reader.IsDBNull(posSalida) ? null : reader.GetDateTime(posSalida);


            return registro;
        }

        // Llena los parametros; si un valor es null manda NULL a la base
        private void FillParameters(MySqlCommand cmd, Registro entity)
        {
            cmd.Parameters.AddWithValue("@personaVisitante", entity.PersonaVisitante);
            cmd.Parameters.AddWithValue("@personaVisitada", entity.PersonaVisitada);
            cmd.Parameters.AddWithValue("@personaVisitada", entity.PersonaVisitada);
            cmd.Parameters.AddWithValue("@idVivienda", entity.IdVivienda);
            cmd.Parameters.AddWithValue("@fechaIngreso", entity.FechaIngreso);
            cmd.Parameters.AddWithValue("@fechaSalida", (object?)entity.FechaSalida ?? DBNull.Value);
        }

        public void Add(Registro entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"INSERT INTO registro
                               (persona_visitante, persona_visitada, vivienda_id_vivienda, fecha_ingreso, fecha_salida)
                               VALUES
                               (@personaVisitante, @personaVisitada, @idVivienda, @fechaIngreso, @fechaSalida);";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    FillParameters(cmd, entity);
                    cmd.ExecuteNonQuery();
                    // El id lo genera la base (AUTO_INCREMENT): lo guardamos en el objeto
                    entity.Id = Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public void Update(Registro entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"UPDATE registro
                               SET persona_visitante = @personaVisitante,
                                   persona_visitada = @personaVisitada,
                                   vivienda_id_vivienda = @idVivienda,
                                   fecha_ingreso = @fechaIngreso,
                                   fecha_salida = @fechaSalida
                               WHERE id_estancia = @id;";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    FillParameters(cmd, entity);
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
                string sql = "DELETE FROM registro WHERE id_registro = @id;";
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
                string sql = "SELECT COUNT(*) FROM registro WHERE id_registro = @id;";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public Registro? GetById(int id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM registro WHERE id_registro = @id;";
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

        public IEnumerable<Registro> GetAll()
        {
            List<Registro> registros = new List<Registro>();
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM registro ORDER BY fecha_ingreso;";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            registros.Add(Map(reader));
                        }
                    }
                }
            }
            return registros;
        }

        // Extra (no esta en IRepository): estancias que aun no han salido
        public IEnumerable<Registro> GetActivas()
        {
            List<Registro> estancias = new List<Registro>();
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM registro WHERE fecha_salida IS NULL ORDER BY fecha_ingreso;";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            estancias.Add(Map(reader));
                        }
                    }
                }
            }
            return estancias;
        }
    }
}
