using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class CuentasTelepeaje
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(30)]
        [Required]
        public string NumCuenta { get; set; }

        [Required]
        public double SaldoCuenta { get; set; }

        [StringLength(20)]
        [Required]
        public string TypeCuenta { get; set; }

        [Required]
        public bool StatusCuenta { get; set; }

        public bool StatusResidenteCuenta { get; set; }

        [Required]
        public DateTime DateTCuenta { get; set; }

        public virtual long ClienteId { get; set; }

        public Clientes Clientes { get; set; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        [Required]
        public string IdCajero { get; set; }

        public ICollection<Tags> Tags { get; set; }
    }
}