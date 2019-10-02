using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class Operadores
    {
        public byte IdOperador { get; set; }
        public string Operador { get; set; }


        public virtual ICollection<Historico> Historicos { get; set; }
    }
}