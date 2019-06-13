using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using PuntoDeVenta.Models;
using PuntoDeVenta.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        [Authorize(Roles = "GenerarReporte, Cajero, SuperUsuario")]
        public async Task<ActionResult> ListaNegraIndex()
        {
            return View(await db.ListaNegras.ToListAsync());
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

                //var result = _roleManager.Create(new IdentityRole("SuperUsuario"));
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
                                DateFin = item.DateTCierre.Value
                            });
                        }
                    }

                    model.PropertiesList = prop;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // POST: ReporteCajero
        [AllowAnonymous]
        public async Task<ActionResult> ReporteCajero(long? id)
        {
            try
            {
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

                var nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };

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
                double recargas = 0.0d;

                var movimientos = await db.OperacionesCajeros.Where(x => x.CorteId == result.Id).ToListAsync();

                var model = new List<object>();

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
                    var response = await client.PostAsync(new Uri("http://10.1.10.109:56342/api/cajero?authenticationToken=abcxyz"), postContent);
                    var message = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                }

                return View("ReportViewerCajero", encabezado);
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
                    ListaOperaciones.Add(AddOperacion(item.Id, item.Concepto, item.TipoPago, item.Monto, item.DateTOperacion, item.CorteId, item.Numero, item.Tipo, item.CobroTag, item.NoReferencia, item.StatusCancelacion));
                }
                return View("MovimientoCajero", ListaOperaciones.AsEnumerable());
            }
            catch (Exception Ex)
            {
                return View("Error");
            }
        }
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
                    model.Add(AddOperacion(item.Id, item.Concepto, item.TipoPago, item.Monto, item.DateTOperacion, item.CorteId, item.Numero, item.Tipo, item.CobroTag, item.NoReferencia, item.StatusCancelacion));
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
                    model.Add(AddOperacion(item.Id, item.Concepto, item.TipoPago, item.Monto, item.DateTOperacion, item.CorteId, item.Numero, item.Tipo, item.CobroTag, item.NoReferencia, item.StatusCancelacion));
                }
                ViewBag.Corte = CorteId.ToString();
                return View("MovimientoCajero", model);
            }
        }


        //public ActionResult CancelarOperacion(int id, long corteid, string Filtro)
        //{
        //    MethodsGlb method = new MethodsGlb();
        //    var SelectedOperacion = db.OperacionesCajeros.Where(x => x.Id == id).FirstOrDefault();
        //    if (SelectedOperacion.StatusCancelacion == false)                       //Se verifica si el movimiento ya fue cancelado
        //    {
        //        db.OperacionesCajeros.Add(new OperacionesCajero                     //Se agrega un movimiento para notificar la cancelación de este movimiento
        //        {
        //            Concepto = SelectedOperacion.Concepto.Contains("RECARGA") ? "CANCELACION RECARGA" : "CANCELACION ACTIVACION",
        //            TipoPago = "CAN",
        //            Monto = 0 - SelectedOperacion.Monto,
        //            DateTOperacion = DateTime.Now,
        //            CorteId = SelectedOperacion.CorteId,
        //            Numero = SelectedOperacion.NoReferencia,
        //            Tipo = SelectedOperacion.Tipo + " CAN",
        //            CobroTag = 0 - SelectedOperacion.CobroTag,
        //            NoReferencia = method.RandomNumReferencia2().ToString(),
        //            StatusCancelacion = false
        //        });
        //        if (SelectedOperacion.Concepto.Contains("RECARGA"))                     //Si el movimiento fue una RECARGA
        //        {
        //            if (SelectedOperacion.Tipo == "TAG")                                //Si el movimiento cancelado en una recarga de Tag:
        //            {
        //                var UpdatedTag = db.Tags.Where(x => x.NumTag == SelectedOperacion.Numero).FirstOrDefault();
        //                UpdatedTag.SaldoTag = (Convert.ToDouble(UpdatedTag.SaldoTag) - (Convert.ToDouble(SelectedOperacion.Monto) * 100)).ToString();
        //                if (Convert.ToDouble(UpdatedTag.SaldoTag) < 1525)               //Si el saldo es menor a 15.25 Quetzales, se convierte en tag inválido
        //                    UpdatedTag.StatusTag = false;
        //            }
        //            else //Si el movimiento cancelado era una recarga de Cuenta:
        //            {
        //                var UpdatedCuenta = db.CuentasTelepeajes.Where(x => x.NumCuenta == SelectedOperacion.Numero).FirstOrDefault();
        //                var UpdatedTags = db.Tags.Where(x => x.CuentaId == UpdatedCuenta.Id).ToList();
        //                UpdatedCuenta.SaldoCuenta = (Convert.ToDouble(UpdatedCuenta.SaldoCuenta) - (Convert.ToDouble(SelectedOperacion.Monto)) * 100).ToString();
        //                if (Convert.ToDouble(UpdatedCuenta.SaldoCuenta) < 5000)         //Si el saldo es menor a 50 Quetzales, se convierte en cuenta invalida
        //                    UpdatedCuenta.StatusCuenta = false;
        //                foreach (var item in UpdatedTags)
        //                {
        //                    item.SaldoTag = (Convert.ToDouble(item.SaldoTag) - (Convert.ToDouble(SelectedOperacion.Monto)) * 100).ToString(); //Se le resta el monto cancelado a cada tag relacionado con la cuenta
        //                    if (!UpdatedCuenta.StatusCuenta)                            //Si la cuenta es invalida, por ende los Tags relacionados a ella también son inválidos.
        //                        item.StatusTag = false;
        //                }
        //            }
        //        }
        //        else //Si el movimiento fue una ACTIVACIÓN
        //        {
        //            var OpeAfterActivacion = db.OperacionesCajeros.Where(x => x.Id > SelectedOperacion.Id).ToList();    //Para no evaluar todas las operaciones, descartamos las que hayan sido antes de la activación de la cuenta
        //            if (SelectedOperacion.Tipo == "TAG")        //En caso de que se haya cancelado la Activación de un TAG:
        //            {
        //                var CreatedTag = db.Tags.Where(x => x.NumTag == SelectedOperacion.Numero).FirstOrDefault();     //Se identifica el Tag cancelado
        //                int i = 1;
        //                foreach (var item in OpeAfterActivacion)    //Se evalúa cada operación después de la activación para cancelar posibles recargas a ese TAG
        //                {
        //                    if (item.Numero == CreatedTag.NumTag)   //Si el Número de la operacaión coíncide con el NumTag del TAG, significa que se realizó una recarga al TAG que se quiere cancelar
        //                    {
        //                        db.OperacionesCajeros.Add(new OperacionesCajero     //Se crea un movimiento para notificar que esa recarga ya no será valida debido a la cancelación de la cuenta
        //                        {
        //                            Concepto = "CANCELACION RECARGA",
        //                            TipoPago = "CAN",
        //                            Monto = 0 - item.Monto,
        //                            DateTOperacion = DateTime.Now,
        //                            CorteId = SelectedOperacion.CorteId,
        //                            Numero = item.NoReferencia,
        //                            Tipo = "TAG CAN",
        //                            CobroTag = 0 - item.CobroTag,
        //                            NoReferencia = string.Format("{0}", (Convert.ToInt32(method.RandomNumReferencia2()) + i).ToString("D7")), //Se le suma 'i' ya que el método 'RandomNumReferencia2()' evalúa el último registro de la BD y en este punto aún no se suben los cambios a la BD
        //                            StatusCancelacion = false
        //                        });
        //                        item.StatusCancelacion = true;      //Se notifica este movimiento como cancelado
        //                        i++;
        //                    }
        //                }
        //                db.Tags.Remove(CreatedTag);     //Se elimina el Tag cancelado
        //            }
        //            else    //En caso de que se haya cancelado la Activación de una CUENTA
        //            {
        //                var CreatedCuenta = db.CuentasTelepeajes.Where(x => x.NumCuenta == SelectedOperacion.Numero).FirstOrDefault();      //Se identifica la Cuenta a cancelar
        //                var RelatedTags = db.Tags.Where(x => x.CuentaId == CreatedCuenta.Id).ToList();          //Se identifican los Tags relacionados a la Cuenta a cancelar
        //                for (int i = 0; i < RelatedTags.Count; i++)     //Se buscará si hay movimientos con cada Tag relacionado
        //                {
        //                    foreach (var item in OpeAfterActivacion)    //Se buscará en los movimientos después de la activación
        //                    {
        //                        if ((RelatedTags[i].NumTag == item.Numero) && (item.StatusCancelacion == false))        //Si el Numero del movimiento coíncide con el del Tag relacionado y además no ha sido cancelado, se generá un movimiento de cancelación
        //                        {
        //                            db.OperacionesCajeros.Add(new OperacionesCajero
        //                            {
        //                                Concepto = "CANCELACION RECARGA",
        //                                TipoPago = "CAN",
        //                                Monto = 0 - item.Monto,
        //                                DateTOperacion = DateTime.Now,
        //                                CorteId = SelectedOperacion.CorteId,
        //                                Numero = item.NoReferencia,
        //                                Tipo = "TAG CAN",
        //                                CobroTag = 0 - item.CobroTag,
        //                                NoReferencia = string.Format("{0}", (Convert.ToInt32(method.RandomNumReferencia2()) + (i + 1)).ToString("D7")),
        //                                StatusCancelacion = false
        //                            });
        //                            item.StatusCancelacion = true;      //Se cancelará el movimiento
        //                        }
        //                    }
        //                }
        //                foreach (var item in OpeAfterActivacion)        //Se buscará en los movimientos después de la activación si ha habido recargas a la Cuenta
        //                {
        //                    int i = 1;
        //                    if (item.Numero == CreatedCuenta.NumCuenta) //Si el número de movimiento corresponde al Número de Cuenta, se agregará un movimiento de cancelación de operación
        //                    {
        //                        db.OperacionesCajeros.Add(new OperacionesCajero
        //                        {
        //                            Concepto = "CANCELACION RECARGA",
        //                            TipoPago = SelectedOperacion.TipoPago != null ? "CAN" : null,
        //                            Monto = 0 - item.Monto,
        //                            DateTOperacion = DateTime.Now,
        //                            CorteId = SelectedOperacion.CorteId,
        //                            Numero = item.NoReferencia,
        //                            Tipo = "CUENTA CAN",
        //                            CobroTag = null,
        //                            NoReferencia = string.Format("{0}", (Convert.ToInt32(method.RandomNumReferencia2()) + (RelatedTags.Count + 1 + i)).ToString("D7")),
        //                            StatusCancelacion = false
        //                        });
        //                        item.StatusCancelacion = true;      //Cambia el status a cancelado
        //                    }
        //                }
        //                db.CuentasTelepeajes.Remove(CreatedCuenta);     //Se elminará la Cuenta, junto con sus Tags
        //            }
        //        }
        //        SelectedOperacion.StatusCancelacion = true;     //La operación principal se cancela
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("MovimientoCajero", new { CorteId = corteid, Concepto = Filtro });
        //}

        public ActionResult CancelarOperacion(int id, long corteid, string Filtro)
        {
            MethodsGlb method = new MethodsGlb();
            var SelectedOperacion = db.OperacionesCajeros.Where(x => x.Id == id).FirstOrDefault();
            if (SelectedOperacion.StatusCancelacion == false)                       //Se verifica si el movimiento ya fue cancelado
            {
                if (SelectedOperacion.Concepto.Contains("RECARGA"))
                {
                    db.OperacionesCajeros.Add(NewOperacion("CANCELACION RECARGA", "CAN", 0 - SelectedOperacion.Monto, DateTime.Now, SelectedOperacion.CorteId,
                                              SelectedOperacion.NoReferencia, SelectedOperacion.Tipo + "CAN", SelectedOperacion.CobroTag, method.RandomNumReferencia2(), false));
                    CancelarRecarga(SelectedOperacion);
                }
                else if (SelectedOperacion.Concepto.Contains("ELIMINADO"))
                {
                    db.OperacionesCajeros.Add(NewOperacion("CANCELACION ELIMINADO", "CAN", 0 - SelectedOperacion.Monto, DateTime.Now, SelectedOperacion.CorteId,
                                              SelectedOperacion.NoReferencia, SelectedOperacion.Tipo + "CAN", SelectedOperacion.CobroTag, method.RandomNumReferencia2(), false));
                    CancelarEliminacion(SelectedOperacion);
                }
                else
                {
                    db.OperacionesCajeros.Add(NewOperacion("CANCELACION ACTIVACIÓN", "CAN", 0 - SelectedOperacion.Monto, DateTime.Now, SelectedOperacion.CorteId,
                                              SelectedOperacion.NoReferencia, SelectedOperacion.Tipo + "CAN", SelectedOperacion.CobroTag, method.RandomNumReferencia2(), false));
                    CancelarActivacion(SelectedOperacion, method);
                }

                SelectedOperacion.StatusCancelacion = true;     //La operación principal se cancela
                db.SaveChanges();
            }
            return RedirectToAction("MovimientoCajero", new { CorteId = corteid, Concepto = Filtro });
        }



        public OperacionesCajero AddOperacion(long NewId, string NewConcepto, string NewTipoPago, double? NewMonto, DateTime NewDate, long NewCorteId, string NewNumero, string NewTipo, double? NewCobroTag, string NewNoReferencia, bool NewStatus)
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

        public OperacionesCajero NewOperacion(string NewConcepto, string NewTipoPago, double? NewMonto, DateTime NewDate, long NewCorteId, string NewNumero, string NewTipo, double? NewCobroTag, string NewNoReferencia, bool NewStatus)
        {
            return new OperacionesCajero
            {
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

        public void CancelarRecarga(OperacionesCajero SelectedOperacion)
        {
            if (SelectedOperacion.Tipo == "TAG")
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
                    item.SaldoTag = (Convert.ToDouble(item.SaldoTag) - (Convert.ToDouble(SelectedOperacion.Monto)) * 100).ToString();
                    if (!UpdatedCuenta.StatusCuenta)
                        item.StatusTag = false;
                }
            }
        }
        public void CancelarActivacion(OperacionesCajero SelectedOperacion, MethodsGlb method)
        {
            var OpeAfterActivacion = db.OperacionesCajeros.Where(x => x.Id > SelectedOperacion.Id).ToList();
            if (SelectedOperacion.Tipo == "TAG")        //En caso de que se haya cancelado la Activación de un TAG:
            {
                var CreatedTag = db.Tags.Where(x => x.NumTag == SelectedOperacion.Numero).FirstOrDefault();
                int i = 1;
                foreach (var item in OpeAfterActivacion)
                {
                    if (item.Numero == CreatedTag.NumTag)
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
                            NoReferencia = string.Format("{0}", (Convert.ToInt32(method.RandomNumReferencia2()) + i).ToString("D7")), //Se le suma 'i' ya que el método 'RandomNumReferencia2()' evalúa el último registro de la BD y en este punto aún no se suben los cambios a la BD
                            StatusCancelacion = false
                        });
                        item.StatusCancelacion = true;
                        i++;
                    }
                }
                db.Tags.Remove(CreatedTag);
            }
            else    //En caso de que se haya cancelado la Activación de una CUENTA
            {
                var CreatedCuenta = db.CuentasTelepeajes.Where(x => x.NumCuenta == SelectedOperacion.Numero).FirstOrDefault();
                var RelatedTags = db.Tags.Where(x => x.CuentaId == CreatedCuenta.Id).ToList();
                for (int i = 0; i < RelatedTags.Count; i++)
                {
                    foreach (var item in OpeAfterActivacion)
                    {
                        if ((RelatedTags[i].NumTag == item.Numero) && (item.StatusCancelacion == false))
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
                            item.StatusCancelacion = true;
                        }
                    }
                }
                foreach (var item in OpeAfterActivacion)
                {
                    int i = 1;
                    if (item.Numero == CreatedCuenta.NumCuenta)
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
                        item.StatusCancelacion = true;
                    }
                }
                db.CuentasTelepeajes.Remove(CreatedCuenta);
            }

        }
        public void CancelarEliminacion(OperacionesCajero SelectedOperacion)
        {
            var DeletedTag = db.ListaNegras.Where(x => x.Numero == SelectedOperacion.Numero).FirstOrDefault();
            db.Tags.Add(new Tags
            {
                NumTag = DeletedTag.Numero,
                SaldoTag = DeletedTag.SaldoAnterior.ToString(),
                StatusTag = DeletedTag.SaldoAnterior > 1525 ? true : false,
                StatusResidente = false,
                DateTTag = DeletedTag.Date,
                IdCajero = DeletedTag.IdCajero,
                CuentaId = db.CuentasTelepeajes.Where(x => x.NumCuenta == DeletedTag.NumCuenta).FirstOrDefault().Id
            });
            db.ListaNegras.Remove(DeletedTag);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
