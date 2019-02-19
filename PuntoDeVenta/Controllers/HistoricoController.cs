using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PuntoDeVenta.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.testutils;
using System.IO;


namespace PuntoDeVenta.Controllers
{
    public class HistoricoController : Controller
    {

        public static DataTable dtstatic = new DataTable();
        public static string Fecha1;
        public static string Fecha2;
        public static string Plaza;
        // GET: Historico
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Regresar()
        {
            return View("Index");
        }

        public JsonResult GetOperadores()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.Add(new SelectListItem
            {
                Text = "Todos",
                Value = "Todos"
            });

            Items.Add(new SelectListItem
            {
                Text = "Prosis",
                Value = "Prosis"
            });

            Items.Add(new SelectListItem
            {
                Text = "IAVE",
                Value = "IAVE"
            });

            Items.Add(new SelectListItem
            {
                Text = "Pase Urbano",
                Value = "Pase Urbano"
            });

            Items.Add(new SelectListItem
            {
                Text = "Viapass",
                Value = "Viapass"
            });

            Items.Add(new SelectListItem
            {
                Text = "Quickpass ",
                Value = "Quickpass "
            });

            Items.Add(new SelectListItem
            {
                Text = "E-Pass",
                Value = "E-Pass"
            });

            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Generar(TableHistorico model)
        {
            Fecha1 = string.Empty;
            Fecha2 = string.Empty;
            Plaza = string.Empty;
            string Fecha_Inicio = model.Fecha_Inicio.ToString("dd/MM/yyyy");
            string Fecha_Fin = model.Fecha_Fin.ToString("dd/MM/yyyy");
            string Operador = model.Operador;
            string Tag = model.Tag;
            Fecha1 = Fecha_Inicio;
 
            
            DataTable dt = new DataTable();
            if (Fecha_Fin == "01/01/0001")
            {
                if (Fecha_Inicio != "01/01/0001")
                {
                        
                        dt = Llenar_Tabla(model.Fecha_Inicio, Operador, Tag);
                        dtstatic = dt;

                }
                else if (Fecha_Inicio == "01/01/0001")
                    return View("Index");
            }
            else if (model.Fecha_Fin > model.Fecha_Inicio)
            {
                dt = Llenar_Tabla(model.Fecha_Inicio, model.Fecha_Fin, Operador, Tag);
                dtstatic = dt;
                Fecha2 = Fecha_Fin;
            }
            else if (Fecha_Fin == "01/01/0001")
            {
                if (Fecha_Inicio == "01/01/0001")
                    return View("Index");
            }
            else
            {
                return View("Index");
            }
            List<Historicos> Lista = new List<Historicos>();
            int id = 0;
            foreach (DataRow item in dt.Rows)
            {
                Historicos newrow = new Historicos();
                newrow.Id = id++;
                //newrow.Tag = item["NumTag"].ToString();
                newrow.Tag = item["Tag"].ToString();
                newrow.Delegacion = item["Delegacion"].ToString();
                newrow.Plaza = item["Plaza"].ToString();
                newrow.Cuerpo = item["Cuerpo"].ToString();
                newrow.Carril = item["Carril"].ToString();
                newrow.Fecha = item["Fecha"].ToString();
                newrow.Clase = item["Clase"].ToString();
                //newrow.Saldo = item["SaldoTag"].ToString();
                newrow.Saldo = item["Saldo"].ToString();
                newrow.Operador = item["Operador"].ToString();
                Lista.Add(newrow);
            }

            if (Lista.Count > 0)
            {
                Plaza = Lista[0].Plaza.ToString();
            }
            model.ListaHistorico = Lista;

            return View("Tabla_Historico", model);
        }

        public DataTable Llenar_Tabla(DateTime Fecha_Inicio, DateTime Fecha_Fin, string Operador, string Tag)
        {
            DataTable dt = new DataTable();
            string conexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(conexion);


            if (Operador == null || Operador == "Todos")
            {
                using (SqlCommand cmd = new SqlCommand("", sqlConnection))
                {
                    sqlConnection.Open();
                    string _Fecha = Fecha_Inicio.ToString("yyyyMMdd");
                    string Fecha_ = Fecha_Fin.AddDays(1).ToString("yyyyMMdd");
                    cmd.CommandText = "Select * From Historico Where Fecha >= '" + _Fecha + "' and Fecha <= '" + Fecha_ + "' and NumTag Like '%" + Tag + "%' order by Fecha desc";
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
                    sqlData.Fill(dt);
                    sqlConnection.Close();


                }
            }
            else
            {

                using (SqlCommand cmd = new SqlCommand("", sqlConnection))
                {
                    sqlConnection.Open();
                    string _Fecha = Fecha_Inicio.ToString("yyyyMMdd");
                    string Fecha_ = Fecha_Fin.AddDays(1).ToString("yyyyMMdd");
                    cmd.CommandText = "Select * From Historico Where Fecha >= '" + _Fecha + "' and Fecha <= '" + Fecha_ + "'and Operador = '" + Operador + "'  and NumTag Like '%" + Tag + "%' order by Fecha desc";
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
                    sqlData.Fill(dt);
                    sqlConnection.Close();

                }
            }


            return dt;
        }
        public DataTable Llenar_Tabla(DateTime Fecha_Inicio, string Operador, string Tag)
        {
            DataTable dt = new DataTable();
            string conexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(conexion);

            if (Operador == null || Operador == "Todos")
            {
                using (SqlCommand cmd = new SqlCommand("", sqlConnection))
                {
                    sqlConnection.Open();
                    string _Fecha = Fecha_Inicio.ToString("yyyyMMdd");
                    string Fecha_ = Fecha_Inicio.AddDays(1).ToString("yyyyMMdd");
                    //cmd.CommandText = "Select * From Historico Where Fecha >= '" + _Fecha + "' and Fecha <= '" + Fecha_ + "' and NumTag Like '%" + Tag + "%' order by Fecha desc";
                    cmd.CommandText = "Select * From Historico Where Fecha >= '" + _Fecha + "' and Fecha <= '" + Fecha_ + "' and NumTag Like '%" + Tag + "%' order by Fecha desc";
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
                    sqlData.Fill(dt);
                    sqlConnection.Close();
                }
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("", sqlConnection))
                {
                    sqlConnection.Open();
                    string _Fecha = Fecha_Inicio.ToString("yyyyMMdd");
                    string Fecha_ = Fecha_Inicio.AddDays(1).ToString("yyyyMMdd");
                    cmd.CommandText = "Select * From Historico Where Fecha >= '" + _Fecha + "' and Fecha <= '" + Fecha_ + "' and Operador = '" + Operador + "'  and NumTag Like '%" + Tag + "%' order by Fecha desc";
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
                    sqlData.Fill(dt);
                    sqlConnection.Close();
                }
            }
            return dt;
        }

