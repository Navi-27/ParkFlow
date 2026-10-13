using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Persona
    {
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool MovilidadReducida { get; set; }
    }
}
