using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Zona
    {
        private Zona() 
        {
            Nombre = string.Empty;
        }

        public static Zona Create()
        {
            return new Zona();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public double Ancho { get; set; }                    
        public double Largo { get; set; }

        public double Area
        {
            get { return Ancho * Largo; }


        }
    }
}
