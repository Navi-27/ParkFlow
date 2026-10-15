using ParkFlow.Contracts;
using ParkFlow.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Parqueadero : IEntity<int>
    {
        private Parqueadero()
        {
            Id = 0;
            IdZona = 0;
            Codigo = string.Empty;
            Nivel = 1;
            CoordX = 0;
            CoordY = 0;
            CoordZ = 0;
            Tipo = "VISITANTE";
            PersonaDocumento = string.Empty;
            VehiculoPlaca = string.Empty; ;

        }

        public static Parqueadero Create()
        {
            return new Parqueadero();
        }

        public int Id { get; set; }
        public int IdZona { get; set; }
        public string Codigo { get; set; }
        public int Nivel { get; set; }
        public double CoordX { get; set; }
        public double CoordY { get; set; }
        public double CoordZ { get; set; }
        public string Tipo { get; set; }

        public string PersonaDocumento { get; set; }
        public string VehiculoPlaca { get; set; }

        public bool Libre {
            get { return VehiculoPlaca == string.Empty; }
        }

    }
}
