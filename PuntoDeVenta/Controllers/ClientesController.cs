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
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;

namespace PuntoDeVenta.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public async Task<ActionResult> GetDataClientes()
        {
            List<Clientes> clientes = new List<Clientes>();

            var query = await db.Clientes.ToListAsync();

            foreach (var item in query)
            {
                var value = new Clientes
                {
                    Id = item.Id,
                    NumCliente = item.NumCliente,
                    NombreCompleto = $"{item.Nombre} {item.Apellidos}",
                    EmailCliente = item.EmailCliente,
                    AddressCliente = item.AddressCliente,
                    PhoneCliente = item.PhoneCliente,
                    StatusCliente = item.StatusCliente,
                    DateTCliente = item.DateTCliente
                };
                clientes.Add(value);
            }

            return Json(clientes, JsonRequestBehavior.AllowGet);
        }

        // GET: Clientes
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

            List<SelectListItem> listItemsCuentas = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Colectiva",
                    Value = "Colectiva"
                },

                new SelectListItem
                {
                    Text = "Individual",
                    Value = "Individual"
                }
            };

            ViewBag.TipoCuentas = new SelectList(listItemsCuentas.AsEnumerable(), "Value", "Text");

            return View();
        }

        // GET: Clientes/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = await db.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,NumCliente,Nombre,Apellidos,EmailCliente,AddressCliente,PhoneCliente,StatusCliente,DateTCliente,IdCajero")] Clientes clientes)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            clientes.NumCliente = await RandomNumClient();
            clientes.DateTCliente = DateTime.Now.Date;
            clientes.StatusCliente = true;
            clientes.IdCajero = User.Identity.GetUserId();

            ModelState.Remove("NumCliente");
            ModelState.Remove("IdCajero");

            if (ModelState.IsValid)
            {
                var nameclientes = clientes.Nombre + " " + clientes.Apellidos;

                var query = await db.Clientes.Where(x => x.NumCliente + " " + x.Apellidos == nameclientes || x.NumCliente == clientes.NumCliente || x.EmailCliente == clientes.EmailCliente || x.PhoneCliente == clientes.PhoneCliente).ToListAsync();

                if (query.Count <= 0)
                {
                    db.Clientes.Add(clientes);
                    await db.SaveChangesAsync();
                    TempData["SCreate"] = $"Se registró el contrato correctamente el cliente: {nameclientes}.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ECreate"] = "El cliente ya existe, verifique los datos.";
                    return RedirectToAction("Index");
                }
            }

            TempData["ECreate"] = "¡Ups! Hubo un error inesperado.";
            return RedirectToAction("Index");
        }

        // Método para crar num cliente
        public async Task<string> RandomNumClient()
        {
            return await Task.Run(() =>
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                return string.Format("{0:yy}{0:MM}{0:dd}{1}{0:ss}", DateTime.Now, randomNumber);
            });
        }

        // GET: Clientes/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = await db.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }

            ViewBag.Admin = new LoginViewModel();
            ViewBag.Exist = true;

            return View(clientes);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Clientes clientes)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            ModelState.Remove("IdCajero");
            if (ModelState.IsValid)
            {
                if (clientes.StatusCliente == true)
                {
                    db.Clientes.Attach(clientes);
                    db.Entry(clientes).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["SEdit"] = $"Se actualizó el contrato correctamente el cliente: {clientes.NumCliente}.";
                    return RedirectToAction("Index");
                }
                TempData["EEdit"] = "El cliente no puede ser actualizado porque está dado de baja.";
                return RedirectToAction("Index");
            }
            TempData["EEdit"] = "¡Ups! Hubo un error inesperado.";
            return RedirectToAction("Index");
        }

        // GET: Clientes/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = await db.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                Clientes clientes = await db.Clientes.FindAsync(id);

                if (clientes.StatusCliente == true)
                {
                    clientes.StatusCliente = false;
                    db.Clientes.Attach(clientes);
                    db.Entry(clientes).State = EntityState.Modified;

                    List<CuentasTelepeaje> cuentas = await db.CuentasTelepeajes.Where(x => x.ClienteId == clientes.Id).ToListAsync();
                    if (cuentas.Count > 0)
                    {
                        foreach (var item in cuentas)
                        {
                            item.StatusCuenta = false;

                            db.CuentasTelepeajes.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            List<Tags> tags = await db.Tags.Where(x => x.CuentaId == item.Id).ToListAsync();

                            tags.ForEach(a =>
                            {
                                a.StatusTag = false;
                            });
                        }
                    }

                    await db.SaveChangesAsync();

                    TempData["SDelete"] = $"Se dio de baja correctamente el cliente: {clientes.NumCliente}.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["EDelete"] = $"El cliente: {clientes.NumCliente} ya esta dado de baja.";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                TempData["EDelete"] = $"¡Ups! ha ocurrido un error inesperado, {ex.Message}";
                return RedirectToAction("Index");
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
