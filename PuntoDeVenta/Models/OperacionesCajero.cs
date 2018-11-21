using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class OperacionesCajero
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(70)]
        [Required]
        public string Concepto { get; set; }

        [StringLength(30)]
        [Required]
        public string TipoPago { get; set; }

        [Required]
        public double Monto { get; set; }
    
        [Required]
        public DateTime DateTOperacion { get; set; }

        public virtual long TagId { get; set; }

        public Tags Tags { get; set; }

        public virtual long CorteId { get; set; }

        public CortesCajero CortesCajero { get; set; }
    }
}