using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PuntoDeVenta.Models;

namespace PuntoDeVenta.Controllers
{
    public class HistoricoController : Controller
    {

        // GET: Historico
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generar(TableHistorico model)
        {
            string Fecha_Inicio = model.Fecha_Inicio.ToString("dd/MM/yyyy");
            string Fecha_Fin = model.Fecha_Fin.ToString("dd/MM/yyyy");
            DataTable dt = new DataTable();
            if (Fecha_Fin == "01/01/0001")
            {
                if (Fecha_Inicio != "01/01/0001")
                {
                    dt = Llenar_Tabla(model.Fecha_Inicio);
                }
            }
            else if (model.Fecha_Fin > model.Fecha_Inicio)
            {
                dt = Llenar_Tabla(model.Fecha_Inicio, model.Fecha_Fin);
            }


            return View();
        }

        public DataTable Llenar_Tabla(DateTime Fecha_Inicio, DateTime Fecha_Fin)
        {
            DataTable dt = new DataTable();
            string conexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(conexion);


            using (SqlCommand cmd = new SqlCommand("", sqlConnection))
            {
                sqlConnection.Open();
                string _Fecha = Fecha_Inicio.ToString("yyyyMMdd");
                string Fecha_ = Fecha_Fin.ToString("yyyyMMdd");
                cmd.CommandText = "Select * From Historico Where Fecha >= '" + _Fecha + "' and Fecha <= '" + Fecha_ + "'";
                cmd.ExecuteNonQuery();
                SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
                sqlData.Fill(dt);
                sqlConnection.Close();


            }


            return dt;
        }
        public DataTable Llenar_Tabla(DateTime Fecha_Inicio)
        {
            DataTable dt = new DataTable();
            string conexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(conexion);

            using (SqlCommand cmd = new SqlCommand("", sqlConnection))
            {
                sqlConnection.Open();
                string _Fecha = Fecha_Inicio.ToString("yyyyMMdd");
                string Fecha_ = Fecha_Inicio.AddDays(1).ToString("yyyyMMdd");
                cmd.CommandText = "Select * From Historico Where Fecha = '" + _Fecha + "'";
                cmd.ExecuteNonQuery();
                SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
                sqlData.Fill(dt);
                sqlConnection.Close();
            }
            return dt;
        }
    }
}