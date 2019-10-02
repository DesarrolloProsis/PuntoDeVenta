using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{

    public class Historico
    {

        public string Tag { get; set; }
        public DateTime Fecha { get; set; }
        public string Carril { get; set; }
        public byte IdLocation { get; set; }
        public byte IdClase { get; set; }
        public int Evento { get; set; }
        public byte IdOperador { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoActualizado { get; set; }
        public string NumeroCuenta { get; set; }


        //un regsitro historico tiene un solo carril 
        public virtual Carriles Carriles { get; set; }
        //un regsitro historico tiene una solo localización
        public virtual Location Location{ get; set; }
        public virtual TipoVehiculo TipoVehiculo { get; set; }
        public virtual Operadores Operadores { get; set; }


    }
}