using ParkFlow.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Zona : IEntity<int>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double Ancho { get; set; }
        public double Largo { get; set; }

        private Zona() 
        {
            Id = 0;
            Nombre = string.Empty;
            Ancho = 0;
            Largo = 0;
        }

        public static Zona Create()
        {
            return new Zona();
        }

        public double Area
        {
            get { return Ancho * Largo; }


        }
    }
}
