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

        [Display(Name = "Número de tag")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} sólo acepta números.")]
        [StringLength(50)]
        [Required]
        public string NumTag { get; set; }

        [Display(Name = "Saldo de tag")]
        [StringLength(20)]
        public string SaldoTag { get; set; }

        [Display(Name = "Estatus de tag")]
        [Required]
        public bool StatusTag { get; set; }

        [Required]
        public bool StatusResidente { get; set; }

        [Display(Name = "Fecha registro de tag")]
        [Required]
        public DateTime DateTTag { get; set; }

        public virtual long CuentaId { get; set; }

        public CuentasTelepeaje CuentasTelepeaje { get; set; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        [Required]
        public string IdCajero { get; set; }

        public ICollection<OperacionesSerBIpagos> OperacionesSerBIpagos { get; set; }

        [Display(Name = "Cobro por tag")]
        [NotMapped]
        [Required]
        [Range(0, 100, ErrorMessage = "Cobro por tag invalido.")]
        public double CobroTag { get; set; }

        [Display(Name = "Saldo a recargar")]
        [StringLength(20)]
        [NotMapped]
        public string SaldoARecargar { get; set; }

        [Display(Name = "Confirmar saldo a recargar")]
        [StringLength(20)]
        [NotMapped]
        [Compare("SaldoARecargar", ErrorMessage = "Los saldos no coinciden.")]
        public string ConfSaldoARecargar { get; set; }
    }
}