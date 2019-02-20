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
                var lista = await db.Tags.Join(
                                db.CuentasTelepeajes,
                                tag => tag.CuentaId,
                                cue => cue.Id,
                                (tag, cue) => new { tag, cue })
                                .Join(
                                db.Clientes,
                                cuex => cuex.cue.ClienteId,
                                clie => clie.Id,
                                (cuex, clie) => new { cuex, clie })
                                .Where(x => x.cuex.cue.Id == keyCuenta)
                                .ToListAsync();

                lista.ForEach(x =>
                {
                    model.Add(new Tags
                    {
                        Id = x.cuex.tag.Id,
                        NombreCliente = $"{x.clie.Nombre} {x.clie.Apellidos}",
                        NumCuenta = x.cuex.cue.NumCuenta,
                        CuentaId = x.cuex.cue.Id,
                        DateTTag = x.cuex.tag.DateTTag,
                        NumTag = x.cuex.tag.NumTag,
                        IdCajero = x.cuex.tag.IdCajero,
                        SaldoTag = x.cuex.tag.SaldoTag != null ? (Convert.ToDouble(x.cuex.tag.SaldoTag) / 100).ToString() : "Sin saldo",
                        StatusTag = x.cuex.tag.StatusTag,
                        TypeCuenta = x.cuex.cue.TypeCuenta,
                        StatusResidente = x.cuex.tag.StatusResidente,
                    });
                });
            }
            else
            {
                var lista = await db.Tags.Join(
                                db.CuentasTelepeajes,
                                tag => tag.CuentaId,
                                cue => cue.Id,
                                (tag, cue) => new { tag, cue })
                                .Join(
                                db.Clientes,
                                cuex => cuex.cue.ClienteId,
                                clie => clie.Id,
                                (cuex, clie) => new { cuex, clie })
                                .ToListAsync();

                lista.ForEach(x =>
                {
                    model.Add(new Tags
                    {
                        Id = x.cuex.tag.Id,
                        NombreCliente = $"{x.clie.Nombre} {x.clie.Apellidos}",
                        NumCuenta = x.cuex.cue.NumCuenta,
                        CuentaId = x.cuex.cue.Id,
                        DateTTag = x.cuex.tag.DateTTag,
                        NumTag = x.cuex.tag.NumTag,
                        IdCajero = x.cuex.tag.IdCajero,
                        SaldoTag = x.cuex.tag.SaldoTag != null ? (Convert.ToDouble(x.cuex.tag.SaldoTag) / 100).ToString() : "Sin saldo",
                        StatusTag = x.cuex.tag.StatusTag,
                        TypeCuenta = x.cuex.cue.TypeCuenta,
                        StatusResidente = x.cuex.tag.StatusResidente,
                    });
                });
            }

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
                                        tags.SaldoTag = cuenta.SaldoCuenta;
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
            var cuenta = await db.CuentasTelepeajes.FindAsync(tags.CuentaId);
            tags.TipoTag = cuenta.TypeCuenta;
            tags.SaldoTag = (Convert.ToInt64(tags.SaldoTag) / 100).ToString("F2");
            ViewBag.TagsModelTras = new Tags();

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
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTraspaso(TagsViewModel model)
        {
            db.Configuration.ValidateOnSaveEnabled = false;

            if (model.IdOldTag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tags tagOld = await db.Tags.FindAsync(model.IdOldTag);

            if (tagOld == null)
            {
                return HttpNotFound();
            }

            CuentasTelepeaje cuenta = await db.CuentasTelepeajes.FindAsync(tagOld.CuentaId);

            if (cuenta == null)
            {
                return HttpNotFound();
            }

            if (model.Checked == true)
            {
                var tagNew = new Tags
                {
                    StatusResidente = false,
                    StatusTag = true,
                    DateTTag = DateTime.Now.Date,
                    IdCajero = User.Identity.GetUserId(),
                    NumTag = model.NumNewTag,
                    CobroTag = model.CobroTag,
                    CuentaId = cuenta.Id,
                };

                var UserId = User.Identity.GetUserId();

                var lastCorteUser = await db.CortesCajeros
                                                .Where(x => x.IdCajero == UserId)
                                                .OrderByDescending(x => x.DateTApertura).ToListAsync();
                if (lastCorteUser.Count > 0)
                {
                    var detalle = new OperacionesCajero
                    {
                        Concepto = "TAG TRASPASO",
                        DateTOperacion = DateTime.Now,
                        Numero = tagNew.NumTag,
                        Tipo = "TAG",
                        TipoPago = "TRA",
                        CorteId = lastCorteUser.FirstOrDefault().Id,
                        CobroTag = Convert.ToDouble(tagNew.CobroTag),
                    };

                    switch (cuenta.TypeCuenta)
                    {
                        case "Colectiva":
                            tagNew.SaldoTag = cuenta.SaldoCuenta;
                            detalle.Monto = Convert.ToDouble(tagNew.SaldoTag);
                            break;
                        case "Individual":
                            var SaldoSend = model.SaldoTag;
                            SaldoSend = SaldoSend.Replace(",", string.Empty);
                            tagNew.SaldoTag = SaldoSend.Replace(".", string.Empty);
                            detalle.Monto = Convert.ToDouble(model.SaldoTag);
                            break;
                        default:
                            break;
                    }

                    db.Tags.Add(tagNew);
                    db.OperacionesCajeros.Add(detalle);
                }
            }

            var listNegra = new ListaNegra
            {
                Tipo = "TAG",
                Numero = tagOld.NumTag,
                Observacion = model.Observacion,
                Date = DateTime.Now,
                IdCajero = User.Identity.GetUserId(),
                Clase = cuenta.TypeCuenta,
            };

            switch (cuenta.TypeCuenta)
            {
                case "Individual":
                    listNegra.SaldoAnterior = Convert.ToDouble(model.SaldoTag);
                    break;
                default:
                    break;
            }

            db.ListaNegras.Add(listNegra);
            db.Tags.Remove(tagOld);
            await db.SaveChangesAsync();

            return View("Index");
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
