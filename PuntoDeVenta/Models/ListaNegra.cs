using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    [Table(name: "ListaNegra")]
    public class ListaNegra
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(30)]
        [Required]
        public string Tipo { get; set; }

        [StringLength(30)]
        public string Clase { get; set; }

        [StringLength(50)]
        [Required]
        public string Numero { get; set; }

        [StringLength(280)]
        [Required]
        public string Observacion { get; set; }

        public double? SaldoAnterior { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        [Required]
        public string IdCajero { get; set; }
    }
}