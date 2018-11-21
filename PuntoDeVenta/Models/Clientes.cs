using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    [Table("Clientes")]
    public class Clientes
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(30)]
        [Required]
        public string NumCliente { get; set; }

        [StringLength(150)]
        [Required]
        public string Nombre { get; set; }

        [StringLength(150)]
        [Required]
        public string Apellidos { get; set; }

        [StringLength(150)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required]
        public string EmailCliente { get; set; }

        [StringLength(300)]
        public string AddressCliente { get; set; }

        [StringLength(50)]
        [Phone]
        [Required]
        public string PhoneCliente { get; set; }

        [Required]
        public bool StatusCliente { get; set; }

        [Required]
        public DateTime DateTCliente { get; set; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        [Required]
        public string IdCajero { get; set; }

        public ICollection<CuentasTelepeaje> CuentasTelepeajes { get; set; }

    }
}