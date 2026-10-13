using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.Tools
{
    internal static class Config
    {
        private const string Servidor = "localhost";
        private const string BaseDatos = "proyecto_integrador";
        private const string Usuario = "root";

        public static string ConnectionString
        {
            get
            {
                string clave = Environment.GetEnvironmentVariable("PARKFLOW_DB_PWD") ?? "";
                return $"Server={Servidor};Port=3306;Database={BaseDatos};Uid={Usuario};Pwd={clave};";
            }
        }
    }
}
