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
using PuntoDeVenta.Services;

namespace PuntoDeVenta.Controllers
{
    [Authorize]
    public class CuentasTelepeajesController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private MethodsGlb methods = new MethodsGlb();

        static public long? keyCliente = 0;

        public ActionResult ListCuentas(int? id)
        {
            keyCliente = id;
            return RedirectToAction("Index");
        }

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
                        SaldoCuenta = x.cue.SaldoCuenta != null ? (double.Parse(x.cue.SaldoCuenta) / 100).ToString("F2") : "Sin saldo",
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
                        SaldoCuenta = x.cue.SaldoCuenta != null ? (double.Parse(x.cue.SaldoCuenta) / 100).ToString("F2") : "Sin saldo",
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
        public async Task<ActionResult> RecargarSaldo(CuentasTelepeaje modelCuenta, string ReturnController)
        {
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;

                var FoundCuenta = await db.CuentasTelepeajes
                                        .Join(db.Clientes,
                                        cue => cue.ClienteId,
                                        cli => cli.Id,
                                        (cue, cli) => new { cue, cli })
                                        .SingleOrDefaultAsync(x => x.cue.NumCuenta == modelCuenta.NumCuenta);

                if (FoundCuenta == null)
                {
                    TempData["ECreate"] = $"La cuenta no existe.";
                    return RedirectToAction("Index", ReturnController);
                }

                if (FoundCuenta.cli.StatusCliente == true)
                {
                    if (FoundCuenta.cue.TypeCuenta == "Colectiva")
                    {
                        var UserId = User.Identity.GetUserId();

                        var lastCorteUser = await db.CortesCajeros
                                                        .Where(x => x.IdCajero == UserId)
                                                        .OrderByDescending(x => x.DateTApertura).FirstOrDefaultAsync();

                        if (lastCorteUser != null)
                        {
                            var Saldo = (double.Parse(FoundCuenta.cue.SaldoCuenta) / 100).ToString("F2");

                            var SaldoNuevo = (double.Parse(Saldo) + double.Parse(modelCuenta.SaldoARecargar, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }));

                            var SaldoSend = SaldoNuevo.ToString("F2");

                            SaldoSend = SaldoSend.Replace(",", string.Empty);
                            FoundCuenta.cue.SaldoCuenta = SaldoSend.Replace(".", string.Empty);

                            var detalle = new OperacionesCajero
                            {
                                Concepto = "CUENTA RECARGA",
                                DateTOperacion = DateTime.Now,
                                Numero = FoundCuenta.cue.NumCuenta,
                                Tipo = "CUENTA",
                                TipoPago = "NOR",
                                Monto = double.Parse(modelCuenta.SaldoARecargar, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }),
                                CorteId = lastCorteUser.Id,
                                NoReferencia = await methods.RandomNumReferencia(),
                            };

                            db.OperacionesCajeros.Add(detalle);

                            if ((double.Parse(FoundCuenta.cue.SaldoCuenta, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }) / 100) >= 100)
                            {
                                if (FoundCuenta.cue.StatusCuenta == false)
                                    FoundCuenta.cue.StatusCuenta = true;

                                List<Tags> tags = await db.Tags.Where(x => x.CuentaId == FoundCuenta.cue.Id).ToListAsync();

                                foreach (var item in tags)
                                {
                                    if (item.StatusTag == false)
                                        item.StatusTag = true;

                                    item.SaldoTag = FoundCuenta.cue.SaldoCuenta;
                                    db.Tags.Attach(item);
                                    db.Entry(item).State = EntityState.Modified;
                                }
                            }

                            db.CuentasTelepeajes.Attach(FoundCuenta.cue);
                            db.Entry(FoundCuenta.cue).State = EntityState.Modified;

                            await db.SaveChangesAsync();

                            TempData["SCreate"] = $"Se recargó Q{modelCuenta.SaldoARecargar} a la cuenta: {FoundCuenta.cue.NumCuenta} con éxito.";

                            return RedirectToAction("Index", ReturnController);
                        }