        public ActionResult Pdf()
        {
            //FileStream fs = new FileStream("c://Documentos//Reporte.pdf", FileMode.Create);
            MemoryStream ms = new MemoryStream();
            Document PdfHistorico = new Document(iTextSharp.text.PageSize.A4.Rotate());
            PdfWriter pw = PdfWriter.GetInstance(PdfHistorico, ms);

            PdfHistorico.Open();
            PdfHistorico.GetTop(600f);

            //Image Logo;
            //PdfPCell Celda;
            //PdfPTable Table;

            string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\Logo2.png");
            //Logo = iTextSharp.text.Image.GetInstance(rutaLogo);

            iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
            Logo.SetAbsolutePosition(50, 520);
            PdfHistorico.Add(Logo);

            if (Fecha2 == "" || Fecha2 == null)

            {

                Paragraph titulo = new Paragraph("                                                                                                                         Reporte de Historico\n", new Font(Font.FontFamily.HELVETICA, 12));
                titulo.Alignment = Element.ALIGN_JUSTIFIED;
                PdfHistorico.Add(titulo);


                Paragraph Fecha = new Paragraph("                        Plaza: " + Plaza + "                                                        Fecha: " + Fecha1 + "", new Font(Font.FontFamily.HELVETICA, 12));
                Fecha.Alignment = Element.ALIGN_JUSTIFIED_ALL;
                PdfHistorico.Add(Fecha);

                PdfHistorico.Add(Chunk.NEWLINE);


            }
            else
            {
                Paragraph titulo = new Paragraph("                                                                                                      Reporte de Historico\n", new Font(Font.FontFamily.HELVETICA, 12));
                titulo.Alignment = Element.ALIGN_JUSTIFIED;
                PdfHistorico.Add(titulo);


                Paragraph Fecha = new Paragraph("                        Plaza: " + Plaza + "                                                        Fecha: " + Fecha1 + " al " + Fecha2 + "", new Font(Font.FontFamily.HELVETICA, 12));
                Fecha.Alignment = Element.ALIGN_JUSTIFIED_ALL;
                PdfHistorico.Add(Fecha);

                PdfHistorico.Add(Chunk.NEWLINE);

            }







            PdfPTable table = new PdfPTable(8);
            table.WidthPercentage = 100f;
            var coldWidthPorcentages = new[] { 3f, 2f, 1f, 2f, 5f, 2f, 2f, 3f };
            table.SetWidths(coldWidthPorcentages);

            foreach (DataRow item in dtstatic.Rows)
            {

                PdfPCell _cell = new PdfPCell();
                PdfHistorico.GetLeft(40f);
                PdfHistorico.GetRight(40f);


                //table.AddCell(new Paragraph(item["NumTag"].ToString()));
                //_cell = new PdfPCell(new Paragraph(item["NumTag"].ToString()));
                _cell = new PdfPCell(new Paragraph(item["Tag"].ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Plaza"].ToString()));
                _cell = new PdfPCell(new Paragraph(item["Plaza"].ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Cuerpo"].ToString()));
                _cell = new PdfPCell(new Paragraph(item["Cuerpo"].ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);



                //table.AddCell(new Paragraph(item["Carril"].ToString()));
                _cell = new PdfPCell(new Paragraph(item["Carril"].ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Fecha"].ToString()));
                _cell = new PdfPCell(new Paragraph(item["Fecha"].ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Clase"].ToString()));
                _cell = new PdfPCell(new Paragraph(item["Clase"].ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);

                //table.AddCell(new Paragraph(item["SaldoTag"].ToString()));
                //_cell = new PdfPCell(new Paragraph(item["SaldoTag"].ToString()));
                _cell = new PdfPCell(new Paragraph(item["Saldo"].ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Operador"].ToString()));
                _cell = new PdfPCell(new Paragraph(item["Operador"].ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);

            }
            PdfHistorico.Add(table);

            PdfHistorico.Close();


            byte[] bytesStream = ms.ToArray();
            ms = new MemoryStream();
            ms.Write(bytesStream, 0, bytesStream.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "application/pdf");
            //return View("Index");
        }
    }
}