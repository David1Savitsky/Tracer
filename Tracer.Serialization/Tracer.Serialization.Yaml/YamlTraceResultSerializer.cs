using System.Text;
using Tracer.Core.DataModels;
using Tracer.Serialization.Abstractions;
using Tracer.Serialization.Yaml.DataModels;
using YamlDotNet.Serialization;

namespace Tracer.Serialization.Yaml;

public class YamlTraceResultSerializer : ITraceResultSerializer
{
    public void Serialize(TraceResult traceResult, Stream to)
    {
        var model = new TraceResultInfo(traceResult);
        var serializer = new SerializerBuilder().Build();
        var yaml = serializer.Serialize(model);
        to.Write(Encoding.UTF8.GetBytes(yaml));
    }
}