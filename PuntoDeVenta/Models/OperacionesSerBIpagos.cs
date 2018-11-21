﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    public class OperacionesSerBIpagos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(20)]
        public string NumAutoriProveedor { get; set; }

        [StringLength(20)]
        public string NumAutoriBanco { get; set; }

        public double? SaldoAnterior { get; set; }

        public double? SaldoModificar { get; set; }

        public double? SaldoActual { get; set; }

        public bool StatusOperacion { get; set; }

        public DateTime DateTOpSerBI { get; set; }

        public virtual long TagId { get; set; }

        public Tags Tags { get; set; }
    }
}