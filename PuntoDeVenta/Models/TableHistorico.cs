using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuntoDeVenta.Models
{
    public class TableHistorico
    {

        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public string Tag { get; set; }
        public string Cuenta { get; set; }
        public string TypeMovimiento { get; set; }
        public string TypeMovimiento2 { get; set; }
        public bool Mensaje { get; set; }
        public object Info { get; set; }
        public List<Cruces> ListaCruces { get; set; }
        public List<Movimientos> ListaMovimientos { get; set; }
        public List<CruceMovimiento> ListCruceMovimientos { get; set; }

    }

    public class CruceMovimiento
    {
        public string Concepto { get; set; }
        public string TagCuenta { get; set; }
        public string Fecha { get; set; }
        public string CobroTag { get; set; }
        public string Referencia { get; set; }

    }


    public class Cruces
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string NumCliente { get; set; }
        public string NomCliente { get; set; }
        public string TypeCuenta { get; set; }
        public string Delegacion { get; set; }
        public string Plaza { get; set; }
        public string Fecha { get; set; }
        public string Cuerpo { get; set; }
        public string Carril { get; set; }
        public string Clase { get; set; }
        public string SaldoAntes { get; set; }
        public string Saldo { get; set; }
        public string SaldoDespues { get; set; }
        public string SaldoActual { get; set; }
        public string Operador { get; set; }
    }

    public class Movimientos
    {
        public int Id { get; set; }
        public string Concepto { get; set; }
        public string TipoPago { get; set; }
        public string Monto { get; set; }
        public string Fecha { get; set; }
        public string Tag { get; set; }
        public string TagCuenta { get; set; }
        public string Cuenta { get; set; }
        public string NomCliente { get; set; }
        public string TypeCuenta { get; set; }
        public string Tipo { get; set; }
        public string CobroTag { get; set; }
        public string Referencia { get; set; }
        public string SaldoActual { get; set; }
    }
}
