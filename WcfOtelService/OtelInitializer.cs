using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Collections.Generic;
using System.Configuration;

namespace WcfOtelService
{
    public class OtelInitializer
    {
        public static readonly OtelInitializer Instance = new OtelInitializer();

        private OtelInitializer() 
        {
            var resourceAttributes = new Dictionary<string, object> {
                { "service.name", ConfigurationManager.AppSettings["OtelServiceName"] },
                { "service.namespace", ConfigurationManager.AppSettings["OtelServiceNamespace"] },
                { "service.instance.id", $"{ConfigurationManager.AppSettings["OtelNodeName"]}/{ConfigurationManager.AppSettings["OtelInstance"]}" }
            };

            var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);

            var openTelemetry = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddAzureMonitorTraceExporter(c => c.ConnectionString = ConfigurationManager.AppSettings["ApplicationInsightsConnectionString"])
                .AddWcfInstrumentation()
                .Build();
        }

        public static OtelInitializer EnsureInitialized() 
        {
            return Instance;
        }
    }
}