﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    [NotMapped]
    public class GenerarReportesViewModel
    {
        public DateTime Date { get; set; }
        public List<Properties> PropertiesList { get; set; }
        public EncabezadoReporteCajero EncabezadoReporteCajero { get; set; }
    }

    [NotMapped]
    public class Properties
    {
        public long? Id { get; set; }
        public string NumCorte { get; set; }
        public string NomCajero { get; set; }
        public DateTime DateInicio { get; set; }
        public DateTime DateFin { get; set; }
        public string Type { get; set; }
    }

    [NotMapped]
    public class EncabezadoReporteCajero
    {
        public string Cajero { get; set; }
        public string NumCorte { get; set; }
        public string Fecha { get; set; }
        public string HoraI { get; set; }
        public string HoraF { get; set; }
        public string TotalMonto { get; set; }
        public string Comentario { get; set; }
        public string SubtotalRecar { get; set; }
        public string VentaTag { get; set; }
    }
}
