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

        [StringLength(50)]
        [Required]
        public string Tipo { get; set; }

        [StringLength(50)]
        [Required]
        public string Numero { get; set; }

        public double? Monto { get; set; }

        [StringLength(30)]
        public string TipoPago { get; set; }

        [Required]
        public DateTime DateTOperacion { get; set; }

        public virtual long CorteId { get; set; }

        public CortesCajero CortesCajero { get; set; }
    }
}