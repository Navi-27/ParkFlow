using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using ParkFlow.Contracts;
using ParkFlow.models;
using ParkFlow.Persistence;

namespace ParkFlow.Infraestructure
{
    internal class RepositoryEstancias
    {
        private const string Columnas =
            "id_estancia, vehiculo_placa, id_parqueo, fecha_llegada, fecha_ocupacion, fecha_salida";

        // Convierte una fila de la base en una Estancia
        private Estancia Map(MySqlDataReader reader)
        {
            Estancia estancia = Estancia.Create();
            estancia.Id = reader.GetInt32(reader.GetOrdinal("id_estancia"));
            estancia.VehiculoPlaca = reader.GetString(reader.GetOrdinal("vehiculo_placa"));
            estancia.FechaLlegada = reader.GetDateTime(reader.GetOrdinal("fecha_llegada"));

            // Estas tres columnas pueden ser NULL en la base
            int posParqueo = reader.GetOrdinal("id_parqueo");
            estancia.IdParqueadero = reader.IsDBNull(posParqueo) ? null : reader.GetInt32(posParqueo);

            int posOcupacion = reader.GetOrdinal("fecha_ocupacion");
            estancia.FechaOcupacion = reader.IsDBNull(posOcupacion) ? null : reader.GetDateTime(posOcupacion);

            int posSalida = reader.GetOrdinal("fecha_salida");
            estancia.FechaSalida = reader.IsDBNull(posSalida) ? null : reader.GetDateTime(posSalida);

            return estancia;
        }

        // Llena los parametros; si un valor es null manda NULL a la base
        private void FillParameters(MySqlCommand cmd, Estancia entity)
        {
            cmd.Parameters.AddWithValue("@placa", entity.VehiculoPlaca);
            cmd.Parameters.AddWithValue("@parqueo", (object?)entity.IdParqueadero ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@llegada", entity.FechaLlegada);
            cmd.Parameters.AddWithValue("@ocupacion", (object?)entity.FechaOcupacion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@salida", (object?)entity.FechaSalida ?? DBNull.Value);
        }

        public void Add(Estancia entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"INSERT INTO estancia
                               (vehiculo_placa, id_parqueo, fecha_llegada, fecha_ocupacion, fecha_salida)
                               VALUES
                               (@placa, @parqueo, @llegada, @ocupacion, @salida);";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    FillParameters(cmd, entity);
                    cmd.ExecuteNonQuery();
                    // El id lo genera la base (AUTO_INCREMENT): lo guardamos en el objeto
                    entity.Id = Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public void Update(Estancia entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"UPDATE estancia
                               SET vehiculo_placa = @placa,
                                   id_parqueo = @parqueo,
                                   fecha_llegada = @llegada,
                                   fecha_ocupacion = @ocupacion,
                                   fecha_salida = @salida
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
                string sql = "DELETE FROM estancia WHERE id_estancia = @id;";
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
                string sql = "SELECT COUNT(*) FROM estancia WHERE id_estancia = @id;";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public Estancia? GetById(int id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM estancia WHERE id_estancia = @id;";
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

        public IEnumerable<Estancia> GetAll()
        {
            List<Estancia> estancias = new List<Estancia>();
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM estancia ORDER BY fecha_llegada;";
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

        // Extra (no esta en IRepository): estancias que aun no han salido
        public IEnumerable<Estancia> GetActivas()
        {
            List<Estancia> estancias = new List<Estancia>();
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM estancia WHERE fecha_salida IS NULL ORDER BY fecha_llegada;";
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
