using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class Carriles
    {

        public string Carril { get; set; }
        public byte IdGare { get; set; }
        public DateTime FechaAlta { get; set; }


        public virtual ICollection<Historico> Historicos { get; set; }

    }
}