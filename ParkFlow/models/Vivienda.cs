using ParkFlow.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Vivienda : IEntity<int>
    {
        private Vivienda()
        {
            PersonaDocumento = string.Empty;
        }

        public static Vivienda Create()
        {
            return new Vivienda();
        }

        public int Id { get; set; }       
        public string PersonaDocumento { get; set; } 
        public int Torre { get; set; }
        public int Piso { get; set; }
        public int Apartamento { get; set; }
    }
}
