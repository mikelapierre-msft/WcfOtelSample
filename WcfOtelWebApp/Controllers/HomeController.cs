using OpenTelemetry.Instrumentation.Wcf;
using System;
using System.Configuration;
using System.Web.Mvc;
using WcfOtelWebApp.ServiceReference1;

namespace WcfOtelWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var client = new Service1Client("BasicHttpBinding_IService1", ConfigurationManager.AppSettings["ServiceUrl"]);
            client.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            ViewBag.Message = client.GetData(1);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}