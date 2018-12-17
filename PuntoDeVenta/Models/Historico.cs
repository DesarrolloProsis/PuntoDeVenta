using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    [Table("Historico")]
    public class Historico
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string NumTag { get; set; }
        public string Carril { get; set; }
        public DateTime Fecha { get; set; }
        public float SaldoTag { get; set; }
    }
}