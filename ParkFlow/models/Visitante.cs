using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Visitante : Persona
    {
        private Visitante() { }

        public static Visitante Create()
        {
            return new Visitante();
        }

        public override int Prioridad
        {
            get { return MovilidadReducida ? 0 : 2; }
        }
    }
}
