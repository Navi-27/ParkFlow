using ParkFlow.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Usuario : IEntity<int>
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        private Usuario()
        {
            Id = 0;
            NombreUsuario = string.Empty;
            Password = string.Empty;
            Rol = "PORTERO";
        }

        public static Usuario Create()
        {
            return new Usuario();
        }
      

    }
}

