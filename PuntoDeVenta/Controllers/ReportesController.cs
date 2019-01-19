using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using PuntoDeVenta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PuntoDeVenta.Controllers
{
    public class ReportesController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationDbContext app = new ApplicationDbContext();

        // POST: Reportes
        public async Task<ActionResult> Index(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var result = db.CortesCajeros.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                return HttpNotFound();
            }

            // Cuando agreguemos el username cambiamos en el obj nomcajero a UserName del UserManager
            var _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(app));
            var user = await _UserManager.FindByIdAsync(result.IdCajero);

            var encabezado = new EncabezadoReporteCajero
            {
                Cajero = user.Email,
                NumCorte = result.NumCorte,
                Fecha = result.DateTApertura.ToString("dd/MM/yyyy"),
                HoraI = result.DateTApertura.ToString("HH:mm:ss"),
                HoraF = result.DateTCierre.Value.ToString("HH:mm:ss"),
                TotalMonto = result.MontoTotal.Value.ToString()
            };

            var movimientos = db.OperacionesCajeros.Where(x => x.CorteId == result.Id).ToList();

            var model = new List<object>();

            foreach (var item in movimientos)
            {
                model.Add(new
                {
                    Concepto = item.Concepto,
                    TipoPago = item.TipoPago,
                    Monto = item.Monto,
                    DataTOperacion = item.DateTOperacion,
                    Numero = item.Numero,
                    Tipo = item.Tipo,
                    CobroTag = item.CobroTag
                });
            }


            using (var client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(model);
                HttpContent postContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(new Uri("http://localhost:56342/api/cajero?authenticationToken=abcxyz"), postContent);
                var message = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            }

            return View("ReportViewerCajero", encabezado);
        }
    }
}