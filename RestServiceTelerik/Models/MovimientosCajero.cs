using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceTelerik.Models
{
    public class PropertiesDetalles
    {
        public string Concepto { get; set; }
        public string TipoPago { get; set; }
        public string Monto { get; set; }
        public DateTime DataTOperacion { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public string CobroTag { get; set; }
    }
}