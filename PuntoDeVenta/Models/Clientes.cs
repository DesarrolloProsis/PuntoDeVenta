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

        [Display(Name = "Número de cliente")]
        [StringLength(30)]
        [Required]
        public string NumCliente { get; set; }

        [Display(Name = "Nombre del cliente")]
        [StringLength(150)]
        [Required]
        public string Nombre { get; set; }

        [Display(Name = "Apellidos del cliente")]
        [StringLength(150)]
        [Required]
        public string Apellidos { get; set; }

        [Display(Name = "Correo electrónico del cliente")]
        [StringLength(150)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Required]
        public string EmailCliente { get; set; }

        [Display(Name = "Dirección del cliente")]
        [StringLength(300)]
        public string AddressCliente { get; set; }

        [Display(Name = "Número telefónico del cliente")]
        [StringLength(50)]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [Required]
        public string PhoneCliente { get; set; }

        [Display(Name = "Estatus del cliente")]
        [Required]
        public bool StatusCliente { get; set; }

        [Display(Name = "Fecha registro del cliente")]
        [Required]
        public DateTime DateTCliente { get; set; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        [Required]
        public string IdCajero { get; set; }

        public ICollection<CuentasTelepeaje> CuentasTelepeajes { get; set; }

        [NotMapped]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }

    }
}