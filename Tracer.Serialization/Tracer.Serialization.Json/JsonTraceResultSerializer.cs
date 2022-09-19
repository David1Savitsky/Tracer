using System.Text;
using System.Text.Json;
using Tracer.Core.DataModels;
using Tracer.Serialization.Abstractions;
using Tracer.Serialization.Json.DataModels;

namespace Tracer.Serialization.Json;

public class JsonTraceResultSerializer: ITraceResultSerializer
{
    public void Serialize(TraceResult traceResult, Stream to)
    {
        var model = new TraceResultInfo(traceResult);
        var options = new JsonSerializerOptions {WriteIndented = true};
        //options.Converters.Add(new TimeSpanConverter(options));
        var json = System.Text.Json.JsonSerializer.Serialize(model, options);
        to.Write(Encoding.UTF8.GetBytes(json));
    }
}