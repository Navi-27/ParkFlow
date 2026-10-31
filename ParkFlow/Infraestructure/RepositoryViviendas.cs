using MySqlConnector;
using ParkFlow.Contracts;
using ParkFlow.models;
using ParkFlow.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.Infraestructure
{
    internal class RepositoryViviendas : IRepository<Vivienda, int>
    {
        private const string Columnas =
            "id_vivienda, persona_documento, torre, piso, apartamento";

        // Convierte una fila de la tabla vivienda en una Vivienda
        private Vivienda Map(MySqlDataReader reader)
        {
            Vivienda vivienda = Vivienda.Create();

            vivienda.Id = reader.GetInt32(reader.GetOrdinal("id_vivienda"));
            vivienda.PersonaDocumento = reader.GetString(reader.GetOrdinal("persona_documento"));
            vivienda.Torre = reader.GetInt32(reader.GetOrdinal("torre"));
            vivienda.Piso = reader.GetInt32(reader.GetOrdinal("piso"));
            vivienda.Apartamento = reader.GetInt32(reader.GetOrdinal("apartamento"));

            return vivienda;
        }

        // Llena los parametros comunes de Add y Update
        private void FillParameters(MySqlCommand cmd, Vivienda entity)
        {
            cmd.Parameters.AddWithValue("@persona", entity.PersonaDocumento);
            cmd.Parameters.AddWithValue("@torre", entity.Torre);
            cmd.Parameters.AddWithValue("@piso", entity.Piso);
            cmd.Parameters.AddWithValue("@apartamento", entity.Apartamento);
        }

        // Ejecuta un SELECT y devuelve todas las viviendas que encuentre
        private List<Vivienda> Consultar(string sql, string? documento = null)
        {
            List<Vivienda> viviendas = new List<Vivienda>();

            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    if (documento != null)
                    {
                        cmd.Parameters.AddWithValue("@persona", documento);
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            viviendas.Add(Map(reader));
                        }
                    }
                }
            }
            return viviendas;
        }

        public void Add(Vivienda entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"INSERT INTO vivienda
                (
                    persona_documento,
                    torre,
                    piso,
                    apartamento
                )
                VALUES
                (
                    @persona,
                    @torre,
                    @piso,
                    @apartamento
                );";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    FillParameters(cmd, entity);

                    cmd.ExecuteNonQuery();

                    // El id lo genera la base (AUTO_INCREMENT): lo guardamos en el objeto
                    entity.Id = Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public void Update(Vivienda entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"UPDATE vivienda
                              SET
                               persona_documento = @persona,
                               torre = @torre,
                               piso = @piso,
                               apartamento = @apartamento
                              WHERE id_vivienda = @id;";

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
                string sql = "DELETE FROM vivienda WHERE id_vivienda = @id;";

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
                string sql = "SELECT COUNT(*) FROM vivienda WHERE id_vivienda = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public IEnumerable<Vivienda> GetAll()
        {
            return Consultar("SELECT " + Columnas + " FROM vivienda ORDER BY torre, piso, apartamento;");
        }

        public Vivienda? GetById(int id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT " + Columnas + " FROM vivienda WHERE id_vivienda = @id;";

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

        // Extra (no esta en IRepository): viviendas de un residente.
        // Sirve para elegir la vivienda al registrar una visita.
        public IEnumerable<Vivienda> GetByPersona(string documento)
        {
            return Consultar(
                "SELECT " + Columnas + " FROM vivienda WHERE persona_documento = @persona ORDER BY torre, piso, apartamento;",
                documento);
        }
    }
}
