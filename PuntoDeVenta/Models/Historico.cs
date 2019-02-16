using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
    //[Table("Historico")]
    //public class Historico
    //{
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    [Key]
    //    public int Id { get; set; }
    //    public string NumTag { get; set; }
    //    public string Carril { get; set; }
    //    public DateTime Fecha { get; set; }
    //    public float SaldoTag { get; set; }
    //}

    //public class TableHistorico
    //{
    //    public DateTime Fecha_Inicio { get; set; }
    //    public DateTime Fecha_Fin { get; set; }
    //    public List<Historicos> ListaHistorico { get; set; }
    //    public Historicos Historicos { get; set; }

    //}

    [Table("Historico")]
    public class Historico
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Delegacion { get; set; }
        public string Plaza { get; set; }
        public string Fecha { get; set; }
        public string Cuerpo { get; set; }
        public string Carril { get; set; }
        public string Clase { get; set; }
        public string Saldo { get; set; }
        public string Operador { get; set; }
    }
}