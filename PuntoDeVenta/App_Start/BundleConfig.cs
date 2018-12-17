﻿using System.Web;
using System.Web.Optimization;

namespace PuntoDeVenta
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/css-yisus/estilos.css",
                        "~/Content/css-yisus/intl-tel-input-master/build/css/intlTelInput.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                        "~/Scripts/popper.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/intlTelInput-jquery.min").Include(
                        "~/Content/css-yisus/intl-tel-input-master/build/js/intlTelInput-jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/intlTelInput.min.js").Include(
                        "~/Content/css-yisus/intl-tel-input-master/build/js/intlTelInput.min.js"));
        }
    }
}
