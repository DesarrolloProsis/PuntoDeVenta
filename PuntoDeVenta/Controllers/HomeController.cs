using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using PuntoDeVenta.Models;
using PuntoDeVenta.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVenta.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationDbContext app = new ApplicationDbContext();

        [Authorize(Roles = "Cajero, SuperUsuario")]
        public async Task<ActionResult> Index(string verfiAction)
        {
            var model = new CortesCajero();
            var UserId = User.Identity.GetUserId();

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    switch (verfiAction)
                    {
                        // EN ESTE CASE GENERAMOS UN NUEVO CORTE PERO CON VALIDACIONES (SI HAY PENDIENTES; SI YA GENERARON UN CORTE)
                        case "NewLogin":
                            var digitoscorte = string.Empty;
                            var numcorte = string.Empty;

                            var date = DateTime.Now.ToString("yyMMdd");

                            var query = await db.CortesCajeros.Where(x => x.NumCorte.Substring(0, 6) == date).OrderByDescending(x => x.DateTApertura).ToListAsync();

                            if (query.Count > 0)
                            {
                                var todaycorte = DateTime.Today.AddDays(-5);
                                var lastCorteUser = await db.CortesCajeros
                                                           .Where(x => x.IdCajero == UserId && DbFunctions.TruncateTime(x.DateTApertura) >= todaycorte)
                                                           .OrderByDescending(x => x.DateTApertura).ToListAsync();

                                if (lastCorteUser.Count > 0)
                                {
                                    for (int i = 0; i < lastCorteUser.Count; i++)
                                    {
                                        if (lastCorteUser[i].DateTCierre == null && lastCorteUser[i].Comentario == null)
                                            return RedirectToAction("LogOff", "Account", routeValues: new { id = lastCorteUser[i].Id });
                                    }
                                }

                                digitoscorte = query.FirstOrDefault().NumCorte.Substring(6, 3);
                                numcorte = DateTime.Now.ToString("yyMMdd") + (int.Parse(digitoscorte) + 1).ToString("D3");

                                var verificar = await db.CortesCajeros.Where(x => x.NumCorte == numcorte).ToListAsync();

                                if (verificar.Count == 0)
                                {
                                    var corte = new CortesCajero
                                    {
                                        NumCorte = numcorte,
                                        DateTApertura = DateTime.Now,
                                        IdCajero = User.Identity.GetUserId()
                                    };

                                    db.CortesCajeros.Add(corte);
                                    await db.SaveChangesAsync();
                                }
                                else
                                {

                                    while (verificar.Count > 0)
                                    {
                                        digitoscorte = query.FirstOrDefault().NumCorte.Substring(6, 3);
                                        numcorte = DateTime.Now.ToString("yyMMdd") + (int.Parse(digitoscorte) + 1).ToString("D3");

                                        verificar = await db.CortesCajeros.Where(x => x.NumCorte == numcorte).ToListAsync();
                                    }

                                    var corte = new CortesCajero
                                    {
                                        NumCorte = numcorte,
                                        DateTApertura = DateTime.Now,
                                        IdCajero = User.Identity.GetUserId()
                                    };

                                    db.CortesCajeros.Add(corte);
                                    await db.SaveChangesAsync();
                                }
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                // SI NO HAY CORTES GENERADOS EL DIA DE HOY, GENERAMOS EL PRIMERO
                                // Y VALIDAMOS SI EL CAJERO TIENE CORTES PENDIENTES DE 5 DIAS ANTES
                                var todaycorte = DateTime.Today.AddDays(-5);
                                var lastCorteUser = await db.CortesCajeros
                                                           .Where(x => x.IdCajero == UserId && DbFunctions.TruncateTime(x.DateTApertura) >= todaycorte)
                                                           .OrderByDescending(x => x.DateTApertura).ToListAsync();

                                if (lastCorteUser.Count > 0)
                                {
                                    for (int i = 0; i < lastCorteUser.Count; i++)
                                    {
                                        if (lastCorteUser[i].DateTCierre == null && lastCorteUser[i].Comentario == null)
                                            return RedirectToAction("LogOff", "Account", routeValues: new { id = lastCorteUser[i].Id });
                                    }
                                }

                                var corte = new CortesCajero
                                {
                                    NumCorte = date + "001",
                                    DateTApertura = DateTime.Now,
                                    IdCajero = User.Identity.GetUserId()
                                };

                                db.CortesCajeros.Add(corte);
                                await db.SaveChangesAsync();

                                return RedirectToAction("Index");
                            }
                        default:
                            //if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                            //    return RedirectToAction("LogOff", "Account");
                            break;
                    }
                }

                var today = DateTime.Today;
                var CorteUser = await db.CortesCajeros
                                    .Where(x => x.IdCajero == UserId && DbFunctions.TruncateTime(x.DateTApertura) == today)
                                    .OrderByDescending(x => x.DateTApertura).FirstOrDefaultAsync();

                model.NumCorte = CorteUser.NumCorte;
                model.Id = CorteUser.Id;
                ViewBag.Corte = CorteUser.NumCorte;
                ViewBag.FechaInicio = CorteUser.DateTApertura;

                if (TempData.ContainsKey("SCreate"))
                    ViewBag.Success = TempData["SCreate"].ToString();
                else if (TempData.ContainsKey("SEdit"))
                    ViewBag.Success = TempData["SEdit"].ToString();
                else if (TempData.ContainsKey("SDelete"))
                    ViewBag.Success = TempData["SDelete"].ToString();
                else
                    ViewBag.Success = null;

                if (TempData.ContainsKey("ECreate"))
                    ViewBag.Error = TempData["ECreate"].ToString();
                else if (TempData.ContainsKey("EEdit"))
                    ViewBag.Error = TempData["EEdit"].ToString();
                else if (TempData.ContainsKey("EDelete"))
                    ViewBag.Error = TempData["EDelete"].ToString();
                else
                    ViewBag.Error = null;


                /*** Models para recargas ***/

                ViewBag.ModelCuenta = new CuentasTelepeaje();
                ViewBag.ModelTag = new Tags();
                ViewBag.NombreUsuario = User.Identity.Name;
                ViewBag.Cajero = User.Identity.Name;

                //var listAmount = new List<SelectListItem>();

                //db.AmountConfigurations.Where(x => x.Concept == "RECARGAS").ToListAsync().Result.ForEach(x => listAmount.Add(new SelectListItem { Value = x.Amount.ToString("F2"), Text = x.Amount.ToString("F2") }));

                //ViewBag.Amounts = listAmount;
                //ViewBag.Amounts = new SelectList(db.AmountConfigurations.Where(x => x.Concept == "RECARGAS").AsEnumerable(), "Amount", "Amount");
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GetInfo(string Numero, string Type)
        {
            var model = new List<object>();

            if (Numero == null || Numero == string.Empty)
            {
                model = new List<object>();
                return Json(model, JsonRequestBehavior.AllowGet);
            }

            if (model.Any() || model.Count > 0)
                model = new List<object>();

            try
            {
                switch (Type)
                {
                    case "Cuenta":
                        var listCuentas = await (from cuentas in db.CuentasTelepeajes
                                                 join cliente in db.Clientes on cuentas.ClienteId equals cliente.Id
                                                 where cuentas.NumCuenta.Contains(Numero)
                                                 select new
                                                 {
                                                     cliente.Nombre,
                                                     cliente.Apellidos,
                                                     cuentas.NumCuenta,
                                                     cuentas.SaldoCuenta,
                                                     cuentas.TypeCuenta,
                                                     cuentas.DateTCuenta,
                                                     cuentas.StatusCuenta,
                                                     tags = (from t in cuentas.Tags
                                                             select new { t.NumTag, t.SaldoTag, t.StatusTag }).ToList()
                                                 }).ToListAsync();


                        listCuentas.ForEach(x =>
                        {
                            var listTags = new List<object>();
                            x.tags.ForEach(m =>
                            {
                                listTags.Add(new
                                {
                                    NumTag = m.NumTag,
                                    SaldoTag = string.IsNullOrEmpty(m.SaldoTag) ? "0.00" : (double.Parse(m.SaldoTag) / 100).ToString("F2"),
                                    StatusTag = m.StatusTag == true ? "Válido" : "Inválido"
                                });
                            });

                            model.Add(new
                            {
                                NombreCompleto = $"{x.Nombre} {x.Apellidos}",
                                NumCuenta = x.NumCuenta,
                                SaldoCuenta = string.IsNullOrEmpty(x.SaldoCuenta) ? "0.00" : (double.Parse(x.SaldoCuenta) / 100).ToString("F2"),
                                TypeCuenta = x.TypeCuenta,
                                DateTCuenta = x.DateTCuenta,
                                StatusCuenta = x.StatusCuenta == true ? "Válido" : "Inválido",
                                Tags = listTags
                            });
                        });
                        break;
                    case "Tag":
                        var listTag = await (from tag in db.Tags
                                             join cuenta in db.CuentasTelepeajes on tag.CuentaId equals cuenta.Id
                                             join cliente in db.Clientes on cuenta.ClienteId equals cliente.Id
                                             where tag.NumTag.Contains(Numero)
                                             select new
                                             {
                                                 cliente.Nombre,
                                                 cliente.Apellidos,
                                                 cuenta.NumCuenta,
                                                 cuenta.TypeCuenta,
                                                 tag.NumTag,
                                                 tag.SaldoTag,
                                                 tag.StatusTag,
                                             }).ToListAsync();

                        listTag.ForEach(x =>
                        {
                            model.Add(new
                            {
                                NombreCompleto = $"{x.Nombre} {x.Apellidos}",
                                NumCuenta = x.NumCuenta,
                                TypeCuenta = x.TypeCuenta,
                                NumTag = x.NumTag,
                                SaldoTag = string.IsNullOrEmpty(x.SaldoTag) ? "0.00" : (double.Parse(x.SaldoTag) / 100).ToString("F2"),
                                StatusTag = x.StatusTag == true ? "Válido" : "Inválido",
                            });
                        });
                        break;
                    default:
                        model = new List<object>();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            Numero = string.Empty;
            Type = string.Empty;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "GenerarReporte, Cajero, SuperUsuario, JefeTurno")]
        public async Task<ActionResult> ListaNegraIndex()
        {
            var x = await db.ListaNegras.ToListAsync();

            var data = new List<object>();

            x.ForEach(listanegra =>
            {
                data.Add(new
                {
                    listanegra.Id,
                    listanegra.Tipo,
                    listanegra.Numero,
                    listanegra.Observacion,
                    SaldoAnterior = listanegra.SaldoAnterior.HasValue == true ? listanegra.SaldoAnterior.Value.ToString("F2") : "",
                    listanegra.Date,
                    listanegra.NumCuenta,
                    listanegra.NumCliente,
                    listanegra.Clase
                });
            });

            dynamic model = new ExpandoObject();
            List<ExpandoObject> joinData = new List<ExpandoObject>();

            foreach (var item in data)
            {
                IDictionary<string, object> itemExpando = new ExpandoObject();
                foreach (PropertyDescriptor property
                         in
                         TypeDescriptor.GetProperties(item.GetType()))
                {
                    itemExpando.Add(property.Name, property.GetValue(item));
                }
                joinData.Add(itemExpando as ExpandoObject);
            }

            model.JoinData = joinData;

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            using (ApplicationDbContext app = new ApplicationDbContext())
            {
                var idUser = User.Identity.GetUserId();
                var _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(app));
                var _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(app));

                var result = _roleManager.Create(new IdentityRole("JefeTurno"));
                //var result = _roleManager.Create(new IdentityRole("Cajero"));
                //result = _roleManager.Create(new IdentityRole("GenerarReporte"));
                //var user = _UserManager.AddToRole(idUser, "SuperUsuario");
                //var userRole = _UserManager.IsInRole(idUser, "Cajero");

                //userRole = _UserManager.IsInRole(idUser, "SuperUsuario");
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "GenerarReporte, Cajero, SuperUsuario")]
        public ActionResult GenerarReportes()
        {
            var model = new GenerarReportesViewModel();
            model.PropertiesList = new List<Properties>();
            model.EncabezadoReporteCajero = new EncabezadoReporteCajero();
            model.Date = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GenerarReporte, Cajero, SuperUsuario")]
        public async Task<ActionResult> GenerarReportes(GenerarReportesViewModel model)
        {
            try
            {
                var date = model.Date;
                var prop = new List<Properties>();
                var result = db.CortesCajeros.Where(x => DbFunctions.TruncateTime(x.DateTApertura) == date && x.DateTCierre != null).ToList();
                ///FIRST OR DEFAULT NUM AUTORI PROVEEDOR ARREGLAR CUANDO SEAN MAS BANCOS U OPERACIONES DE EXTERNOS
                var rows_serbipagos = db.OperacionesSerBIpagos.Where(x => DbFunctions.TruncateTime(x.DateTOpSerBI) == date).ToList();

                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        using (ApplicationDbContext app = new ApplicationDbContext())
                        {
                            // Cuando agreguemos el username cambiamos en el obj nomcajero a UserName del UserManager
                            var _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(app));
                            var user = await _UserManager.FindByIdAsync(item.IdCajero);
                            prop.Add(new Properties
                            {
                                Id = item.Id,
                                NumCorte = item.NumCorte,
                                NomCajero = user.Email,
                                DateInicio = item.DateTApertura,
                                DateFin = item.DateTCierre.Value,
                                Type = "PV"
                            });
                        }
                    }

                }

                ///FIRST OR DEFAULT NUM AUTORI PROVEEDOR ARREGLAR CUANDO SEAN MAS BANCOS U OPERACIONES DE EXTERNOS
                if (rows_serbipagos.Any())
                {
                    prop.Add(new Properties
                    {
                        Id = null,
                        NumCorte = "-",
                        NomCajero = rows_serbipagos.FirstOrDefault().NumAutoriProveedor,
                        DateInicio = date,
                        DateFin = date.AddHours(23).AddMinutes(59).AddSeconds(59),
                        Type = "EL"
                    });
                }

                model.PropertiesList = prop;

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // POST: ReporteCajero
        [AllowAnonymous]
        public async Task<ActionResult> ReporteCajero(long? id, string type, string date)
        {
            try
            {
                var nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                var model = new List<object>();
                double recargas = 0.0d;

                switch (type)
                {
                    case "PV":
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                        var result = await db.CortesCajeros.FindAsync(id);

                        if (result == null)
                        {
                            return HttpNotFound();
                        }

                        // Cuando agreguemos el username cambiamos en el obj nomcajero a UserName del UserManager
                        var _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(app));
                        var user = await _UserManager.FindByIdAsync(result.IdCajero);

                        var encabezado = new EncabezadoReporteCajero
                        {
                            Cajero = user.Email,
                            NumCorte = result.NumCorte,
                            HoraI = result.DateTApertura.ToString("dd/MM/yyyy HH:mm:ss"),
                            HoraF = result.DateTCierre.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                            TotalMonto = result.MontoTotal.Value.ToString("#,##0.00", nfi),
                            Comentario = result.Comentario,
                        };

                        double ventatags = 0.0d;

                        var movimientos = await db.OperacionesCajeros.Where(x => x.CorteId == result.Id).ToListAsync();

                        foreach (var item in movimientos)
                        {
                            recargas += item.Monto ?? 0;
                            ventatags += item.CobroTag ?? 0;

                            switch (item.Tipo)
                            {
                                case "TAG":
                                    var foundclientetag = await (from cliente in db.Clientes
                                                                 join cuentas in db.CuentasTelepeajes on cliente.Id equals cuentas.ClienteId
                                                                 join tags in db.Tags on cuentas.Id equals tags.CuentaId
                                                                 where tags.NumTag == item.Numero
                                                                 select new
                                                                 {
                                                                     cliente.NumCliente,
                                                                     cuentas.NumCuenta,
                                                                     tags.NumTag
                                                                 }
                                                            ).FirstOrDefaultAsync();

                                    if (foundclientetag != null)
                                    {
                                        model.Add(new
                                        {
                                            Concepto = item.Concepto,
                                            TipoPago = item.TipoPago,
                                            Monto = item.Monto.HasValue ? item.Monto.Value.ToString("#,##0.00", nfi) : null,
                                            DataTOperacion = item.DateTOperacion.ToString(),
                                            CobroTag = item.CobroTag,
                                            NumeroAdicional = "-",
                                            NumCliente = foundclientetag.NumCliente,
                                            NumCuenta = foundclientetag.NumCuenta,
                                            NumTag = foundclientetag.NumTag,
                                            Unidad = item.Concepto == "TAG ACTIVADO" || item.Concepto == "TAG TRASPASO" ? "1" : "-",
                                            NoReferencia = item.NoReferencia,
                                        });
                                    }
                                    else
                                    {
                                        // LISTA NEGRA
                                        var foundblacklist = await (from list in db.ListaNegras
                                                                    where list.Numero == item.Numero
                                                                    select list).FirstOrDefaultAsync();

                                        if (foundblacklist != null)
                                        {
                                            model.Add(new
                                            {
                                                Concepto = item.Concepto,
                                                TipoPago = item.TipoPago,
                                                Monto = item.Monto.HasValue ? item.Monto.Value.ToString("#,##0.00", nfi) : null,
                                                DataTOperacion = item.DateTOperacion.ToString(),
                                                CobroTag = item.CobroTag,
                                                NumeroAdicional = "-",
                                                NumCliente = foundblacklist.NumCliente,
                                                NumCuenta = foundblacklist.NumCuenta,
                                                NumTag = foundblacklist.Numero,
                                                Unidad = "-",
                                                NoReferencia = item.NoReferencia,
                                            });
                                        }
                                        else
                                        {
                                            model.Add(new
                                            {
                                                Concepto = item.Concepto,
                                                TipoPago = item.TipoPago,
                                                Monto = item.Monto.HasValue ? item.Monto.Value.ToString("#,##0.00", nfi) : null,
                                                DataTOperacion = item.DateTOperacion.ToString(),
                                                CobroTag = item.CobroTag,
                                                NumeroAdicional = "-",
                                                NumCliente = "-",
                                                NumCuenta = "-",
                                                NumTag = item.Numero,
                                                Unidad = item.Concepto == "TAG ACTIVADO" || item.Concepto == "TAG TRASPASO" ? "1" : "-",
                                                NoReferencia = item.NoReferencia,
                                            });
                                        }
                                    }

                                    break;
                                case "CUENTA":
                                    var foundclientecuen = await (from cliente in db.Clientes
                                                                  join cuentas in db.CuentasTelepeajes on cliente.Id equals cuentas.ClienteId
                                                                  where cuentas.NumCuenta == item.Numero
                                                                  select new
                                                                  {
                                                                      cliente.NumCliente,
                                                                      cuentas.NumCuenta,
                                                                  }
                                                            ).FirstOrDefaultAsync();

                                    if (foundclientecuen != null)
                                    {
                                        model.Add(new
                                        {
                                            Concepto = item.Concepto,
                                            TipoPago = item.TipoPago,
                                            Monto = item.Monto.HasValue ? item.Monto.Value.ToString("#,##0.00", nfi) : null,
                                            DataTOperacion = item.DateTOperacion.ToString(),
                                            CobroTag = item.CobroTag,
                                            NumeroAdicional = "-",
                                            NumCliente = foundclientecuen.NumCliente,
                                            NumCuenta = foundclientecuen.NumCuenta,
                                            NumTag = "-",
                                            Unidad = "-",
                                            NoReferencia = item.NoReferencia,
                                        });
                                    }
                                    else
                                    {
                                        model.Add(new
                                        {
                                            Concepto = item.Concepto,
                                            TipoPago = item.TipoPago,
                                            Monto = item.Monto.HasValue ? item.Monto.Value.ToString("#,##0.00", nfi) : null,
                                            DataTOperacion = item.DateTOperacion.ToString(),
                                            CobroTag = item.CobroTag,
                                            NumeroAdicional = "-",
                                            NumCliente = "-",
                                            NumCuenta = item.Numero,
                                            NumTag = "-",
                                            Unidad = "-",
                                            NoReferencia = item.NoReferencia,
                                        });
                                    }
                                    break;
                                default:
                                    model.Add(new
                                    {
                                        Concepto = item.Concepto,
                                        TipoPago = item.TipoPago,
                                        Monto = item.Monto.HasValue ? item.Monto.Value.ToString("#,##0.00", nfi) : null,
                                        DataTOperacion = item.DateTOperacion.ToString(),
                                        CobroTag = item.CobroTag,
                                        NumeroAdicional = item.Numero,
                                        NumCliente = "-",
                                        NumCuenta = "-",
                                        NumTag = "-",
                                        Unidad = "-",
                                        NoReferencia = item.NoReferencia,
                                    });
                                    break;
                            }
                        }

                        encabezado.SubtotalRecar = recargas.ToString("#,##0.00", nfi);
                        encabezado.VentaTag = ventatags.ToString("#,##0.00", nfi);

                        using (var client = new HttpClient())
                        {
                            string json = JsonConvert.SerializeObject(model);
                            HttpContent postContent = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(new Uri("http://localhost:56342/api/cajero?authenticationToken=abcxyz"), postContent);
                            var message = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                        }

                        return View("ReportViewerCajero", encabezado);

                    case "EL":
                        ///FIRST OR DEFAULT NUM AUTORI PROVEEDOR ARREGLAR CUANDO SEAN MAS BANCOS U OPERACIONES DE EXTERNOS
                        DateTime fecha_movimientos = DateTime.Parse(date);
                        var movimientos_serbi = await db.OperacionesSerBIpagos.Where(x => DbFunctions.TruncateTime(x.DateTOpSerBI) == fecha_movimientos).ToListAsync();

                        var encabezado_serbi = new EncabezadoReporteCajero
                        {
                            Cajero = movimientos_serbi.FirstOrDefault().NumAutoriProveedor,
                            NumCorte = "-",
                            HoraI = fecha_movimientos.ToString("dd/MM/yyyy HH:mm:ss"),
                            HoraF = fecha_movimientos.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("dd/MM/yyyy HH:mm:ss"),
                            Comentario = "-",
                            VentaTag = "0.00"
                        };

                        foreach (var item in movimientos_serbi)
                        {
                            switch (item.Tipo)
                            {
                                case "CUENTA":
                                    switch (item.Concepto)
                                    {
                                        case "CUENTA PAGAR":
                                            var foundclientecuen = await (from cliente in db.Clientes
                                                                          join cuentas in db.CuentasTelepeajes on cliente.Id equals cuentas.ClienteId
                                                                          where cuentas.NumCuenta == item.Numero
                                                                          select new
                                                                          {
                                                                              cliente.NumCliente,
                                                                              cuentas.NumCuenta,
                                                                          }
                                                           ).FirstOrDefaultAsync();

                                            model.Add(new
                                            {
                                                Concepto = item.Concepto,
                                                TipoPago = "PEM",
                                                Monto = item.SaldoModificar.HasValue ? item.SaldoModificar.Value.ToString("#,##0.00", nfi) : null,
                                                DataTOperacion = item.DateTOpSerBI.ToString(),
                                                CobroTag = "-",
                                                NumeroAdicional = "-",
                                                NumCliente = foundclientecuen.NumCliente,
                                                NumCuenta = foundclientecuen.NumCuenta,
                                                NumTag = "-",
                                                Unidad = "-",
                                                NoReferencia = item.NoReferencia,
                                            });

                                            recargas += item.SaldoModificar ?? 0;
                                            break;
                                        case "CUENTA REVERSAR":
                                            model.Add(new
                                            {
                                                Concepto = item.Concepto,
                                                TipoPago = "PEM",
                                                Monto = item.SaldoModificar.HasValue ? "-" + item.SaldoModificar.Value.ToString("#,##0.00", nfi) : null,
                                                DataTOperacion = item.DateTOpSerBI.ToString(),
                                                CobroTag = "-",
                                                NumeroAdicional = item.Numero,
                                                NumCliente = "-",
                                                NumCuenta = "-",
                                                NumTag = "-",
                                                Unidad = "-",
                                                NoReferencia = item.NoReferencia,
                                            });

                                            recargas -= item.SaldoModificar ?? 0;
                                            break;
                                    }
                                    break;
                                case "TAG":
                                    var foundclientetag = await (from cliente in db.Clientes
                                                                 join cuentas in db.CuentasTelepeajes on cliente.Id equals cuentas.ClienteId
                                                                 join tags in db.Tags on cuentas.Id equals tags.CuentaId
                                                                 where tags.NumTag == item.Numero
                                                                 select new
                                                                 {
                                                                     cliente.NumCliente,
                                                                     cuentas.NumCuenta,
                                                                     tags.NumTag
                                                                 }
                                                           ).FirstOrDefaultAsync();
                                    switch (item.Concepto)
                                    {
                                        case "TAG PAGAR":
                                            if (foundclientetag != null)
                                            {
                                                model.Add(new
                                                {
                                                    Concepto = item.Concepto,
                                                    TipoPago = "PEM",
                                                    Monto = item.SaldoModificar.HasValue ? item.SaldoModificar.Value.ToString("#,##0.00", nfi) : null,
                                                    DataTOperacion = item.DateTOpSerBI.ToString(),
                                                    CobroTag = "-",
                                                    NumeroAdicional = "-",
                                                    NumCliente = foundclientetag.NumCliente,
                                                    NumCuenta = foundclientetag.NumCuenta,
                                                    NumTag = foundclientetag.NumTag,
                                                    Unidad = "-",
                                                    NoReferencia = item.NoReferencia,
                                                });
                                            }
                                            else
                                            {
                                                // LISTA NEGRA
                                                var foundblacklist = await (from list in db.ListaNegras
                                                                            where list.Numero == item.Numero
                                                                            select list).FirstOrDefaultAsync();

                                                if (foundblacklist != null)
                                                {
                                                    model.Add(new
                                                    {
                                                        Concepto = item.Concepto,
                                                        TipoPago = "PEM",
                                                        Monto = item.SaldoModificar.HasValue ? item.SaldoModificar.Value.ToString("#,##0.00", nfi) : null,
                                                        DataTOperacion = item.DateTOpSerBI.ToString(),
                                                        CobroTag = "-",
                                                        NumeroAdicional = "-",
                                                        NumCliente = foundblacklist.NumCliente,
                                                        NumCuenta = foundblacklist.NumCuenta,
                                                        NumTag = foundblacklist.Numero,
                                                        Unidad = "-",
                                                        NoReferencia = item.NoReferencia,
                                                    });
                                                }
                                                else
                                                {
                                                    model.Add(new
                                                    {
                                                        Concepto = item.Concepto,
                                                        TipoPago = "PEM",
                                                        Monto = item.SaldoModificar.HasValue ? item.SaldoModificar.Value.ToString("#,##0.00", nfi) : null,
                                                        DataTOperacion = item.DateTOpSerBI.ToString(),
                                                        CobroTag = "-",
                                                        NumeroAdicional = item.Numero,
                                                        NumCliente = "-",
                                                        NumCuenta = "-",
                                                        NumTag = "-",
                                                        Unidad = "-",
                                                        NoReferencia = item.NoReferencia,
                                                    });
                                                }
                                            }

                                            recargas += item.SaldoModificar ?? 0;
                                            break;
                                        case "TAG REVERSAR":
                                            model.Add(new
                                            {
                                                Concepto = item.Concepto,
                                                TipoPago = "PEM",
                                                Monto = item.SaldoModificar.HasValue ? "-" + item.SaldoModificar.Value.ToString("#,##0.00", nfi) : null,
                                                DataTOperacion = item.DateTOpSerBI.ToString(),
                                                CobroTag = "-",
                                                NumeroAdicional = item.Numero,
                                                NumCliente = "-",
                                                NumCuenta = "-",
                                                NumTag = "-",
                                                Unidad = "-",
                                                NoReferencia = item.NoReferencia,
                                            });

                                            recargas -= item.SaldoModificar ?? 0;
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        //TotalMonto
                        //SubtotalRecar
                        encabezado_serbi.TotalMonto = recargas.ToString("#,##0.00", nfi);
                        encabezado_serbi.SubtotalRecar = recargas.ToString("#,##0.00", nfi);

                        using (var client = new HttpClient())
                        {
                            string json = JsonConvert.SerializeObject(model);
                            HttpContent postContent = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(new Uri("http://localhost:56342/api/cajero?authenticationToken=abcxyz"), postContent);
                            var message = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                        }

                        return View("ReportViewerCajero", encabezado_serbi);
                    default:
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult MovimientoCajero(string corte, long? CorteId)
        {
            try
            {
                long id;
                if (CorteId == null)
                {
                    var AtrCorte = db.CortesCajeros.Where(x => x.NumCorte == corte).ToList(); //A
                    id = AtrCorte[0].Id;
                }
                else
                {
                    id = Convert.ToInt64(CorteId);
                }

                ViewBag.Corte = id.ToString();
                var MovimientoDeCorte = db.OperacionesCajeros.Where(x => x.CorteId == id).ToList().OrderByDescending(x => x.DateTOperacion);
                var ListaOperaciones = new List<OperacionesCajero>();
                foreach (var item in MovimientoDeCorte)
                {
                    ListaOperaciones.Add(NewOperacion(item.Id, item.Concepto, item.TipoPago, item.Monto, item.DateTOperacion, item.CorteId, item.Numero, item.Tipo, item.CobroTag, item.NoReferencia, item.StatusCancelacion));
                }
                return View("MovimientoCajero", ListaOperaciones.AsEnumerable());
            }
            catch (Exception Ex)
            {
                return View("Error");
            }
        }

        /// <summary>
        /// Método para cancelar operaciones
        /// </summary>
        /// <param name="id">Id de la operación a eliminar</param>
        /// <param name="corteid">Corte actual</param>
        /// <param name="Filtro">Concepto que se cancelará</param>
        /// <returns></returns>
        public ActionResult CancelarOperacion(int id, long corteid, string Filtro)
        {
            MethodsGlb method = new MethodsGlb();
            var SelectedOperacion = db.OperacionesCajeros.Where(x => x.Id == id).FirstOrDefault();
            if (SelectedOperacion.StatusCancelacion == false)                       //Se verifica si el movimiento ya fue cancelado
            {
                db.OperacionesCajeros.Add(new OperacionesCajero                     //Se agrega un movimiento para notificar la cancelación de este movimiento
                {
                    Concepto = SelectedOperacion.Concepto.Contains("RECARGA") ? "CANCELACION RECARGA" : "CANCELACION ACTIVACION",
                    TipoPago = "CAN",
                    Monto = 0 - SelectedOperacion.Monto,
                    DateTOperacion = DateTime.Now,
                    CorteId = SelectedOperacion.CorteId,
                    Numero = SelectedOperacion.NoReferencia,
                    Tipo = SelectedOperacion.Tipo + " CAN",
                    CobroTag = 0 - SelectedOperacion.CobroTag,
                    NoReferencia = method.RandomNumReferencia2().ToString(),
                    StatusCancelacion = false
                });
                if (SelectedOperacion.Concepto.Contains("RECARGA"))                     //Si el movimiento fue una RECARGA
                {
                    if (SelectedOperacion.Tipo == "TAG")                                //Si el movimiento cancelado en una recarga de Tag:
                    {
                        var UpdatedTag = db.Tags.Where(x => x.NumTag == SelectedOperacion.Numero).FirstOrDefault();
                        UpdatedTag.SaldoTag = (Convert.ToDouble(UpdatedTag.SaldoTag) - (Convert.ToDouble(SelectedOperacion.Monto) * 100)).ToString();
                        if (Convert.ToDouble(UpdatedTag.SaldoTag) < 1525)               //Si el saldo es menor a 15.25 Quetzales, se convierte en tag inválido
                            UpdatedTag.StatusTag = false;
                    }
                    else //Si el movimiento cancelado era una recarga de Cuenta:
                    {
                        var UpdatedCuenta = db.CuentasTelepeajes.Where(x => x.NumCuenta == SelectedOperacion.Numero).FirstOrDefault();
                        var UpdatedTags = db.Tags.Where(x => x.CuentaId == UpdatedCuenta.Id).ToList();
                        UpdatedCuenta.SaldoCuenta = (Convert.ToDouble(UpdatedCuenta.SaldoCuenta) - (Convert.ToDouble(SelectedOperacion.Monto)) * 100).ToString();
                        if (Convert.ToDouble(UpdatedCuenta.SaldoCuenta) < 5000)         //Si el saldo es menor a 50 Quetzales, se convierte en cuenta invalida
                            UpdatedCuenta.StatusCuenta = false;
                        foreach (var item in UpdatedTags)
                        {
                            item.SaldoTag = (Convert.ToDouble(item.SaldoTag) - (Convert.ToDouble(SelectedOperacion.Monto)) * 100).ToString(); //Se le resta el monto cancelado a cada tag relacionado con la cuenta
                            if (!UpdatedCuenta.StatusCuenta)                            //Si la cuenta es invalida, por ende los Tags relacionados a ella también son inválidos.
                                item.StatusTag = false;
                        }
                    }
                }
                else //Si el movimiento fue una ACTIVACIÓN
                {
                    var OpeAfterActivacion = db.OperacionesCajeros.Where(x => x.Id > SelectedOperacion.Id).ToList();    //Para no evaluar todas las operaciones, descartamos las que hayan sido antes de la activación de la cuenta
                    if (SelectedOperacion.Tipo == "TAG")        //En caso de que se haya cancelado la Activación de un TAG:
                    {
                        var CreatedTag = db.Tags.Where(x => x.NumTag == SelectedOperacion.Numero).FirstOrDefault();     //Se identifica el Tag cancelado
                        int i = 1;
                        foreach (var item in OpeAfterActivacion)    //Se evalúa cada operación después de la activación para cancelar posibles recargas a ese TAG
                        {
                            if (item.Numero == CreatedTag.NumTag)   //Si el Número de la operacaión coíncide con el NumTag del TAG, significa que se realizó una recarga al TAG que se quiere cancelar
                            {
                                db.OperacionesCajeros.Add(new OperacionesCajero     //Se crea un movimiento para notificar que esa recarga ya no será valida debido a la cancelación de la cuenta
                                {
                                    Concepto = "CANCELACION RECARGA",
                                    TipoPago = "CAN",
                                    Monto = 0 - item.Monto,
                                    DateTOperacion = DateTime.Now,
                                    CorteId = SelectedOperacion.CorteId,
                                    Numero = item.NoReferencia,
                                    Tipo = "TAG CAN",
                                    CobroTag = 0 - item.CobroTag,
                                    NoReferencia = string.Format("{0}", (Convert.ToInt32(method.RandomNumReferencia2()) + i).ToString("D7")), //Se le suma 'i' ya que el método 'RandomNumReferencia2()' evalúa el último registro de la BD y en este punto aún no se suben los cambios a la BD
                                    StatusCancelacion = false
                                });
                                item.StatusCancelacion = true;      //Se notifica este movimiento como cancelado
                                i++;
                            }
                        }
                        db.Tags.Remove(CreatedTag);     //Se elimina el Tag cancelado
                    }
                    else    //En caso de que se haya cancelado la Activación de una CUENTA
                    {
                        var CreatedCuenta = db.CuentasTelepeajes.Where(x => x.NumCuenta == SelectedOperacion.Numero).FirstOrDefault();      //Se identifica la Cuenta a cancelar
                        var RelatedTags = db.Tags.Where(x => x.CuentaId == CreatedCuenta.Id).ToList();          //Se identifican los Tags relacionados a la Cuenta a cancelar
                        for (int i = 0; i < RelatedTags.Count; i++)     //Se buscará si hay movimientos con cada Tag relacionado
                        {
                            foreach (var item in OpeAfterActivacion)    //Se buscará en los movimientos después de la activación
                            {
                                if ((RelatedTags[i].NumTag == item.Numero) && (item.StatusCancelacion == false))        //Si el Numero del movimiento coíncide con el del Tag relacionado y además no ha sido cancelado, se generá un movimiento de cancelación
                                {
                                    db.OperacionesCajeros.Add(new OperacionesCajero
                                    {
                                        Concepto = "CANCELACION RECARGA",
                                        TipoPago = "CAN",
                                        Monto = 0 - item.Monto,
                                        DateTOperacion = DateTime.Now,
                                        CorteId = SelectedOperacion.CorteId,
                                        Numero = item.NoReferencia,
                                        Tipo = "TAG CAN",
                                        CobroTag = 0 - item.CobroTag,
                                        NoReferencia = string.Format("{0}", (Convert.ToInt32(method.RandomNumReferencia2()) + (i + 1)).ToString("D7")),
                                        StatusCancelacion = false
                                    });
                                    item.StatusCancelacion = true;      //Se cancelará el movimiento
                                }
                            }
                        }
                        foreach (var item in OpeAfterActivacion)        //Se buscará en los movimientos después de la activación si ha habido recargas a la Cuenta
                        {
                            int i = 1;
                            if (item.Numero == CreatedCuenta.NumCuenta) //Si el número de movimiento corresponde al Número de Cuenta, se agregará un movimiento de cancelación de operación
                            {
                                db.OperacionesCajeros.Add(new OperacionesCajero
                                {
                                    Concepto = "CANCELACION RECARGA",
                                    TipoPago = SelectedOperacion.TipoPago != null ? "CAN" : null,
                                    Monto = 0 - item.Monto,
                                    DateTOperacion = DateTime.Now,
                                    CorteId = SelectedOperacion.CorteId,
                                    Numero = item.NoReferencia,
                                    Tipo = "CUENTA CAN",
                                    CobroTag = null,
                                    NoReferencia = string.Format("{0}", (Convert.ToInt32(method.RandomNumReferencia2()) + (RelatedTags.Count + 1 + i)).ToString("D7")),
                                    StatusCancelacion = false
                                });
                                item.StatusCancelacion = true;      //Cambia el status a cancelado
                            }
                        }
                        db.CuentasTelepeajes.Remove(CreatedCuenta);     //Se elminará la Cuenta, junto con sus Tags
                    }
                }
                SelectedOperacion.StatusCancelacion = true;     //La operación principal se cancela
                db.SaveChanges();
            }
            return RedirectToAction("MovimientoCajero", new { CorteId = corteid, Concepto = Filtro });
        }

        /// <summary>
        /// Método POST para el filtrado de movimientos
        /// </summary>
        /// <param name="Concepto">Parametro por el cual se filtran los movimientos</param>
        /// <param name="CorteId">Parametro por el cual se identifica el corte actual</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MovimientoCajero(string Concepto, long CorteId)
        {
            ViewBag.Filtro = Concepto;
            if (Concepto == "Todas las operaciones" || Concepto == null) //Si el usuario quiere volver a ver el concentrado de todos los movimientos
            {
                var MovimientosdeCorte = db.OperacionesCajeros.Where(x => x.CorteId == CorteId).ToList().OrderByDescending(x => x.DateTOperacion);
                List<OperacionesCajero> model = new List<OperacionesCajero>();
                foreach (var item in MovimientosdeCorte)
                {
                    model.Add(NewOperacion(item.Id, item.Concepto, item.TipoPago, item.Monto, item.DateTOperacion, item.CorteId, item.Numero, item.Tipo, item.CobroTag, item.NoReferencia, item.StatusCancelacion));
                }
                ViewBag.Corte = CorteId.ToString();
                return View("MovimientoCajero", model);
            }
            else //Si hay un filtro:
            {
                var MovimientosdeCorte = db.OperacionesCajeros.Where(x => x.CorteId == CorteId && x.Concepto.Trim() == Concepto.Trim()).ToList().OrderByDescending(x => x.DateTOperacion); //Selecciona los movimientos en donde el concepto y el corte se cumplan
                List<OperacionesCajero> model = new List<OperacionesCajero>();
                foreach (var item in MovimientosdeCorte)
                {
                    model.Add(NewOperacion(item.Id, item.Concepto, item.TipoPago, item.Monto, item.DateTOperacion, item.CorteId, item.Numero, item.Tipo, item.CobroTag, item.NoReferencia, item.StatusCancelacion));
                }
                ViewBag.Corte = CorteId.ToString();
                return View("MovimientoCajero", model);
            }
        }

        public OperacionesCajero NewOperacion(long NewId, string NewConcepto, string NewTipoPago, double? NewMonto, DateTime NewDate, long NewCorteId, string NewNumero, string NewTipo, double? NewCobroTag, string NewNoReferencia, bool NewStatus)
        {
            return new OperacionesCajero
            {
                Id = NewId,
                Concepto = NewConcepto,
                TipoPago = NewTipoPago,
                Monto = NewMonto,
                DateTOperacion = NewDate,
                CorteId = NewCorteId,
                Numero = NewNumero,
                Tipo = NewTipo,
                CobroTag = NewCobroTag,
                NoReferencia = NewNoReferencia,
                StatusCancelacion = NewStatus
            };
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //PRUEBA DE CONFIGURACION VIEW
        [HttpGet]
        public ActionResult Configuracion()
        {
            return View();
        }
        //PRUEBA DE JEFE DE TURN0 VIEW

        [Authorize(Roles = "JefeTurno")]
        [HttpGet]
        public ActionResult Jefedeturno()
        {
            ViewBag.NombreUsuario = User.Identity.Name;
            ViewBag.Cajero = User.Identity.Name;

            return View();
        }

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            var usersWithRoles = (from user in app.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in app.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).AsEnumerable().Select(p => new
                                  {
                                      Id = p.UserId,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });

            return Json(usersWithRoles, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(string uid)
        {
            if (uid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(app);
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);

                //get User Data from Userid
                var user = await UserManager.FindByIdAsync(uid);

                //List Logins associated with user
                var logins = user.Logins;

                //Gets list of Roles associated with current user
                var rolesForUser = await UserManager.GetRolesAsync(uid);

                using (var transaction = app.Database.BeginTransaction())
                {
                    foreach (var login in logins.ToList())
                    {
                        await UserManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    }

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
                        }
                    }

                    //Delete User
                    await UserManager.DeleteAsync(user);

                    transaction.Commit();

                    return Json(new { success = $"Usuario: {user.Email} eliminado correctamente." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, success = "" });
            }
        }
    }
}
