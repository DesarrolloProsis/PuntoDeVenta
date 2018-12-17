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
            //var model = new CortesCajero();
            //var _IdCajero = User.Identity.GetUserId();

            //if (User.Identity.IsAuthenticated)
            //{
            //    switch (verfiAction)
            //    {
            //        case "NewLogin":
            //            var query = await db.CortesCajeros.ToListAsync();

            //            if (query.Count > 0)
            //            {
            //                GenerateCorte();
            //            }
            //            else
            //            {
            //                GenerateCorte();
            //            }

            //            break;
            //        case "LogOut":
            //            return RedirectToAction("LogOff", "Account");
            //        default:
            //            break;
            //    }
            //}

            if (verfiAction == "LogOut")
                return RedirectToAction("LogOff", "Account");


            return View();
        }

        //public string GenerateCorte()
        //{
        //    int i = 0;

        //    string rpt = "";
        //    var digitoscorte = string.Empty;
        //    var numcorte = string.Empty;

        //    var date = DateTime.Now.ToString("yyMMdd");

        //    var query = db.CortesCajeros.Where(x => x.NumCorte.Substring(0, 6) == date).OrderByDescending(x => x.DateTApertura).ToList();

        //    if (query.Count > 0)
        //    {
        //        digitoscorte = query.FirstOrDefault().NumCorte.Substring(6, 7);
        //        numcorte = DateTime.Now.ToString("yyMMdd") + (int.Parse(digitoscorte) + 1).ToString("D3");

        //        var verificar = db.CortesCajeros.Where(x => x.NumCorte == numcorte).ToList();

        //        if (verificar.Count == 0)
        //        {
        //            var corte = new CortesCajero
        //            {
        //                NumCorte = numcorte,
        //                DateTApertura = DateTime.Now,
        //                IdCajero = User.Identity.GetUserId()
        //            };

        //            db.CortesCajeros.Add(corte);
        //            db.SaveChanges();
        //        }
        //        else
        //        {

        //            while (verificar.Count > 0)
        //            {
        //                digitoscorte = query.FirstOrDefault().NumCorte.Substring(7, 9);
        //                numcorte = DateTime.Now.ToString("yyMMdd") + (int.Parse(digitoscorte) + 1).ToString("D3");

        //                verificar = db.CortesCajeros.Where(x => x.NumCorte == numcorte).ToList();
        //            }

        //            var corte = new CortesCajero
        //            {
        //                NumCorte = numcorte,
        //                DateTApertura = DateTime.Now,
        //                IdCajero = User.Identity.GetUserId()
        //            };

        //            db.CortesCajeros.Add(corte);
        //            db.SaveChanges();
        //        }
        //    }
        //    else
        //    {
        //        var corte = new CortesCajero
        //        {
        //            NumCorte = DateTime.Now.ToString("yyMMdd") + "001",
        //            DateTApertura = DateTime.Now,
        //            IdCajero = User.Identity.GetUserId()
        //        };

        //        db.CortesCajeros.Add(corte);
        //        db.SaveChanges();
        //    }

        //    return rpt;

        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            using (ApplicationDbContext app = new ApplicationDbContext())
            {
                var idUser = User.Identity.GetUserId();
                var _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(app));
                var _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(app));

                //var result = _roleManager.Create(new IdentityRole("SuperUsuario"));

                var user = _UserManager.AddToRole(idUser, "SuperUsuario");
                //userRole = _UserManager.IsInRole(idUser, "Cajero");

               var userRole = _UserManager.IsInRole(idUser, "SuperUsuario");
            }

            //var string1 = GenerateCorte();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

/*
 * switch (verfiAction)
                {
                    case "NewLogin":
                        var userId = User.Identity.GetUserId();
                        bool ExistComment = true;
                        string numRandom = string.Empty;
                        string LastDigNumCorte = string.Empty;
                        int i = 0;

                        var listAllCortes = db.CortesCajeros.OrderByDescending(x => x.NumCorte).ToList();
                        var listCortesCajero = db.CortesCajero.Where(x => x.IdCajero == userId).OrderByDescending(x => x.DateCorte).ToList();

                        if (listAllCortes.Any())
                        {
                            ExistComment = listCortesCajero.Count == 0 ? false : true;
                            if (ExistComment)
                            {
                                ExistComment = listCortesCajero.FirstOrDefault().Comentario == null ? false : true;

                                if (ExistComment)
                                {
                                    LastDigNumCorte = listAllCortes.FirstOrDefault().NumCorte.Substring(6, 4);
                                    numRandom = DateTime.Now.ToString("yyMMdd") + (int.Parse(LastDigNumCorte) + 1).ToString("D4");

                                    while (listAllCortes.Any())
                                    {
                                        if (listAllCortes[i].NumCorte == numRandom)
                                            numRandom = DateTime.Now.ToString("yyMMdd") + (int.Parse(LastDigNumCorte) + 1).ToString("D4");
                                        else
                                        {
                                            var corte = new CortesCajero { NumCorte = numRandom, DateCorte = DateTime.Now, IdCajero = userId, HoraApertura = TimeSpan.Parse(string.Format("{0:hh\\:mm\\:ss}", DateTime.Now.TimeOfDay)) };
                                            db.CortesCajero.Add(corte);
                                            db.SaveChanges();
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    return RedirectToAction("LogOff", "Account");
                                }
                            }
                            else
                            {
                                LastDigNumCorte = listAllCortes.FirstOrDefault().NumCorte.Substring(6, 4);
                                numRandom = DateTime.Now.ToString("yyMMdd") + (int.Parse(LastDigNumCorte) + 1).ToString("D4");

                                while (listAllCortes.Any())
                                {
                                    if (listAllCortes[i].NumCorte == numRandom)
                                        numRandom = DateTime.Now.ToString("yyMMdd") + (int.Parse(LastDigNumCorte) + 1).ToString("D4");
                                    else
                                    {
                                        var corte = new CortesCajero { NumCorte = numRandom, DateCorte = DateTime.Now, IdCajero = userId, HoraApertura = TimeSpan.Parse(string.Format("{0:hh\\:mm\\:ss}", DateTime.Now.TimeOfDay)) };
                                        db.CortesCajero.Add(corte);
                                        db.SaveChanges();
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            numRandom = DateTime.Now.ToString("yyMMdd") + "0001";
                            var corte = new CortesCajero { NumCorte = numRandom, DateCorte = DateTime.Now, IdCajero = userId, HoraApertura = TimeSpan.Parse(string.Format("{0:hh\\:mm\\:ss}", DateTime.Now.TimeOfDay)) };
                            db.CortesCajero.Add(corte);
                            db.SaveChanges();
                        }
                        break;
                    case "LogOut":
                        userId = User.Identity.GetUserId();
                        listCortesCajero = db.CortesCajero.Where(x => x.IdCajero == userId).OrderByDescending(x => x.DateCorte).ToList();
                        model.Id = listCortesCajero.FirstOrDefault().Id;
                        model.IdCajero = userId;
                        model.NumCorte = listCortesCajero.FirstOrDefault().NumCorte;
                        model.DateCorte = listCortesCajero.FirstOrDefault().DateCorte;
                        model.HoraApertura = listCortesCajero.FirstOrDefault().HoraApertura;
                        model.HoraCierre = listCortesCajero.FirstOrDefault().HoraCierre;
                        break;
                    default:
                        break;
                }
 */
