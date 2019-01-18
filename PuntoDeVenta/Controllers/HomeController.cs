using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PuntoDeVenta.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PuntoDeVenta.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();

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
                                                           .OrderByDescending(x => x.DateTApertura).ToListAsync();

                                if (lastCorteUser.Count > 0)
                                {
                                    if (lastCorteUser.FirstOrDefault().DateTCierre == null && lastCorteUser.FirstOrDefault().Comentario == null)
                                        return RedirectToAction("LogOff", "Account", routeValues: new { id = lastCorteUser.FirstOrDefault().Id });
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
                                        .OrderByDescending(x => x.DateTApertura).ToListAsync();
                model.NumCorte = cortelast.FirstOrDefault().NumCorte;
                model.Id = cortelast.FirstOrDefault().Id;
            }
            catch (Exception ex)
            {

            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            using (ApplicationDbContext app = new ApplicationDbContext())
            {
                var idUser = User.Identity.GetUserId();
                var _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(app));
                var _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(app));

                //var result = _roleManager.Create(new IdentityRole("SuperUsuario"));

                //var user = _UserManager.AddToRole(idUser, "SuperUsuario");
                //userRole = _UserManager.IsInRole(idUser, "Cajero");

                //var userRole = _UserManager.IsInRole(idUser, "SuperUsuario");
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GenerarReportes()
        {
            var model = new GenerarReportesViewModel();
            model.PropertiesList = new List<Properties>();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
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

                return PartialView("_ListaCortes", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
