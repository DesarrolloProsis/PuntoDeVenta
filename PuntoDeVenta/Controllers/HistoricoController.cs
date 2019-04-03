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
    [Authorize(Roles = "SuperUsuario, Cajero")]
    public class HistoricoController : Controller
    {

        public static DataTable dtstatic = new DataTable();
        public static List<Historicos> ListPDF = new List<Historicos>();
        public static string Fecha1;
        public static string Fecha2;
        public static string Plaza;
        public static string cuenta;
        public static string saldo;
        public static string eventos;
        // GET: Historico
        [HttpGet]
        public ActionResult Index()
        {
            var model = new TableHistorico();
            model.Fecha_Inicio = DateTime.Now;
            model.Fecha_Fin = DateTime.Now;
            return View(model);
        }

        public ActionResult Regresar()
        {

            var model = new TableHistorico();
            model.Fecha_Inicio = DateTime.Now;
            model.Fecha_Fin = DateTime.Now;
            return View("Index", model);
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
                Text = "SIVA",
                Value = "SIVA"
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

        public ActionResult GenerarXCuenta(TableHistorico model)

        {

            ListPDF.Clear();
            Fecha1 = string.Empty;
            Fecha2 = string.Empty;
            Plaza = string.Empty;
            string Fecha_Inicio = model.Fecha_Inicio.ToString("dd/MM/yyyy");
            string Fecha_Fin = model.Fecha_Fin.ToString("dd/MM/yyyy");
            string Tag = model.Tag;
            string Cuenta = model.Cuenta;
            //object Info;
            AppDbContext db = new AppDbContext();
            List<Historicos> List = new List<Historicos>();
            DataTable dt = new DataTable();

            if (Tag != null && Tag != "")
            {
                if (model.Fecha_Fin > model.Fecha_Inicio || Fecha_Inicio != "01/01/0001" && Fecha_Fin != "01/01/0001")
                {
                    if (Fecha_Inicio == Fecha_Fin)
                    {
                        DateTime Fecha_Ayuda = model.Fecha_Inicio.AddDays(1);


                        var Saldo = db.Tags.Where(x => x.NumTag == Tag).ToList();
                        string Saldo_ = Convert.ToString((Convert.ToInt32(Saldo[0].SaldoTag.ToString()) / 100));


                        if (Saldo.Count > 0)
                        {

                            var Lista = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha < Fecha_Ayuda).Where(x => x.Tag == Tag).ToList();
                            var Movimientos = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha < Fecha_Ayuda).Where(x => x.Tag == Tag).Count();
                            //Si es un Tag Mandar un True en el object
                            model.Info = new { Saldo_, Fecha_Inicio, Fecha_Fin, Movimientos, Cuenta = Tag };
                            cuenta = Tag;
                            saldo = Saldo_;
                            eventos = Convert.ToString(Movimientos);
                            Crear_Lista(Lista);
                        }
                        else
                        {
                            //Terminar TAG NO VALIDO
                        }

                    }
                    else
                    {
                        List<string> Pruebas = new List<string>();

                        var Saldo = db.Tags.Where(x => x.NumTag == Tag).ToList();


                        foreach (var item in Saldo)
                        {
                            Pruebas.Add(item.NumTag);
                        }

                        if (Saldo.Count > 0)
                        {
                            string Saldo_ = Convert.ToString((Convert.ToInt32(Saldo[0].SaldoTag.ToString()) / 100));
                            var Lista = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha <= model.Fecha_Fin).Where(x => Pruebas.Contains(x.Tag)).ToList();
                            var Movimientos = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha <= model.Fecha_Fin).Where(x => Pruebas.Contains(x.Tag)).Count();
                            //Si es un Tag Mandar un True en el object
                            model.Info = new { Saldo_, Fecha_Inicio, Fecha_Fin, Movimientos, Cuenta = Tag };
                            cuenta = Tag;
                            saldo = Saldo_;
                            eventos = Convert.ToString(Movimientos);
                            Crear_Lista(Lista);
                        }

                        else
                        {
                            //Terminar TAG NO VALIDO
                        }
                    }
                }


            }
            else if (Cuenta != null && Cuenta != "")
            {
                if (model.Fecha_Fin > model.Fecha_Inicio || Fecha_Inicio != "01/01/0001" && Fecha_Fin != "01/01/0001")
                {
                    if (Fecha_Inicio == Fecha_Fin)
                    {
                        DateTime Fecha_Ayuda = model.Fecha_Inicio.AddDays(1);
                        List<string> Pruebas = new List<string>();
                        var Saldo = db.CuentasTelepeajes.Where(x => x.NumCuenta == Cuenta).ToList();



                        if (Saldo.Count > 0)
                        {
                            var ID = Saldo[0].Id;
                            string Saldo_ = Convert.ToString((Convert.ToInt32(Saldo[0].SaldoCuenta.ToString()) / 100));
                            var her = db.Tags.Where(x => x.CuentaId == ID).ToList();

                            foreach (var item in her)
                            {
                                Pruebas.Add(item.NumTag);
                            }

                            var Lista = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha < Fecha_Ayuda).Where(x => Pruebas.Contains(x.Tag)).ToList();
                            var Movimientos = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha < Fecha_Ayuda).Where(x => Pruebas.Contains(x.Tag)).Count();
                            model.Info = new { Saldo_, Fecha_Inicio, Fecha_Fin, Movimientos, Cuenta };
                            cuenta = Cuenta;
                            saldo = Saldo_;
                            eventos = Convert.ToString(Movimientos);
                            Crear_Lista(Lista);
                        }
                        else
                        {

                            //Terminar TAG NO VALIDO
                        }
                    }
                    else
                    {
                        List<string> Pruebas = new List<string>();
                        var Saldo = db.CuentasTelepeajes.Where(x => x.NumCuenta == Cuenta).ToList();


                        if (Saldo.Count > 0)
                        {
                            var ID = Saldo[0].Id;
                            string Saldo_ = Convert.ToString((Convert.ToInt32(Saldo[0].SaldoCuenta.ToString()) / 100));
                            var her = db.Tags.Where(x => x.CuentaId == ID).ToList();

                            foreach (var item in her)
                            {
                                Pruebas.Add(item.NumTag);
                            }
                            var Lista = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha <= model.Fecha_Fin).Where(x => Pruebas.Contains(x.Tag)).ToList();
                            var Movimientos = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha <= model.Fecha_Fin).Where(x => Pruebas.Contains(x.Tag)).Count();
                            model.Info = new { Saldo_, Fecha_Inicio, Fecha_Fin, Movimientos, Cuenta };
                            cuenta = Cuenta;
                            saldo = Saldo_;
                            eventos = Convert.ToString(Movimientos);
                            Crear_Lista(Lista);
                        }
                        else
                        {
                            //Terminar TAG NO VALIDO
                        }
                    }
                }
            }
            else
            {
                //Terminar
            }

            if (ListPDF.Count == 0)
            {
                model.Mensaje = true;
                return View("Tabla_Historico", model);
            }
            else
            {
                model.ListaHistorico = ListPDF;
                Fecha1 = Fecha_Inicio;
                Fecha2 = Fecha_Fin;
                return View("Tabla_Historico", model);

            }


        }

        public ActionResult GenerarXFecha(TableHistorico model)
        {

            ListPDF.Clear();
            Fecha1 = string.Empty;
            Fecha2 = string.Empty;
            Plaza = string.Empty;
            string Fecha_Inicio = model.Fecha_Inicio.ToString("dd/MM/yyyy");
            string Fecha_Fin = model.Fecha_Fin.ToString("dd/MM/yyyy");
            string Tag = model.Tag;
            string Cuenta = model.Cuenta;
            //object Info;
            AppDbContext db = new AppDbContext();
            List<Historicos> List = new List<Historicos>();
            DataTable dt = new DataTable();


            if (model.Fecha_Fin > model.Fecha_Inicio)
            {
                var Lista = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha < model.Fecha_Fin).ToList();
                model.Info = new { Fecha_Inicio, Fecha_Fin, Tipo = "SOLO_FECHA" };
                Crear_Lista(Lista);
            }
            else if (Fecha_Inicio == Fecha_Fin)
            {
                DateTime Fecha_Ayuda = model.Fecha_Inicio.AddDays(1);
                var Lista = db.Historicos.Where(x => x.Fecha >= model.Fecha_Inicio && x.Fecha < model.Fecha_Fin).ToList();
                model.Info = new { Fecha_Inicio, Fecha_Fin, Tipo = "SOLO_FECHA" };
                Crear_Lista(Lista);
            }


            if (ListPDF.Count == 0)
            {
                model.Mensaje = true;
                cuenta = null;
                return View("Tabla_Historico", model);
            }
            else
            {

                Fecha1 = Fecha_Inicio;
                Fecha2 = Fecha_Fin;
                model.ListaHistorico = ListPDF;
                cuenta = null;
                return View("Tabla_Historico", model);

            }

        }
        public List<Historico> Crear_Lista(System.Collections.Generic.List<PuntoDeVenta.Models.Historico> enumerables)
        {
            List<Historicos> List = new List<Historicos>();

            foreach (var item in enumerables)
            {

                List.Add(new Historicos
                {
                    Id = item.Id,
                    Tag = item.Tag,
                    Delegacion = item.Delegacion,
                    Plaza = item.Plaza,
                    Fecha = item.Fecha.ToString(),
                    Cuerpo = item.Cuerpo,
                    Carril = item.Carril,
                    Clase = item.Clase,
                    Saldo = item.Saldo.ToString(),
                    Operador = item.Operador


                });
            }


            ListPDF = List;

            return null;
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

            string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\SIVAREPORT.png");
            //Logo = iTextSharp.text.Image.GetInstance(rutaLogo);

            iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
            //Logo.SetAbsolutePosition(50, 520);
            Logo.SetAbsolutePosition(30, 456);
            PdfHistorico.Add(Logo);


            if (cuenta != "" && cuenta != null)
            {


                if (Fecha1 == Fecha2)
                {

                    Paragraph titulo = new Paragraph("                                                                                                                         REPORTE DEL HISTORICO\n", new Font(Font.FontFamily.HELVETICA, 12));
                    titulo.Alignment = Element.ALIGN_JUSTIFIED;
                    PdfHistorico.Add(titulo);


                    Paragraph fecha = new Paragraph("                        Cuenta: " + cuenta + "                                                        Fecha: " + Fecha1 + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.ALIGN_JUSTIFIED_ALL;
                    PdfHistorico.Add(fecha);


                    Paragraph Saldo = new Paragraph("                                        Movimiento: " + eventos + "                                                                                                            Saldo: " + saldo + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.ALIGN_JUSTIFIED;
                    PdfHistorico.Add(Saldo);

                    PdfHistorico.Add(Chunk.NEWLINE);


                }
                else
                {
                    Paragraph titulo = new Paragraph("                                                                                                      REPORTE DEL HISTORICO\n", new Font(Font.FontFamily.HELVETICA, 12));
                    titulo.Alignment = Element.ALIGN_JUSTIFIED;
                    PdfHistorico.Add(titulo);


                    Paragraph fecha = new Paragraph("                        Cuenta: " + cuenta + "                                                        Fecha: " + Fecha1 + " al " + Fecha2 + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.ALIGN_JUSTIFIED_ALL;
                    PdfHistorico.Add(fecha);



                    Paragraph Saldo = new Paragraph("                                        Movimientos: " + eventos + "                                                                                                           Saldo: " + saldo + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.ALIGN_JUSTIFIED_ALL;
                    PdfHistorico.Add(Saldo);

                    PdfHistorico.Add(Chunk.NEWLINE);

                }




            }
            else
            {

                if (Fecha1 == Fecha2)
                {

                    Paragraph titulo = new Paragraph("                                                                                                                         REPORTE DEL HISTORICO\n", new Font(Font.FontFamily.HELVETICA, 12));
                    titulo.Alignment = Element.ALIGN_JUSTIFIED;
                    PdfHistorico.Add(titulo);


                    Paragraph fecha = new Paragraph("                        Cuenta: " + "-----------" + "                                                        Fecha: " + Fecha1 + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.ALIGN_JUSTIFIED_ALL;
                    PdfHistorico.Add(fecha);

                    PdfHistorico.Add(Chunk.NEWLINE);


                }
                else
                {
                    Paragraph titulo = new Paragraph("                                                                                                      REPORTE DEL HISTORICO\n", new Font(Font.FontFamily.HELVETICA, 12));
                    titulo.Alignment = Element.ALIGN_JUSTIFIED;
                    PdfHistorico.Add(titulo);


                    Paragraph fecha = new Paragraph("                        Cuenta: " + "-----------" + "                                                        Fecha: " + Fecha1 + " al " + Fecha2 + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.ALIGN_JUSTIFIED_ALL;
                    PdfHistorico.Add(fecha);

                    PdfHistorico.Add(Chunk.NEWLINE);

                }
            }


            PdfPTable table = new PdfPTable(8);
            table.WidthPercentage = 100f;
            var coldWidthPorcentages = new[] { 3f, 2f, 1f, 2f, 5f, 2f, 2f, 3f };
            table.SetWidths(coldWidthPorcentages);

            foreach (var item in ListPDF)
            {

                PdfPCell _cell = new PdfPCell();
                PdfHistorico.GetLeft(40f);
                PdfHistorico.GetRight(40f);


                //table.AddCell(new Paragraph(item["NumTag"].ToString()));
                //_cell = new PdfPCell(new Paragraph(item["NumTag"].ToString()));
                _cell = new PdfPCell(new Paragraph(item.Tag.ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Plaza"].ToString()));
                _cell = new PdfPCell(new Paragraph(item.Plaza.ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Cuerpo"].ToString()));
                _cell = new PdfPCell(new Paragraph(item.Cuerpo.ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);



                //table.AddCell(new Paragraph(item["Carril"].ToString()));
                _cell = new PdfPCell(new Paragraph(item.Carril.ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Fecha"].ToString()));
                _cell = new PdfPCell(new Paragraph(item.Fecha.ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Clase"].ToString()));
                _cell = new PdfPCell(new Paragraph(item.Clase.ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);

                //table.AddCell(new Paragraph(item["SaldoTag"].ToString()));
                //_cell = new PdfPCell(new Paragraph(item["SaldoTag"].ToString()));
                _cell = new PdfPCell(new Paragraph(item.Saldo.ToString()));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.FixedHeight = 10f;
                table.AddCell(_cell);


                //table.AddCell(new Paragraph(item["Operador"].ToString()));
                _cell = new PdfPCell(new Paragraph(item.Operador.ToString()));
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
