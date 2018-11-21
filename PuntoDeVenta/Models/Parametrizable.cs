using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class Parametrizable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }

        public string origen { get; set; }

        public string destino { get; set; }

        public int extension { get; set; }

        public string destinoresidente { get; set; }

        public short cruzes { get; set; }

        public short minutos { get; set; }
    }
}