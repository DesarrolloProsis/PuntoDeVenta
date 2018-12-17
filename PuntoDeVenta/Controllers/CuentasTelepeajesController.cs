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
    public class CuentasTelepeajesController : Controller
    {
        private AppDbContext db = new AppDbContext();
        static public long? keyCliente = 0;

        public async Task<ActionResult> GetDataCuentas()
        {
            List<CuentasTelepeaje> model = new List<CuentasTelepeaje>();

            if (keyCliente != 0)
            {
                var lista = await db.CuentasTelepeajes.Where(x => x.ClienteId == keyCliente).ToListAsync();

                model = lista;

                keyCliente = 0;
            }
            else
                model = await db.CuentasTelepeajes.ToListAsync();

            keyCliente = 0;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RecargarSaldo(CuentasTelepeaje model)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            var FoundCuenta = await db.CuentasTelepeajes.SingleOrDefaultAsync(x => x.NumCuenta == model.NumCuenta);

            if (FoundCuenta != null)
            {
                if (FoundCuenta.TypeCuenta == "Colectiva")
                {
                    FoundCuenta.SaldoCuenta = FoundCuenta.SaldoCuenta + model.SaldoARecargar;
                    if (FoundCuenta.StatusCuenta == false)
                        FoundCuenta.StatusCuenta = true;

                    db.CuentasTelepeajes.Attach(FoundCuenta);
                    db.Entry(FoundCuenta).State = EntityState.Modified;

                    List<Tags> tags = await db.Tags.Where(x => x.CuentaId == FoundCuenta.Id).ToListAsync();

                    foreach (var item in tags)
                    {
                        if (item.StatusTag == false)
                        {
                            item.StatusTag = true;
                            db.Tags.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }

                    await db.SaveChangesAsync();

                    ViewBag.Success = $"Se guardó correctamente la cuenta: {FoundCuenta.NumCuenta}.";
                    return View("Index");
                }
                ViewBag.Error = "La cuenta: " + model.NumCuenta + " es individual o puede que este dado de baja.";
                return View("Index");
            }
            ViewBag.Error = "La cuenta: " + model.NumCuenta + " no existe.";
            return View("Index");
        }

        public ActionResult ListCuentas(int? id)
        {
            keyCliente = id;
            return RedirectToAction("Index");
        }

        // GET: CuentasTelepeajes
        public ActionResult Index()
        {
            ViewBag.Success = null;
            ViewBag.Error = null;

            //var cuentasTelepeajes = db.CuentasTelepeajes.Include(c => c.Clientes);
            //return View(await cuentasTelepeajes.ToListAsync());
            return View();
        }

        // GET: CuentasTelepeajes/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentasTelepeaje cuentasTelepeaje = await db.CuentasTelepeajes.FindAsync(id);
            if (cuentasTelepeaje == null)
            {
                return HttpNotFound();
            }
            return View(cuentasTelepeaje);
        }

        // GET: CuentasTelepeajes/Create
        public ActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "NumCliente");
            return View();
        }

        // POST: CuentasTelepeajes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,NumCuenta,SaldoCuenta,TypeCuenta,StatusCuenta,StatusResidenteCuenta,DateTCuenta,ClienteId,IdCajero")] CuentasTelepeaje cuentasTelepeaje)
        {
            try
            {
                //ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "NumCliente", cuentasTelepeaje.ClienteId);
                db.Configuration.ValidateOnSaveEnabled = false;
                Clientes cliente = await db.Clientes.FindAsync(cuentasTelepeaje.ClienteId);

                if (cliente.StatusCliente == true)
                {
                    var cuentasTelepeajes = db.CuentasTelepeajes.Include(c => c.Clientes);

                    cuentasTelepeaje.NumCuenta = RandomNumCuenta();

                    if (cuentasTelepeaje.TypeCuenta == "Individual")
                        cuentasTelepeaje.SaldoCuenta = null;

                    cuentasTelepeaje.StatusCuenta = true;
                    cuentasTelepeaje.StatusResidenteCuenta = false;
                    cuentasTelepeaje.DateTCuenta = DateTime.Now.Date;
                    cuentasTelepeaje.IdCajero = User.Identity.GetUserId();

                    ModelState.Remove("NumCuenta");
                    ModelState.Remove("IdCajero");
                    ModelState.Remove("SaldoARecargar");
                    ModelState.Remove("ConfSaldoARecargar");
                    if (ModelState.IsValid)
                    {
                        var query = db.CuentasTelepeajes.Where(x => x.NumCuenta == cuentasTelepeaje.NumCuenta).ToList();

                        if (query.Count <= 0)
                        {
                            db.CuentasTelepeajes.Add(cuentasTelepeaje);
                            await db.SaveChangesAsync();
                            ViewBag.Success = "Se registró correctamente la cuenta: " + cuentasTelepeaje.NumCuenta + ".";
                            return View("Index");
                        }
                        else
                        {
                            ViewBag.Error = "La cuenta: " + cuentasTelepeaje.NumCuenta + " ya existe!";
                            return View("Index");
                        }
                    }

                    ViewBag.Error = "¡Ups! ocurrio un error inesperado.";
                    return View("Index");
                }

                ViewBag.Error = "El cliente no puede crear una cuenta porque esta dado de baja.";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "¡Ups! ocurrio un error inesperado.";
                return View("Index");
            }
        }

        private string RandomNumCuenta()
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            return string.Format("{0:yy}{0:MM}{1}", DateTime.Now, randomNumber);
        }

        [Authorize(Roles = "SuperUsuario")]
        // GET: CuentasTelepeajes/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentasTelepeaje cuentasTelepeaje = await db.CuentasTelepeajes.FindAsync(id);
            if (cuentasTelepeaje == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "NumCliente", cuentasTelepeaje.ClienteId);
            return View(cuentasTelepeaje);
        }

        // POST: CuentasTelepeajes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[Bind(Include = "Id,NumCuenta,SaldoCuenta,TypeCuenta,StatusCuenta,StatusResidenteCuenta,DateTCuenta,ClienteId,IdCajero")]
        [Authorize(Roles = "SuperUsuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CuentasTelepeaje cuentasTelepeaje)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            if (ModelState.IsValid)
            {
                if (cuentasTelepeaje.StatusCuenta == true)
                {
                    db.CuentasTelepeajes.Attach(cuentasTelepeaje);
                    db.Entry(cuentasTelepeaje).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    ViewBag.Success = "Se actualizó correctamente la cuenta: " + cuentasTelepeaje.NumCuenta + ".";
                    return View("Index");
                }
                ViewBag.Error = "La cuenta no puede ser actualizado porque está dado de baja.";
                return RedirectToAction("Index");
            }
            ViewBag.Error = "¡Ups! ocurrio un error inesperado.";
            return View("Index");
        }

        [Authorize(Roles = "SuperUsuario")]
        // GET: CuentasTelepeajes/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentasTelepeaje cuentasTelepeaje = await db.CuentasTelepeajes.FindAsync(id);
            if (cuentasTelepeaje == null)
            {
                return HttpNotFound();
            }
            return View(cuentasTelepeaje);
        }

        // POST: CuentasTelepeajes/Delete/5
        [Authorize(Roles = "SuperUsuario")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            CuentasTelepeaje cuentasTelepeaje = await db.CuentasTelepeajes.FindAsync(id);

            if (cuentasTelepeaje.StatusCuenta == true)
            {
                cuentasTelepeaje.StatusCuenta = false;
                db.CuentasTelepeajes.Attach(cuentasTelepeaje);
                db.Entry(cuentasTelepeaje).State = EntityState.Modified;
                List<Tags> tags = await db.Tags.Where(x => x.CuentaId == cuentasTelepeaje.Id).ToListAsync();

                tags.ForEach(a =>
                {
                    a.StatusTag = false;
                });

                await db.SaveChangesAsync();

                ViewBag.Success = $"Se dio de baja correctamente la cuenta: {cuentasTelepeaje.NumCuenta}.";
                return View("Index");
            }

            ViewBag.Error = "La cuenta ya esta dada de baja.";
            return View("Index");

            //CuentasTelepeaje cuentasTelepeaje = await db.CuentasTelepeajes.FindAsync(id);
            //db.CuentasTelepeajes.Remove(cuentasTelepeaje);
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
