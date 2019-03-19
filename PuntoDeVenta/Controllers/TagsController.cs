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
using System.Globalization;

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
                        SaldoTag = x.cuex.tag.SaldoTag != null ? (double.Parse(x.cuex.tag.SaldoTag) / 100).ToString("F2") : "Sin saldo",
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
                        SaldoTag = x.cuex.tag.SaldoTag != null ? (double.Parse(x.cuex.tag.SaldoTag) / 100).ToString("F2") : "Sin saldo",
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
        public async Task<ActionResult> RecargarSaldo(Tags modelTag, string ReturnController)
        {
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var FoundTag = await db.Tags.Join(
                                        db.CuentasTelepeajes,
                                        tag => tag.CuentaId,
                                        cue => cue.Id,
                                        (tag, cue) => new { tag, cue })
                                        .Where(x => x.tag.NumTag == modelTag.NumTag)
                                        .FirstOrDefaultAsync();

                if (FoundTag == null)
                {
                    TempData["ECreate"] = $"El tag no existe.";
                    return RedirectToAction("Index", ReturnController);
                }

                var UserId = User.Identity.GetUserId();

                var lastCorteUser = await db.CortesCajeros
                                                .Where(x => x.IdCajero == UserId)
                                                .OrderByDescending(x => x.DateTApertura).FirstOrDefaultAsync();
                if (lastCorteUser == null)
                {
                    return HttpNotFound();
                }

                if (FoundTag.cue.TypeCuenta == "Individual")
                {
                    if (FoundTag.cue.StatusCuenta == true)
                    {
                        var Saldo = (double.Parse(FoundTag.tag.SaldoTag) / 100).ToString("F2");

                        var SaldoNuevo = (Convert.ToDouble(Saldo) + double.Parse(modelTag.SaldoARecargar, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }));

                        var SaldoSend = SaldoNuevo.ToString("F2");

                        SaldoSend = SaldoSend.Replace(",", string.Empty);
                        FoundTag.tag.SaldoTag = SaldoSend.Replace(".", string.Empty);

                        if (FoundTag.tag.StatusTag == false)
                            FoundTag.tag.StatusTag = true;

                        if (lastCorteUser != null)
                        {
                            var detalle = new OperacionesCajero
                            {
                                Concepto = "TAG RECARGA",
                                DateTOperacion = DateTime.Now,
                                Numero = FoundTag.tag.NumTag,
                                Tipo = "TAG",
                                TipoPago = "NOR",
                                Monto = double.Parse(modelTag.SaldoARecargar, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }),
                                CorteId = lastCorteUser.Id
                            };

                            db.OperacionesCajeros.Add(detalle);

                            db.Tags.Attach(FoundTag.tag);
                            db.Entry(FoundTag.tag).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            TempData["SCreate"] = $"Se recargó ${modelTag.SaldoARecargar} al tag: {FoundTag.tag.NumTag} con éxito.";
                            return RedirectToAction("Index", ReturnController);
                        }

                        TempData["ECreate"] = "¡Ups! ocurrio un error inesperado.";
                        return View("Index", ReturnController);
                    }

                    TempData["ECreate"] = "No se puede recargar saldo al tag: " + modelTag.NumTag + " porque la cuenta a la que pertenece está dada de baja.";
                    return View("Index", ReturnController);
                }

                TempData["ECreate"] = "El tag: " + modelTag.NumTag + " es colectivo.";
                return View("Index", ReturnController);
            }
            catch (Exception ex)
            {
                TempData["ECreate"] = $"¡Ups! ocurrio un error inesperado, {ex.Message}";
                return View("Index", ReturnController);
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
        // [Bind(Include = "Id,NumTag,SaldoTag,StatusTag,StatusResidente,DateTTag,CuentaId,IdCajero,CobroTag")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Tags tags)
        {
            try
            {
                //ViewBag.CuentaId = new SelectList(db.CuentasTelepeajes, "Id", "NumCuenta", tags.CuentaId);
                db.Configuration.ValidateOnSaveEnabled = false;

                ModelState.Remove("SaldoARecargar");
                ModelState.Remove("ConfSaldoARecargar");
                ModelState.Remove("IdCajero");

                tags.NumTag.Trim();

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
                                                            .OrderByDescending(x => x.DateTApertura).FirstOrDefaultAsync();

                            if (lastCorteUser != null)
                            {
                                var detalle = new OperacionesCajero
                                {
                                    Concepto = "TAG ACTIVADO",
                                    DateTOperacion = DateTime.Now,
                                    Numero = tags.NumTag,
                                    Tipo = "TAG",
                                    TipoPago = "NOR",
                                    CorteId = lastCorteUser.Id,
                                    CobroTag = double.Parse(tags.CobroTag, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }),
                                };

                                switch (cuenta.TypeCuenta)
                                {
                                    case "Colectiva":
                                        tags.SaldoTag = cuenta.SaldoCuenta;
                                        detalle.Monto = null;
                                        break;
                                    case "Individual":
                                        var SaldoSend = tags.SaldoTag;
                                        SaldoSend = SaldoSend.Replace(",", string.Empty);
                                        tags.SaldoTag = SaldoSend.Replace(".", string.Empty);
                                        detalle.Monto = double.Parse(tags.SaldoTag, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," });
                                        break;
                                    default:
                                        break;
                                }

                                if (tags.Checked)
                                {
                                    var listnegra = new ListaNegra { Date = DateTime.Now, IdCajero = User.Identity.GetUserId(), Observacion = tags.Observacion, Numero = tags.OldTag, Tipo = "TAG" };

                                    listnegra.SaldoAnterior = tags.OldSaldo == null || tags.OldSaldo == string.Empty ? (double?)null : double.Parse(tags.OldSaldo, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," });
                                    db.ListaNegras.Add(listnegra);
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
                await db.SaveChangesAsync();

                TempData["SDelete"] = $"Se eliminó correctamente el tag: {tag.NumTag}.";
                return RedirectToAction("Index", "Clientes");
            }

            TempData["EDelete"] = "Primero debe deshabilitar el tag.";
            return RedirectToAction("Index", "Clientes");

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

                await db.SaveChangesAsync();

                TempData["SDelete"] = $"Se dio de baja correctamente el tag: {tag.NumTag}.";
                return RedirectToAction("Index", "Clientes");
            }

            TempData["EDelete"] = $"El tag: {tag.NumTag} ya está dado de baja.";
            return RedirectToAction("Index", "Clientes");
        }

        // POST: Tags/Activate/5
        public async Task<ActionResult> Activate(long? id)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            Tags tag = await db.Tags.FindAsync(id);

            if (tag.StatusTag == false)
            {
                if (((double.Parse(tag.SaldoTag, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }) / 100) >= 20))
                {
                    tag.StatusTag = true;
                    db.Tags.Attach(tag);
                    db.Entry(tag).State = EntityState.Modified;

                    await db.SaveChangesAsync();

                    TempData["SDelete"] = $"Se dio de alta correctamente el tag: {tag.NumTag}.";
                    return RedirectToAction("Index", "Clientes");
                }

                TempData["EDelete"] = $"El tag: {tag.NumTag} no tiene saldo válido.";
                return RedirectToAction("Index", "Clientes");
            }

            TempData["EDelete"] = $"El tag: {tag.NumTag} ya está dado de alta.";
            return RedirectToAction("Index", "Clientes");
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

            var UserId = User.Identity.GetUserId();

            var lastCorteUser = await db.CortesCajeros
                                            .Where(x => x.IdCajero == UserId)
                                            .OrderByDescending(x => x.DateTApertura).ToListAsync();
            if (lastCorteUser.Count > 0)
            {

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
                else
                {
                    var detalle = new OperacionesCajero
                    {
                        Concepto = "TAG ELIMINADO",
                        DateTOperacion = DateTime.Now,
                        Numero = tagOld.NumTag,
                        Tipo = "TAG",
                        TipoPago = null,
                        CorteId = lastCorteUser.FirstOrDefault().Id,
                        CobroTag = null,
                        Monto = null
                    };

                    db.OperacionesCajeros.Add(detalle);
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

                TempData["SDelete"] = $"Se eliminó correctamente el tag: {tagOld.NumTag}.";
                return RedirectToAction("Index", "Clientes");
            }

            TempData["EDelete"] = $"¡Ups! ha ocurrido un error inesperado.";
            return RedirectToAction("Index", "Clientes");
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
