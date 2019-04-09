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
using System.Web;
using System.Web.Mvc;

namespace PuntoDeVenta.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationDbContext app = new ApplicationDbContext();

        [Authorize(Roles = "Cajero, SuperUsuario")]
        public async Task<ActionResult> Administrador()
        {
            return View();
        }
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
                        case "NewLogin":
                            var digitoscorte = string.Empty;
                            var numcorte = string.Empty;

                            var date = DateTime.Now.ToString("yyMMdd");

                            var query = await db.CortesCajeros.Where(x => x.NumCorte.Substring(0, 6) == date).OrderByDescending(x => x.DateTApertura).ToListAsync();

                            if (query.Count > 0)
                            {
                                var lastCorteUser = await db.CortesCajeros
                                                           .Where(x => x.IdCajero == UserId)
                                                           .OrderByDescending(x => x.DateTApertura).FirstOrDefaultAsync();

                                if (lastCorteUser != null)
                                {
                                    if (lastCorteUser.DateTCierre == null && lastCorteUser.Comentario == null)
                                        return RedirectToAction("LogOff", "Account", routeValues: new { id = lastCorteUser.Id });
                                }

                                digitoscorte = query.FirstOrDefault().NumCorte.Substring(6, 3);
                                numcorte = DateTime.Now.ToString("yyMMdd") + (int.Parse(digitoscorte) + 1).ToString("D3");

                                var verificar = db.CortesCajeros.Where(x => x.NumCorte == numcorte).ToList();

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

                                        verificar = db.CortesCajeros.Where(x => x.NumCorte == numcorte).ToList();
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
                            }
                            else
                            {
                                var lastCorteUser = await db.CortesCajeros
                                                            .Where(x => x.IdCajero == UserId)
                                                            .OrderByDescending(x => x.DateTApertura).ToListAsync();
                                if (lastCorteUser.Count > 0)
                                {
                                    if (lastCorteUser.FirstOrDefault().DateTCierre == null && lastCorteUser.FirstOrDefault().Comentario == null)
                                        return RedirectToAction("LogOff", "Account", routeValues: new { id = lastCorteUser.FirstOrDefault().Id });
                                }

                                var corte = new CortesCajero
                                {
                                    NumCorte = date + "001",
                                    DateTApertura = DateTime.Now,
                                    IdCajero = User.Identity.GetUserId()
                                };

                                db.CortesCajeros.Add(corte);
                                await db.SaveChangesAsync();
                            }
                            break;
                        case "LogOut":
                            //return RedirectToAction("LogOff", "Account");
                            break;
                        default:
                            break;
                    }
                }

                var cortelast = await db.CortesCajeros
                                        .Where(x => x.IdCajero == UserId)
                                        .OrderByDescending(x => x.DateTApertura).FirstOrDefaultAsync();

                model.NumCorte = cortelast.NumCorte;
                model.Id = cortelast.Id;

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
                ViewBag.Corte = cortelast.NumCorte;
                ViewBag.FechaInicio = cortelast.DateTApertura;
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Cajero, SuperUsuario")]
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
                Fecha = result.DateTApertura.ToString("dd/MM/yyyy"),
                HoraI = result.DateTApertura.ToString("HH:mm:ss"),
                HoraF = result.DateTCierre.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                TotalMonto = result.MontoTotal.Value.ToString("#,##0.00", nfi),
                Comentario = result.Comentario
            };

            var movimientos = await db.OperacionesCajeros.Where(x => x.CorteId == result.Id).ToListAsync();

            var model = new List<object>();

            foreach (var item in movimientos)
            {
                model.Add(new
                {
                    Concepto = item.Concepto,
                    TipoPago = item.TipoPago,
                    Monto = item.Monto.HasValue ? item.Monto.Value.ToString("#,##0.00", nfi) : null,
                    DataTOperacion = item.DateTOperacion.ToString(),
                    Numero = item.Numero,
                    Tipo = item.Tipo,
                    CobroTag = item.CobroTag,
                });
            }


            using (var client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(model);
                HttpContent postContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(new Uri("http://localhost:56342/api/cajero?authenticationToken=abcxyz"), postContent);
                var message = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            }

            return View("ReportViewerCajero", encabezado);
        }
        public ActionResult MovimientoCajero(string corte, long? CorteId)
        {
            try
            {
                long id;
                if (CorteId == null)
                {
                    var AtrCorte = db.CortesCajeros.Where(x => x.NumCorte == corte).ToList();
                    id = AtrCorte[0].Id;
                }
                else
                {
                    id = Convert.ToInt64(CorteId);
                }

                ViewBag.Corte = id.ToString();
                var MovimientoDeCorte = db.OperacionesCajeros.Where(x => x.CorteId == id).ToList();
                var ListaOperaciones = new List<OperacionesCajero>();
                foreach (var item in MovimientoDeCorte)
                {

                    var model = new OperacionesCajero
                    {
                        Id = item.Id,
                        Concepto = item.Concepto,
                        TipoPago = item.TipoPago,
                        Monto = item.Monto,
                        DateTOperacion = item.DateTOperacion,
                        CorteId = item.CorteId,
                        Numero = item.Numero,
                        Tipo = item.Tipo,
                        CobroTag = item.CobroTag,
                        StatusCancelacion = item.StatusCancelacion,
                        NoReferencia = item.NoReferencia

                    };
                    ListaOperaciones.Add(model);
                }
                return View("MovimientoCajero", ListaOperaciones.AsEnumerable());
            }
            catch (Exception Ex)
            {
                return View("Index");
            }
        }

        public ActionResult CancelarOperacion(int id, long corteid, string Filtro)
        {
            MethodsGlb method = new MethodsGlb();
            var SelectedOperacion = db.OperacionesCajeros.Where(x => x.Id == id).FirstOrDefault();
            if (SelectedOperacion.StatusCancelacion == false)
            {
                db.OperacionesCajeros.Add(new OperacionesCajero
                {
                    Concepto = "CANCELACIÓN",
                    TipoPago = "CAN",
                    Monto = SelectedOperacion.Monto,
                    DateTOperacion = DateTime.Now,
                    CorteId = SelectedOperacion.CorteId,
                    Numero = SelectedOperacion.Numero,
                    Tipo = SelectedOperacion.Tipo,
                    CobroTag = SelectedOperacion.CobroTag,
                    NoReferencia = method.RandomNumReferencia2().ToString(),
                    StatusCancelacion = true
                });
                if (SelectedOperacion.Tipo == "TAG")
                {
                    var UpdatedTag = db.Tags.Where(x => x.NumTag == SelectedOperacion.Numero).FirstOrDefault();
                    UpdatedTag.SaldoTag = (Convert.ToDouble(UpdatedTag.SaldoTag) - (Convert.ToDouble(SelectedOperacion.Monto) * 100)).ToString();

                    if (Convert.ToDouble(UpdatedTag.SaldoTag) < 1525)
                        UpdatedTag.StatusTag = false;
                }
                else
                {
                    var UpdatedCuenta = db.CuentasTelepeajes.Where(x => x.NumCuenta == SelectedOperacion.Numero).FirstOrDefault();
                    var UpdatedTags = db.Tags.Where(x => x.CuentaId == UpdatedCuenta.Id).ToList();
                    UpdatedCuenta.SaldoCuenta = (Convert.ToDouble(UpdatedCuenta.SaldoCuenta) - (Convert.ToDouble(SelectedOperacion.Monto)) * 100).ToString();
                    if (Convert.ToDouble(UpdatedCuenta.SaldoCuenta) < 60000)
                        UpdatedCuenta.StatusCuenta = false;
                    foreach (var item in UpdatedTags)
                    {
                        item.SaldoTag = (Convert.ToDouble(item.SaldoTag) - (Convert.ToDouble(SelectedOperacion.Monto)) * 100).ToString();
                        if (!UpdatedCuenta.StatusCuenta)
                            item.StatusTag = false;
                    }
                }
                SelectedOperacion.StatusCancelacion = true;
                db.SaveChanges();
            }
            return RedirectToAction("MovimientoCajero", new { CorteId = corteid, Concepto = Filtro });
        }
        [HttpPost]
        public ActionResult MovimientoCajero(string Concepto, long CorteId)
        {
            ViewBag.Filtro = Concepto;
            if (Concepto == "Todas las operaciones" || Concepto == null)
            {
                var MovimientosdeCorte = db.OperacionesCajeros.Where(x => x.CorteId == CorteId).ToList();
                List<OperacionesCajero> model = new List<OperacionesCajero>();
                foreach (var item in MovimientosdeCorte)
                {
                    var operacion = new OperacionesCajero
                    {
                        Id = item.Id,
                        Concepto = item.Concepto,
                        TipoPago = item.TipoPago,
                        Monto = item.Monto,
                        DateTOperacion = item.DateTOperacion,
                        CorteId = item.CorteId,
                        Numero = item.Numero,
                        Tipo = item.Tipo,
                        CobroTag = item.CobroTag,
                        NoReferencia = item.NoReferencia,
                        StatusCancelacion = item.StatusCancelacion
                    };
                    model.Add(operacion);
                }
                ViewBag.Corte = CorteId.ToString();
                return View("MovimientoCajero", model);
            }
            else
            {
                var MovimientosdeCorte = db.OperacionesCajeros.Where(x => x.CorteId == CorteId && x.Concepto.Trim() == Concepto.Trim()).ToList();
                List<OperacionesCajero> model = new List<OperacionesCajero>();
                foreach (var item in MovimientosdeCorte)
                {
                    var operacion = new OperacionesCajero
                    {
                        Id = item.Id,
                        Concepto = item.Concepto,
                        TipoPago = item.TipoPago,
                        Monto = item.Monto,
                        DateTOperacion = item.DateTOperacion,
                        CorteId = item.CorteId,
                        Numero = item.Numero,
                        Tipo = item.Tipo,
                        CobroTag = item.CobroTag,
                        NoReferencia = item.NoReferencia,
                        StatusCancelacion = item.StatusCancelacion
                    };
                    model.Add(operacion);
                }
                ViewBag.Corte = CorteId.ToString();
                return View("MovimientoCajero", model);
            }
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