                        TempData["ECreate"] = $"¡Ups! ocurrio un error inesperado.";
                        return RedirectToAction("Index", ReturnController);
                    }

                    TempData["ECreate"] = "La cuenta: " + modelCuenta.NumCuenta + " es individual o puede que este dada de baja.";
                    return RedirectToAction("Index", ReturnController);
                }

                TempData["ECreate"] = "No se puede recargar saldo a la cuenta: " + modelCuenta.NumCuenta + " porque el cliente al que pertenece está dado de baja.";
                return RedirectToAction("Index", "Clientes");
            }
            catch (Exception ex)
            {
                TempData["ECreate"] = $"¡Ups! ocurrio un error inesperado, {ex.Message}";
                return RedirectToAction("Index", ReturnController);
            }
        }

        // GET: CuentasTelepeajes
        public ActionResult Index()
        {
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
        // [Bind(Include = "Id,NumCuenta,SaldoCuenta,TypeCuenta,StatusCuenta,StatusResidenteCuenta,DateTCuenta,ClienteId,IdCajero")]
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CuentasTelepeaje cuentasTelepeaje)
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
                                                    .OrderByDescending(x => x.DateTApertura).FirstOrDefaultAsync();

                    if (lastCorteUser != null)
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
                                    CorteId = lastCorteUser.Id,
                                    NoReferencia = await methods.RandomNumReferencia(),
                                };

                                if (cuentasTelepeaje.TypeCuenta == "Colectiva")
                                {
                                    detalle.TipoPago = "NOR";
                                    detalle.Monto = double.Parse(cuentasTelepeaje.SaldoCuenta, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," });

                                    var SaldoSend = cuentasTelepeaje.SaldoCuenta;
                                    SaldoSend = SaldoSend.Replace(",", string.Empty);
                                    cuentasTelepeaje.SaldoCuenta = SaldoSend.Replace(".", string.Empty);
                                }

                                db.OperacionesCajeros.Add(detalle);

                                db.CuentasTelepeajes.Add(cuentasTelepeaje);
                                await db.SaveChangesAsync();
                                TempData["SCreate"] = "Se registró correctamente la cuenta: " + cuentasTelepeaje.NumCuenta + " para el cliente: " + cliente.NumCliente + " " + cliente.Nombre + " " + cliente.Apellidos + ".";
                                return RedirectToAction("Index", "Clientes");
                            }
                            else
                            {
                                TempData["ECreate"] = "La cuenta: " + cuentasTelepeaje.NumCuenta + " ya existe!";
                                return RedirectToAction("Index", "Clientes");
                            }
                        }
                    }

                    TempData["ECreate"] = $"¡Ups! ocurrio un error inesperado.";
                    return RedirectToAction("Index", "Clientes");
                }

                TempData["ECreate"] = "El cliente no puede crear una cuenta porque esta dado de baja.";
                return RedirectToAction("Index", "Clientes");
            }
            catch (Exception ex)
            {
                TempData["ECreate"] = $"¡Ups! ocurrio un error inesperado, {ex.Message}";
                return RedirectToAction("Index", "Clientes");
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            db.Configuration.ValidateOnSaveEnabled = false;

            var cuentasTelepeaje = await (from cuentas in db.CuentasTelepeajes
                                          join tags in db.Tags on cuentas.Id equals tags.CuentaId into tagslist
                                          select new
                                          {
                                              cuentas,
                                              Tags = tagslist.ToList()
                                          }).SingleOrDefaultAsync(x => x.cuentas.Id == id);

            if (cuentasTelepeaje.cuentas.StatusCuenta == true)
            {
                switch (cuentasTelepeaje.cuentas.TypeCuenta)
                {
                    case "Colectiva":
                        cuentasTelepeaje.cuentas.StatusCuenta = false;
                        db.CuentasTelepeajes.Attach(cuentasTelepeaje.cuentas);
                        db.Entry(cuentasTelepeaje.cuentas).State = EntityState.Modified;

                        cuentasTelepeaje.Tags.ForEach(x =>
                        {
                            x.StatusTag = false;
                            db.Tags.Attach(x);
                            db.Entry(x).State = EntityState.Modified;
                        });
                        break;

                    case "Individual":
                        cuentasTelepeaje.cuentas.StatusCuenta = false;
                        db.CuentasTelepeajes.Attach(cuentasTelepeaje.cuentas);
                        db.Entry(cuentasTelepeaje.cuentas).State = EntityState.Modified;

                        cuentasTelepeaje.Tags.ForEach(x =>
                        {
                            x.StatusTag = false;
                            db.Tags.Attach(x);
                            db.Entry(x).State = EntityState.Modified;
                        });
                        break;
                    default:
                        break;
                }

                await db.SaveChangesAsync();
                TempData["SDelete"] = $"Se dio de baja correctamente la cuenta: {cuentasTelepeaje.cuentas.NumCuenta}.";
                return RedirectToAction("Index", "Clientes");
            }

            TempData["EDelete"] = "La cuenta ya esta dada de baja.";

            return RedirectToAction("Index", "Clientes");

        }

        // GET: CuentasTelepeajes/Habilitar/5
        public async Task<ActionResult> Habilitar(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var cuentasTelepeaje = await (from cuentas in db.CuentasTelepeajes
                                              join tags in db.Tags on cuentas.Id equals tags.CuentaId into tagslist
                                              where cuentas.Id == id
                                              select new
                                              {
                                                  cuentas,
                                                  Tags = tagslist.ToList(),
                                              }).FirstOrDefaultAsync();

                if (cuentasTelepeaje == null)
                {
                    return HttpNotFound();
                }

                db.Configuration.ValidateOnSaveEnabled = false;

                var FoundCliente = await db.CuentasTelepeajes.Join(
                       db.Clientes,
                       cue => cue.ClienteId,
                       cli => cli.Id,
                       (cue, cli) => new { cue, cli })
                       .Where(x => x.cli.Id == cuentasTelepeaje.cuentas.ClienteId)
                       .FirstOrDefaultAsync();

                if (FoundCliente.cli.StatusCliente == false)
                {
                    TempData["ECreate"] = "No se puede habilitar la cuenta: " + cuentasTelepeaje.cuentas.NumCuenta + " porque el cliente al que pertenece está dado de baja.";
                    return RedirectToAction("Index", "Clientes");
                }

                if (cuentasTelepeaje.cuentas.StatusCuenta == false)
                {
                    switch (cuentasTelepeaje.cuentas.TypeCuenta)
                    {
                        case "Colectiva":
                            if ((double.Parse(cuentasTelepeaje.cuentas.SaldoCuenta, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }) / 100) >= 100)
                            {
                                cuentasTelepeaje.cuentas.StatusCuenta = true;
                                db.CuentasTelepeajes.Attach(cuentasTelepeaje.cuentas);
                                db.Entry(cuentasTelepeaje.cuentas).State = EntityState.Modified;

                                cuentasTelepeaje.Tags.ForEach(x =>
                                {
                                    x.StatusTag = true;
                                    db.Tags.Attach(x);
                                    db.Entry(x).State = EntityState.Modified;
                                });
                            }
                            else
                            {
                                TempData["EDelete"] = $"No es posible habilitar la cuenta: {cuentasTelepeaje.cuentas.NumCuenta} por saldo insuficiente.";
                                return RedirectToAction("Index", "Clientes");
                            }
                            break;

                        case "Individual":

                            cuentasTelepeaje.cuentas.StatusCuenta = true;
                            db.CuentasTelepeajes.Attach(cuentasTelepeaje.cuentas);
                            db.Entry(cuentasTelepeaje.cuentas).State = EntityState.Modified;

                            cuentasTelepeaje.Tags.ForEach(x =>
                            {
                                if ((double.Parse(x.SaldoTag, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }) / 100) >= 20)
                                {
                                    x.StatusTag = true;
                                    db.Tags.Attach(x);
                                    db.Entry(x).State = EntityState.Modified;
                                }
                            });

                            break;
                        default:
                            break;
                    }

                    await db.SaveChangesAsync();

                    TempData["SDelete"] = $"Se dio de alta correctamente la cuenta: {cuentasTelepeaje.cuentas.NumCuenta}, junto con sus tags validos por saldo.";
                    return RedirectToAction("Index", "Clientes");
                }

                TempData["EDelete"] = $"La cuenta: {cuentasTelepeaje.cuentas.NumCuenta} ya esta dada de alta.";
                return RedirectToAction("Index", "Clientes");
            }
            catch (Exception ex)
            {
                TempData["EDelete"] = $"¡Ups! ha ocurrido un error inesperado, {ex.Message}";
                return RedirectToAction("Index", "Clientes");
            }
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
