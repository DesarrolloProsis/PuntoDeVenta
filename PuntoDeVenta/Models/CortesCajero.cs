using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class CortesCajero
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(20)]
        [Required]
        public string NumCorte { get; set; }

        [Required]
        public DateTime DateTApertura { get; set; }

        public DateTime? DateTCierre { get; set; }

        public double? MontoTotal { get; set; }

        [StringLength(300)]
        public string Comentario { get; set; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        public string IdCajero { get; set; }

        public ICollection<OperacionesCajero> OperacionesCajeros { get; set; }
    }
}