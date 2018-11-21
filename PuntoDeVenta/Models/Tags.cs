using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class Tags
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(50)]
        [Required]
        public string NumTag { get; set; }

        [Required]
        public double SaldoTag { get; set; }

        [Required]
        public bool StatusTag { get; set; }

        [Required]
        public bool StatusResidente { get; set; }

        [Required]
        public DateTime DateTTag { get; set; }

        public virtual long CuentaId { get; set; }

        public CuentasTelepeaje CuentasTelepeaje { get; set; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        [Required]
        public string IdCajero { get; set; }

        public ICollection<OperacionesSerBIpagos> OperacionesSerBIpagos { get; set; }

        public ICollection<OperacionesCajero> OperacionesCajeros { get; set; }
    }
}