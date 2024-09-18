using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WcfOtelWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private TracerProvider tracerProvider;
        private MeterProvider meterProvider;
        // The LoggerFactory needs to be accessible from the rest of your application.
        internal static ILoggerFactory loggerFactory;

        protected void Application_Start()
        {
            var resourceAttributes = new Dictionary<string, object> {
                { "service.name", ConfigurationManager.AppSettings["OtelServiceName"] },
                { "service.namespace", ConfigurationManager.AppSettings["OtelServiceNamespace"] },
                { "service.instance.id", $"{ConfigurationManager.AppSettings["OtelNodeName"]}/{ConfigurationManager.AppSettings["OtelInstance"]}" }
            };

            var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);

            this.tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetInstrumentation()                
                .AddHttpClientInstrumentation()
                .AddWcfInstrumentation()
                .AddAzureMonitorTraceExporter(c => c.ConnectionString = ConfigurationManager.AppSettings["ApplicationInsightsConnectionString"])
                .Build();

            this.meterProvider = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAzureMonitorMetricExporter(c => c.ConnectionString = ConfigurationManager.AppSettings["ApplicationInsightsConnectionString"])
                .Build();

            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddOpenTelemetry();
            });

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            this.tracerProvider?.Dispose();
            this.meterProvider?.Dispose();
            loggerFactory?.Dispose();
        }
    }
}
