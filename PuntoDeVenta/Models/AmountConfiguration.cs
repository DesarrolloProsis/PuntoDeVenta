using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class AmountConfiguration
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Concept { get; set; }
        [Key]
        [Column(Order = 1)]
        public double Amount { get; set; }
        public short Type { get; set; }
    }
}