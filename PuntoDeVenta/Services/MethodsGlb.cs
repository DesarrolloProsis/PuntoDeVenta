using PuntoDeVenta.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PuntoDeVenta.Services
{
    public class MethodsGlb
    {
        private AppDbContext db = new AppDbContext();
        public NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };

        /// <summary>
        /// Método para crear num referencia en operaciones cajeros.
        /// </summary>
        /// <returns></returns>
        public async Task<string> RandomNumReferencia()
        {
            try
            {
                var numreferencia = string.Empty;
                int num = 1;
                var date = DateTime.Today;

                var query = await db.OperacionesCajeros.Where(t => DbFunctions.TruncateTime(t.DateTOperacion) == date).ToListAsync();

                if (query.Count == 0)
                    numreferencia = string.Format("{0}", num.ToString("D7"));
                else
                {
                    var operaciones = query.OrderByDescending(i => i.Id).FirstOrDefault();

                    var numreferantiguo = operaciones.NoReferencia;

                    var convertnum = Convert.ToUInt64(numreferantiguo);

                    numreferencia = string.Format("{0}", (convertnum + 1).ToString("D7"));
                }

                return numreferencia;
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        public string RandomNumReferencia2()
        {
            try
            {
                var numreferencia = string.Empty;
                int num = 1;
                var date = DateTime.Today;

                var query = db.OperacionesCajeros.Where(t => DbFunctions.TruncateTime(t.DateTOperacion) == date).ToList();

                if (query.Count == 0)
                    numreferencia = string.Format("{0}", num.ToString("D7"));
                else
                {
                    var operaciones = query.OrderByDescending(i => i.Id).FirstOrDefault();

                    var numreferantiguo = operaciones.NoReferencia;

                    var convertnum = Convert.ToUInt64(numreferantiguo);

                    numreferencia = string.Format("{0}", (convertnum + 1).ToString("D7"));
                }

                return numreferencia;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}