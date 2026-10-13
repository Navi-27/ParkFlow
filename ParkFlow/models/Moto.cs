using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Moto : Vehiculo
    {
        private Moto() { }
        public static Moto Create()
        {
            return new Moto();
        }

        public override string Tipo() => "MOTO";
        
    }
}
