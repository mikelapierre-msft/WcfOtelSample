using OpenTelemetry.Instrumentation.Wcf;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Web.Mvc;
using WcfOtelWebApp.ServiceReference1;

namespace WcfOtelWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var wcfClient = new Service1Client("BasicHttpBinding_IService1", ConfigurationManager.AppSettings["ServiceUrl"]);
            wcfClient.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());
            try
            {
                ViewBag.Message = wcfClient.GetData(1);
                wcfClient.Close();
            }
            catch (TimeoutException)
            {
                // Handle the timeout exception.
                wcfClient.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                // Handle the communication exception.
                wcfClient.Abort();
                throw;
            }            
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