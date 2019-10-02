using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class Location
    {
        public byte IdLocation { get; set; }
        public string Delegacion { get; set; }
        public string Plaza { get; set; }

        public virtual  ICollection<Historico> Historicos{ get; set; }
    }
}

