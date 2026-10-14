using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Residente : Persona
    {
        private Residente() { } 

        public static Residente Create()
        {
            return new Residente();
        }

        public override int Prioridad
        {
            get { return MovilidadReducida ? 0 : 1; }
         
        }
    }
}
