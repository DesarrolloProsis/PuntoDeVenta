using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PuntoDeVenta.Models;
using Microsoft.AspNet.Identity;

namespace PuntoDeVenta.Controllers
{
    [Authorize]
    public class TagsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        static public long? keyCuenta = 0;

        [HttpGet]
        public async Task<ActionResult> GetDataTags()
        {
            List<Tags> model = new List<Tags>();

            if (keyCuenta != 0)
            {
                var lista = await db.Tags.Where(x => x.CuentaId == keyCuenta).ToListAsync();
                model = lista;
                keyCuenta = 0;
            }
            else
                model = await db.Tags.ToListAsync();

            keyCuenta = 0;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListTags(int? id)
        {
            keyCuenta = id;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> RecargarSaldo(Tags model)
        {
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var FoundTag = await db.Tags.Join(
                                        db.CuentasTelepeajes,
                                        tag => tag.CuentaId,
                                        cue => cue.Id,
                                        (tag, cue) => new { tag, cue })
                                        .Where(x => x.tag.NumTag == model.NumTag)
                                        .FirstOrDefaultAsync();

                if (FoundTag == null)
                {
                    return HttpNotFound();
                }

                if (FoundTag.cue.TypeCuenta == "Individual")
                {
                    if (FoundTag.cue.StatusCuenta == true)
                    {
                        //var SaldoNuevo = (Convert.ToDouble(FoundTag.tag.SaldoTag) + Convert.ToDouble(model.SaldoARecargar)).ToString();
                        //SaldoNuevo = SaldoNuevo.Replace(",", string.Empty);
                        //FoundTag.tag.SaldoTag = SaldoNuevo.Replace(".", string.Empty);

                        var Saldo = (Convert.ToDouble(FoundTag.tag.SaldoTag) / 100).ToString("F2");

                        var SaldoNuevo = (Convert.ToDouble(Saldo) + Convert.ToDouble(model.SaldoARecargar));

                        var SaldoSend = SaldoNuevo.ToString("F2");

                        SaldoSend = SaldoSend.Replace(",", string.Empty);
                        FoundTag.tag.SaldoTag = SaldoSend.Replace(".", string.Empty);

                        if (FoundTag.tag.StatusTag == false)
                            FoundTag.tag.StatusTag = true;

                        var UserId = User.Identity.GetUserId();

                        var lastCorteUser = await db.CortesCajeros
                                                        .Where(x => x.IdCajero == UserId)
                                                        .OrderByDescending(x => x.DateTApertura).ToListAsync();

                        if (lastCorteUser.Count > 0)
                        {
                            var detalle = new OperacionesCajero
                            {
                                Concepto = "TAG RECARGA",
                                DateTOperacion = DateTime.Now,
                                Numero = FoundTag.tag.NumTag,
                                Tipo = "TAG",
                                TipoPago = "NOR",
                                Monto = Convert.ToDouble(model.SaldoARecargar),
                                CorteId = lastCorteUser.FirstOrDefault().Id
                            };

                            db.OperacionesCajeros.Add(detalle);

                            db.Tags.Attach(FoundTag.tag);
                            db.Entry(FoundTag.tag).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            ViewBag.Success = $"Se recargó correctamente el tag: {FoundTag.tag.NumTag}.";
                            return View("Index");
                        }

                        ViewBag.Error = "¡Ups! ocurrio un error inesperado.";
                        return View("Index");
                    }

                    ViewBag.Error = "No se puede recargar saldo al tag: " + model.NumTag + " porque la cuenta a la que pertenece está dada de baja.";
                    return View("Index");
                }

                ViewBag.Error = "El tag: " + model.NumTag + " es colectivo.";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"¡Ups! ocurrio un error inesperado, {ex.Message}";
                return View("Index");
            }
        }

        // GET: Tags
        public ActionResult Index()
        {
            ViewBag.Success = null;
            ViewBag.Error = null;

            return View();
        }

        // GET: Tags/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tags tags = await db.Tags.FindAsync(id);
            if (tags == null)
            {
                return HttpNotFound();
            }
            return View(tags);
        }

