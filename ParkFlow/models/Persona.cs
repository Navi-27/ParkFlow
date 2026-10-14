using ParkFlow.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal abstract class Persona : IEntity<string>
    {
        protected Persona()
        { 
            Id = string.Empty;
            TipoDocumento = string.Empty;
            Nombre = string.Empty;
            Apellido = string.Empty;
            Telefono = string.Empty;
            Correo = string.Empty;
        }

        public string Id { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool MovilidadReducida { get; set; }

        public abstract int Prioridad { get;}
    }
}
