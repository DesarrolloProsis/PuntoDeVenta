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
        [Required]
        public string NumCliente { get; set; }

        [Display(Name = "Nombre del cliente")]
        [Required]
        public string Nombre { get; set; }

        [Display(Name = "Apellidos del cliente")]
        [Required]
        public string Apellidos { get; set; }

        [Display(Name = "Correo electrónico del cliente")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        //[Required]
        public string EmailCliente { get; set; }

        [Display(Name = "Dirección del cliente")]
        public string AddressCliente { get; set; }

        [Display(Name = "Número telefónico del cliente")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        //[Required]
        public string PhoneCliente { get; set; }

        [Display(Name = "Estatus del cliente")]
        [Required]
        public bool StatusCliente { get; set; }

        [Display(Name = "Fecha registro del cliente")]
        [Required]
        public DateTime DateTCliente { get; set; }

        [Column(TypeName = "nvarchar")]
        [Required]
        public string IdCajero { get; set; }

        public ICollection<CuentasTelepeaje> CuentasTelepeajes { get; set; }

        [NotMapped]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }

        [Display(Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(Name = "Código postal")]
        public string CP { get; set; }

        [Display(Name = "País")]
        public string Pais { get; set; }

        [Display(Name = "Ciudad")]
        public string City { get; set; }

        [Display(Name = "Departamento")]
        public string Departamento { get; set; }

        [Display(Name = "NIT")]
        public string NIT { get; set; }

        [Display(Name = "Tel. Oficina")]
        public string PhoneOffice { get; set; }
    }
}