        // GET: Tags/Create
        public ActionResult Create()
        {
            ViewBag.CuentaId = new SelectList(db.CuentasTelepeajes, "Id", "NumCuenta");
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,NumTag,SaldoTag,StatusTag,StatusResidente,DateTTag,CuentaId,IdCajero,CobroTag")] Tags tags)
        {
            try
            {
                //ViewBag.CuentaId = new SelectList(db.CuentasTelepeajes, "Id", "NumCuenta", tags.CuentaId);
                db.Configuration.ValidateOnSaveEnabled = false;

                ModelState.Remove("SaldoARecargar");
                ModelState.Remove("ConfSaldoARecargar");
                ModelState.Remove("IdCajero");

                var cuenta = await db.CuentasTelepeajes.FindAsync(tags.CuentaId);

                if (cuenta == null)
                {
                    return HttpNotFound();
                }

                if (cuenta.StatusCuenta == true)
                {
                    var query = await db.Tags.Where(x => x.NumTag == tags.NumTag).ToListAsync();
                    if (query.Count == 0)
                    {
                        tags.StatusResidente = false;
                        tags.StatusTag = true;
                        tags.DateTTag = DateTime.Now.Date;
                        tags.IdCajero = User.Identity.GetUserId();

                        if (ModelState.IsValid)
                        {
                            var UserId = User.Identity.GetUserId();

                            var lastCorteUser = await db.CortesCajeros
                                                            .Where(x => x.IdCajero == UserId)
                                                            .OrderByDescending(x => x.DateTApertura).ToListAsync();

                            if (lastCorteUser.Count > 0)
                            {
                                var detalle = new OperacionesCajero
                                {
                                    Concepto = "TAG ACTIVADO",
                                    DateTOperacion = DateTime.Now,
                                    Numero = tags.NumTag,
                                    Tipo = "TAG",
                                    TipoPago = "NOR",
                                    Monto = Convert.ToDouble(tags.SaldoTag),
                                    CorteId = lastCorteUser.FirstOrDefault().Id,
                                    CobroTag = Convert.ToDouble(tags.CobroTag),
                                };

                                switch (cuenta.TypeCuenta)
                                {
                                    case "Colectiva":
                                        tags.SaldoTag = null;
                                        break;
                                    case "Individual":
                                        var SaldoSend = tags.SaldoTag;
                                        SaldoSend = SaldoSend.Replace(",", string.Empty);
                                        tags.SaldoTag = SaldoSend.Replace(".", string.Empty);
                                        break;
                                    default:
                                        break;
                                }

                                db.Tags.Add(tags);
                                db.OperacionesCajeros.Add(detalle);

                                await db.SaveChangesAsync();
                                ViewBag.Success = "Se activó correctamente el tag: " + tags.NumTag + ".";
                                return View("Index");
                            }
                        }

                        ViewBag.Error = "¡Ups! ocurrio un error inesperado.";
                        return View("Index");
                    }
                    else
                    {
                        ViewBag.Error = "El tag: " + tags.NumTag + " ya esta activado.";
                        return View("Index");
                    }
                }

                ViewBag.Error = $"No se puede agregar el tag: {tags.NumTag} a la cuenta porque esta dada de baja.";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"¡Ups! ocurrio un error inesperado, {ex.Message}";
                return View("Index");
            }
        }

        [Authorize(Roles = "SuperUsuario")]
        // GET: Tags/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tags tags = await db.Tags.FindAsync(id);
            if (tags == null)
            {
                return HttpNotFound();
            }
            ViewBag.CuentaId = new SelectList(db.CuentasTelepeajes, "Id", "NumCuenta", tags.CuentaId);
            return View(tags);
        }

        [Authorize(Roles = "SuperUsuario")]
        // POST: Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NumTag,SaldoTag,StatusTag,StatusResidente,DateTTag,CuentaId,IdCajero")] Tags tags)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            if (ModelState.IsValid)
            {
                db.Entry(tags).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CuentaId = new SelectList(db.CuentasTelepeajes, "Id", "NumCuenta", tags.CuentaId);
            return View(tags);
        }

        [Authorize(Roles = "SuperUsuario")]
        // GET: Tags/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tags tags = await db.Tags.FindAsync(id);
            if (tags == null)
            {
                return HttpNotFound();
            }
            return View(tags);
        }
        [Authorize(Roles = "SuperUsuario")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            Tags tag = await db.Tags.FindAsync(id);

            if (tag.StatusTag == false)
            {
                Tags tags = await db.Tags.FindAsync(id);
                db.Tags.Remove(tags);
                ViewBag.Success = $"Se elimino correctamente el tag: {tag.NumTag}.";
                await db.SaveChangesAsync();
                return View("Index");
            }

            ViewBag.Error = "Primero debe deshabilitar el tag.";
            return View("Index");

        }
        [Authorize(Roles = "SuperUsuario")]
        // deshabilitar
        public async Task<ActionResult> Deshabilitar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tags tags = await db.Tags.FindAsync(id);
            if (tags == null)
            {
                return HttpNotFound();
            }
            return View(tags);
        }
        [Authorize(Roles = "SuperUsuario")]

        [HttpPost, ActionName("Deshabilitar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeshabilitarConfirmed(long id)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            Tags tag = await db.Tags.FindAsync(id);

            if (tag.StatusTag == true)
            {
                tag.StatusTag = false;
                db.Tags.Attach(tag);
                db.Entry(tag).State = EntityState.Modified;

                ViewBag.Success = $"Se dio de baja correctamente el tag: {tag.NumTag}.";

                await db.SaveChangesAsync();
                return View("Index");
            }

            ViewBag.Error = "El tag ya esta dada de baja.";
            return View("Index");
            //Tags tags = await db.Tags.FindAsync(id);
            //db.Tags.Remove(tags);
            //await db.SaveChangesAsync();
            //return RedirectToAction("Index");
        }


        [Authorize(Roles = "SuperUsuario")]
        // GET: Tags/Delete/5
        public async Task<ActionResult> Activate(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tags tags = await db.Tags.FindAsync(id);
            if (tags == null)
            {
                return HttpNotFound();
            }
            return View(tags);
        }

        [Authorize(Roles = "SuperUsuario")]
        // POST: Tags/Delete/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActivateConfirmed(long id)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            Tags tag = await db.Tags.FindAsync(id);

            if (tag.StatusTag == false)
            {
                tag.StatusTag = true;
                db.Tags.Attach(tag);
                db.Entry(tag).State = EntityState.Modified;

                ViewBag.Success = $"Se dio de Alta correctamente el tag: {tag.NumTag}.";

                await db.SaveChangesAsync();
                return View("Index");
            }

            ViewBag.Error = "El tag ya esta dada de Alta.";
            return View("Index");
            //Tags tags = await db.Tags.FindAsync(id);
            //db.Tags.Remove(tags);
            //await db.SaveChangesAsync();
            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
