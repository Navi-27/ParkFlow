using MySqlConnector;
using ParkFlow.Contracts;
using ParkFlow.models;
using ParkFlow.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.Infraestructure
{
    internal class RepositoryPersonas : IRepository<Persona, string>
    {
        private Persona Map(MySqlDataReader reader)
        {
            Persona persona;
            string tipo = reader.GetString(reader.GetOrdinal("tipo_persona"));

            if(tipo.ToUpper() == "RESIDENTE")
            {
                persona = Residente.Create();
            }
            else
            {
                persona = Visitante.Create();
            }
            persona.Id = reader.GetString(reader.GetOrdinal("documento"));
            persona.TipoDocumento = reader.GetString(reader.GetOrdinal("tipo_documento"));
            persona.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
            persona.Apellido = reader.GetString(reader.GetOrdinal("apellido"));
            persona.MovilidadReducida = reader.GetBoolean(reader.GetOrdinal("movilidad_reducida"));

            int posTelefono = reader.GetOrdinal("telefono");
            persona.Telefono = reader.IsDBNull(posTelefono) ? string.Empty : reader.GetString(posTelefono);

            int posCorreo = reader.GetOrdinal("correo");
            persona.Correo = reader.IsDBNull(posCorreo) ? string.Empty : reader.GetString(posCorreo);

            return persona;

        }
        public void Add(Persona entity)
        {
            using(MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"INSERT INTO persona
                (
                    documento,
                    tipo_documento,
                    nombre,
                    apellido,
                    movilidad_reducida,
                    telefono,
                    correo,
                    tipo_persona
                )
                VALUES
                (
                    @documento,
                    @tipoDocumento,
                    @nombre,
                    @apellido,
                    @movilidad,
                    @telefono,
                    @correo,
                    @tipoPersona
                );";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@documento", entity.Id);
                    cmd.Parameters.AddWithValue("@tipoDocumento", entity.TipoDocumento);
                    cmd.Parameters.AddWithValue("@nombre", entity.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", entity.Apellido);
                    cmd.Parameters.AddWithValue("@movilidad", entity.MovilidadReducida);
                    cmd.Parameters.AddWithValue("@telefono", entity.Telefono);
                    cmd.Parameters.AddWithValue("@correo", entity.Correo);
                    cmd.Parameters.AddWithValue("@tipoPersona",
                    entity is Residente ? "RESIDENTE" : "VISITANTE");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Persona entity)
        {
            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = @"UPDATE persona
                              SET
                               tipo_documento = @tipoDocumento,
                               nombre = @nombre,
                               apellido = @apellido,
                               movilidad_reducida = @movilidad,
                               telefono = @telefono,
                               correo = @correo,
                               tipo_persona = @tipoPersona
                              WHERE documento = @id;";

                using(MySqlCommand cmd = new MySqlCommand( sql, con))
                {
                    cmd.Parameters.AddWithValue("@tipoDocumento", entity.TipoDocumento);
                    cmd.Parameters.AddWithValue("@nombre", entity.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", entity.Apellido);
                    cmd.Parameters.AddWithValue("@movilidad", entity.MovilidadReducida);
                    cmd.Parameters.AddWithValue("@telefono", entity.Telefono);
                    cmd.Parameters.AddWithValue("@correo", entity.Correo);

                    cmd.Parameters.AddWithValue("@tipoPersona",
                        entity is Residente ? "RESIDENTE" : "VISITANTE");

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

                string sql = @"DELETE FROM persona WHERE documento = @id;";

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
                string sql = "SELECT COUNT(*) FROM persona WHERE documento = @id;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }

            }
        }

        public IEnumerable<Persona> GetAll()
        {
            List<Persona> personas = new List<Persona>();

            using (MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT documento, tipo_documento, nombre, apellido, movilidad_reducida, telefono, correo, tipo_persona FROM persona";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            personas.Add(Map(reader));
                        }
                    }
                }

            }
            return personas;
        }

        public Persona? GetById(string id)
        {
            using(MySqlConnection con = Database.CreateConnection())
            {
                con.Open();
                string sql = "SELECT documento, tipo_documento, nombre, apellido, movilidad_reducida, telefono, correo, tipo_persona FROM persona WHERE documento = @id;";
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
