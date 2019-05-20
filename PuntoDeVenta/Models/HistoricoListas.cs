using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntoDeVenta.Models
{
  public class HistoricoListas
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public long Id { get; set; }
    public string Extension { get; set; }
    public string Tamaño { get; set; }
    public DateTime Fecha_Creacion { get; set; }
    public string Tipo { get; set; }
  }
}
