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
using System.Collections;
using System.Globalization;
using iTextSharp.text.html;

namespace PuntoDeVenta.Controllers
{
    [Authorize]
    public class HistoricoController : Controller
    {

        public static DataTable dtstatic = new DataTable();
        public static List<Cruces> ListPDFCruces = new List<Cruces>();
        public static List<Movimientos> ListPDFMovimientos = new List<Movimientos>();
        public static List<CruceMovimiento> ListPDFCrucesMovimientos = new List<CruceMovimiento>();
        public static bool ClienteAdmin;
        public static string cliente;
        public static string eventos;
        public static string cuenta;
        public static string Fecha1;
        public static string Fecha2;
        public static string Plaza;
        public static string saldo;
        public static string saldoMov;
        public static string saldoCru;
        public static object Info;
        public string NmbredelCLiente;

        // GET: Historico

        [HttpGet]
        [Authorize(Roles = "GenerarReporte, Cajero, SuperUsuario")]
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
        public ActionResult RepoMensual()
        {
            return View("ReporteMensual");
        }

        public JsonResult GetMesAnyo()
        {
            List<SelectListItem> mes = new List<SelectListItem>();
            List<SelectListItem> anyo = new List<SelectListItem>();

            mes.Add(new SelectListItem
            {
                Text = "Enero",
                Value = "01"
            });
            mes.Add(new SelectListItem
            {
                Text = "Febrero",
                Value = "02"
            });
            mes.Add(new SelectListItem
            {
                Text = "Marzo",
                Value = "03"
            });
            mes.Add(new SelectListItem
            {
                Text = "Abril",
                Value = "04"
            });
            mes.Add(new SelectListItem
            {
                Text = "Mayo",
                Value = "05"
            });

            mes.Add(new SelectListItem
            {
                Text = "Junio",
                Value = "06"
            });
            mes.Add(new SelectListItem
            {
                Text = "Julio",
                Value = "07"
            });
            mes.Add(new SelectListItem
            {
                Text = "Agosto",
                Value = "08"
            });
            mes.Add(new SelectListItem
            {
                Text = "Septiembre",
                Value = "09"
            });
            mes.Add(new SelectListItem
            {
                Text = "Octubre",
                Value = "10"
            });
            mes.Add(new SelectListItem
            {
                Text = "Noviembre",
                Value = "11"
            });
            mes.Add(new SelectListItem
            {
                Text = "Diciembre",
                Value = "12"
            });

            anyo.Add(new SelectListItem
            {
                Text = "2019",
                Value = "2019"
            });
            anyo.Add(new SelectListItem
            {
                Text = "2020",
                Value = "2020"
            });
            anyo.Add(new SelectListItem
            {
                Text = "2021",
                Value = "2021"
            });
            anyo.Add(new SelectListItem
            {
                Text = "2022",
                Value = "2022"
            });
            anyo.Add(new SelectListItem
            {
                Text = "2023",
                Value = "2023"
            });
            anyo.Add(new SelectListItem
            {
                Text = "2024",
                Value = "2024"
            });
            anyo.Add(new SelectListItem
            {
                Text = "2025",
                Value = "2025"
            });
            anyo.Add(new SelectListItem
            {
                Text = "2026",
                Value = "2026"
            });

            object json = new { mes, anyo };

            return Json(json, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetMovimientos2()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.Add(new SelectListItem
            {
                Text = "Movimientos y Cruces",
                Value = "00"
            });

            Items.Add(new SelectListItem
            {
                Text = "Recargas",
                Value = "01"
            });

            Items.Add(new SelectListItem
            {
                Text = "Cruces",
                Value = "02"
            });

            return Json(Items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMovimientos()
        {
            List<SelectListItem> Items = new List<SelectListItem>();


            Items.Add(new SelectListItem
            {
                Text = "Recargas",
                Value = "01"
            });

            Items.Add(new SelectListItem
            {
                Text = "Cruces",
                Value = "02"
            });

            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteMes(TableHistorico model)
        {
            var RangosFecha = IntervalosMes(model.Mes, model.Anyo);

            if (RangosFecha == null)
            {


                MemoryStream ms = new MemoryStream();
                Document PdfHistorico = new Document(iTextSharp.text.PageSize.LETTER.Rotate());
                PdfWriter pw = PdfWriter.GetInstance(PdfHistorico, ms);

                PdfHistorico.Open();
                PdfHistorico.GetTop(600f);


                string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\SIVAREPORT.png");

                iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                Logo.SetAbsolutePosition(650, 400);
                PdfHistorico.Add(Logo);


                Paragraph titulo = new Paragraph("FECHA INVALIDA O SIN DATOS\n", new Font(Font.FontFamily.HELVETICA, 22));
                titulo.Alignment = Element.ALIGN_CENTER;
                PdfHistorico.Add(titulo);

                PdfHistorico.Close();


                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;

                return new FileStreamResult(ms, "application/pdf");
            }
            else
            {

                MemoryStream ms = new MemoryStream();
                Document PdfHistorico = new Document(iTextSharp.text.PageSize.LETTER.Rotate());
                PdfWriter pw = PdfWriter.GetInstance(PdfHistorico, ms);

                PdfHistorico.Open();
                PdfHistorico.GetTop(600f);





                string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\SIVAREPORT.png");

                iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                Logo.SetAbsolutePosition(650, 400);
                PdfHistorico.Add(Logo);





                Paragraph titulo = new Paragraph("REPORTE MENSUAL PUNTO DE VENTA\n", new Font(Font.FontFamily.HELVETICA, 22));
                titulo.Alignment = Element.ALIGN_CENTER;
                PdfHistorico.Add(titulo);

                PdfHistorico.Add(Chunk.NEWLINE);

                Paragraph fecha = new Paragraph("Fecha de Generacion Reporte: " + DateTime.Today.ToShortDateString().ToString() + "", new Font(Font.FontFamily.HELVETICA, 14));
                fecha.Alignment = Element.PTABLE;
                PdfHistorico.Add(fecha);


                Paragraph _cliente = new Paragraph("Mes del Reporte: " + RangosFecha[4] + "", new Font(Font.FontFamily.HELVETICA, 14));
                _cliente.Alignment = Element.PTABLE;
                PdfHistorico.Add(_cliente);


                Paragraph Saldo = new Paragraph("Año del Reporte: " + model.Anyo + "", new Font(Font.FontFamily.HELVETICA, 14));
                Saldo.Alignment = Element.PTABLE;
                PdfHistorico.Add(Saldo);


                PdfHistorico.Add(Chunk.NEWLINE);
                PdfHistorico.Add(Chunk.NEWLINE);
                PdfHistorico.Add(Chunk.NEWLINE);



                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100f;
                var coldWidthPorcentagesCliente = new[] { 2f, 2f, 2f };
                table.SetWidths(coldWidthPorcentagesCliente);

                PdfPCell _cellIni = new PdfPCell();
                PdfHistorico.GetLeft(40f);
                PdfHistorico.GetRight(40f);


                var FontColour1 = new BaseColor(255, 0, 0);
                var ColorRojo = FontFactory.GetFont("Times New Roman", 12, FontColour1);

                var FontColour2 = new BaseColor(0, 255, 0);
                var ColorVerde = FontFactory.GetFont("Times New Roman", 12, FontColour2);


                var Tabla1List = TablaRepoMes1(RangosFecha);


                _cellIni = new PdfPCell(new Paragraph("", new Font(Font.FontFamily.HELVETICA, 12)));
                _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                table.AddCell(_cellIni);


                _cellIni = new PdfPCell(new Paragraph("NUEVOS ESTE MES", new Font(Font.FontFamily.HELVETICA, 12)));
                _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                table.AddCell(_cellIni);

                _cellIni = new PdfPCell(new Paragraph("TOTAL", new Font(Font.FontFamily.HELVETICA, 12)));
                _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                table.AddCell(_cellIni);


                foreach (var item in Tabla1List)
                {

                    _cellIni = new PdfPCell(new Paragraph(item.columnaDatos.ToString(), new Font(Font.FontFamily.HELVETICA, 12)));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph(item.totalActual.ToString(), new Font(Font.FontFamily.HELVETICA, 12)));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cellIni);

                    _cellIni = new PdfPCell(new Paragraph(item.totalRegistros.ToString(), new Font(Font.FontFamily.HELVETICA, 12)));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cellIni);
                }

                PdfHistorico.Add(table);


                //-----------

                PdfHistorico.Add(Chunk.NEWLINE);
                PdfHistorico.Add(Chunk.NEWLINE);
                PdfHistorico.Add(Chunk.NEWLINE);

                PdfPTable table2 = new PdfPTable(3);
                table2.WidthPercentage = 100f;
                var coldWidthPorcentagesCliente2 = new[] { 1f, 1f, 1f };
                table2.SetWidths(coldWidthPorcentagesCliente2);

                PdfPCell _cellIni2 = new PdfPCell();
                PdfHistorico.GetLeft(40f);
                PdfHistorico.GetRight(40f);

                var Tabla2List = TablaRepoMes2(RangosFecha);


                _cellIni2 = new PdfPCell(new Paragraph("RECARGAS DE ESTE MES", new Font(Font.FontFamily.HELVETICA, 12)));
                _cellIni2.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni2.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                table2.AddCell(_cellIni2);

                _cellIni2 = new PdfPCell(new Paragraph("CRUCES DE ESTE MES", new Font(Font.FontFamily.HELVETICA, 12)));
                _cellIni2.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni2.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                table2.AddCell(_cellIni2);

                _cellIni2 = new PdfPCell(new Paragraph("TOTAL DE ESTE MES", new Font(Font.FontFamily.HELVETICA, 12)));
                _cellIni2.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni2.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                table2.AddCell(_cellIni2);

                foreach (var item2 in Tabla2List)
                {


                    _cellIni2 = new PdfPCell(new Paragraph(item2.recargaActual.ToString(), ColorVerde));
                    _cellIni2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table2.AddCell(_cellIni2);

                    _cellIni2 = new PdfPCell(new Paragraph(item2.cruceActual.ToString(), ColorRojo));
                    _cellIni2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table2.AddCell(_cellIni2);

                    _cellIni2 = new PdfPCell(new Paragraph(item2.totalEfectivo.ToString(), new Font(Font.FontFamily.HELVETICA, 12)));
                    _cellIni2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table2.AddCell(_cellIni2);

                }
                //---------


                PdfHistorico.Add(table2);

                PdfHistorico.Close();


                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;

                return new FileStreamResult(ms, "application/pdf");
            }
        }

        public ActionResult GenerarXCuenta(TableHistorico model)
        {

            ListPDFCruces.Clear();
            ListPDFMovimientos.Clear();
            Fecha1 = string.Empty;
            Fecha2 = string.Empty;
            Plaza = string.Empty;
            string Fecha_Inicio = model.Fecha_Inicio.ToString("dd/MM/yyyy");
            string Fecha_Fin = model.Fecha_Fin.ToString("dd/MM/yyyy");
            string Tag = model.Tag;
            string Cuenta = model.Cuenta;
            string TypeMovimiento = model.TypeMovimiento;
            string Tipo = "";
            saldoCru = string.Empty;
            saldoMov = string.Empty;
            AppDbContext db = new AppDbContext();
            List<Cruces> ListCruces = new List<Cruces>();
            List<Movimientos> ListMovimiento = new List<Movimientos>();
            DataTable dt = new DataTable();
            CultureInfo culture = new CultureInfo("es-MX", false);

            if (Tag != null && Tag != "")
            {

                if (model.Fecha_Fin > model.Fecha_Inicio || Fecha_Inicio != "01/01/0001" && Fecha_Fin != "01/01/0001")
                {
                    if (model.Fecha_Fin == DateTime.Now.Date)
                    {
                        DateTime DateAyuda = DateTime.Now;
                        if (TypeMovimiento == "00")
                        {


                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, DateAyuda, 1, true);
                            ListCruces = Cruces(Tag, model.Fecha_Inicio, DateAyuda, 1, true);

                            if (ListMovimiento.Count != 0 && ListCruces.Count != 0)
                            {

                                //if (ListMovimiento.Count != 0)
                                //{
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                                //}
                                //if (ListCruces.Count != 0)
                                //{
                                //    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                                //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                                //    model.ListaMovimientos = null;
                                //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count);
                                //    model.ListaCruces = null;
                                //    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                //    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                //    ClienteAdmin = false;
                                //    model.Info = Info;
                                //    return View("Tabla_Historico", model);
                                //}

                            }
                            else
                            {
                                if (ListMovimiento.Count != 0)
                                {
                                    Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                    model.ListaMovimientos = null;
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                    model.ListaCruces = null;
                                    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                    ClienteAdmin = false;
                                    model.Info = Info;
                                    return View("Tabla_Historico", model);
                                }
                                if (ListCruces.Count != 0)
                                {
                                    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListCruces[0].TotalMonetarioCruces };
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count); saldoCru = ListCruces[0].TotalMonetarioCruces;
                                    model.ListaMovimientos = null;
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count);
                                    model.ListaCruces = null;
                                    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                    ClienteAdmin = false;
                                    model.Info = Info;
                                    return View("Tabla_Historico", model);
                                }
                            }

                        }
                        else if (TypeMovimiento == "01")
                        {
                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, DateAyuda, 1, true);

                            if (ListMovimiento.Any())
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString(); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                model.ListaMovimientos = ListMovimiento;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                                model.ListaCruces = null;
                                model.ListCruceMovimientos = null;
                                ListPDFMovimientos = ListMovimiento;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);

                            }
                        }
                        else
                        {
                            ListCruces = Cruces(Tag, model.Fecha_Inicio, DateAyuda, 1, true);

                            if (ListCruces.Any())
                            {
                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListCruces[0].TotalMonetarioCruces };
                                model.ListaCruces = ListCruces;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString(); ; saldoCru = ListCruces[0].TotalMonetarioCruces;
                                model.ListaMovimientos = null;
                                model.ListCruceMovimientos = null;
                                ListPDFCruces = ListCruces;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);

                            }
                        }


                    }
                    else if (Fecha_Inicio == Fecha_Fin)
                    {
                        DateTime DateAyuda = model.Fecha_Inicio.AddDays(1);
                        if (TypeMovimiento == "00")
                        {


                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, DateAyuda, 1, false);
                            ListCruces = Cruces(Tag, model.Fecha_Inicio, DateAyuda, 1, false);




                            if (ListMovimiento.Count != 0 && ListCruces.Count != 0)
                            {

                                //if (ListMovimiento.Count != 0)
                                //{
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                                //}
                                //else if (ListCruces.Count != 0)
                                //{
                                //    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                                //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                                //    model.ListaMovimientos = null;
                                //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count);
                                //    model.ListaCruces = null;
                                //    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                //    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                //    ClienteAdmin = false;
                                //    model.Info = Info;
                                //    return View("Tabla_Historico", model);
                                //}

                            }
                            else
                            {
                                if (ListMovimiento.Count != 0)
                                {
                                    Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                    model.ListaMovimientos = null;
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                    model.ListaCruces = null;
                                    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                    ClienteAdmin = false;
                                    model.Info = Info;
                                    return View("Tabla_Historico", model);
                                }
                                if (ListCruces.Count != 0)
                                {
                                    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListCruces[0].TotalMonetarioCruces };
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count); saldoCru = ListCruces[0].TotalMonetarioCruces;
                                    model.ListaMovimientos = null;
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count);
                                    model.ListaCruces = null;
                                    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                    ClienteAdmin = false;
                                    model.Info = Info;
                                    return View("Tabla_Historico", model);
                                }
                            }


                        }

                        else if (TypeMovimiento == "01")
                        {
                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, DateAyuda, 1, false);

                            if (ListMovimiento.Any())
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                model.Info = Info;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString(); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                model.ListaMovimientos = ListMovimiento;
                                model.ListaCruces = null;
                                model.ListCruceMovimientos = null;
                                ListPDFMovimientos = ListMovimiento;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);

                            }
                        }
                        else
                        {
                            ListCruces = Cruces(Tag, model.Fecha_Inicio, DateAyuda, 1, false);

                            if (ListCruces.Any())
                            {
                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListCruces[0].TotalMonetarioCruces };
                                model.Info = Info;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString(); saldoCru = ListCruces[0].TotalMonetarioCruces;
                                model.ListaCruces = ListCruces;
                                model.ListaMovimientos = null;
                                model.ListCruceMovimientos = null;
                                ListPDFCruces = ListCruces;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);

                            }
                        }


                    }
                    else
                    {

                        if (TypeMovimiento == "00")
                        {


                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, model.Fecha_Fin, 1, false);
                            ListCruces = Cruces(Tag, model.Fecha_Inicio, model.Fecha_Fin, 1, false);




                            if (ListMovimiento.Count != 0 && ListCruces.Count != 0)
                            {

                                //if (ListMovimiento.Count != 0)
                                //{
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                                //}
                                //if (ListCruces.Count != 0)
                                //{
                                //    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                                //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                                //    model.ListaMovimientos = null;
                                //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                //    model.ListaCruces = null;
                                //    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                //    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                //    ClienteAdmin = false;
                                //    model.Info = Info;
                                //    return View("Tabla_Historico", model);
                                //}

                            }
                            else
                            {
                                if (ListMovimiento.Count != 0)
                                {
                                    Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                    model.ListaMovimientos = null;
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                    model.ListaCruces = null;
                                    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                    ClienteAdmin = false;
                                    model.Info = Info;
                                    return View("Tabla_Historico", model);
                                }
                                if (ListCruces.Count != 0)
                                {
                                    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListCruces[0].TotalMonetarioCruces };
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count); saldoCru = ListCruces[0].TotalMonetarioCruces;
                                    model.ListaMovimientos = null;
                                    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count);
                                    model.ListaCruces = null;
                                    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                    ClienteAdmin = false;
                                    model.Info = Info;
                                    return View("Tabla_Historico", model);
                                }
                            }


                        }
                        else if (TypeMovimiento == "01")
                        {
                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, model.Fecha_Fin, 1, false);

                            if (ListMovimiento.Any())
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                model.Info = Info;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString(); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                model.ListaMovimientos = ListMovimiento;
                                model.ListaCruces = null;
                                model.ListCruceMovimientos = null;
                                ListPDFMovimientos = ListMovimiento;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);

                            }
                        }
                        else
                        {
                            ListCruces = Cruces(Tag, model.Fecha_Inicio, model.Fecha_Fin, 1, false);

                            if (ListCruces.Any())
                            {
                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, ListCruces[0].TotalMonetarioCruces };
                                model.Info = Info;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString(); saldoCru = ListCruces[0].TotalMonetarioCruces;
                                model.ListaCruces = ListCruces;
                                model.ListaMovimientos = null;
                                model.ListCruceMovimientos = null;
                                ListPDFCruces = ListCruces;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);

                            }
                        }


                    }
                }


            }
            else if (Cuenta != null && Cuenta != "")
            {
                double total = 0;

                if (model.Fecha_Fin == DateTime.Now.Date)
                {
                    DateTime DateAyuda = DateTime.Now;
                    if (TypeMovimiento == "00")
                    {


                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, DateAyuda, 2, true);
                        ListCruces = Cruces(Cuenta, model.Fecha_Inicio, DateAyuda, 2, true);




                        if (ListMovimiento.Count != 0 && ListCruces.Count != 0)
                        {

                            foreach (var item in ListCruces)
                            {
                                total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                            }
                            //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                            var totalfinal = Convercion(total.ToString().Replace(".", ","));

                            //if (ListMovimiento.Count != 0)
                            //{
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, TotalMonetarioCruces = totalfinal };
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = totalfinal;
                            model.ListaMovimientos = null;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                            model.ListaCruces = null;
                            ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, true);
                            model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);
                            //}
                            //if (ListCruces.Count != 0)
                            //{
                            //    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                            //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                            //    model.ListaMovimientos = null;
                            //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                            //    model.ListaCruces = null;
                            //    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, true);
                            //    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                            //    ClienteAdmin = false;
                            //    model.Info = Info;
                            //    return View("Tabla_Historico", model);
                            //}

                        }
                        else
                        {
                            if (ListMovimiento.Count != 0)
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                            }
                            if (ListCruces.Count != 0)
                            {

                                foreach (var item in ListCruces)
                                {
                                    total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                                }
                                //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                                var totalfinal = Convercion(total.ToString().Replace(".", ","));

                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, TotalMonetarioCruces = totalfinal };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count); saldoCru = totalfinal;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                            }
                        }


                    }
                    else if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, DateAyuda, 2, true);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Cuenta, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString(); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
                            model.ListCruceMovimientos = null;
                            ListPDFMovimientos = ListMovimiento;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);

                        }
                    }
                    else
                    {
                        ListCruces = Cruces(Cuenta, model.Fecha_Inicio, DateAyuda, 2, true);

                        if (ListCruces.Any())
                        {


                            foreach (var item in ListCruces)
                            {
                                total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                            }
                            //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                            var totalfinal = Convercion(total.ToString().Replace(".", ","));

                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo, TotalMonetarioCruces = totalfinal };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString(); saldoCru = totalfinal;
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
                            model.ListCruceMovimientos = null;
                            ListPDFCruces = ListCruces;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);

                        }
                    }

                }
                else if (Fecha_Inicio == Fecha_Fin)
                {
                    DateTime DateAyuda = model.Fecha_Inicio.AddDays(1);
                    if (TypeMovimiento == "00")
                    {


                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, DateAyuda, 2, false);
                        ListCruces = Cruces(Cuenta, model.Fecha_Inicio, DateAyuda, 2, false);




                        if (ListMovimiento.Count != 0 && ListCruces.Count != 0)
                        {
                            foreach (var item in ListCruces)
                            {
                                total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                            }
                            //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                            var totalfinal = Convercion(total.ToString().Replace(".", ","));
                            //if (ListMovimiento.Count != 0)
                            //{
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, TotalMonetarioCruces = totalfinal };
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = totalfinal;
                            model.ListaMovimientos = null;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                            model.ListaCruces = null;
                            ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, true);
                            model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                            ClienteAdmin = false;
                            model.Info = Info;
                            //    return View("Tabla_Historico", model);
                            //}
                            //if (ListCruces.Count != 0)
                            //{
                            //    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                            //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                            //    model.ListaMovimientos = null;
                            //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                            //    model.ListaCruces = null;
                            //    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, true);
                            //    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                            //    ClienteAdmin = false;
                            //    model.Info = Info;
                            //    return View("Tabla_Historico", model);
                            //}

                        }
                        else
                        {
                            if (ListMovimiento.Count != 0)
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                            }
                            if (ListCruces.Count != 0)
                            {
                                foreach (var item in ListCruces)
                                {
                                    total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                                }
                                //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                                var totalfinal = Convercion(total.ToString().Replace(".", ","));

                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, TotalMonetarioCruces = totalfinal };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count); saldoCru = totalfinal;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                            }
                        }


                    }

                    else if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, DateAyuda, 2, false);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString(); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
                            model.ListCruceMovimientos = null;
                            ListPDFMovimientos = ListMovimiento;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);

                        }
                    }
                    else
                    {
                        ListCruces = Cruces(Cuenta, model.Fecha_Inicio, DateAyuda, 2, false);

                        if (ListCruces.Any())
                        {
                            foreach (var item in ListCruces)
                            {
                                total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                            }
                            //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                            var totalfinal = Convercion(total.ToString().Replace(".", ","));

                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo, TotalMonetarioCruces = totalfinal };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString(); saldoCru = totalfinal;
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
                            model.ListCruceMovimientos = null;
                            ListPDFCruces = ListCruces;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);

                        }
                    }


                }
                else
                {
                    if (TypeMovimiento == "00")
                    {


                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, model.Fecha_Fin, 2, true);
                        ListCruces = Cruces(Cuenta, model.Fecha_Inicio, model.Fecha_Fin, 2, true);




                        if (ListMovimiento.Count != 0 && ListCruces.Count != 0)
                        {
                            //if (ListMovimiento.Count != 0)
                            //{
                            foreach (var item in ListCruces)
                            {
                                total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                            }
                            //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                            var totalfinal = Convercion(total.ToString().Replace(".", ","));
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, TotalMonetarioCruces = totalfinal };
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = totalfinal;
                            model.ListaMovimientos = null;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                            model.ListaCruces = null;
                            ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, true);
                            model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);
                            //}
                            //if (ListCruces.Count != 0)
                            //{
                            //    Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos, ListCruces[0].TotalMonetarioCruces };
                            //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos; saldoCru = ListCruces[0].TotalMonetarioCruces;
                            //    model.ListaMovimientos = null;
                            //    Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                            //    model.ListaCruces = null;
                            //    ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, true);
                            //    model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                            //    ClienteAdmin = false;
                            //    model.Info = Info;
                            //    return View("Tabla_Historico", model);
                            //}

                        }
                        else
                        {
                            if (ListMovimiento.Count != 0)
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListMovimiento.Count + ListCruces.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = Convert.ToString(ListMovimiento.Count + ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                            }
                            if (ListCruces.Count != 0)
                            {
                                foreach (var item in ListCruces)
                                {
                                    total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                                }
                                //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                                var totalfinal = Convercion(total.ToString().Replace(".", ","));
                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, Count = ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo, TotalMonetarioCruces = totalfinal };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count); saldoCru = totalfinal;
                                model.ListaMovimientos = null;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = Convert.ToString(ListCruces.Count);
                                model.ListaCruces = null;
                                ListPDFCrucesMovimientos = FusionarListas(ListCruces, ListMovimiento, false);
                                model.ListCruceMovimientos = ListPDFCrucesMovimientos;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);
                            }
                        }


                    }
                    else if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, model.Fecha_Fin, 2, true);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString(); saldoMov = ListMovimiento[0].TotalMonetarioMovimientos;
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
                            model.ListCruceMovimientos = null;
                            ListPDFMovimientos = ListMovimiento;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);

                        }
                    }
                    else
                    {
                        ListCruces = Cruces(Cuenta, model.Fecha_Inicio, model.Fecha_Fin, 2, true);

                        if (ListCruces.Any())
                        {
                            foreach (var item in ListCruces)
                            {
                                total += Convert.ToDouble(item.Saldo.Replace("Q", "").Replace(".", ","));
                            }
                            //var totalfinal = Convercion((total / 100).ToString().Replace(".", ","));
                            var totalfinal = Convercion(total.ToString().Replace(".", ","));
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo, TotalMonetarioCruces = totalfinal };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString(); saldoCru = totalfinal;
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
                            model.ListCruceMovimientos = null;
                            ListPDFCruces = ListCruces;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);

                        }
                    }

                }
            }
            else
            {
                //Terminar
            }

            model.Mensaje = true;
            return View("Tabla_Historico", model);

        }

        public ActionResult GenerarXFecha(TableHistorico model)
        {

            ListPDFCruces.Clear();
            ListPDFMovimientos.Clear();
            Fecha1 = string.Empty;
            Fecha2 = string.Empty;
            Plaza = string.Empty;
            string Fecha_Inicio = model.Fecha_Inicio.ToString("dd/MM/yyyy");
            string Fecha_Fin = model.Fecha_Fin.ToString("dd/MM/yyyy");
            string Tag = model.Tag;
            string Cuenta = model.Cuenta;
            string TypeMovimiento = model.TypeMovimiento2;
            string Tipo = "SOLO_FECHA";
            //object Info;
            AppDbContext db = new AppDbContext();
            List<Cruces> ListCruces = new List<Cruces>();
            List<Movimientos> ListMovimiento = new List<Movimientos>();
            DataTable dt = new DataTable();


            if (model.Fecha_Fin > model.Fecha_Inicio || Fecha_Inicio != "01/01/0001" && Fecha_Fin != "01/01/0001")
            {
                if (model.Fecha_Fin == DateTime.Now.Date)
                {
                    DateTime DateAyuda = DateTime.Now;


                    if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, DateAyuda, 3, true);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
                            ListPDFMovimientos = ListMovimiento;
                            ClienteAdmin = true;
                            return View("Tabla_Historico", model);

                        }
                    }
                    else
                    {
                        ListCruces = Cruces(Tag, model.Fecha_Inicio, DateAyuda, 3, true);

                        if (ListCruces.Any())
                        {
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, Tipo, ListCruces[0].TotalMonetarioCruces };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
                            ListPDFCruces = ListCruces;
                            ClienteAdmin = true;
                            return View("Tabla_Historico", model);

                        }
                    }

                }
                else if (Fecha_Inicio == Fecha_Fin)
                {
                    DateTime DateAyuda = model.Fecha_Inicio.AddDays(1);

                    if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, DateAyuda, 3, false);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
                            ListPDFMovimientos = ListMovimiento;
                            ClienteAdmin = true;
                            return View("Tabla_Historico", model);

                        }
                    }
                    else
                    {
                        ListCruces = Cruces(Tag, model.Fecha_Inicio, DateAyuda, 3, false);

                        if (ListCruces.Any())
                        {
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, Tipo, ListCruces[0].TotalMonetarioCruces };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
                            ListPDFCruces = ListCruces;
                            ClienteAdmin = true;
                            return View("Tabla_Historico", model);

                        }
                    }


                }
                else
                {

                    if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, model.Fecha_Fin.AddDays(1), 3, false);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, Tipo, ListMovimiento[0].TotalMonetarioMovimientos };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
                            ListPDFMovimientos = ListMovimiento;
                            ClienteAdmin = true;
                            return View("Tabla_Historico", model);

                        }
                    }
                    else
                    {
                        ListCruces = Cruces(Tag, model.Fecha_Inicio, model.Fecha_Fin.AddDays(1), 3, false);

                        if (ListCruces.Any())
                        {
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, Tipo, ListCruces[0].TotalMonetarioCruces };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
                            ListPDFCruces = ListCruces;
                            ClienteAdmin = true;
                            return View("Tabla_Historico", model);

                        }
                    }


                }
            }


            model.Mensaje = true;
            return View("Tabla_Historico", model);


        }

        public List<Cruces> Cruces(string TagCuenta, DateTime Fecha_Inicio, DateTime Fecha_Fin, int Tipo, bool Rango)
        {
            CultureInfo culture = new CultureInfo("es-MX", false);
            AppDbContext db = new AppDbContext();
            //Tag
            if (Tipo == (int)DecisionesMetodos.Tag)
            {
                if (Rango)
                {

                    var ListaCompleta = (from historico in db.Historicos
                                         join tags in db.Tags on historico.Tag equals tags.NumTag
                                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (historico.Fecha >= Fecha_Inicio && historico.Fecha <= Fecha_Fin)
                                         where (historico.Tag == TagCuenta)
                                         orderby (historico.Fecha) descending
                                         select new
                                         {
                                             _Tag = historico.Tag,
                                             _CuentaID = tags.CuentaId,
                                             _ClienteID = cliente.NumCliente,
                                             _Nombre = cliente.Nombre + cliente.Apellidos,
                                             _TypeCuenta = cuentas.TypeCuenta,
                                            // _Plaza = historico.Delegacion,
                                             //_Cuerpo = historico.Cuerpo,
                                             _Carril = historico.Carril,
                                             _Fecha = historico.Fecha,
                                             //_Clase = historico.Clase,
                                             _SaldoAntes = historico.SaldoAnterior,
                                             _Saldo = historico.Saldo,
                                             _SaldoNuevo = historico.SaldoActualizado,
                                             //_Operadora = historico.Operador,
                                             _SaldoActual = tags.SaldoTag

                                         }).ToList();


                    var ListaNegra = (from historico in db.Historicos
                                      join listaNegra in db.ListaNegras on historico.Tag equals listaNegra.Numero
                                      join cuentas in db.CuentasTelepeajes on listaNegra.NumCuenta equals cuentas.NumCuenta
                                      join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                      where (historico.Fecha >= Fecha_Inicio && historico.Fecha <= Fecha_Fin)
                                      where (listaNegra.Numero == TagCuenta)
                                      orderby (historico.Fecha) descending
                                      select new
                                      {
                                          _Tag = historico.Tag,
                                          _ClienteID = cliente.NumCliente,
                                          _Nombre = cliente.Nombre + cliente.Apellidos,
                                          _TypeCuenta = cuentas.TypeCuenta,
                                         // _Plaza = historico.Delegacion,
                                          //_Cuerpo = historico.Cuerpo,
                                          //_Carril = historico.Carril,
                                          _Fecha = historico.Fecha,
                                          //_Clase = historico.Clase,
                                          _SaldoAntes = historico.SaldoAnterior,
                                          _Saldo = historico.Saldo,
                                          _SaldoNuevo = historico.SaldoActualizado,
                                          //_Operadora = historico.Operador,

                                      }).ToList();

                    //var total = ListaCompleta.Sum(x => x._Saldo);
                    double total = 0;

                    foreach (var ite in ListaCompleta)
                    {
                       // total = total + ite._Saldo;
                    }

                    List<Cruces> List = new List<Cruces>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Cruces
                        {
                            Id = id,
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            Tag = item._Tag,
                            //Plaza = item._Plaza,
                            Fecha = Convert.ToString(item._Fecha),
                            //Cuerpo = item._Cuerpo,
                            Carril = item._Carril,
                            //Clase = item._Clase,
                            //SaldoAntes = Convercion(item._SaldoAntes),
                            Saldo = Convercion(item._Saldo.ToString().Replace(".", ",")),
                            //SaldoDespues = Convercion(item._SaldoNuevo),
                            //SaldoAntes = Convert.ToDouble(item._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //Saldo = Convert.ToDouble(Convert.ToString(item._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //SaldoDespues = Convert.ToDouble(item._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            SaldoActual = (Convert.ToDouble(item._SaldoActual) / 100).ToString("C2", culture).Replace("$", "Q"),
                            //Operador = item._Operadora,
                            TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")


                        });
                        id++;
                    }
                    foreach (var item2 in ListaNegra)
                    {
                        List.Add(new Cruces
                        {
                            Id = id,
                            NomCliente = item2._Nombre,
                            TypeCuenta = item2._TypeCuenta,
                            Tag = item2._Tag,
                            //Plaza = item2._Plaza,
                            Fecha = Convert.ToString(item2._Fecha),
                            //Cuerpo = item2._Cuerpo,
                            //Carril = item2._Carril,
                            //Clase = item2._Clase,
                            //SaldoAntes = Convercion(item2._SaldoAntes),
                            //Saldo = Convercion(item2._Saldo.ToString().Replace(".", ",")),
                            //SaldoDespues = Convercion(item2._SaldoNuevo),
                            //SaldoAntes = Convert.ToDouble(item._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //Saldo = Convert.ToDouble(Convert.ToString(item._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //SaldoDespues = Convert.ToDouble(item._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            SaldoActual = "Tag en Lista Negra",
                            //Operador = item2._Operadora,
                            TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")
                        });
                    }

                    return List;


                }
                else
                {
                    var ListaCompleta = (from historico in db.Historicos
                                         join tags in db.Tags on historico.Tag equals tags.NumTag
                                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (historico.Fecha >= Fecha_Inicio && historico.Fecha < Fecha_Fin)
                                         where (historico.Tag == TagCuenta)
                                         orderby (historico.Fecha) descending
                                         select new
                                         {
                                             _CuentaID = tags.CuentaId,
                                             _ClienteID = cliente.NumCliente,
                                             _Nombre = cliente.Nombre + cliente.Apellidos,
                                             _TypeCuenta = cuentas.TypeCuenta,
                                             _Tag = tags.NumTag,
                                            // _Plaza = historico.Delegacion,
                                            // _Cuerpo = historico.Cuerpo,
                                             _Carril = historico.Carril,
                                             _Fecha = historico.Fecha,
                                            // _Clase = historico.Clase,
                                             _SaldoAntes = historico.SaldoAnterior,
                                             _Saldo = historico.Saldo,
                                             _SaldoNuevo = historico.SaldoActualizado,
                                            // _Operadora = historico.Operador,
                                             _SaldoActual = tags.SaldoTag
                                         }).ToList();

                    //var total = ListaCompleta.Sum(x => x._Saldo);
                    double total = 0;
                    foreach (var ite in ListaCompleta)
                    {
                       // total = total + ite._Saldo;
                    }

                    var ListaNegra = (from historico in db.Historicos
                                      join listaNegra in db.ListaNegras on historico.Tag equals listaNegra.Numero
                                      join cuentas in db.CuentasTelepeajes on listaNegra.NumCuenta equals cuentas.NumCuenta
                                      join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                      where (historico.Fecha >= Fecha_Inicio && historico.Fecha < Fecha_Fin)
                                      where (listaNegra.Numero == TagCuenta)
                                      orderby (historico.Fecha) descending
                                      select new
                                      {
                                          _Tag = historico.Tag,
                                          _ClienteID = cliente.NumCliente,
                                          _Nombre = cliente.Nombre + cliente.Apellidos,
                                          _TypeCuenta = cuentas.TypeCuenta,
                                         // _Plaza = historico.Delegacion,
                                         // _Cuerpo = historico.Cuerpo,
                                         // _Carril = historico.Carril,
                                          _Fecha = historico.Fecha,
                                        // _Clase = historico.Clase,
                                          _SaldoAntes = historico.SaldoAnterior,
                                         // _Saldo = historico.Saldo,
                                          _SaldoNuevo = historico.SaldoActualizado,
                                          //_Operadora = historico.Operador,

                                      }).ToList();

                    List<Cruces> List = new List<Cruces>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Cruces
                        {
                            Id = id,
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            Tag = item._Tag,
                          //  Plaza = item._Plaza,
                            Fecha = Convert.ToString(item._Fecha),
                          //  Cuerpo = item._Cuerpo,
                            Carril = item._Carril,
                           // Clase = item._Clase,
                           // SaldoAntes = Convercion(item._SaldoAntes),
                          //  Saldo = Convercion(item._Saldo.ToString().Replace(".", ",")),
                           // SaldoDespues = Convercion(item._SaldoNuevo),
                            //SaldoAntes = Convert.ToDouble(item._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //Saldo = Convert.ToDouble(Convert.ToString(item._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //SaldoDespues = Convert.ToDouble(item._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            SaldoActual = (Convert.ToDouble(item._SaldoActual) / 100).ToString("C2", culture).Replace("$", "Q"),
                          //  Operador = item._Operadora,
                            TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")

                        });
                        id++;
                    }

                    foreach (var item2 in ListaNegra)
                    {
                        List.Add(new Cruces
                        {
                            Id = id,
                            NomCliente = item2._Nombre,
                            TypeCuenta = item2._TypeCuenta,
                            Tag = item2._Tag,
                           // Plaza = item2._Plaza,
                            Fecha = Convert.ToString(item2._Fecha),
                            //Cuerpo = item2._Cuerpo,
                            //Carril = item2._Carril,
                            //Clase = item2._Clase,
                            //SaldoAntes = Convercion(item2._SaldoAntes),
                            //Saldo = Convercion(item2._Saldo.ToString().Replace(".", ",")),
                            //SaldoDespues = Convercion(item2._SaldoNuevo),
                            //SaldoAntes = Convert.ToDouble(item._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //Saldo = Convert.ToDouble(Convert.ToString(item._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //SaldoDespues = Convert.ToDouble(item._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            SaldoActual = "Tag en Lista Negra",
                            //Operador = item2._Operadora,
                            TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")
                        });
                    }

                    return List;
                }
            }
            //Cuenta
            else if (Tipo == (int)DecisionesMetodos.Cuenta)
            {


                if (Rango)
                {
                    List<Cruces> List = new List<Cruces>();

                    var QueTipo = (from cuentas in db.CuentasTelepeajes
                                   join tag in db.Tags on cuentas.Id equals tag.CuentaId
                                   where (cuentas.NumCuenta == TagCuenta)
                                   select new
                                   {
                                       _Tag = tag.NumTag
                                   }).ToList();

                    var ListaNegra = (from historico in db.Historicos
                                      join listaNegra in db.ListaNegras on historico.Tag equals listaNegra.Numero
                                      join cuentas in db.CuentasTelepeajes on listaNegra.NumCuenta equals cuentas.NumCuenta
                                      join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                      where (historico.Fecha >= Fecha_Inicio && historico.Fecha <= Fecha_Fin)
                                      where (listaNegra.NumCuenta == TagCuenta)
                                      orderby (historico.Fecha) descending
                                      select new
                                      {
                                          _Tag = historico.Tag,
                                          _ClienteID = cliente.NumCliente,
                                          _Nombre = cliente.Nombre + cliente.Apellidos,
                                          _TypeCuenta = cuentas.TypeCuenta,
                                          //_Plaza = historico.Delegacion,
                                          //_Cuerpo = historico.Cuerpo,
                                          _Carril = historico.Carril,
                                          _Fecha = historico.Fecha,
                                          //_Clase = historico.Clase,
                                          _SaldoAntes = historico.SaldoAnterior,
                                          _Saldo = historico.Saldo,
                                          _SaldoNuevo = historico.SaldoActualizado,
                                          //_Operadora = historico.Operador,

                                      }).ToList();


                    foreach (var item2 in ListaNegra)
                    {
                        List.Add(new Cruces
                        {
                            Id = 1,
                            NomCliente = item2._Nombre,
                            TypeCuenta = item2._TypeCuenta,
                            Tag = item2._Tag,
                            //Plaza = item2._Plaza,
                            Fecha = Convert.ToString(item2._Fecha),
                            //Cuerpo = item2._Cuerpo,
                            Carril = item2._Carril,
                            //Clase = item2._Clase,
                            //SaldoAntes = Convercion(item2._SaldoAntes),
                            //Saldo = Convercion(item2._Saldo.ToString().Replace(".", ",")),
                            //SaldoDespues = Convercion(item2._SaldoNuevo),
                            //SaldoAntes = Convert.ToDouble(item._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //Saldo = Convert.ToDouble(Convert.ToString(item._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //SaldoDespues = Convert.ToDouble(item._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            SaldoActual = "Tag en Lista Negra",
                            //Operador = item2._Operadora,
                            TotalMonetarioCruces = "Tag en Lista Negra"
                        });
                    }


                    int id = 1;

                    foreach (var item in QueTipo)
                    {

                        var ListaCompleta = (from historico in db.Historicos
                                             join tags in db.Tags on historico.Tag equals tags.NumTag
                                             join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                             join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                             where (historico.Fecha >= Fecha_Inicio && historico.Fecha <= Fecha_Fin)
                                             where (historico.Tag == item._Tag)
                                             orderby (historico.Fecha) descending
                                             select new
                                             {

                                                 _CuentaID = tags.CuentaId,
                                                 _ClienteID = cliente.NumCliente,
                                                 _Nombre = cliente.Nombre + cliente.Apellidos,
                                                 _TypeCuenta = cuentas.TypeCuenta,
                                                 //_Plaza = historico.Delegacion,
                                                 //_Cuerpo = historico.Cuerpo,
                                                 _Carril = historico.Carril,
                                                 _Fecha = historico.Fecha,
                                                 //_Clase = historico.Clase,
                                                 _SaldoAntes = historico.SaldoAnterior,
                                                 _Saldo = historico.Saldo,
                                                 _SaldoNuevo = historico.SaldoActualizado,
                                                 //_Operadora = historico.Operador,
                                                 _SaldoActual = cuentas.SaldoCuenta

                                             }).ToList();



                        //var total = ListaCompleta.Sum(x => x._Saldo);

                        foreach (var item2 in ListaCompleta)
                        {
                            List.Add(new Cruces
                            {
                                Id = id,
                                NomCliente = item2._Nombre,
                                TypeCuenta = item2._TypeCuenta,
                                Tag = item._Tag,
                                //Plaza = item2._Plaza,
                                Fecha = Convert.ToString(item2._Fecha),
                                //Cuerpo = item2._Cuerpo,
                                //Carril = item2._Carril,
                                //Clase = item2._Clase,
                                //SaldoAntes = Convercion(item2._SaldoAntes),
                                Saldo = Convercion(item2._Saldo.ToString().Replace(".", ",")),
                                //SaldoDespues = Convercion(item2._SaldoNuevo),
                                //SaldoAntes = Convert.ToDouble(item2._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                                //Saldo = Convert.ToDouble(Convert.ToString(item2._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                                //SaldoDespues = Convert.ToDouble(item2._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                                SaldoActual = (Convert.ToDouble(item2._SaldoActual) / 100).ToString("C2", culture).Replace("$", "Q"),
                                //Operador = item2._Operadora,
                                //TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")

                            });
                            id++;
                        }




                    }


                    return List;
                }
                else
                {
                    List<Cruces> List = new List<Cruces>();

                    var QueTipo = (from cuentas in db.CuentasTelepeajes
                                   join tag in db.Tags on cuentas.Id equals tag.CuentaId
                                   where (cuentas.NumCuenta == TagCuenta)
                                   select new
                                   {
                                       _Tag = tag.NumTag
                                   }).ToList();

                    var ListaNegra = (from historico in db.Historicos
                                      join listaNegra in db.ListaNegras on historico.Tag equals listaNegra.Numero
                                      join cuentas in db.CuentasTelepeajes on listaNegra.NumCuenta equals cuentas.NumCuenta
                                      join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                      where (historico.Fecha >= Fecha_Inicio && historico.Fecha < Fecha_Fin)
                                      where (listaNegra.NumCuenta == TagCuenta)
                                      orderby (historico.Fecha) descending
                                      select new
                                      {
                                          _Tag = historico.Tag,
                                          _ClienteID = cliente.NumCliente,
                                          _Nombre = cliente.Nombre + cliente.Apellidos,
                                          _TypeCuenta = cuentas.TypeCuenta,
                                          //_Plaza = historico.Delegacion,
                                          //_Cuerpo = historico.Cuerpo,
                                          _Carril = historico.Carril,
                                          _Fecha = historico.Fecha,
                                          //_Clase = historico.Clase,
                                          _SaldoAntes = historico.SaldoAnterior,
                                          _Saldo = historico.Saldo,
                                          _SaldoNuevo = historico.SaldoActualizado,
                                          //_Operadora = historico.Operador,

                                      }).ToList();


                    foreach (var item2 in ListaNegra)
                    {
                        List.Add(new Cruces
                        {
                            Id = 1,
                            NomCliente = item2._Nombre,
                            TypeCuenta = item2._TypeCuenta,
                            Tag = item2._Tag,
                            //Plaza = item2._Plaza,
                            Fecha = Convert.ToString(item2._Fecha),
                            //Cuerpo = item2._Cuerpo,
                            Carril = item2._Carril,
                            //Clase = item2._Clase,
                            //SaldoAntes = Convercion(item2._SaldoAntes),
                            Saldo = Convercion(item2._Saldo.ToString().Replace(".", ",")),
                            //SaldoDespues = Convercion(item2._SaldoNuevo),
                            //SaldoAntes = Convert.ToDouble(item._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //Saldo = Convert.ToDouble(Convert.ToString(item._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            //SaldoDespues = Convert.ToDouble(item._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                            SaldoActual = "Tag en Lista Negra",
                            //Operador = item2._Operadora,
                            TotalMonetarioCruces = "Tag en Lista Negra"
                        });
                    }

                    int id = 1;

                    foreach (var item in QueTipo)
                    {

                        var ListaCompleta = (from historico in db.Historicos
                                             join tags in db.Tags on historico.Tag equals tags.NumTag
                                             join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                             join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                             where (historico.Fecha >= Fecha_Inicio && historico.Fecha < Fecha_Fin)
                                             where (historico.Tag == item._Tag)
                                             orderby (historico.Fecha) descending
                                             select new
                                             {
                                                 _Tag = historico.Tag,
                                                 _CuentaID = tags.CuentaId,
                                                 _ClienteID = cliente.NumCliente,
                                                 _Nombre = cliente.Nombre + cliente.Apellidos,
                                                 _TypeCuenta = cuentas.TypeCuenta,
                                                 //_Plaza = historico.Delegacion,
                                                 //_Cuerpo = historico.Cuerpo,
                                                 _Carril = historico.Carril,
                                                 _Fecha = historico.Fecha,
                                                 //_Clase = historico.Clase,
                                                 _SaldoAntes = historico.SaldoAnterior,
                                                 _Saldo = historico.Saldo,
                                                 _SaldoNuevo = historico.SaldoActualizado,
                                                 //_Operadora = historico.Operador,
                                                 _SaldoActual = cuentas.SaldoCuenta


                                             }).ToList();

                        //var total = ListaCompleta.Sum(x => x._Saldo);

                        foreach (var item2 in ListaCompleta)
                        {
                            List.Add(new Cruces
                            {
                                Id = id,
                                NomCliente = item2._Nombre,
                                TypeCuenta = item2._TypeCuenta,
                                Tag = item._Tag,
                                //Plaza = item2._Plaza,
                                Fecha = Convert.ToString(item2._Fecha),
                                //Cuerpo = item2._Cuerpo,
                                Carril = item2._Carril,
                                //Clase = item2._Clase,
                                //SaldoAntes = Convercion(item2._SaldoAntes),
                                Saldo = Convercion(item2._Saldo.ToString().Replace(".", ",")),
                                //SaldoDespues = Convercion(item2._SaldoNuevo),
                                //SaldoAntes = Convert.ToDouble(item2._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                                //Saldo = Convert.ToDouble(Convert.ToString(item2._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                                //SaldoDespues = Convert.ToDouble(item2._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                                SaldoActual = (Convert.ToDouble(item2._SaldoActual) / 100).ToString("C2", culture).Replace("$", "Q"),
                                //Operador = item2._Operadora,
                                //TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")

                            });
                            id++;
                        }
                    }

                    return List;
                }
            }
            //RangoFecha
            else if (Tipo == (int)DecisionesMetodos.RangoFecha)
            {

                var ListaCompleta = (from historico in db.Historicos
                                     where (historico.Fecha >= Fecha_Inicio && historico.Fecha < Fecha_Fin)
                                     orderby (historico.Fecha) descending
                                     select new
                                     {
                                         _Tag = historico.Tag,
                                         //_Plaza = historico.Delegacion,
                                         //_Cuerpo = historico.Cuerpo,
                                         _Carril = historico.Carril,
                                         _Fecha = historico.Fecha,
                                         //_Clase = historico.Clase,
                                         _SaldoAntes = historico.SaldoAnterior,
                                         _Saldo = historico.Saldo,
                                         _SaldoNuevo = historico.SaldoActualizado,
                                         //_Operadora = historico.Operador,


                                     }).ToList();



                //var total = ListaCompleta.Sum(x => x._Saldo);
                double total = 0;

                foreach (var ite in ListaCompleta)
                {
                    //total = total + ite._Saldo;
                }


                List<Cruces> List = new List<Cruces>();

                int id = 1;

                foreach (var item in ListaCompleta)
                {
                    List.Add(new Cruces
                    {
                        Id = id,
                        Tag = item._Tag,
                        NumCliente = "",
                        NomCliente = "",
                        TypeCuenta = "",
                        //Plaza = item._Plaza,
                        Fecha = Convert.ToString(item._Fecha),
                        //Cuerpo = item._Cuerpo,
                        Carril = item._Carril,
                        //Clase = item._Clase,
                        //SaldoAntes = Convercion(item._SaldoAntes),
                        Saldo = Convercion(item._Saldo.ToString().Replace(".", ",")),
                        //SaldoDespues = Convercion(item._SaldoNuevo),
                        SaldoActual = "",
                        //Operador = item._Operadora,
                        TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")

                    });
                    id++;
                }

                return List;
                //if (Rango)
                //{
                //    var ListaCompleta = (from historico in db.Historicos
                //                         join tags in db.Tags on historico.Tag equals tags.NumTag
                //                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                //                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                //                         where (historico.Fecha >= Fecha_Inicio && historico.Fecha <= Fecha_Fin)
                //                         orderby (historico.Fecha) descending
                //                         select new
                //                         {
                //                             _Tag = tags.NumTag,
                //                             _CuentaID = tags.CuentaId,
                //                             _ClienteID = cliente.NumCliente,
                //                             _Nombre = cliente.Nombre + cliente.Apellidos,
                //                             _TypeCuenta = cuentas.TypeCuenta,
                //                             _Plaza = historico.Delegacion,
                //                             _Cuerpo = historico.Cuerpo,
                //                             _Carril = historico.Carril,
                //                             _Fecha = historico.Fecha,
                //                             _Clase = historico.Clase,
                //                             _SaldoAntes = historico.SaldoAnterior,
                //                             _Saldo = historico.Saldo,
                //                             _SaldoNuevo = historico.SaldoActualizado,
                //                             _Operadora = historico.Operador,
                //                             _SaldoActual = cuentas.SaldoCuenta

                //                         }).ToList();



                //    var ListaNegra = (from historico in db.Historicos
                //                      join listaNegra in db.ListaNegras on historico.Tag equals listaNegra.Numero
                //                      join cuentas in db.CuentasTelepeajes on listaNegra.NumCuenta equals cuentas.NumCuenta
                //                      join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                //                      where (historico.Fecha >= Fecha_Inicio && historico.Fecha <= Fecha_Fin)
                //                      orderby (historico.Fecha) descending
                //                      select new
                //                      {
                //                          _Tag = historico.Tag,
                //                          _ClienteID = cliente.NumCliente,
                //                          _Nombre = cliente.Nombre + cliente.Apellidos,
                //                          _TypeCuenta = cuentas.TypeCuenta,
                //                          _Plaza = historico.Delegacion,
                //                          _Cuerpo = historico.Cuerpo,
                //                          _Carril = historico.Carril,
                //                          _Fecha = historico.Fecha,
                //                          _Clase = historico.Clase,
                //                          _SaldoAntes = historico.SaldoAnterior,
                //                          _Saldo = historico.Saldo,
                //                          _SaldoNuevo = historico.SaldoActualizado,
                //                          _Operadora = historico.Operador,

                //                      }).ToList();



                //    //var total = ListaCompleta.Sum(x => x._Saldo);
                //    double total = 0;

                //    foreach (var ite in ListaCompleta)
                //    {
                //        total = total + ite._Saldo;
                //    }

                //    List<Cruces> List = new List<Cruces>();

                //    int id = 1;

                //    foreach (var item in ListaCompleta)
                //    {

                //        List.Add(new Cruces
                //        {
                //            Id = id,
                //            Tag = item._Tag,
                //            NumCliente = item._ClienteID,
                //            NomCliente = item._Nombre,
                //            TypeCuenta = item._TypeCuenta,
                //            Plaza = item._Plaza,
                //            Fecha = Convert.ToString(item._Fecha),
                //            Cuerpo = item._Cuerpo,
                //            Carril = item._Carril,
                //            Clase = item._Clase,
                //            SaldoAntes = Convercion(item._SaldoAntes),
                //            Saldo = Convercion(item._Saldo.ToString().Replace(".", ",")),
                //            SaldoDespues = Convercion(item._SaldoNuevo),
                //            SaldoActual = (Convert.ToDouble(item._SaldoActual) / 100).ToString("C", culture).Replace("$", "Q"),
                //            Operador = item._Operadora,
                //            TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")

                //        });
                //        id++;
                //    }

                //    foreach (var item2 in ListaNegra)
                //    {
                //        List.Add(new Cruces
                //        {
                //            Id = id,
                //            NomCliente = item2._Nombre,
                //            TypeCuenta = item2._TypeCuenta,
                //            Tag = item2._Tag,
                //            Plaza = item2._Plaza,
                //            Fecha = Convert.ToString(item2._Fecha),
                //            Cuerpo = item2._Cuerpo,
                //            Carril = item2._Carril,
                //            Clase = item2._Clase,
                //            SaldoAntes = Convercion(item2._SaldoAntes),
                //            Saldo = Convercion(item2._Saldo.ToString().Replace(".", ",")),
                //            SaldoDespues = Convercion(item2._SaldoNuevo),
                //            //SaldoAntes = Convert.ToDouble(item._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                //            //Saldo = Convert.ToDouble(Convert.ToString(item._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                //            //SaldoDespues = Convert.ToDouble(item._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                //            SaldoActual = "Tag en Lista Negra",
                //            Operador = item2._Operadora,
                //            TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")
                //        });
                //    }

                //    return List;
                //}
                //else
                //{
                //    var ListaCompleta = (from historico in db.Historicos
                //                         join tags in db.Tags on historico.Tag equals tags.NumTag
                //                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                //                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                //                         where (historico.Fecha >= Fecha_Inicio && historico.Fecha < Fecha_Fin)
                //                         orderby (historico.Fecha) descending
                //                         select new
                //                         {
                //                             _Tag = historico.Tag,
                //                             _CuentaID = tags.CuentaId,
                //                             _ClienteID = cliente.NumCliente,
                //                             _Nombre = cliente.Nombre + cliente.Apellidos,
                //                             _TypeCuenta = cuentas.TypeCuenta,
                //                             _Plaza = historico.Delegacion,
                //                             _Cuerpo = historico.Cuerpo,
                //                             _Carril = historico.Carril,
                //                             _Fecha = historico.Fecha,
                //                             _Clase = historico.Clase,
                //                             _SaldoAntes = historico.SaldoAnterior,
                //                             _Saldo = historico.Saldo,
                //                             _SaldoNuevo = historico.SaldoActualizado,
                //                             _Operadora = historico.Operador,
                //                             _SaldoActual = cuentas.SaldoCuenta


                //                         }).ToList();
                //    var ListaNegra = (from historico in db.Historicos
                //                      join listaNegra in db.ListaNegras on historico.Tag equals listaNegra.Numero
                //                      join cuentas in db.CuentasTelepeajes on listaNegra.NumCuenta equals cuentas.NumCuenta
                //                      join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                //                      where (historico.Fecha >= Fecha_Inicio && historico.Fecha < Fecha_Fin)
                //                      orderby (historico.Fecha) descending
                //                      select new
                //                      {
                //                          _Tag = historico.Tag,
                //                          _ClienteID = cliente.NumCliente,
                //                          _Nombre = cliente.Nombre + cliente.Apellidos,
                //                          _TypeCuenta = cuentas.TypeCuenta,
                //                          _Plaza = historico.Delegacion,
                //                          _Cuerpo = historico.Cuerpo,
                //                          _Carril = historico.Carril,
                //                          _Fecha = historico.Fecha,
                //                          _Clase = historico.Clase,
                //                          _SaldoAntes = historico.SaldoAnterior,
                //                          _Saldo = historico.Saldo,
                //                          _SaldoNuevo = historico.SaldoActualizado,
                //                          _Operadora = historico.Operador,

                //                      }).ToList();


                //    //var total = ListaCompleta.Sum(x => x._Saldo);
                //    double total = 0;

                //    foreach (var ite in ListaCompleta)
                //    {
                //        total = total + ite._Saldo;
                //    }
                //    foreach(var it in ListaNegra)
                //    {
                //        total += it._Saldo;
                //    }

                //    List<Cruces> List = new List<Cruces>();

                //    int id = 1;

                //    foreach (var item in ListaCompleta)
                //    {
                //        List.Add(new Cruces
                //        {
                //            Id = id,
                //            Tag = item._Tag,
                //            NumCliente = item._ClienteID,
                //            NomCliente = item._Nombre,
                //            TypeCuenta = item._TypeCuenta,
                //            Plaza = item._Plaza,
                //            Fecha = Convert.ToString(item._Fecha),
                //            Cuerpo = item._Cuerpo,
                //            Carril = item._Carril,
                //            Clase = item._Clase,
                //            SaldoAntes = Convercion(item._SaldoAntes),
                //            Saldo = Convercion(item._Saldo.ToString().Replace(".",",")),
                //            SaldoDespues = Convercion(item._SaldoNuevo),
                //            SaldoActual = (Convert.ToDouble(item._SaldoActual) / 100).ToString("C", culture).Replace("$", "Q"),
                //            Operador = item._Operadora,
                //            TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")

                //        });
                //        id++;
                //    }

                //    foreach (var item2 in ListaNegra)
                //    {
                //        List.Add(new Cruces
                //        {
                //            Id = id,
                //            NomCliente = item2._Nombre,
                //            TypeCuenta = item2._TypeCuenta,
                //            Tag = item2._Tag,
                //            Plaza = item2._Plaza,
                //            Fecha = Convert.ToString(item2._Fecha),
                //            Cuerpo = item2._Cuerpo,
                //            Carril = item2._Carril,
                //            Clase = item2._Clase,
                //            SaldoAntes = Convercion(item2._SaldoAntes),
                //            Saldo = Convercion(item2._Saldo.ToString().Replace(".", ",")),
                //            SaldoDespues = Convercion(item2._SaldoNuevo),
                //            //SaldoAntes = Convert.ToDouble(item._SaldoAntes.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                //            //Saldo = Convert.ToDouble(Convert.ToString(item._Saldo).Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                //            //SaldoDespues = Convert.ToDouble(item._SaldoNuevo.Replace(",", ",")).ToString("C2", culture).Replace("$", "Q"),
                //            SaldoActual = "Tag en Lista Negra",
                //            Operador = item2._Operadora,
                //            TotalMonetarioCruces = total.ToString("C2", culture).Replace("$", "Q")
                //        });
                //    }

                //    return List;
                //}

            }
            return null;
        }

        public List<Movimientos> Movimientos(string TagCuenta, DateTime Fecha_Inicio, DateTime Fecha_Fin, int Tipo, bool Rango)
        {
            AppDbContext db = new AppDbContext();
            CultureInfo culture = new CultureInfo("es-MX", false);
            if (Tipo == (int)DecisionesMetodos.Tag)
            {
                if (Rango)
                {
                    //var ListaCompleta = db.OperacionesCajeros.Where(x => x.Numero == TagCuenta).Where(x => x.DateTOperacion >= Fecha_Inicio && x.DateTOperacion <= Fecha_Fin).OrderByDescending(x => x.DateTOperacion).ToList();
                    var ListaCompleta = (from operaciones in db.OperacionesCajeros
                                         join tags in db.Tags on operaciones.Numero equals tags.NumTag
                                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion <= Fecha_Fin)
                                         where (operaciones.Numero == TagCuenta)
                                         where (operaciones.Concepto == "TAG RECARGA" || operaciones.Concepto == "TAG ACTIVADO")
                                         where (operaciones.StatusCancelacion == false)
                                         orderby (operaciones.DateTOperacion) descending
                                         select new
                                         {
                                             _Tag = tags.NumTag,
                                             _CuentaID = tags.CuentaId,
                                             _ClienteID = cliente.NumCliente,
                                             _Nombre = cliente.Nombre + cliente.Apellidos,
                                             _TypeCuenta = cuentas.TypeCuenta,
                                             _Fecha = operaciones.DateTOperacion,
                                             _Concepto = operaciones.Concepto,
                                             _TipoPago = operaciones.TipoPago,
                                             _Monto = operaciones.Monto,
                                             _SaldoActual = tags.SaldoTag,
                                             _TipoCuenta = operaciones.Tipo,
                                             _CobroTag = operaciones.CobroTag,
                                             _Referencia = operaciones.NoReferencia,
                                             _TagCuenta = operaciones.Numero

                                         }).ToList();

                    var total = ListaCompleta.Sum(x => x._Monto);

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = Convert.ToDouble(item._Monto).ToString("C2", culture).Replace("$", "Q"),
                            Fecha = Convert.ToString(item._Fecha),
                            Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = Convert.ToDouble(item._CobroTag).ToString("C2", culture).Replace("$", "Q"),
                            Referencia = item._Referencia,
                            SaldoActual = (Convert.ToDouble(item._SaldoActual) / 100).ToString("C2", culture).Replace("$", "Q"),
                            TotalMonetarioMovimientos = Convert.ToDouble(total).ToString("C2", culture).Replace("$", "Q")


                        });
                        id++;
                    }

                    return List;


                }
                else
                {
                    //var ListaCompleta = db.OperacionesCajeros.Where(x => x.Numero == TagCuenta).Where(x => x.DateTOperacion >= Fecha_Inicio && x.DateTOperacion < Fecha_Fin).OrderByDescending(x => x.DateTOperacion).ToList();
                    var ListaCompleta = (from operaciones in db.OperacionesCajeros
                                         join tags in db.Tags on operaciones.Numero equals tags.NumTag
                                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion < Fecha_Fin)
                                         where (operaciones.Numero == TagCuenta)
                                         where (operaciones.Concepto == "TAG RECARGA" || operaciones.Concepto == "TAG ACTIVADO")
                                         where (operaciones.StatusCancelacion == false)
                                         orderby (operaciones.DateTOperacion) descending
                                         select new
                                         {
                                             _Tag = tags.NumTag,
                                             _CuentaID = tags.CuentaId,
                                             _ClienteID = cliente.NumCliente,
                                             _Nombre = cliente.Nombre + cliente.Apellidos,
                                             _TypeCuenta = cuentas.TypeCuenta,
                                             _Fecha = operaciones.DateTOperacion,
                                             _Concepto = operaciones.Concepto,
                                             _TipoPago = operaciones.TipoPago,
                                             _Monto = operaciones.Monto,
                                             _SaldoActual = tags.SaldoTag,
                                             _TipoCuenta = operaciones.Tipo,
                                             _CobroTag = operaciones.CobroTag,
                                             _Referencia = operaciones.NoReferencia,
                                             _TagCuenta = operaciones.Numero

                                         }).ToList();

                    var total = ListaCompleta.Sum(x => x._Monto);

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = Convert.ToDouble(item._Monto).ToString("C2", culture).Replace("$", "Q"),
                            Fecha = Convert.ToString(item._Fecha),
                            Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = Convert.ToDouble(item._CobroTag).ToString("C2", culture).Replace("$", "Q"),
                            Referencia = item._Referencia,
                            SaldoActual = (Convert.ToDouble(item._SaldoActual) / 100).ToString("C2", culture).Replace("$", "Q"),
                            TotalMonetarioMovimientos = Convert.ToDouble(total).ToString("C2", culture).Replace("$", "Q")


                        });
                        id++;
                    }

                    return List;
                }
            }
            else if (Tipo == (int)DecisionesMetodos.Cuenta)
            {
                if (Rango)
                {
                    //var ListaCompleta = db.OperacionesCajeros.Where(x => x.Numero == TagCuenta).Where(x => x.DateTOperacion >= Fecha_Inicio && x.DateTOperacion <= Fecha_Fin).OrderByDescending(x => x.DateTOperacion).ToList();
                    var ListaCompleta = (from operaciones in db.OperacionesCajeros
                                             //join tags in db.Tags on operaciones.Numero equals tags.NumTag
                                             //join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cuentas in db.CuentasTelepeajes on operaciones.Numero equals cuentas.NumCuenta
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion <= Fecha_Fin)
                                         where (operaciones.Numero == TagCuenta)
                                         where (operaciones.Concepto == "CUENTA RECARGA" || operaciones.Concepto == "CUENTA ACTIVADA")
                                         where (operaciones.StatusCancelacion == false)
                                         orderby (operaciones.DateTOperacion) descending
                                         select new
                                         {
                                             //_Tag = tags.NumTag,
                                             //                                             _CuentaID = tags.CuentaId,
                                             _ClienteID = cliente.NumCliente,
                                             _Nombre = cliente.Nombre + cliente.Apellidos,
                                             _TypeCuenta = cuentas.TypeCuenta,
                                             _Fecha = operaciones.DateTOperacion,
                                             _Concepto = operaciones.Concepto,
                                             _TipoPago = operaciones.TipoPago,
                                             _Monto = operaciones.Monto,
                                             _SaldoActual = cuentas.SaldoCuenta,
                                             _TipoCuenta = operaciones.Tipo,
                                             _CobroTag = operaciones.CobroTag,
                                             _Referencia = operaciones.NoReferencia,
                                             _TagCuenta = operaciones.Numero

                                         }).ToList();

                    var total = ListaCompleta.Sum(x => x._Monto);

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = Convert.ToDouble(item._Monto).ToString("C2", culture).Replace("$", "Q"),
                            Fecha = Convert.ToString(item._Fecha),
                            //Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            //Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = Convert.ToDouble(item._CobroTag).ToString("C2", culture).Replace("$", "Q"),
                            Referencia = item._Referencia,
                            SaldoActual = (Convert.ToDouble(item._SaldoActual) / 100).ToString("C2", culture).Replace("$", "Q"),
                            TotalMonetarioMovimientos = Convert.ToDouble(total).ToString("C2", culture).Replace("$", "Q")
                            //SaldoActual = "Q" + item._SaldoActual

                        });
                        id++;
                    }

                    return List;


                }
                else
                {
                    //var ListaCompleta = db.OperacionesCajeros.Where(x => x.Numero == TagCuenta).Where(x => x.DateTOperacion >= Fecha_Inicio && x.DateTOperacion < Fecha_Fin).OrderByDescending(x => x.DateTOperacion).ToList();
                    var ListaCompleta = (from operaciones in db.OperacionesCajeros
                                             //join tags in db.Tags on operaciones.Numero equals tags.NumTag
                                             //join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cuentas in db.CuentasTelepeajes on operaciones.Numero equals cuentas.NumCuenta
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion < Fecha_Fin)
                                         where (operaciones.Numero == TagCuenta)
                                         where (operaciones.Concepto == "CUENTA RECARGA" || operaciones.Concepto == "CUENTA ACTIVADA")
                                         where (operaciones.StatusCancelacion == false)
                                         orderby (operaciones.DateTOperacion) descending
                                         select new
                                         {
                                             //_Tag = tags.NumTag,
                                             //_CuentaID = tags.CuentaId,
                                             _ClienteID = cliente.NumCliente,
                                             _Nombre = cliente.Nombre + cliente.Apellidos,
                                             _TypeCuenta = cuentas.TypeCuenta,
                                             _Fecha = operaciones.DateTOperacion,
                                             _Concepto = operaciones.Concepto,
                                             _TipoPago = operaciones.TipoPago,
                                             _Monto = operaciones.Monto,
                                             _SaldoActual = cuentas.SaldoCuenta,
                                             _TipoCuenta = operaciones.Tipo,
                                             _CobroTag = operaciones.CobroTag,
                                             _Referencia = operaciones.NoReferencia,
                                             _TagCuenta = operaciones.Numero

                                         }).ToList();


                    var total = ListaCompleta.Sum(x => x._Monto);
                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = Convert.ToDouble(item._Monto).ToString("C2", culture).Replace("$", "Q"),
                            Fecha = Convert.ToString(item._Fecha),
                            //Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            //Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = Convert.ToDouble(item._CobroTag).ToString("C2", culture).Replace("$", "Q"),
                            Referencia = item._Referencia,
                            SaldoActual = (Convert.ToDouble(item._SaldoActual) / 100).ToString("C2", culture).Replace("$", "Q"),
                            TotalMonetarioMovimientos = Convert.ToDouble(total).ToString("C2", culture).Replace("$", "Q")

                        });
                        id++;

                    }

                    return List;
                }
            }
            else if (Tipo == (int)DecisionesMetodos.RangoFecha)
            {
                //if (Rango)
                //{


                //var ListaCompleta = db.OperacionesCajeros.Where(x => x.DateTOperacion >= Fecha_Inicio && x.DateTOperacion <= Fecha_Fin).OrderByDescending(x => x.DateTOperacion).ToList();

                //    var ListaCompleta = (from operaciones in db.OperacionesCajeros
                //                         join tags in db.Tags on operaciones.Numero equals tags.NumTag
                //                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                //                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                //                         where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion <= Fecha_Fin)
                //                         where (operaciones.StatusCancelacion == false)
                //                         orderby (operaciones.DateTOperacion) descending
                //                         select new
                //                         {
                //                             _Tag = tags.NumTag,
                //                             _CuentaID = tags.CuentaId,
                //                             _ClienteID = cliente.NumCliente,
                //                             _Nombre = cliente.Nombre + cliente.Apellidos,
                //                             _TypeCuenta = cuentas.TypeCuenta,
                //                             _Fecha = operaciones.DateTOperacion,
                //                             _Concepto = operaciones.Concepto,
                //                             _TipoPago = operaciones.TipoPago,
                //                             _Monto = operaciones.Monto,
                //                             _SaldoActual = tags.SaldoTag,
                //                             _TipoCuenta = operaciones.Tipo,
                //                             _CobroTag = operaciones.CobroTag,
                //                             _Referencia = operaciones.NoReferencia,
                //                             _TagCuenta = operaciones.Numero

                //                         }).ToList();


                //    var total = ListaCompleta.Sum(x => x._Monto);
                //    List<Movimientos> List = new List<Movimientos>();

                //    int id = 1;

                //    foreach (var item in ListaCompleta)
                //    {
                //        List.Add(new Movimientos
                //        {
                //            Id = id,
                //            Concepto = item._Concepto,
                //            TipoPago = item._TipoPago,
                //            Monto = Convert.ToDouble(item._Monto).ToString("C2", culture).Replace("$", "Q"),
                //            Fecha = Convert.ToString(item._Fecha),
                //            Tag = item._Tag,
                //            TagCuenta = item._TagCuenta,
                //            Cuenta = Convert.ToString(item._CuentaID),
                //            NomCliente = item._Nombre,
                //            TypeCuenta = item._TypeCuenta,
                //            CobroTag = Convert.ToDouble(item._CobroTag).ToString("C2", culture).Replace("$", "Q"),
                //            Referencia = item._Referencia,
                //            TotalMonetarioMovimientos = Convert.ToDouble(total).ToString("C2", culture).Replace("$", "Q")


                //        });
                //        id++;
                //    }

                //    return List;
                //}
                //else
                //{
                //var ListaCompleta = db.OperacionesCajeros.Where(x => x.DateTOperacion >= Fecha_Inicio && x.DateTOperacion < Fecha_Fin).OrderByDescending(x => x.DateTOperacion).ToList();
                //var ListaCompleta = (from operaciones in db.OperacionesCajeros
                //                     join tags in db.Tags on operaciones.Numero equals tags.NumTag
                //                     join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                //                     join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                //                     where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion <= Fecha_Fin)
                //                     where (operaciones.StatusCancelacion == false)
                //                     orderby (operaciones.DateTOperacion) descending
                //                     select new
                //                     {
                //                         _Tag = tags.NumTag,
                //                         _CuentaID = tags.CuentaId,
                //                         _ClienteID = cliente.NumCliente,
                //                         _Nombre = cliente.Nombre + cliente.Apellidos,
                //                         _TypeCuenta = cuentas.TypeCuenta,
                //                         _Fecha = operaciones.DateTOperacion,
                //                         _Concepto = operaciones.Concepto,
                //                         _TipoPago = operaciones.TipoPago,
                //                         _Monto = operaciones.Monto,
                //                         _SaldoActual = tags.SaldoTag,
                //                         _TipoCuenta = operaciones.Tipo,
                //                         _CobroTag = operaciones.CobroTag,
                //                         _Referencia = operaciones.NoReferencia,
                //                         _TagCuenta = operaciones.Numero

                //                     }).ToList();

                var ListaCompleta = (from operaciones in db.OperacionesCajeros
                                     where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion < Fecha_Fin)
                                     orderby (operaciones.DateTOperacion) descending
                                     select new
                                     {
                                         _Tag = operaciones.Numero,
                                         _TypeCuenta = operaciones.Tipo,
                                         _Fecha = operaciones.DateTOperacion,
                                         _Concepto = operaciones.Concepto,
                                         _TipoPago = operaciones.TipoPago,
                                         _Monto = operaciones.Monto,
                                         _TipoCuenta = operaciones.Tipo,
                                         _CobroTag = operaciones.CobroTag,
                                         _Referencia = operaciones.NoReferencia,
                                         _TagCuenta = operaciones.Numero

                                     }).ToList();


                //var total = ListaCompleta.Sum(x => x._Monto);
                double total = 0;

                foreach (var item2 in ListaCompleta)
                {
                    total += Convert.ToDouble(item2._Monto);
                }
                List<Movimientos> List = new List<Movimientos>();

                int id = 1;

                foreach (var item in ListaCompleta)
                {
                    List.Add(new Movimientos
                    {
                        Id = id,
                        Concepto = item._Concepto,
                        TipoPago = item._TipoPago,
                        Monto = Convert.ToDouble(item._Monto).ToString("C2", culture).Replace("$", "Q"),
                        Fecha = Convert.ToString(item._Fecha),
                        Tag = item._Tag,
                        TagCuenta = item._TagCuenta,
                        Cuenta = "",
                        NomCliente = "",
                        TypeCuenta = item._TypeCuenta,
                        CobroTag = Convert.ToDouble(item._CobroTag).ToString("C2", culture).Replace("$", "Q"),
                        Referencia = item._Referencia,
                        TotalMonetarioMovimientos = Convert.ToDouble(total).ToString("C2", culture).Replace("$", "Q")


                    });
                    id++;
                }

                //foreach (var item in ListaCompleta)
                //{
                //    List.Add(new Movimientos
                //    {
                //        Id = id,
                //        Concepto = item._Concepto,
                //        TipoPago = item._TipoPago,
                //        Monto = Convert.ToDouble(item._Monto).ToString("C2", culture).Replace("$", "Q"),
                //        Fecha = Convert.ToString(item._Fecha),
                //        Tag = item._Tag,
                //        TagCuenta = item._TagCuenta,
                //        Cuenta = Convert.ToString(item._CuentaID),
                //        NomCliente = item._Nombre,
                //        TypeCuenta = item._TypeCuenta,
                //        CobroTag = Convert.ToDouble(item._CobroTag).ToString("C2", culture).Replace("$", "Q"),
                //        Referencia = item._Referencia,
                //        TotalMonetarioMovimientos = Convert.ToDouble(total).ToString("C2", culture).Replace("$", "Q")


                //    });
                //    id++;
                //}

                return List;
                //}
            }

            return null;
        }

        public List<CruceMovimiento> FusionarListas(List<Cruces> ListCruces, List<Movimientos> ListMovimientos, bool TagOCuenta)
        {

            List<CruceMovimiento> Lista = new List<CruceMovimiento>();

            if (TagOCuenta)
            {

                foreach (var item in ListCruces)
                {
                    Lista.Add(new CruceMovimiento
                    {


                        Concepto = "CRUCE" + "   " + "#TAG:  " + item.Tag,
                        Fecha = item.Fecha,
                        CobroTag = "-" + item.Saldo,
                        Carril = item.Carril,
                        Referencia = "-----------",


                    });
                }
            }
            else
            {
                foreach (var item in ListCruces)
                {
                    Lista.Add(new CruceMovimiento
                    {


                        Concepto = "CRUCE",
                        Fecha = item.Fecha,
                        CobroTag = "-" + item.Saldo,
                        Carril = item.Carril,
                        Referencia = "-----------",


                    });
                }
            }

            foreach (var item in ListMovimientos)
            {
                Lista.Add(new CruceMovimiento
                {

                    Concepto = item.Concepto,
                    Fecha = item.Fecha,
                    CobroTag = item.Monto,
                    Carril = "-------------",
                    Referencia = item.Referencia

                });
            }

            var otraLista = Lista.OrderBy(x => x.Fecha).ToList();

            List<CruceMovimiento> ListaLista = new List<CruceMovimiento>();

            foreach (var item in otraLista)
            {
                ListaLista.Add(new CruceMovimiento
                {
                    Concepto = item.Concepto,
                    Fecha = item.Fecha,
                    CobroTag = item.CobroTag,
                    Carril = item.Carril,
                    Referencia = item.Referencia
                });
            }

            return ListaLista;
        }

        public List<CruceMovimientoRepocliente> FusionarListasMes(List<Cruces> ListCruces, List<Movimientos> ListMovimientos, bool TagOCuenta)
        {

            List<CruceMovimientoRepocliente> Lista = new List<CruceMovimientoRepocliente>();
            List<CruceMovimientoRepocliente> otraLista = new List<CruceMovimientoRepocliente>();

            if (TagOCuenta)
            {

                foreach (var item in ListCruces)
                {
                    Lista.Add(new CruceMovimientoRepocliente
                    {


                        Concepto = "CRUCE" + "   " + "#TAG:  " + item.Tag,
                        Fecha = Convert.ToDateTime(item.Fecha),
                        CobroTag = "-" + item.Saldo,
                        SaldoNuevo = item.SaldoDespues,
                        Carril = item.Carril,
                        Referencia = item.Tag,


                    });
                }
            }
            else
            {
                foreach (var item in ListCruces)
                {
                    Lista.Add(new CruceMovimientoRepocliente
                    {


                        Concepto = "CRUCE",
                        Fecha = Convert.ToDateTime(item.Fecha),
                        CobroTag = "-" + item.Saldo,
                        SaldoNuevo = item.SaldoDespues,
                        Carril = item.Carril,
                        Referencia = item.Tag,


                    });
                }
            }

            foreach (var item in ListMovimientos)
            {
                Lista.Add(new CruceMovimientoRepocliente
                {

                    Concepto = item.Concepto,
                    Fecha = Convert.ToDateTime(item.Fecha),
                    CobroTag = item.Monto,
                    SaldoNuevo = item.SaldoActual,
                    Carril = "-------------",
                    Referencia = item.Referencia

                });
            }

            var c = Lista.OrderByDescending(x => x.Fecha);

            foreach (var item in c)
            {
                otraLista.Add(new CruceMovimientoRepocliente
                {
                    Concepto = item.Concepto,
                    Fecha = item.Fecha,
                    CobroTag = item.CobroTag,
                    SaldoNuevo = item.SaldoNuevo,
                    Carril = item.Carril,
                    Referencia = item.Referencia
                });
            }

            return otraLista;
        }
        class HeaderFooter : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document PdfHistorico)
            {
                //base.OnEndPage(writer, document);
                PdfPTable tbHeder = new PdfPTable(3);
                tbHeder.TotalWidth = PdfHistorico.PageSize.Width - PdfHistorico.LeftMargin - PdfHistorico.RightMargin;
                tbHeder.DefaultCell.Border = 0;

                tbHeder.AddCell(new Paragraph(""));


                PdfPCell _cell = new PdfPCell(new Paragraph(""));
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = 0;

                tbHeder.AddCell(_cell);


                tbHeder.WriteSelectedRows(0, -1, PdfHistorico.LeftMargin, writer.PageSize.GetTop(PdfHistorico.TopMargin) + 40, writer.DirectContent);


                PdfPTable tbFoter = new PdfPTable(3);
                tbFoter.TotalWidth = PdfHistorico.PageSize.Width - PdfHistorico.LeftMargin - PdfHistorico.RightMargin;
                tbFoter.DefaultCell.Border = 0;
                tbFoter.AddCell(new Paragraph());

                _cell = new PdfPCell(new Paragraph());
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = 0;

                tbFoter.AddCell(_cell);

                _cell = new PdfPCell(new Paragraph("Pagina" + writer.PageNumber));
                _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _cell.Border = 0;

                tbFoter.AddCell(_cell);



                tbFoter.WriteSelectedRows(0, -1, PdfHistorico.LeftMargin, writer.PageSize.GetBottom(PdfHistorico.BottomMargin) - 5, writer.DirectContent);


            }
        }

        public List<RepoMensual1> TablaRepoMes1(string[] fechas)
        {
            AppDbContext db = new AppDbContext();
            List<RepoMensual1> Lista = new List<RepoMensual1>();

            DateTime AnteriorInicio = Convert.ToDateTime(fechas[2]);
            DateTime AnteriorFin = Convert.ToDateTime(fechas[3]).AddDays(1);
            DateTime ActualInicio = Convert.ToDateTime(fechas[0]);
            DateTime ActualFin = Convert.ToDateTime(fechas[1]).AddDays(1);

            int clienteActual = db.Clientes.Where(x => x.DateTCliente >= ActualInicio && x.DateTCliente < ActualFin).Count();
            int totalCliente = db.Clientes.Where(x => x.DateTCliente < ActualFin).Count();

            int cuentaActual = db.CuentasTelepeajes.Where(x => x.DateTCuenta >= ActualInicio && x.DateTCuenta < ActualFin).Count();
            int totalCuenta = db.CuentasTelepeajes.Where(x => x.DateTCuenta < ActualFin).Count();

            int tagActual = db.Tags.Where(x => x.DateTTag >= ActualInicio && x.DateTTag < ActualFin).Count();
            int totalTag = db.Tags.Where(x => x.DateTTag < ActualFin).Count();


            Lista.Add(new RepoMensual1
            {
                columnaDatos = "CLIENTES",
                totalActual = Convert.ToString(clienteActual),
                totalRegistros = Convert.ToString(totalCliente)
            });

            Lista.Add(new RepoMensual1
            {
                columnaDatos = "CUENTAS",
                totalActual = Convert.ToString(cuentaActual),
                totalRegistros = Convert.ToString(totalCuenta)
            });

            Lista.Add(new RepoMensual1
            {
                columnaDatos = "TAGS",
                totalActual = Convert.ToString(tagActual),
                totalRegistros = Convert.ToString(totalTag)
            });
            return Lista;
        }

        public List<RepoMensual2> TablaRepoMes2(string[] fechas)
        {

            AppDbContext db = new AppDbContext();
            List<RepoMensual2> Lista = new List<RepoMensual2>();

            DateTime ActualInicio = Convert.ToDateTime(fechas[0]);
            DateTime ActualFin = Convert.ToDateTime(fechas[1]).AddDays(1);


            var listCruces = db.Historicos.Where(x => x.Fecha >= ActualInicio && x.Fecha < ActualFin).ToList();
            double crucesMes = 0;
            foreach (var ite in listCruces)
            {
                //crucesMes += ite.Saldo;
            }
            //var crucesMes = Convert.ToDouble(db.Historicos.Where(x => x.Fecha >= ActualInicio && x.Fecha < ActualFin).Sum(x => x.Saldo));
            //var recargasMes = Convert.ToDouble(db.OperacionesCajeros.Where(x => x.DateTOperacion >= ActualInicio && x.DateTOperacion < ActualFin).Sum(x => x.Monto));
            var listRecarga = db.OperacionesCajeros.Where(x => x.DateTOperacion >= ActualInicio && x.DateTOperacion < ActualFin).ToList();
            double recargasMes = 0;
            foreach (var item in listRecarga)
            {
                recargasMes += Convert.ToDouble(item.Monto);
            }
            var totales = recargasMes - crucesMes;


            Lista.Add(new RepoMensual2
            {

                recargaActual = Convercion(Convert.ToString(recargasMes).Replace(".", ",")),
                cruceActual = "",
                totalEfectivo = ""

            });

            Lista.Add(new RepoMensual2
            {

                recargaActual = "",
                cruceActual = Convercion(Convert.ToString(crucesMes).Replace(".", ",")),
                totalEfectivo = ""

            });

            Lista.Add(new RepoMensual2
            {
                recargaActual = "",
                cruceActual = "",
                totalEfectivo = Convercion(Convert.ToString(totales).Replace(".", ","))

            });


            return Lista;
        }

        public ActionResult ReporteClienteMes(TableHistorico model)
        {

            AppDbContext db = new AppDbContext();

            string[] fechas = IntervalosMes(model.Mes, model.Anyo);
            DateTime FechaInicio = Convert.ToDateTime(fechas[0]);
            DateTime FechaFin = Convert.ToDateTime(fechas[1]).AddDays(1);
            DateTime FechaActual = DateTime.Now;


            //var DatosCliente = BuscarInformacionCliente(model.Cliente, FechaInicio, FechaFin);

            if (FechaActual <= FechaFin)
            {

                MemoryStream ms = new MemoryStream();
                Document PdfHistorico = new Document(iTextSharp.text.PageSize.A4);
                PdfWriter pw = PdfWriter.GetInstance(PdfHistorico, ms);

                PdfHistorico.Open();
                PdfHistorico.GetTop(600f);


                string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\SIVAREPORT.png");

                iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                Logo.SetAbsolutePosition(700, 420);
                PdfHistorico.Add(Logo);


                Paragraph titulo = new Paragraph("FECHA INVALIDA O SIN DATOS\n", new Font(Font.FontFamily.HELVETICA, 22));
                titulo.Alignment = Element.ALIGN_CENTER;
                PdfHistorico.Add(titulo);

                PdfHistorico.Close();


                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;

                return new FileStreamResult(ms, "application/pdf");
            }
            else
            {
                MemoryStream ms = new MemoryStream();
                Document PdfHistorico = new Document(iTextSharp.text.PageSize.LETTER);
                PdfWriter pw = PdfWriter.GetInstance(PdfHistorico, ms);
                pw.PageEvent = new HeaderFooter();
                //pw.PageEvent.OnEndPage()
                PdfHistorico.Open();
                PdfHistorico.GetTop(600f);

                //string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\SIVAREPORT.png");

                //iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                //Logo.SetAbsolutePosition(465, 570);
                //PdfHistorico.Add(Logo);

                //Paragraph titulo = new Paragraph("ESTADO DE CUENTA DEL MES DE " + fechas[4] + " " + model.Anyo + " \n", new Font(Font.FontFamily.HELVETICA, 20));
                //titulo.Alignment = Element.ALIGN_CENTER;
                //PdfHistorico.Add(titulo);

                //----------------------
                var listaCliente = (from c in db.Clientes
                                    where c.NumCliente == model.Cliente
                                    select new
                                    {
                                        Nombre = c.Nombre,
                                        Apellido = c.Apellidos,
                                        numCliente = c.NumCliente,
                                        IdCliente = c.Id,
                                    }).ToList();



                var clientePruebas = listaCliente[0].IdCliente;
                var numCLientes = listaCliente[0].numCliente;

                var listaCuentas = (from c in db.CuentasTelepeajes
                                    where c.ClienteId == clientePruebas
                                    select new
                                    {
                                        Id = c.Id,
                                        cuentaId = c.NumCuenta,
                                        typeCuenta = c.TypeCuenta

                                    }).ToList();

                //-------------------------------------------------------------------------------------------------------------------------------------------------------

                if (listaCuentas.Count > 0)
                {

                    foreach (var itemPrincipal in listaCuentas)
                    {
                        var DatosCliente = BuscarInformacionCliente(model.Cliente, itemPrincipal.cuentaId, FechaInicio, FechaFin);

                        string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\SIVAREPORT.png");

                        iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                        Logo.SetAbsolutePosition(465, 570);
                        PdfHistorico.Add(Logo);

                        Paragraph titulo = new Paragraph("ESTADO DE CUENTA DEL MES DE " + fechas[4] + " " + model.Anyo + " \n", new Font(Font.FontFamily.HELVETICA, 20));
                        titulo.Alignment = Element.ALIGN_CENTER;
                        PdfHistorico.Add(titulo);


                        PdfHistorico.Add(Chunk.NEWLINE);

                        Paragraph _cliente = new Paragraph("NOMBRE: " + DatosCliente[0] + "", new Font(Font.FontFamily.HELVETICA, 10));
                        _cliente.Alignment = Element.PTABLE;
                        PdfHistorico.Add(_cliente);

                        Paragraph _clientenum = new Paragraph("#Cliente: " + numCLientes + "", new Font(Font.FontFamily.HELVETICA, 10));
                        _clientenum.Alignment = Element.PTABLE;
                        PdfHistorico.Add(_clientenum);

                        Paragraph _clienteCuenta = new Paragraph("#Cuenta: " + itemPrincipal.cuentaId + "", new Font(Font.FontFamily.HELVETICA, 10));
                        _clienteCuenta.Alignment = Element.PTABLE;
                        PdfHistorico.Add(_clienteCuenta);


                        Paragraph Saldo = new Paragraph("SALDO ANTERIOR: " + Convercion(DatosCliente[1].Replace(".", ",")) + "", new Font(Font.FontFamily.HELVETICA, 10));
                        Saldo.Alignment = Element.PTABLE;
                        PdfHistorico.Add(Saldo);

                        Paragraph fecha = new Paragraph("RECARGAS DEL MES: " + Convercion(DatosCliente[2].Replace(".", ",")) + "", new Font(Font.FontFamily.HELVETICA, 10));
                        fecha.Alignment = Element.PTABLE;
                        PdfHistorico.Add(fecha);


                        Paragraph Event = new Paragraph("CONSUMO DEL MES: " + Convercion(DatosCliente[3].Replace(".", ",")) + "", new Font(Font.FontFamily.HELVETICA, 10));
                        Event.Alignment = Element.PTABLE;
                        PdfHistorico.Add(Event);


                        Paragraph saldo_ = new Paragraph("SALDO FINAL: " + Convercion(DatosCliente[4].Replace(".", ",")) + "", new Font(Font.FontFamily.HELVETICA, 10));
                        saldo_.Alignment = Element.PTABLE;
                        PdfHistorico.Add(saldo_);
                        //PdfHistorico.Add(tableIncio);

                        PdfHistorico.Add(Chunk.NEWLINE);



                        if (listaCliente.Count() > 0)
                        {




                            PdfPTable table = new PdfPTable(6);
                            table.WidthPercentage = 100f;
                            var coldWidthPorcentagesCliente = new[] { 2f, 2f, 1f, 1f, 1f, 2f };
                            table.SetWidths(coldWidthPorcentagesCliente);

                            PdfPCell _cellIni = new PdfPCell();
                            PdfHistorico.GetLeft(40f);
                            PdfHistorico.GetRight(40f);



                            _cellIni = new PdfPCell(new Paragraph("Concepto", new Font(Font.FontFamily.HELVETICA, 14, 3)));
                            _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                            _cellIni.FixedHeight = 10f;
                            table.AddCell(_cellIni);


                            _cellIni = new PdfPCell(new Paragraph("Fecha", new Font(Font.FontFamily.HELVETICA, 14, 3)));
                            _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                            table.AddCell(_cellIni);



                            _cellIni = new PdfPCell(new Paragraph("Monto", new Font(Font.FontFamily.HELVETICA, 14, 3)));
                            _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                            _cellIni.FixedHeight = 10f;
                            table.AddCell(_cellIni);

                            _cellIni = new PdfPCell(new Paragraph("Saldo Final", new Font(Font.FontFamily.HELVETICA, 14, 3)));
                            _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                            _cellIni.FixedHeight = 10f;
                            table.AddCell(_cellIni);


                            _cellIni = new PdfPCell(new Paragraph("Carril", new Font(Font.FontFamily.HELVETICA, 14, 3)));
                            _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                            _cellIni.FixedHeight = 10f;
                            table.AddCell(_cellIni);


                            _cellIni = new PdfPCell(new Paragraph("Referencia", new Font(Font.FontFamily.HELVETICA, 14, 3)));
                            _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cellIni.BackgroundColor = new iTextSharp.text.BaseColor(239, 127, 26);
                            _cellIni.FixedHeight = 10f;
                            table.AddCell(_cellIni);

                            int separadorHojas = 0;



                            if (itemPrincipal.typeCuenta == "Colectiva")
                            {

                                var listaCruces = Cruces(itemPrincipal.cuentaId, FechaInicio, FechaFin, 2, false);
                                var listaMovimientos = Movimientos(itemPrincipal.cuentaId, FechaInicio, FechaFin, 2, false);
                                var fusion = FusionarListasMes(listaCruces, listaMovimientos, false);



                                foreach (var itemfusin in fusion)
                                {
                                    separadorHojas++;



                                    PdfPCell _cell = new PdfPCell();
                                    PdfHistorico.GetLeft(40f);
                                    PdfHistorico.GetRight(40f);


                                    _cell = new PdfPCell(new Paragraph(itemfusin.Concepto.ToString(), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    _cell.FixedHeight = 10f;
                                    table.AddCell(_cell);


                                    _cell = new PdfPCell(new Paragraph(itemfusin.Fecha.ToString("dd/MM/yyyy HH:mm:ss"), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    table.AddCell(_cell);

                                    if (itemfusin.Concepto == "CRUCE")
                                    {

                                        var FontColour = new BaseColor(255, 0, 0);
                                        var ColorRojo = FontFactory.GetFont("Times New Roman", 9, FontColour);

                                        _cell = new PdfPCell(new Paragraph(itemfusin.CobroTag.ToString(), ColorRojo));
                                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        _cell.FixedHeight = 10f;
                                        table.AddCell(_cell);

                                    }
                                    else
                                    {
                                        var FontColour = new BaseColor(0, 255, 0);
                                        var ColorRojo = FontFactory.GetFont("Times New Roman", 9, FontColour);

                                        _cell = new PdfPCell(new Paragraph(itemfusin.CobroTag.ToString(), ColorRojo));
                                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        _cell.FixedHeight = 10f;
                                        table.AddCell(_cell);
                                    }


                                    _cell = new PdfPCell(new Paragraph(itemfusin.SaldoNuevo.ToString(), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    _cell.FixedHeight = 10f;
                                    table.AddCell(_cell);


                                    _cell = new PdfPCell(new Paragraph(itemfusin.Carril.ToString(), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    _cell.FixedHeight = 10f;
                                    table.AddCell(_cell);



                                    _cell = new PdfPCell(new Paragraph(itemfusin.Referencia.ToString(), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    _cell.FixedHeight = 10f;
                                    table.AddCell(_cell);


                                }
                                PdfHistorico.Add(table);

                                PdfHistorico.Add(Chunk.NEWLINE);
                            }

                            if (itemPrincipal.typeCuenta == "Individual")
                            {
                                var Tag = db.Tags.Where(x => x.CuentaId == itemPrincipal.Id).ToList();
                                string NumTAG = Tag[0].NumTag;
                                var listaCruces = Cruces(NumTAG, FechaInicio, FechaFin, 1, false);
                                var listaMovimientos = Movimientos(NumTAG, FechaInicio, FechaFin, 1, false);
                                var fusion = FusionarListasMes(listaCruces, listaMovimientos, false);



                                foreach (var itemfusin in fusion)
                                {
                                    separadorHojas++;



                                    PdfPCell _cell = new PdfPCell();
                                    PdfHistorico.GetLeft(40f);
                                    PdfHistorico.GetRight(40f);


                                    _cell = new PdfPCell(new Paragraph(itemfusin.Concepto.ToString(), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    _cell.FixedHeight = 10f;
                                    table.AddCell(_cell);


                                    _cell = new PdfPCell(new Paragraph(itemfusin.Fecha.ToString("dd/MM/yyyy HH:mm:ss"), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    table.AddCell(_cell);

                                    if (itemfusin.Concepto == "CRUCE")
                                    {

                                        var FontColour = new BaseColor(255, 0, 0);
                                        var ColorRojo = FontFactory.GetFont("Times New Roman", 9, FontColour);

                                        _cell = new PdfPCell(new Paragraph(itemfusin.CobroTag.ToString(), ColorRojo));
                                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        _cell.FixedHeight = 10f;
                                        table.AddCell(_cell);

                                    }
                                    else
                                    {
                                        var FontColour = new BaseColor(0, 255, 0);
                                        var ColorRojo = FontFactory.GetFont("Times New Roman", 9, FontColour);

                                        _cell = new PdfPCell(new Paragraph(itemfusin.CobroTag.ToString(), ColorRojo));
                                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        _cell.FixedHeight = 10f;
                                        table.AddCell(_cell);
                                    }



                                    _cell = new PdfPCell(new Paragraph(itemfusin.SaldoNuevo.ToString(), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    _cell.FixedHeight = 10f;
                                    table.AddCell(_cell);

                                    _cell = new PdfPCell(new Paragraph(itemfusin.Carril.ToString(), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    _cell.FixedHeight = 10f;
                                    table.AddCell(_cell);



                                    _cell = new PdfPCell(new Paragraph(itemfusin.Referencia.ToString(), new Font(Font.FontFamily.HELVETICA, 9)));
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    _cell.FixedHeight = 10f;
                                    table.AddCell(_cell);


                                }
                                PdfHistorico.Add(table);

                            }


                            PdfHistorico.Add(Chunk.NEWLINE);
                            PdfHistorico.NewPage();

                        }

                    }
                    PdfHistorico.Close();
                }
                else
                {


                    string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\SIVAREPORT.png");

                    iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                    Logo.SetAbsolutePosition(465, 570);
                    PdfHistorico.Add(Logo);

                    Paragraph titulo = new Paragraph("ESTADO DE CUENTA DEL MES DE " + fechas[4] + " " + model.Anyo + " \n", new Font(Font.FontFamily.HELVETICA, 20));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    PdfHistorico.Add(titulo);


                    PdfHistorico.Add(Chunk.NEWLINE);



                    Paragraph _clientenum = new Paragraph("#Cliente: " + numCLientes + "", new Font(Font.FontFamily.HELVETICA, 10));
                    _clientenum.Alignment = Element.PTABLE;
                    PdfHistorico.Add(_clientenum);

                    Paragraph _clienteCuenta = new Paragraph("#Cuenta: SIN CUENTAS", new Font(Font.FontFamily.HELVETICA, 10));
                    _clienteCuenta.Alignment = Element.PTABLE;
                    PdfHistorico.Add(_clienteCuenta);


                    PdfHistorico.Add(Chunk.NEWLINE);

                    PdfHistorico.Close();

                }

                //***********************************************************************************************************************************************************************




                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;

                return new FileStreamResult(ms, "application/pdf");
            }
        }

        public string[] BuscarInformacionCliente(string numeroCliente, string numeroCuenta, DateTime fecha1, DateTime fecha2)
        {

            AppDbContext db = new AppDbContext();

            string[] DatosCliente = new string[6];


            var listaCliente = (from c in db.Clientes
                                where c.NumCliente == numeroCliente
                                select new
                                {
                                    Nombre = c.Nombre,
                                    Apellido = c.Apellidos,
                                    IdCliente = c.Id
                                }).ToList();

            if (listaCliente.Count() > 0)
            {

                DatosCliente[0] = listaCliente[0].Nombre + "  " + listaCliente[0].Apellido;
                var clientePruebas = listaCliente[0].IdCliente;


                var listaCuentas = (from c in db.CuentasTelepeajes
                                    where c.NumCuenta == numeroCuenta
                                    select new
                                    {
                                        Id = c.Id,
                                        cuentaId = c.NumCuenta,
                                        typeCuenta = c.TypeCuenta

                                    }).ToList();

                string SaldoCliente = string.Empty;
                string SaldoFinal = string.Empty;
                double CrucesTotales = 0;
                double RecargasTotales = 0;

                foreach (var item in listaCuentas)
                {


                    if (item.typeCuenta == "Colectiva")
                    {
                        //var listcolectivo = (from c in db.CuentasTelepeajes
                        //                     where c.Id == item.Id
                        //                     select new
                        //                     {
                        //                         saldos = c.SaldoCuenta
                        //                     }).ToList();
                        var listcolectivo = (from h in db.Historicos
                                             join t in db.Tags on h.Tag equals t.NumTag
                                             join c in db.CuentasTelepeajes on t.CuentaId equals c.Id
                                             where h.Fecha >= fecha1 && h.Fecha < fecha2 && c.Id == item.Id
                                             select new
                                             {
                                                 fecha = h.Fecha,
                                                 saldoANteriro = h.SaldoAnterior,
                                                 saldoActualizado = h.SaldoActualizado
                                             }).ToList();

                        if (listcolectivo.Count > 0)
                        {
                            for (int i = 0; i < 1; i++)
                            {

                                SaldoCliente = Convert.ToString(listcolectivo[i].saldoANteriro);
                            }

                        }

                        var ListaVolteada = listcolectivo.OrderByDescending(x => x.fecha).ToList();


                        if (ListaVolteada.Count > 0)
                        {
                            for (int i = 0; i < 1; i++)
                            {

                                SaldoFinal = Convert.ToString(ListaVolteada[i].saldoActualizado);
                            }

                        }

                        RecargasTotales += Convert.ToDouble(db.OperacionesCajeros.Where(x => x.DateTOperacion >= fecha1 && x.DateTOperacion < fecha2 && x.Numero == item.cuentaId).Sum(x => x.Monto));





                    }
                    if (item.typeCuenta == "Individual")
                    {

                        var listindividual = (from h in db.Historicos
                                              join t in db.Tags on h.Tag equals t.NumTag
                                              join c in db.CuentasTelepeajes on t.CuentaId equals c.Id
                                              where h.Fecha >= fecha1 && h.Fecha < fecha2 && c.Id == item.Id
                                              select new
                                              {
                                                  fecha = h.Fecha,
                                                  saldoANteriro = h.SaldoAnterior,
                                                  saldoActual = h.SaldoActualizado
                                              }).ToList();

                        //var listindividual = (from c in db.Tags
                        //                      where c.CuentaId == item.Id
                        //                      select new
                        //                      {
                        //                          saldos = c.SaldoTag
                        //                      }).ToList();
                        if (listindividual.Count() > 0)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                SaldoCliente = Convert.ToString(listindividual[i].saldoANteriro);
                            }

                        }


                        var ListaVolteada = listindividual.OrderByDescending(x => x.fecha).ToList();


                        if (ListaVolteada.Count > 0)
                        {
                            for (int i = 0; i < 1; i++)
                            {

                                SaldoFinal = Convert.ToString(ListaVolteada[i].saldoActual);
                            }

                        }


                        var Tag = db.Tags.Where(x => x.CuentaId == item.Id).ToList();
                        string NumTAG = Tag[0].NumTag;
                        RecargasTotales += Convert.ToDouble(db.OperacionesCajeros.Where(x => x.DateTOperacion >= fecha1 && x.DateTOperacion < fecha2 && x.Numero == NumTAG).Sum(x => x.Monto));



                    }


                    foreach (var item2 in listaCuentas)
                    {
                        var listaCruces = (from h in db.Historicos
                                           join t in db.Tags on h.Tag equals t.NumTag
                                           join c in db.CuentasTelepeajes on t.CuentaId equals c.Id
                                           where h.Fecha >= fecha1 && h.Fecha < fecha2
                                           where c.NumCuenta == item2.cuentaId
                                           select new
                                           {
                                               saldosCruces = h.Saldo
                                           }).ToList();

                        //CrucesTotales += listaCruces.Sum(x => x.saldosCruces);
                    }


                    //                    SaldoCliente += Math.Round(Convert.ToDouble(item.SALDOCUENTA), 2);
                }

                DatosCliente[1] = SaldoCliente;
                DatosCliente[2] = RecargasTotales.ToString("F");
                DatosCliente[3] = CrucesTotales.ToString("F");
                DatosCliente[4] = SaldoFinal;

                return DatosCliente;

            }
            else return null;
        }


        public string BuscaTramo(string IdGare)
        {
            if (IdGare == "21")
                return "SUR";
            else
                return "NORTE";

        }
        public ActionResult Pdf()
        {

            Document PdfHistorico = new Document(iTextSharp.text.PageSize.LETTER.Rotate());
            MemoryStream ms = new MemoryStream();
            PdfWriter pw = PdfWriter.GetInstance(PdfHistorico, ms);

            PdfHistorico.Open();
            PdfHistorico.GetTop(600f);

            string rutaLogo = Server.MapPath("..\\Content\\css-yisus\\img\\SIVAREPORT.png");

            iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance(rutaLogo);
            Logo.SetAbsolutePosition(650, 400);
            PdfHistorico.Add(Logo);


            if (cuenta != "" && cuenta != null)
            {


                if (Fecha1 == Fecha2)
                {



                    Paragraph titulo = new Paragraph("REPORTE DEL HISTORICO\n", new Font(Font.FontFamily.HELVETICA, 22));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    PdfHistorico.Add(titulo);


                    Paragraph _cliente = new Paragraph("Cliente: " + cliente + "", new Font(Font.FontFamily.HELVETICA, 12));
                    _cliente.Alignment = Element.PTABLE;
                    PdfHistorico.Add(_cliente);


                    Paragraph Saldo = new Paragraph("Cuenta: " + cuenta + "", new Font(Font.FontFamily.HELVETICA, 12));
                    Saldo.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Saldo);

                    Paragraph fecha = new Paragraph("Fecha: " + Fecha1 + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.PTABLE;
                    PdfHistorico.Add(fecha);


                    Paragraph Event = new Paragraph("Movimientos: " + eventos + "", new Font(Font.FontFamily.HELVETICA, 12));
                    Event.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Event);


                    Paragraph saldo_ = new Paragraph("Saldo: " + saldo + "", new Font(Font.FontFamily.HELVETICA, 12));
                    saldo_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldo_);

                    Paragraph saldoRecargas_ = new Paragraph("Saldo: " + saldoMov + "", new Font(Font.FontFamily.HELVETICA, 12));
                    saldoRecargas_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldoRecargas_);

                    Paragraph saldoCruces_ = new Paragraph("Saldo: " + saldoCru + "", new Font(Font.FontFamily.HELVETICA, 12));
                    saldoCruces_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldoCruces_);


                    PdfHistorico.Add(Chunk.NEWLINE);


                }
                else
                {

                    Paragraph titulo = new Paragraph("REPORTE DEL HISTORICO\n", new Font(Font.FontFamily.HELVETICA, 22));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    PdfHistorico.Add(titulo);


                    Paragraph _cliente = new Paragraph("Cliente: " + cliente + "", new Font(Font.FontFamily.HELVETICA, 12));
                    _cliente.Alignment = Element.PTABLE;
                    PdfHistorico.Add(_cliente);


                    Paragraph Saldo = new Paragraph("Cuenta: " + cuenta + "", new Font(Font.FontFamily.HELVETICA, 12));
                    Saldo.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Saldo);

                    Paragraph fecha = new Paragraph("Fecha: " + Fecha1 + "al" + Fecha2 + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.PTABLE;
                    PdfHistorico.Add(fecha);

                    Paragraph Event = new Paragraph("Movimientos: " + eventos + "", new Font(Font.FontFamily.HELVETICA, 12));
                    Event.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Event);

                    Paragraph saldo_ = new Paragraph("Saldo: " + saldo + "", new Font(Font.FontFamily.HELVETICA, 12));
                    saldo_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldo_);

                    Paragraph saldoRecargas_ = new Paragraph("Total de Recargas: " + saldoMov + "", new Font(Font.FontFamily.HELVETICA, 12));
                    saldoRecargas_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldoRecargas_);

                    Paragraph saldoCruces_ = new Paragraph("Total de Cruces: " + saldoCru + "", new Font(Font.FontFamily.HELVETICA, 12));
                    saldoCruces_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldoCruces_);


                    PdfHistorico.Add(Chunk.NEWLINE);

                }




            }
            else
            {

                if (Fecha1 == Fecha2)
                {
                    Paragraph titulo = new Paragraph("REPORTE DEL HISTORICO\n", new Font(Font.FontFamily.HELVETICA, 22));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    PdfHistorico.Add(titulo);


                    Paragraph fecha = new Paragraph("Fecha: " + Fecha1 + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.PTABLE;
                    PdfHistorico.Add(fecha);


                    Paragraph Event = new Paragraph("Movimientos: " + eventos + "", new Font(Font.FontFamily.HELVETICA, 12));
                    Event.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Event);

                    Paragraph _cliente = new Paragraph("Cliente: ---------- ", new Font(Font.FontFamily.HELVETICA, 12));
                    _cliente.Alignment = Element.PTABLE;
                    PdfHistorico.Add(_cliente);


                    Paragraph Saldo = new Paragraph("Cuenta: --------- ", new Font(Font.FontFamily.HELVETICA, 12));
                    Saldo.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Saldo);

                    Paragraph saldo_ = new Paragraph("Saldo: -----------", new Font(Font.FontFamily.HELVETICA, 12));
                    saldo_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldo_);

                    PdfHistorico.Add(Chunk.NEWLINE);

                }
                else
                {
                    Paragraph titulo = new Paragraph("REPORTE DEL HISTORICO\n", new Font(Font.FontFamily.HELVETICA, 22));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    PdfHistorico.Add(titulo);


                    Paragraph fecha = new Paragraph("Fecha: " + Fecha1 + " al " + Fecha2 + "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.PTABLE;
                    PdfHistorico.Add(fecha);

                    Paragraph Event = new Paragraph("Movimientos: " + eventos + "", new Font(Font.FontFamily.HELVETICA, 12));
                    Event.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Event);

                    Paragraph _cliente = new Paragraph("Cliente: --------- ", new Font(Font.FontFamily.HELVETICA, 12));
                    _cliente.Alignment = Element.PTABLE;
                    PdfHistorico.Add(_cliente);


                    Paragraph Saldo = new Paragraph("Cuenta: --------- ", new Font(Font.FontFamily.HELVETICA, 12));
                    Saldo.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Saldo);

                    Paragraph saldo_ = new Paragraph("Saldo: --------", new Font(Font.FontFamily.HELVETICA, 12));
                    saldo_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldo_);




                    PdfHistorico.Add(Chunk.NEWLINE);

                }
            }





            if (ListPDFCruces.Count > 0)
            {
                //Reporte Willy
                if (ClienteAdmin)
                {

                    PdfPTable table = new PdfPTable(8);
                    table.WidthPercentage = 100f;
                    var coldWidthPorcentagesCliente = new[] { 2f, 2f, 1f, 1f, 1f, 2f, 2f, 2f };
                    table.SetWidths(coldWidthPorcentagesCliente);

                    PdfPCell _cellIni = new PdfPCell();
                    PdfHistorico.GetLeft(40f);
                    PdfHistorico.GetRight(40f);


                    _cellIni = new PdfPCell(new Paragraph("Numero de Tag"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Fecha"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Tramo"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Carril"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);

                    _cellIni = new PdfPCell(new Paragraph("Clase"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Saldo Anterior"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Descuento"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);

                    _cellIni = new PdfPCell(new Paragraph("Saldo Actualizado"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);




                    foreach (var item in ListPDFCruces)
                    {


                        PdfPCell _cell = new PdfPCell();
                        PdfHistorico.GetLeft(40f);
                        PdfHistorico.GetRight(40f);


                        _cell = new PdfPCell(new Paragraph(item.Tag.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Fecha.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Cuerpo.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.Carril.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Clase.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.SaldoAntes.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.Saldo.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.SaldoDespues.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);


                        //PdfPCell _cellnew = new PdfPCell();
                        //PdfHistorico.GetLeft(40f);
                        //PdfHistorico.GetRight(40f);


                        //_cellnew = new PdfPCell(new Paragraph(item.Tag.ToString()));
                        //_cellnew.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cellnew.FixedHeight = 10f;
                        //table.AddCell(_cellnew);

                        //_cellnew = new PdfPCell(new Paragraph(item.Fecha.ToString()));
                        //_cellnew.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cellnew.FixedHeight = 10f;
                        //table.AddCell(_cellnew);

                        //_cellnew = new PdfPCell(new Paragraph(item.Cuerpo.ToString()));
                        //_cellnew.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cellnew.FixedHeight = 10f;
                        //table.AddCell(_cellnew);

                        //_cellnew = new PdfPCell(new Paragraph(item.Carril.ToString()));
                        //_cellnew.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cellnew.FixedHeight = 10f;
                        //table.AddCell(_cellnew);

                        //_cellnew = new PdfPCell(new Paragraph(item.Clase.ToString()));
                        //_cellnew.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cellnew.FixedHeight = 10f;
                        //table.AddCell(_cellnew);

                        //_cellnew = new PdfPCell(new Paragraph(item.SaldoAntes.ToString()));
                        //_cellnew.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cellnew.FixedHeight = 10f;
                        //table.AddCell(_cellnew);


                        //_cellnew = new PdfPCell(new Paragraph(item.Saldo.ToString()));
                        //_cellnew.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cellnew.FixedHeight = 10f;
                        //table.AddCell(_cellnew);

                        //_cellnew = new PdfPCell(new Paragraph(item.SaldoDespues.ToString()));
                        //_cellnew.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cellnew.FixedHeight = 10f;
                        //table.AddCell(_cellnew);



                    }
                    PdfHistorico.Add(table);

                }
                //Reporte Cliente
                else
                {
                    PdfPTable table = new PdfPTable(7);
                    table.WidthPercentage = 100f;
                    var coldWidthPorcentagesCliente = new[] { 2f, 3f, 3f, 1f, 2f, 1f, 1f };
                    table.SetWidths(coldWidthPorcentagesCliente);

                    PdfPCell _cellIni = new PdfPCell();
                    PdfHistorico.GetLeft(40f);
                    PdfHistorico.GetRight(40f);


                    _cellIni = new PdfPCell(new Paragraph("Numero de Tag"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Plaza"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Fecha de Cruce"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Tramo"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);

                    _cellIni = new PdfPCell(new Paragraph("Cobro del Cruce"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Carril"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Operadora"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);



                    foreach (var item in ListPDFCruces)
                    {

                        PdfPCell _cell = new PdfPCell();
                        PdfHistorico.GetLeft(40f);
                        PdfHistorico.GetRight(40f);


                        _cell = new PdfPCell(new Paragraph(item.Tag.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.Plaza.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Fecha.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Cuerpo.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.Saldo.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Carril.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.Operador.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);



                    }
                    PdfHistorico.Add(table);
                }
            }
            else if (ListPDFMovimientos.Count > 0)
            {
                //Reporte Willy
                if (ClienteAdmin)
                {
                    PdfPTable table = new PdfPTable(8);
                    table.WidthPercentage = 100f;
                    var coldWidthPorcentagesCliente = new[] { 2f, 1f, 1f, 2f, 1f, 1f, 1f, 1f };
                    table.SetWidths(coldWidthPorcentagesCliente);

                    PdfPCell _cellIni = new PdfPCell();
                    PdfHistorico.GetLeft(40f);
                    PdfHistorico.GetRight(40f);


                    _cellIni = new PdfPCell(new Paragraph("Concepto"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Tipo Pago"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Monto"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Fecha de Operacion"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Tag o Cuenta"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    //_cellIni = new PdfPCell(new Paragraph(item.TagCuenta.ToString()));
                    //_cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    //_cellIni.FixedHeight = 10f;
                    //table.AddCell(_cellIni);

                    _cellIni = new PdfPCell(new Paragraph("Tipo de Cuenta"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Cobro"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);

                    _cellIni = new PdfPCell(new Paragraph("Numero de Referencia"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    foreach (var item in ListPDFMovimientos)
                    {

                        PdfPCell _cell = new PdfPCell();
                        PdfHistorico.GetLeft(40f);
                        PdfHistorico.GetRight(40f);


                        _cell = new PdfPCell(new Paragraph(item.Concepto.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        if (item.TipoPago == null)
                        {
                            _cell = new PdfPCell(new Paragraph("-----------"));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(_cell);
                        }

                        else
                        {

                            _cell = new PdfPCell(new Paragraph(item.TipoPago));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(_cell);
                        }


                        _cell = new PdfPCell(new Paragraph(item.Monto.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Fecha.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.TagCuenta.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        //_cell = new PdfPCell(new Paragraph(item.numtag .ToString()));
                        //_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cell.FixedHeight = 10f;
                        //table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.TypeCuenta.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.CobroTag.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.Referencia));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);




                    }
                    PdfHistorico.Add(table);
                }
                //Reporte Cliente
                else
                {
                    PdfPTable table = new PdfPTable(8);
                    table.WidthPercentage = 100f;
                    var coldWidthPorcentagesCliente = new[] { 5f, 3f, 3f, 3f, 3f, 3f, 3f, 3f };
                    table.SetWidths(coldWidthPorcentagesCliente);

                    PdfPCell _cellIni = new PdfPCell();
                    PdfHistorico.GetLeft(40f);
                    PdfHistorico.GetRight(40f);


                    _cellIni = new PdfPCell(new Paragraph("Concepto"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Tipo de Pago"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Monto"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Fecha de Operacion"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Tag o Cuenta"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    //_cellIni = new PdfPCell(new Paragraph(item.TagCuenta.ToString()));
                    //_cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    //_cellIni.FixedHeight = 10f;
                    //table.AddCell(_cellIni);

                    _cellIni = new PdfPCell(new Paragraph("Tipo de Cuenta"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Cobro del Tag"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);

                    _cellIni = new PdfPCell(new Paragraph("Numero de Referencia"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);

                    foreach (var item in ListPDFMovimientos)
                    {

                        PdfPCell _cell = new PdfPCell();
                        PdfHistorico.GetLeft(40f);
                        PdfHistorico.GetRight(40f);


                        _cell = new PdfPCell(new Paragraph(item.Concepto.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.TipoPago.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Monto.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Fecha.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.TagCuenta.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        //_cell = new PdfPCell(new Paragraph(item.TagCuenta.ToString()));
                        //_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cell.FixedHeight = 10f;
                        //table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.TypeCuenta.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.CobroTag.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.Referencia));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);

                    }
                    PdfHistorico.Add(table);
                }
            }
            else if (ListPDFCrucesMovimientos.Count > 0)
            {
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100f;
                var coldWidthPorcentagesCliente = new[] { 4f, 4f, 3f, 3f, 3f };
                table.SetWidths(coldWidthPorcentagesCliente);

                PdfPCell _cellIni = new PdfPCell();
                PdfHistorico.GetLeft(40f);
                PdfHistorico.GetRight(40f);


                _cellIni = new PdfPCell(new Paragraph("Concepto"));
                _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni.FixedHeight = 10f;
                table.AddCell(_cellIni);


                _cellIni = new PdfPCell(new Paragraph("Fecha"));
                _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(_cellIni);



                _cellIni = new PdfPCell(new Paragraph("Monto"));
                _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni.FixedHeight = 10f;
                table.AddCell(_cellIni);

                _cellIni = new PdfPCell(new Paragraph("Carril"));
                _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni.FixedHeight = 10f;
                table.AddCell(_cellIni);




                _cellIni = new PdfPCell(new Paragraph("Referencia"));
                _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellIni.FixedHeight = 10f;
                table.AddCell(_cellIni);


                foreach (var item in ListPDFCrucesMovimientos)
                {

                    PdfPCell _cell = new PdfPCell();
                    PdfHistorico.GetLeft(40f);
                    PdfHistorico.GetRight(40f);


                    _cell = new PdfPCell(new Paragraph(item.Concepto.ToString()));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.FixedHeight = 10f;
                    table.AddCell(_cell);


                    _cell = new PdfPCell(new Paragraph(item.Fecha.ToString()));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cell);



                    _cell = new PdfPCell(new Paragraph(item.CobroTag.ToString()));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.FixedHeight = 10f;
                    table.AddCell(_cell);

                    _cell = new PdfPCell(new Paragraph(item.Carril.ToString()));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.FixedHeight = 10f;
                    table.AddCell(_cell);



                    _cell = new PdfPCell(new Paragraph(item.Referencia));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.FixedHeight = 10f;
                    table.AddCell(_cell);


                }
                PdfHistorico.Add(table);
            }



            PdfHistorico.Close();


            byte[] bytesStream = ms.ToArray();
            ms = new MemoryStream();
            ms.Write(bytesStream, 0, bytesStream.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "application/pdf");
        }
        enum DecisionesMetodos
        {
            Tag = 1,
            Cuenta = 2,
            RangoFecha = 3
        }
        public string Convercion(string saldo)
        {
            string[] tipo;
            string Mandar = string.Empty;
            tipo = saldo.Split(',');
            int contar = 0;
            CultureInfo culture = new CultureInfo("es-MX", false);
            if (tipo.Count() == 2)
            {
                if (tipo[0].Length > 3)
                {
                    var numeroDigitos = tipo[0].Length;
                    var modulo = numeroDigitos % 3;

                    if (modulo > 0)
                    {
                        var array = tipo[0].ToCharArray();
                        for (int i = 0; i < modulo; i++)
                        {
                            Mandar = Mandar + array[i];
                        }
                        Mandar = Mandar + ",";
                        var sigue = array.Length - modulo;
                        for (int i = modulo; i < array.Length; i++)
                        {
                            if (contar == 3)
                            {
                                Mandar = Mandar + "," + array[i];
                                contar = 1;
                            }
                            else
                            {
                                Mandar = Mandar + array[i];
                                contar++;
                            }



                        }
                        Mandar = Mandar.TrimEnd(',');
                        Mandar = Mandar + '.' + tipo[1];
                    }
                    else
                    {
                        var array = tipo[0].ToCharArray();

                        for (int i = 0; i < array.Length; i++)
                        {
                            if (contar == 3)
                            {
                                Mandar = Mandar + "," + array[i];
                                contar = 0;
                            }
                            else
                            {
                                Mandar = Mandar + array[i];
                                contar++;
                            }


                        }
                        Mandar = Mandar.TrimEnd(',');
                        if (tipo[1].Length == 1)
                        {
                            Mandar = Mandar + '.' + tipo[1] + '0';
                        }
                        else
                        {
                            Mandar = Mandar + '.' + tipo[1];
                        }
                    }

                    //if (tipo[1].Length == 2)
                    //{
                    //    Mandar = Convert.ToDouble(saldo).ToString("C2", culture).Replace("$", "Q");
                    //}
                    //else
                    //{
                    //    Mandar = Convert.ToDouble(saldo).ToString("C1", culture).Replace("$", "Q");
                    //}
                }
                else
                {
                    if (tipo[1].Length == 1)
                    {
                        Mandar = tipo[0] + '.' + tipo[1] + '0';
                    }
                    else
                    {
                        Mandar = tipo[0] + '.' + tipo[1];
                    }

                }

            }
            else
            {
                if (tipo[0].Length > 3)
                {
                    var numeroDigitos = tipo[0].Length;
                    var modulo = numeroDigitos % 3;

                    if (modulo > 0)
                    {
                        var array = tipo[0].ToCharArray();
                        for (int i = 0; i < modulo; i++)
                        {
                            Mandar = Mandar + array[i];
                        }
                        Mandar = Mandar + ",";
                        var sigue = array.Length - modulo;
                        for (int i = modulo; i < array.Length; i++)
                        {
                            if (contar == 3)
                            {
                                Mandar = Mandar + "," + array[i];
                                contar = 1;
                            }
                            else
                            {
                                Mandar = Mandar + array[i];
                                contar++;
                            }



                        }
                        Mandar = Mandar.TrimEnd(',');
                        Mandar = Mandar + ".00";
                    }
                    else
                    {
                        var array = tipo[0].ToCharArray();

                        for (int i = 0; i < array.Length; i++)
                        {
                            if (contar == 3)
                            {
                                Mandar = Mandar + "," + array[i];
                                contar = 0;
                            }
                            else
                            {
                                Mandar = Mandar + array[i];
                                contar++;
                            }


                        }
                        Mandar = Mandar.TrimEnd(',');
                        Mandar = Mandar + ".00";
                    }
                }
                else

                    Mandar = tipo[0] + ".00";
            }

            return 'Q' + Mandar;
        }

        //public double CruceColetivo(DateTime fecha1, DateTime fecha2)
        //{
        //    AppDbContext db = new AppDbContext();

        //    var lista = (from h in db.Historicos
        //                 join t in db.Tags on h.Tag equals t.NumTag
        //                 join c in db.CuentasTelepeajes on t.CuentaId equals c.Id
        //                 where h.Fecha >= fecha1 && h.Fecha < fecha2
        //                 where c.TypeCuenta == "Colectiva"
        //                 select new
        //                 {
        //                     saldos = h.Saldo

        //                 }).ToList();

        //    return lista.Sum(x => x.saldos);
        //}

        //public double CruceIndividual(DateTime fecha1, DateTime fecha2)
        //{

        //    AppDbContext db = new AppDbContext();

        //    var lista = (from h in db.Historicos
        //                 join t in db.Tags on h.Tag equals t.NumTag
        //                 join c in db.CuentasTelepeajes on t.CuentaId equals c.Id
        //                 where h.Fecha >= fecha1 && h.Fecha < fecha2
        //                 where c.TypeCuenta == "Individual"
        //                 select new
        //                 {
        //                     saldos = h.Saldo

        //                 }).ToList();

        //    return lista.Sum(x => x.saldos);
        //}

        public string[] IntervalosMes(string mes, string anyo)
        {
            int DiaActual, DiaAnterior;
            //string FechaInicioActual = string.Empty;
            //string FechaFinActual = string.Empty;

            //string FechaInicioAnterior = string.Empty;
            //string FechaFinAnterior = string.Empty;

            //string MesNombre = string.Empty;

            string[] Fechas = new string[5];

            DateTime Actual = DateTime.Today;

            if (Convert.ToInt32(mes) <= Actual.Month && Convert.ToInt32(anyo) <= Actual.Year)
            {

                switch (mes)
                {
                    case "01":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 01);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 12);
                        Fechas[2] = "01" + "/" + 12 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 12 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "ENERO";
                        break;
                    case "02":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 02);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 01);
                        Fechas[2] = "01" + "/" + 01 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 01 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "FEBRERO";
                        break;
                    case "03":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 03);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 02);
                        Fechas[2] = "01" + "/" + 02 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 02 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "MARZO";
                        break;
                    case "04":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 04);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 03);
                        Fechas[2] = "01" + "/" + 03 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 03 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "ABRIL";
                        break;
                    case "05":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 05);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 04);
                        Fechas[2] = "01" + "/" + 04 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 04 + "/" + anyo + " " + "23:59:59";
                        Fechas[4] = "MAYO";
                        break;
                    case "06":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 06);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 05);
                        Fechas[2] = "01" + "/" + 05 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 05 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "JUNIO";
                        break;
                    case "07":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 07);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "23:59:59";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 06);
                        Fechas[2] = "01" + "/" + 06 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 06 + "/" + anyo + " " + "23:59:59";
                        Fechas[4] = "JULIO";
                        break;
                    case "08":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 08);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 07);
                        Fechas[2] = "01" + "/" + 07 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 07 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "AGOSTO";
                        break;
                    case "09":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 09);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 08);
                        Fechas[2] = "01" + "/" + 08 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 08 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "SEPTIEMBRE";
                        break;
                    case "10":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 10);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 09);
                        Fechas[2] = "01" + "/" + 09 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 09 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "OCTUBRE";
                        break;
                    case "11":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 11);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 10);
                        Fechas[2] = "01" + "/" + 10 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 10 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "NOVIEMBRE";
                        break;
                    case "12":
                        DiaActual = DateTime.DaysInMonth(Convert.ToInt32(anyo), 12);
                        Fechas[0] = "01" + "/" + mes + "/" + anyo + " " + "00:00:00";
                        Fechas[1] = DiaActual + "/" + mes + "/" + anyo + " " + "00:00:00";
                        DiaAnterior = DateTime.DaysInMonth(Convert.ToInt32(anyo), 11);
                        Fechas[2] = "01" + "/" + 11 + "/" + anyo + " " + "00:00:00";
                        Fechas[3] = DiaAnterior + "/" + 11 + "/" + anyo + " " + "00:00:00";
                        Fechas[4] = "DICIEMBRE";
                        break;
                    default:
                        break;

                }

                //Object Fechas = new { FechaInicioActual, FechaFinActual, FechaInicioAnterior, FechaFinAnterior, MesNombre };
                return Fechas;
            }
            else return null;
        }


    }

}