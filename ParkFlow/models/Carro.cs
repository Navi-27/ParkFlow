using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Carro : Vehiculo
    {
        private Carro() { }
        public static Carro Create()
        {
            return new Carro();
        }

        public override string Tipo() => "CARRO";
        
    }
}
