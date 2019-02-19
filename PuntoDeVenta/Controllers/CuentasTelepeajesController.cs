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

        [HttpGet]
        public async Task<ActionResult> GetDataCuentas()
        {
            List<CuentasTelepeaje> model = new List<CuentasTelepeaje>();

            if (keyCliente != 0)
            {
                var lista = await db.CuentasTelepeajes.Join(
                    db.Clientes,
                    cue => cue.ClienteId,
                    cli => cli.Id,
                    (cue, cli) => new { cue, cli })
                    .Where(x => x.cli.Id == keyCliente)
                    .ToListAsync();

                lista.ForEach(x =>
                {
                    model.Add(new CuentasTelepeaje
                    {
                        Id = x.cue.Id,
                        NombreCliente = $"{x.cli.Nombre} {x.cli.Apellidos}",
                        NumCliente = x.cli.NumCliente,
                        ClienteId = x.cli.Id,
                        DateTCuenta = x.cue.DateTCuenta,
                        SaldoCuenta = x.cue.SaldoCuenta,
                        NumCuenta = x.cue.NumCuenta,
                        IdCajero = x.cue.IdCajero,
                        TypeCuenta = x.cue.TypeCuenta,
                        StatusCuenta = x.cue.StatusCuenta,
                        StatusResidenteCuenta = x.cue.StatusResidenteCuenta,
                    });
                });
            }
            else
            {
                var lista = await db.CuentasTelepeajes.Join(
                    db.Clientes,
                    cue => cue.ClienteId,
                    cli => cli.Id,
                    (cue, cli) => new { cue, cli })
                    .ToListAsync();

                lista.ForEach(x =>
                {
                    model.Add(new CuentasTelepeaje
                    {
                        Id = x.cue.Id,
                        NombreCliente = $"{x.cli.Nombre} {x.cli.Apellidos}",
                        NumCliente = x.cli.NumCliente,
                        ClienteId = x.cli.Id,
                        DateTCuenta = x.cue.DateTCuenta,
                        SaldoCuenta = x.cue.SaldoCuenta,
                        NumCuenta = x.cue.NumCuenta,
                        IdCajero = x.cue.IdCajero,
                        TypeCuenta = x.cue.TypeCuenta,
                        StatusCuenta = x.cue.StatusCuenta,
                        StatusResidenteCuenta = x.cue.StatusResidenteCuenta,
                    });
                });
            }

            keyCliente = 0;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> RecargarSaldo(CuentasTelepeaje model)
        {
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;

                var FoundCuenta = await db.CuentasTelepeajes
                                        .Join(db.Clientes,
                                        cue => cue.ClienteId,
                                        cli => cli.Id,
                                        (cue, cli) => new { cue, cli })
                                        .SingleOrDefaultAsync(x => x.cue.NumCuenta == model.NumCuenta);

                if (FoundCuenta == null)
                {
                    return HttpNotFound();
                }

                if (FoundCuenta.cli.StatusCliente == true)
                {
                    if (FoundCuenta.cue.TypeCuenta == "Colectiva")
                    {
                        var UserId = User.Identity.GetUserId();

                        var lastCorteUser = await db.CortesCajeros
                                                        .Where(x => x.IdCajero == UserId)
                                                        .OrderByDescending(x => x.DateTApertura).ToListAsync();

                        if (lastCorteUser.Count > 0)
                        {
                            var Saldo = (Convert.ToDouble(FoundCuenta.cue.SaldoCuenta) / 100).ToString("F2");

                            var SaldoNuevo = (Convert.ToDouble(Saldo) + Convert.ToDouble(model.SaldoARecargar));

                            var SaldoSend = SaldoNuevo.ToString("F2");

                            SaldoSend = SaldoSend.Replace(",", string.Empty);
                            FoundCuenta.cue.SaldoCuenta = SaldoSend.Replace(".", string.Empty);

                            if (FoundCuenta.cue.StatusCuenta == false)
                                FoundCuenta.cue.StatusCuenta = true;

                            var detalle = new OperacionesCajero
                            {
                                Concepto = "CUENTA RECARGA",
                                DateTOperacion = DateTime.Now,
                                Numero = FoundCuenta.cue.NumCuenta,
                                Tipo = "CUENTA",
                                TipoPago = "NOR",
                                Monto = Convert.ToDouble(model.SaldoARecargar),
                                CorteId = lastCorteUser.FirstOrDefault().Id,
                            };

                            db.OperacionesCajeros.Add(detalle);

                            db.CuentasTelepeajes.Attach(FoundCuenta.cue);
                            db.Entry(FoundCuenta.cue).State = EntityState.Modified;

                            List<Tags> tags = await db.Tags.Where(x => x.CuentaId == FoundCuenta.cue.Id).ToListAsync();

                            foreach (var item in tags)
                            {
                                if (item.StatusTag == false)
                                    item.StatusTag = true;

                                item.SaldoTag = FoundCuenta.cue.SaldoCuenta;
                                db.Tags.Attach(item);
                                db.Entry(item).State = EntityState.Modified;
                            }

                            await db.SaveChangesAsync();

                            ViewBag.Success = $"Se guardó correctamente la cuenta: {FoundCuenta.cue.NumCuenta}.";
                            return View("Index");
                        }

                        ViewBag.Error = $"¡Ups! ocurrio un error inesperado.";
                        return View("Index");
                    }

                    ViewBag.Error = "La cuenta: " + model.NumCuenta + " es individual o puede que este dado de baja.";
                    return View("Index");
                }

                ViewBag.Error = "No se puede recargar saldo a la cuenta: " + model.NumCuenta + " porque el cliente al que pertenece está dado de baja.";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"¡Ups! ocurrio un error inesperado, {ex.Message}";
                return View("Index");
            }
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
                    var UserId = User.Identity.GetUserId();

                    var lastCorteUser = await db.CortesCajeros
                                                    .Where(x => x.IdCajero == UserId)
                                                    .OrderByDescending(x => x.DateTApertura).ToListAsync();

                    if (lastCorteUser.Count > 0)
                    {
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
                            var query = await db.CuentasTelepeajes.Where(x => x.NumCuenta == cuentasTelepeaje.NumCuenta).ToListAsync();

                            if (query.Count == 0)
                            {
                                var detalle = new OperacionesCajero
                                {
                                    Concepto = "CUENTA ACTIVADA",
                                    DateTOperacion = DateTime.Now,
                                    Numero = cuentasTelepeaje.NumCuenta,
                                    Tipo = "CUENTA",
                                    Monto = Convert.ToDouble(cuentasTelepeaje.SaldoCuenta),
                                    CorteId = lastCorteUser.FirstOrDefault().Id,
                                };

                                if (cuentasTelepeaje.TypeCuenta == "Colectiva")
                                {
                                    detalle.TipoPago = "NOR";

                                    var SaldoSend = cuentasTelepeaje.SaldoCuenta;
                                    SaldoSend = SaldoSend.Replace(",", string.Empty);
                                    cuentasTelepeaje.SaldoCuenta = SaldoSend.Replace(".", string.Empty);
                                }

                                db.OperacionesCajeros.Add(detalle);

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
                    }

                    ViewBag.Error = $"¡Ups! ocurrio un error inesperado.";
                    return View("Index");
                }

                ViewBag.Error = "El cliente no puede crear una cuenta porque esta dado de baja.";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"¡Ups! ocurrio un error inesperado, {ex.Message}";
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
