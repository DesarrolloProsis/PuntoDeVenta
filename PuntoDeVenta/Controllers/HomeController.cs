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
                var response = await client.PostAsync(new Uri("http://192.168.1.65:56342/api/cajero?authenticationToken=abcxyz"), postContent);
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
    }
}
