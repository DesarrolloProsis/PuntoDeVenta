using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PuntoDeVenta.Models
{
    [DataContract(IsReference = true)]
    public class Tags
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        //[RegularExpression("^[0-9]*$", ErrorMessage = "{0} sólo acepta números.")]
        [Display(Name = "Número de tag")]
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

        [Display(Name = "Cobro por tag")]
        [NotMapped]
        //[Required]
        public string CobroTag { get; set; }

        [Display(Name = "Saldo a recargar")]
        [StringLength(20)]
        [NotMapped]
        [Compare("ConfSaldoARecargar", ErrorMessage = "Los saldos no coinciden.")]
        public string SaldoARecargar { get; set; }

        [Display(Name = "Confirmar saldo a recargar")]
        [StringLength(20)]
        [NotMapped]
        [Compare("SaldoARecargar", ErrorMessage = "Los saldos no coinciden.")]
        public string ConfSaldoARecargar { get; set; }

        [NotMapped]
        public string TipoTag { get; set; }

        [NotMapped]
        public string NombreCliente { get; set; }

        [NotMapped]
        public string TypeCuenta { get; set; }

        [NotMapped]
        public string NumCuenta { get; set; }

        [NotMapped]
        [Display(Name = "Antigüo tag")]
        [StringLength(20)]
        public string OldTag { get; set; }

        [NotMapped]
        public bool Checked { get; set; }

        [NotMapped]
        [Display(Name = "Observación")]
        public string Observacion { get; set; }

        [NotMapped]
        //[Compare("SaldoTag", ErrorMessage = "El saldo de tag y el antiguo saldo no coninciden.")]
        public string OldSaldo { get; set; }
    }

    [NotMapped]
    public class TagsViewModel
    {
        public long? IdOldTag { get; set; }
        public string NumNewTag { get; set; }
        public string Observacion { get; set; }
        public string CobroTag { get; set; }
        public string SaldoTag { get; set; }
        public bool Checked { get; set; }
    }
}