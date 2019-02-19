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

        [Display(Name = "Número de cuenta")]
        [StringLength(30)]
        [Required]
        public string NumCuenta { get; set; }

        [Display(Name = "Saldo de cuenta")]
        [StringLength(20)]
        public string SaldoCuenta { get; set; }

        [Display(Name = "Tipo de cuenta")]
        [StringLength(20)]
        [Required]
        public string TypeCuenta { get; set; }

        [Display(Name = "Estatus de cuenta")]
        [Required]
        public bool StatusCuenta { get; set; }

        public bool StatusResidenteCuenta { get; set; }

        [Display(Name = "Fecha registro de cuenta")]
        [Required]
        public DateTime DateTCuenta { get; set; }

        public virtual long ClienteId { get; set; }

        public Clientes Clientes { get; set; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        [Required]
        public string IdCajero { get; set; }

        public ICollection<Tags> Tags { get; set; }

        [NotMapped]
        [Display(Name = "Saldo a recargar")]
        [StringLength(20)]
        public string SaldoARecargar { get; set; }

        [NotMapped]
        [Display(Name = "Confirmar saldo a recargar")]
        [StringLength(20)]
        [Compare("SaldoARecargar", ErrorMessage = "Los saldos no coinciden.")]
        public string ConfSaldoARecargar { get; set; }

        [NotMapped]
        public string NombreCliente { get; set; }
        [NotMapped]
        public string NumCliente { get; set; }
    }
}