using MySqlConnector;
using ParkFlow.Contracts;
using ParkFlow.models;
using ParkFlow.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.Infraestructure
{
    internal class RepositoyVehiculos : IRepository<Vehiculo, string>
    {
        private Vehiculo Map(MySqlDataReader reader)
        {
            Vehiculo vehiculo;
            string tipo = reader.GetString(reader.GetOrdinal("tipo_vehiculo"));

            if (tipo.ToUpper() == "CARRO")
            {
                vehiculo = Carro.Create();
            }
            else
            {
                vehiculo = Moto.Create();
            }
            vehiculo.Id = reader.GetString(reader.GetOrdinal("placa"));
            vehiculo.PersonaDocumento = reader.GetString(reader.GetOrdinal("persona_documento"));
            vehiculo.Modelo = reader.GetString(reader.GetOrdinal("modelo"));
            vehiculo.Marca = reader.GetString(reader.GetOrdinal("marca"));

            return vehiculo;

        }
        public void Add(Vehiculo entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"INSERT INTO vehiculo
                (
                    placa,
                    tipo_vehiculo,
                    persona_documento,
                    modelo,
                    marca,
                )
                VALUES
                (
                    @placa,
                    @tipo,
                    @persona,
                    @modelo,
                    @marca
                );";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@placa", entity.Id);
                    cmd.Parameters.AddWithValue("@tipo", entity.Tipo());
                    cmd.Parameters.AddWithValue("@persona", entity.PersonaDocumento);
                    cmd.Parameters.AddWithValue("@modelo", entity.Modelo);
                    cmd.Parameters.AddWithValue("@marca", entity.Marca);

                    cmd.ExecuteNonQuery();

                }
            }
        }

        public void Update(Vehiculo entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"UPDATE vehiculo
                              SET
                              tipo_vehiculo = @tipo,
                              persona_documento = @persona,
                              modelo = @modelo,
                              marca = @marca
                              WHERE placa = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@tipo", entity.Tipo());
                    cmd.Parameters.AddWithValue("@persona", entity.PersonaDocumento);
                    cmd.Parameters.AddWithValue("@modelo", entity.Modelo);
                    cmd.Parameters.AddWithValue("@marca", entity.Marca);
                    cmd.Parameters.AddWithValue("@id", entity.Id);

                    cmd.ExecuteNonQuery();
                }

            }
        }

        public void Delete(string id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "DELETE FROM vehiculo WHERE placa = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool Exist(string id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM vehiculo WHERE placa = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public IEnumerable<Vehiculo> GetAll()
        {
            List<Vehiculo> vehiculos = new List<Vehiculo>();

            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT placa, tipo_vehiculo, persona_documento, modelo, marca FROM vehiculo";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vehiculos.Add(Map(reader));
                        }
                    }
                }
            }
            return vehiculos;
        }

        public Vehiculo? GetById(string id)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT placa, tipo_vehiculo, persona_documento, modelo, marca FROM vehiculo WHERE placa = @id;";

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
