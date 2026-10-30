using MySqlConnector;
using ParkFlow.Contracts;
using ParkFlow.models;
using ParkFlow.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.Infraestructure
{
    internal class RepositoryParqueaderos : IRepository<Parqueadero, int>
    {
        private const string Columnas =
            "id_parqueo, id_zona, persona_documento, vehiculo_placa, codigo, nivel, coord_x, coord_y, coord_z, tipo";

        // Convierte una fila de la tabla parqueo en un Parqueadero
        private Parqueadero Map(MySqlDataReader reader)
        {
            Parqueadero parqueadero = Parqueadero.Create();

            parqueadero.Id = reader.GetInt32(reader.GetOrdinal("id_parqueo"));
            parqueadero.IdZona = reader.GetInt32(reader.GetOrdinal("id_zona"));
            parqueadero.Codigo = reader.GetString(reader.GetOrdinal("codigo"));
            parqueadero.Nivel = reader.GetInt32(reader.GetOrdinal("nivel"));
            parqueadero.CoordX = reader.GetDouble(reader.GetOrdinal("coord_x"));
            parqueadero.CoordY = reader.GetDouble(reader.GetOrdinal("coord_y"));
            parqueadero.CoordZ = reader.GetDouble(reader.GetOrdinal("coord_z"));
            parqueadero.Tipo = reader.GetString(reader.GetOrdinal("tipo"));

            // Estas dos columnas pueden ser NULL (plaza libre / sin dueño)
            int posPersona = reader.GetOrdinal("persona_documento");
            parqueadero.PersonaDocumento = reader.IsDBNull(posPersona) ? string.Empty : reader.GetString(posPersona);

            int posPlaca = reader.GetOrdinal("vehiculo_placa");
            parqueadero.VehiculoPlaca = reader.IsDBNull(posPlaca) ? string.Empty : reader.GetString(posPlaca);

            return parqueadero;
        }

        // Llena los parametros; un texto vacio se manda como NULL a la base
        private void FillParameters(MySqlCommand cmd, Parqueadero entity)
        {
            cmd.Parameters.AddWithValue("@zona", entity.IdZona);
            cmd.Parameters.AddWithValue("@codigo", entity.Codigo);
            cmd.Parameters.AddWithValue("@nivel", entity.Nivel);
            cmd.Parameters.AddWithValue("@x", entity.CoordX);
            cmd.Parameters.AddWithValue("@y", entity.CoordY);
            cmd.Parameters.AddWithValue("@z", entity.CoordZ);
            cmd.Parameters.AddWithValue("@tipo", entity.Tipo);
            cmd.Parameters.AddWithValue("@persona",
                string.IsNullOrEmpty(entity.PersonaDocumento) ? DBNull.Value : entity.PersonaDocumento);
            cmd.Parameters.AddWithValue("@placa",
                string.IsNullOrEmpty(entity.VehiculoPlaca) ? DBNull.Value : entity.VehiculoPlaca);
        }

        public void Add(Parqueadero entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"INSERT INTO parqueo
                (
                    id_zona,
                    persona_documento,
                    vehiculo_placa,
                    codigo,
                    nivel,
                    coord_x,
                    coord_y,
                    coord_z,
                    tipo
                )
                VALUES
                (
                    @zona,
                    @persona,
                    @placa,
                    @codigo,
                    @nivel,
                    @x,
                    @y,
                    @z,
                    @tipo
                );";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    FillParameters(cmd, entity);

                    cmd.ExecuteNonQuery();

                    entity.Id = Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public void Update(Parqueadero entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"UPDATE parqueo
                              SET
                               id_zona = @zona,
                               persona_documento = @persona,
                               vehiculo_placa = @placa,
                               codigo = @codigo,
                               nivel = @nivel,
                               coord_x = @x,
                               coord_y = @y,
                               coord_z = @z,
                               tipo = @tipo
                              WHERE id_parqueo = @id;";

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
                string sql = "DELETE FROM parqueo WHERE id_parqueo = @id;";

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
                string sql = "SELECT COUNT(*) FROM parqueo WHERE id_parqueo = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public IEnumerable<Parqueadero> GetAll()
        {
            List<Parqueadero> parqueaderos = new List<Parqueadero>();

            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM parqueo";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            parqueaderos.Add(Map(reader));
                        }
                    }
                }
            }
            return parqueaderos;
        }

        public Parqueadero? GetById(int id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM parqueo WHERE id_parqueo = @id;";

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

        // Plazas sin vehiculo (vehiculo_placa NULL = libre)
        public IEnumerable<Parqueadero> GetLibres()
        {
            List<Parqueadero> parqueaderos = new List<Parqueadero>();

            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM parqueo WHERE vehiculo_placa IS NULL;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            parqueaderos.Add(Map(reader));
                        }
                    }
                }
            }
            return parqueaderos;
        }
    }
}