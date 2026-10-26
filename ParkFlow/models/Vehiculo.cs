using ParkFlow.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal abstract class Vehiculo : IEntity<string>
    {
        public string Id { get; set; }
        public string PersonaDocumento { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }

        public abstract string Tipo();
        protected Vehiculo()
        {
            Id = string.Empty;
            PersonaDocumento = string.Empty;
            Modelo = string.Empty;
            Marca = string.Empty;

        }
       
    }
}
