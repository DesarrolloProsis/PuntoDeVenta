using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class TipoVehiculo
    {

        public byte IdTipoVehiculo { get; set; }
        public string Tipo { get; set; }

        public virtual ICollection<Historico> Historicos { get; set; }
    }
}