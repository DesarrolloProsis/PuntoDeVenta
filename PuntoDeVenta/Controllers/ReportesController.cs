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

        // POST: Reportes
        public async Task<ActionResult> Index(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var result = db.OperacionesCajeros.Where(x => x.CorteId == 64).ToList();

            if (result == null)
            {
                return HttpNotFound();
            }

            var model = new List<object>();

            foreach (var item in result)
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

            return View("ReportViewerCajero");
        }
    }
}