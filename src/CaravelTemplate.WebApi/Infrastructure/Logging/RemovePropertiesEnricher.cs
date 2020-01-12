using Serilog.Core;
using Serilog.Events;

namespace CaravelTemplate.WebApi.Infrastructure.Logging
{
    class RemovePropertiesEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
        {
            le.RemovePropertyIfPresent("SourceContext");
            le.RemovePropertyIfPresent("RequestId");
            le.RemovePropertyIfPresent("RequestPath");
        }
    }
}