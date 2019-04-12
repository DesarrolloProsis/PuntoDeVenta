using RestServiceTelerik.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RestServiceTelerik.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [DataObject]
    public class CajeroController : ApiController
    {
        private static List<PropertiesDetalles> PropertiesDetalles { get; set; }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PropertiesDetalles> GetPropertiesDetalles()
        {
            return PropertiesDetalles;
        }

        [HttpPost]
        [ActionName("PostCajero")]
        public HttpResponseMessage PostCajero([FromBody]List<PropertiesDetalles> detalles, [FromUri]string authenticationToken)
        {
            try
            {
                var prop = new List<PropertiesDetalles>();

                if (detalles != null)
                {
                    detalles.ForEach(i => prop.Add(i));

                    PropertiesDetalles = prop;
                }
                return Request.CreateResponse(HttpStatusCode.OK, $"Success {authenticationToken}");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, $"Error {ex.Message} {ex.StackTrace} {authenticationToken}");
            }
        }
    }
}
