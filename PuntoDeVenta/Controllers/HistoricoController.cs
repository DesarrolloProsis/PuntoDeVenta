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

namespace PuntoDeVenta.Controllers
{
    [Authorize(Roles = "SuperUsuario, Cajero")]
    public class HistoricoController : Controller
    {

        public static DataTable dtstatic = new DataTable();
        public static List<Cruces> ListPDFCruces = new List<Cruces>();
        public static List<Movimientos> ListPDFMovimientos = new List<Movimientos>();
        public static string Fecha1;
        public static string Fecha2;
        public static string Plaza;
        public static string cuenta;
        public static string cliente;
        public static string saldo;        
        public static string eventos;
        public static object Info;
        public static bool ClienteAdmin;
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
            AppDbContext db = new AppDbContext();            
            List<Cruces> ListCruces = new List<Cruces>();
            List<Movimientos> ListMovimiento = new List<Movimientos>();
            DataTable dt = new DataTable();

            if (Tag != null && Tag != "")
            {
                if (model.Fecha_Fin > model.Fecha_Inicio || Fecha_Inicio != "01/01/0001" && Fecha_Fin != "01/01/0001")
                {
                    if (model.Fecha_Fin == DateTime.Now.Date)
                    {
                        DateTime DateAyuda = DateTime.Now;

                        if (TypeMovimiento == "01")
                        {
                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, DateAyuda, 1, true);

                            if (ListMovimiento.Any())
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag,Tipo };
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                                model.ListaMovimientos = ListMovimiento;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                                model.ListaCruces = null;
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
                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo };                                
                                model.ListaCruces = ListCruces;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                                model.ListaMovimientos = null;
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


                        if (TypeMovimiento == "01")
                        {
                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, DateAyuda, 1, false);

                            if (ListMovimiento.Any())
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo };
                                model.Info = Info;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                                model.ListaMovimientos = ListMovimiento;
                                model.ListaCruces = null;
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
                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo };
                                model.Info = Info;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                                model.ListaCruces = ListCruces;
                                model.ListaMovimientos = null;
                                ListPDFCruces = ListCruces;
                                ClienteAdmin = false;
                                model.Info = Info;
                                return View("Tabla_Historico", model);

                            }
                        }


                    }
                    else
                    {

                        if (TypeMovimiento == "01")
                        {
                            ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, model.Fecha_Fin, 1, false);

                            if (ListMovimiento.Any())
                            {
                                Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Tag, Tipo };
                                model.Info = Info;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                                model.ListaMovimientos = ListMovimiento;
                                model.ListaCruces = null;
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
                                Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Tag, Tipo };
                                model.Info = Info;
                                Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Tag; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                                model.ListaCruces = ListCruces;
                                model.ListaMovimientos = null;
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

                if (model.Fecha_Fin == DateTime.Now.Date)
                {
                    DateTime DateAyuda = DateTime.Now;


                    if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, DateAyuda, 2, true);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Cuenta, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
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
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
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

                    if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, DateAyuda, 2, false);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
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
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
                            ListPDFCruces = ListCruces;
                            ClienteAdmin = false;
                            model.Info = Info;
                            return View("Tabla_Historico", model);

                        }
                    }


                }
                else
                {
                    if (TypeMovimiento == "01")
                    {
                        ListMovimiento = Movimientos(Cuenta, model.Fecha_Inicio, model.Fecha_Fin, 2, true);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, TagCuenta = Cuenta, Tipo };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListMovimiento[0].NomCliente; saldo = ListMovimiento[0].SaldoActual; eventos = ListMovimiento.Count.ToString();
                            model.ListaMovimientos = ListMovimiento;
                            model.ListaCruces = null;
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
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, TagCuenta = Cuenta, Tipo };
                            model.Info = Info;
                            Fecha1 = Fecha_Inicio; Fecha2 = Fecha_Fin; Plaza = ""; cuenta = Cuenta; cliente = ListCruces[0].NomCliente; saldo = ListCruces[0].SaldoActual; eventos = ListCruces.Count.ToString();
                            model.ListaCruces = ListCruces;
                            model.ListaMovimientos = null;
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
                        ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, model.Fecha_Fin, 3, true);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, Tipo};
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
                        ListCruces = Cruces(Tag, model.Fecha_Inicio, model.Fecha_Fin, 3, true);

                        if (ListCruces.Any())
                        {
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, Tipo };
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
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, Tipo };
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
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, Tipo };
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
                        ListMovimiento = Movimientos(Tag, model.Fecha_Inicio, model.Fecha_Fin, 3, false);

                        if (ListMovimiento.Any())
                        {
                            Info = new { ListMovimiento[0].NomCliente, Tag, ListMovimiento[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListMovimiento.Count, ListMovimiento[0].SaldoActual, Tipo };
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
                        ListCruces = Cruces(Tag, model.Fecha_Inicio, model.Fecha_Fin, 3, false);

                        if (ListCruces.Any())
                        {
                            Info = new { ListCruces[0].NomCliente, Tag, ListCruces[0].TypeCuenta, Fecha_Inicio, Fecha_Fin, ListCruces.Count, ListCruces[0].SaldoActual, Tipo };
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
                                             _Plaza = historico.Delegacion,
                                             _Cuerpo = historico.Cuerpo,
                                             _Carril = historico.Carril,
                                             _Fecha = historico.Fecha,
                                             _Clase = historico.Clase,
                                             _SaldoAntes = historico.SaldoAnterior,
                                             _Saldo = historico.Saldo,
                                             _SaldoNuevo = historico.SaldoActualizado,
                                             _Operadora = historico.Operador,
                                             _SaldoActual = tags.SaldoTag

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
                            Plaza = item._Plaza,
                            Fecha = Convert.ToString(item._Fecha),
                            Cuerpo = item._Cuerpo,
                            Carril = item._Carril,
                            Clase = item._Clase,
                            SaldoAntes = "Q" + item._SaldoAntes,
                            Saldo = "Q" + item._Saldo,
                            SaldoDespues = "Q" + item._SaldoNuevo,
                            SaldoActual = "Q" + Convert.ToString(Convert.ToInt32(item._SaldoActual)/100),
                            Operador = item._Operadora
                            

                        });
                        id++;
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
                                             _Plaza = historico.Delegacion,
                                             _Cuerpo = historico.Cuerpo,
                                             _Carril = historico.Carril,                                             
                                             _Fecha = historico.Fecha,
                                             _Clase = historico.Clase,
                                             _SaldoAntes = historico.SaldoAnterior,
                                             _Saldo = historico.Saldo,
                                             _SaldoNuevo = historico.SaldoActualizado,
                                             _Operadora = historico.Operador,
                                             _SaldoActual = tags.SaldoTag
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
                            Plaza = item._Plaza,
                            Fecha = Convert.ToString(item._Fecha),
                            Cuerpo = item._Cuerpo,
                            Carril = item._Carril,
                            Clase = item._Clase,
                            SaldoAntes = "Q" + item._SaldoAntes,
                            Saldo = "Q" + item._Saldo,
                            SaldoDespues = "Q" + item._SaldoNuevo,
                            SaldoActual = "Q" + Convert.ToString(Convert.ToInt32(item._SaldoActual) / 100),
                            Operador = item._Operadora

                        });
                        id++;
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
                                                 _Plaza = historico.Delegacion,
                                                 _Cuerpo = historico.Cuerpo,
                                                 _Carril = historico.Carril,
                                                 _Fecha = historico.Fecha,
                                                 _Clase = historico.Clase,
                                                 _SaldoAntes = historico.SaldoAnterior,
                                                 _Saldo = historico.Saldo,
                                                 _SaldoNuevo = historico.SaldoActualizado,
                                                 _Operadora = historico.Operador,
                                                 _SaldoActual = cuentas.SaldoCuenta

                                             }).ToList();



                        foreach (var item2 in ListaCompleta)
                        {
                            List.Add(new Cruces
                            {
                                Id = id, 
                                NomCliente = item2._Nombre,
                                TypeCuenta = item2._TypeCuenta,
                                Tag = item._Tag,
                                Plaza = item2._Plaza,
                                Fecha = Convert.ToString(item2._Fecha),
                                Cuerpo = item2._Cuerpo,
                                Carril = item2._Carril,
                                Clase = item2._Clase,
                                SaldoAntes = "Q" + item2._SaldoAntes,
                                Saldo = "Q" + item2._Saldo,
                                SaldoDespues = "Q" + item2._SaldoNuevo,
                                SaldoActual = "Q" + Convert.ToString(Convert.ToInt32(item2._SaldoActual)/100),
                                Operador = item2._Operadora

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

                    int id = 1;

                    foreach (var item in QueTipo)
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
                                                 _Plaza = historico.Delegacion,
                                                 _Cuerpo = historico.Cuerpo,
                                                 _Carril = historico.Carril,
                                                 _Fecha = historico.Fecha,
                                                 _Clase = historico.Clase,
                                                 _SaldoAntes = historico.SaldoAnterior,
                                                 _Saldo = historico.Saldo,
                                                 _SaldoNuevo = historico.SaldoActualizado,
                                                 _Operadora = historico.Operador,
                                                 _SaldoActual = cuentas.SaldoCuenta


                                             }).ToList();

                        

                        foreach (var item2 in ListaCompleta)
                        {
                            List.Add(new Cruces
                            {
                                Id = id,
                                NomCliente = item2._Nombre,
                                TypeCuenta = item2._TypeCuenta,
                                Tag = item._Tag,
                                Plaza = item2._Plaza,
                                Fecha = Convert.ToString(item2._Fecha),
                                Cuerpo = item2._Cuerpo,
                                Carril = item2._Carril,
                                Clase = item2._Clase,
                                SaldoAntes = "Q" + item2._SaldoAntes,
                                Saldo = "Q" + item2._Saldo,
                                SaldoDespues = "Q" + item2._SaldoNuevo,
                                SaldoActual = "Q" + Convert.ToString(Convert.ToInt32(item2._SaldoActual)/100),
                                Operador = item2._Operadora

                            });
                            id++;
                        }
                    }

                    return List;
                }
            }
            else if (Tipo == (int)DecisionesMetodos.RangoFecha)
            {
                if (Rango)
                {
                    var ListaCompleta = (from historico in db.Historicos
                                         join tags in db.Tags on historico.Tag equals tags.NumTag
                                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (historico.Fecha >= Fecha_Inicio && historico.Fecha <= Fecha_Fin)                                         
                                         orderby (historico.Fecha) descending
                                         select new
                                         {
                                             _Tag = tags.NumTag,
                                             _CuentaID = tags.CuentaId,
                                             _ClienteID = cliente.NumCliente,                                              
                                             _Nombre = cliente.Nombre + cliente.Apellidos,
                                             _TypeCuenta = cuentas.TypeCuenta,
                                             _Plaza = historico.Delegacion,
                                             _Cuerpo = historico.Cuerpo,
                                             _Carril = historico.Carril,
                                             _Fecha = historico.Fecha,
                                             _Clase = historico.Clase,
                                             _SaldoAntes = historico.SaldoAnterior,
                                             _Saldo = historico.Saldo,
                                             _SaldoNuevo = historico.SaldoActualizado,
                                             _Operadora = historico.Operador,
                                             _SaldoActual = cuentas.SaldoCuenta

                                         }).ToList();

                    List<Cruces> List = new List<Cruces>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Cruces
                        {
                            Id = id,
                            Tag = item._Tag,
                            NumCliente = item._ClienteID,
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            Plaza = item._Plaza,
                            Fecha = Convert.ToString(item._Fecha),
                            Cuerpo = item._Cuerpo,
                            Carril = item._Carril,
                            Clase = item._Clase,
                            SaldoAntes = "Q" + item._SaldoAntes,
                            Saldo = "Q" + item._Saldo,
                            SaldoDespues = "Q" + item._SaldoNuevo,
                            SaldoActual = "Q" + item._SaldoActual,
                            Operador = item._Operadora

                        });
                        id++;
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
                                         orderby (historico.Fecha) descending
                                         select new
                                         {
                                             _Tag = historico.Tag,
                                             _CuentaID = tags.CuentaId,
                                             _ClienteID = cliente.NumCliente,
                                             _Nombre = cliente.Nombre + cliente.Apellidos,
                                             _TypeCuenta = cuentas.TypeCuenta,
                                             _Plaza = historico.Delegacion,
                                             _Cuerpo = historico.Cuerpo,
                                             _Carril = historico.Carril,
                                             _Fecha = historico.Fecha,
                                             _Clase = historico.Clase,
                                             _SaldoAntes = historico.SaldoAnterior,
                                             _Saldo = historico.Saldo,
                                             _SaldoNuevo = historico.SaldoActualizado,
                                             _Operadora = historico.Operador,
                                             _SaldoActual = cuentas.SaldoCuenta


                                         }).ToList();

                    List<Cruces> List = new List<Cruces>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Cruces
                        {
                            Id= id,
                            Tag = item._Tag,
                            NumCliente = item._ClienteID,
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            Plaza = item._Plaza,
                            Fecha = Convert.ToString(item._Fecha),
                            Cuerpo = item._Cuerpo,
                            Carril = item._Carril,
                            Clase = item._Clase,
                            SaldoAntes = "Q" + item._SaldoAntes,
                            Saldo = "Q" + item._Saldo,
                            SaldoDespues = "Q" + item._SaldoNuevo,
                            SaldoActual = "Q" + item._SaldoActual,
                            Operador = item._Operadora

                        });
                        id++;
                    }

                    return List;
                }

            }
            return null;
        }
        
        public List<Movimientos> Movimientos(string TagCuenta, DateTime Fecha_Inicio, DateTime Fecha_Fin, int Tipo, bool Rango)
        {
            AppDbContext db = new AppDbContext();

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
                                         where (operaciones.Concepto == "TAG RECARGA")
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

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = Convert.ToString(item._Monto),
                            Fecha = Convert.ToString(item._Fecha),
                            Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = "Q" + Convert.ToString(item._CobroTag),
                            Referencia = item._Referencia,
                            SaldoActual = "Q" + item._SaldoActual


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
                                         where (operaciones.Concepto == "TAG RECARGA")
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

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = Convert.ToString(item._Monto),
                            Fecha = Convert.ToString(item._Fecha),
                            Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = "Q" + Convert.ToString(item._CobroTag),
                            Referencia = item._Referencia,


                        });
                        id++;
                    }

                    return List;
                }
            }
            else if(Tipo == (int)DecisionesMetodos.Cuenta)
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
                                         where (operaciones.Concepto == "CUENTA RECARGA")
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
                                             //_SaldoActual = tags.SaldoTag,
                                             _TipoCuenta = operaciones.Tipo,
                                             _CobroTag = operaciones.CobroTag,                                             
                                             _Referencia = operaciones.NoReferencia,
                                             _TagCuenta = operaciones.Numero

                                         }).ToList();

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = Convert.ToString(item._Monto),
                            Fecha = Convert.ToString(item._Fecha),
                            //Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            //Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = "Q" + Convert.ToString(item._CobroTag),
                            Referencia = item._Referencia,
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
                                         where (operaciones.Concepto == "CUENTA RECARGA")
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
                                             //_SaldoActual = tags.SaldoTag,
                                             _TipoCuenta = operaciones.Tipo,
                                             _CobroTag = operaciones.CobroTag,
                                             _Referencia = operaciones.NoReferencia,
                                             _TagCuenta = operaciones.Numero

                                         }).ToList();

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = Convert.ToString(item._Monto),
                            Fecha = Convert.ToString(item._Fecha),
                            //Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            //Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = "Q" + Convert.ToString(item._CobroTag),
                            Referencia = item._Referencia,
                            //SaldoActual = "Q" + item._SaldoActual

                        });
                        id++;

                    }

                    return List;
                }
            }
            else if(Tipo == (int)DecisionesMetodos.RangoFecha)
            {
                if (Rango)
                {


                    //var ListaCompleta = db.OperacionesCajeros.Where(x => x.DateTOperacion >= Fecha_Inicio && x.DateTOperacion <= Fecha_Fin).OrderByDescending(x => x.DateTOperacion).ToList();

                    var ListaCompleta = (from operaciones in db.OperacionesCajeros
                                         join tags in db.Tags on operaciones.Numero equals tags.NumTag
                                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion <= Fecha_Fin)                                         
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

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = "Q" + Convert.ToString(item._Monto),
                            Fecha = Convert.ToString(item._Fecha),
                            Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = "Q" + Convert.ToString(item._CobroTag),
                            Referencia = item._Referencia,


                        });
                        id++;
                    }

                    return List;
                }
                else
                {
                    //var ListaCompleta = db.OperacionesCajeros.Where(x => x.DateTOperacion >= Fecha_Inicio && x.DateTOperacion < Fecha_Fin).OrderByDescending(x => x.DateTOperacion).ToList();
                    var ListaCompleta = (from operaciones in db.OperacionesCajeros
                                         join tags in db.Tags on operaciones.Numero equals tags.NumTag
                                         join cuentas in db.CuentasTelepeajes on tags.CuentaId equals cuentas.Id
                                         join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                         where (operaciones.DateTOperacion >= Fecha_Inicio && operaciones.DateTOperacion < Fecha_Fin)                                                                                  
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

                    List<Movimientos> List = new List<Movimientos>();

                    int id = 1;

                    foreach (var item in ListaCompleta)
                    {
                        List.Add(new Movimientos
                        {
                            Id = id,
                            Concepto = item._Concepto,
                            TipoPago = item._TipoPago,
                            Monto = "Q" + Convert.ToString(item._Monto),
                            Fecha = Convert.ToString(item._Fecha),
                            Tag = item._Tag,
                            TagCuenta = item._TagCuenta,
                            Cuenta = Convert.ToString(item._CuentaID),
                            NomCliente = item._Nombre,
                            TypeCuenta = item._TypeCuenta,
                            CobroTag = "Q" + Convert.ToString(item._CobroTag),
                            Referencia = item._Referencia,


                        });
                        id++;
                    }

                    return List;
                }
            }

            return null;
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
            
            MemoryStream ms = new MemoryStream();
            Document PdfHistorico = new Document(iTextSharp.text.PageSize.A4.Rotate());
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

                    Paragraph fecha = new Paragraph("Fecha: " + Fecha1 + "al" +Fecha2+ "", new Font(Font.FontFamily.HELVETICA, 12));
                    fecha.Alignment = Element.PTABLE;
                    PdfHistorico.Add(fecha);

                    Paragraph Event = new Paragraph("Movimientos: " + eventos + "", new Font(Font.FontFamily.HELVETICA, 12));
                    Event.Alignment = Element.PTABLE;
                    PdfHistorico.Add(Event);

                    Paragraph saldo_ = new Paragraph("Saldo: " + saldo + "", new Font(Font.FontFamily.HELVETICA, 12));
                    saldo_.Alignment = Element.PTABLE;
                    PdfHistorico.Add(saldo_);



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


                    Paragraph Event = new Paragraph("Movimientos: "+ eventos + "", new Font(Font.FontFamily.HELVETICA, 12));
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
                if(ClienteAdmin)
                {
                    PdfPTable table = new PdfPTable(9);
                    table.WidthPercentage = 100f;
                    var coldWidthPorcentagesCliente = new[] { 2f, 2f, 2f, 1f, 1f, 1f, 2f, 2f, 2f };
                    table.SetWidths(coldWidthPorcentagesCliente);

                    PdfPCell _cellIni = new PdfPCell();
                    PdfHistorico.GetLeft(40f);
                    PdfHistorico.GetRight(40f);




                    _cellIni = new PdfPCell(new Paragraph("Numero de Tag"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
                    table.AddCell(_cellIni);


                    _cellIni = new PdfPCell(new Paragraph("Numero de Cliente"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(_cellIni);



                    //_cellIni = new PdfPCell(new Paragraph("Plaza"));
                    //_cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    //_cellIni.FixedHeight = 10f;
                    //table.AddCell(_cellIni);



                    _cellIni = new PdfPCell(new Paragraph("Fecha"));
                    _cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cellIni.FixedHeight = 10f;
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


                    //_cellIni = new PdfPCell(new Paragraph("Operadora"));
                    //_cellIni.HorizontalAlignment = Element.ALIGN_CENTER;
                    //_cellIni.FixedHeight = 10f;
                    //table.AddCell(_cellIni);




                    foreach (var item in ListPDFCruces)
                    {
                        PdfPCell _cell = new PdfPCell();
                        PdfHistorico.GetLeft(40f);
                        PdfHistorico.GetRight(40f);



                        _cell = new PdfPCell(new Paragraph(item.Tag.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.NumCliente.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(_cell);



                        //_cell = new PdfPCell(new Paragraph(item.Plaza.ToString()));
                        //_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cell.FixedHeight = 10f;
                        //table.AddCell(_cell);



                        _cell = new PdfPCell(new Paragraph(item.Fecha.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.Cuerpo.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.Carril.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.Clase.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.SaldoAntes.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        _cell = new PdfPCell(new Paragraph(item.Saldo.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);

                        _cell = new PdfPCell(new Paragraph(item.SaldoDespues.ToString()));
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.FixedHeight = 10f;
                        table.AddCell(_cell);


                        //_cell = new PdfPCell(new Paragraph(item.Operador.ToString()));
                        //_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        //_cell.FixedHeight = 10f;
                        //table.AddCell(_cell);




                    }
                    PdfHistorico.Add(table);

                }
                //Reporte Cliente
                else
                {
                    PdfPTable table = new PdfPTable(6);
                    table.WidthPercentage = 100f;
                    var coldWidthPorcentagesCliente = new[] { 2f, 3f, 3f, 1f, 1f, 1f };
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
                    var coldWidthPorcentagesCliente = new[] { 2f, 1f, 1f, 2f, 1f, 1f, 1f,  1f };
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


                        _cell = new PdfPCell(new Paragraph(item.TipoPago));
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

    }
}
