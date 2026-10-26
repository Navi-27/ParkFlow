using ParkFlow.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.models
{
    internal class Estancia : IEntity<int>
    {
        public int Id { get; set; }
        public string VehiculoPlaca { get; set; }
        public int? IdParqueadero { get; set; }
        public DateTime FechaLlegada { get; set; }
        public DateTime? FechaOcupacion { get; set; }
        public DateTime? FechaSalida { get; set; }

        public bool Activa
        {
            get { return FechaSalida == null; }
        }

        private Estancia()
        {
            Id = 0;
            VehiculoPlaca = string.Empty;
            FechaLlegada = DateTime.Now;
        
        }

        public static Estancia Create()
        {
            return new Estancia();
        }

        
                                                    
    }

}
