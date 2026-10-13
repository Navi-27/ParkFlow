// Persistence/Database.cs
using MySqlConnector;
using ParkFlow.Tools;
using System.Configuration;

namespace ParkFlow.Persistence
{
    internal static class Database
    {
        public static MySqlConnection CreateConnection()
        {
            return new MySqlConnection(Config.ConnectionString);
        }

        public static bool TestConnection(out string message)
        {
            try
            {
                using (MySqlConnection con = CreateConnection())
                {
                    con.Open();
                    message = "Conexión exitosa a la base: " + con.Database;
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "No se pudo conectar: " + ex.Message;
                return false;
            }
        }
    }
}