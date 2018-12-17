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

        public async Task<ActionResult> RecargarSaldo(Tags model)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            var FoundTag = await db.Tags.Join(
                                    db.CuentasTelepeajes,
                                    tag => tag.CuentaId,
                                    cue => cue.Id,
                                    (tag, cue) => new { tag, cue })
                                    .Where(x => x.tag.NumTag == model.NumTag)
                                    .FirstOrDefaultAsync();

            if (FoundTag != null)
            {
                if (FoundTag.cue.TypeCuenta == "Individual")
                {
                    if (FoundTag.cue.StatusCuenta == false)
                    {
                        FoundTag.tag.SaldoTag = FoundTag.tag.SaldoTag + model.SaldoARecargar;

                        if (FoundTag.tag.StatusTag == false)
                            FoundTag.tag.StatusTag = true;

                        db.Tags.Attach(FoundTag.tag);
                        db.Entry(FoundTag.tag).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        ViewBag.Success = $"Se recargó correctamente el tag: {FoundTag.tag.NumTag}.";
                        return View("Index");
                    }
                    ViewBag.Error = "No se puede recargar saldo al tag: " + model.NumTag + " porque la cuenta a la que pertenece está dada de baja.";
                    return View("Index");
                }
                ViewBag.Error = "El tag: " + model.NumTag + " es colectivo.";
                return View("Index");
            }
            ViewBag.Error = "El tag no existe.";
            return View("Index");
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
            //ViewBag.CuentaId = new SelectList(db.CuentasTelepeajes, "Id", "NumCuenta", tags.CuentaId);
            db.Configuration.ValidateOnSaveEnabled = false;
            var cuenta = db.CuentasTelepeajes.Find(tags.CuentaId);
            ModelState.Remove("SaldoARecargar");
            ModelState.Remove("ConfSaldoARecargar");
            if (cuenta != null)
            {
                if (cuenta.StatusCuenta == true)
                {
                    var query = await db.Tags.Where(x => x.NumTag == tags.NumTag).ToListAsync();
                    if (query.Count <= 0)
                    {
                        tags.StatusResidente = false;
                        tags.StatusTag = true;
                        tags.DateTTag = DateTime.Now.Date;
                        tags.IdCajero = User.Identity.GetUserId();

                        ModelState.Remove("IdCajero");

                        switch (cuenta.TypeCuenta)
                        {
                            case "Individual":
                                if (ModelState.IsValid)
                                {
                                    db.Tags.Add(tags);
                                    await db.SaveChangesAsync();
                                    ViewBag.Success = "Se activó correctamente el tag: " + tags.NumTag + ".";
                                    return View("Index");
                                }

                                ViewBag.Error = "¡Ups! ocurrio un error inesperado.";
                                return View("Index");

                            case "Colectiva":
                                tags.SaldoTag = null;
                                if (ModelState.IsValid)
                                {
                                    db.Tags.Add(tags);
                                    await db.SaveChangesAsync();
                                    ViewBag.Success = "Se activó correctamente el tag: " + tags.NumTag + ".";
                                    return View("Index");
                                }

                                ViewBag.Error = "¡Ups! ocurrio un error inesperado.";
                                return View("Index");

                            default:
                                break;
                        }
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

            ViewBag.Error = "La cuenta no existe.";
            return View("Index");
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
        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
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
