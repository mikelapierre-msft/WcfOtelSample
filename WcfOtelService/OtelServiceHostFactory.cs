using System;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Activation;
using OpenTelemetry.Instrumentation.Wcf;

namespace WcfOtelService
{    
    public class OtelServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            OtelInitializer.EnsureInitialized();
            var host = base.CreateServiceHost(serviceType, baseAddresses);
            if (!host.Description.Behaviors.Contains(typeof(TelemetryServiceBehavior)))
                host.Description.Behaviors.Add(new TelemetryServiceBehavior());
            return host;
        }

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            OtelInitializer.EnsureInitialized();
            var host = base.CreateServiceHost(constructorString, baseAddresses);
            if (!host.Description.Behaviors.Contains(typeof(TelemetryServiceBehavior)))
                host.Description.Behaviors.Add(new TelemetryServiceBehavior());
            return host;
        }
    }
}