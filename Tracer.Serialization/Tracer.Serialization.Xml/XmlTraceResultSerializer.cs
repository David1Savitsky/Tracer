using System.Text;
using System.Xml.Serialization;
using Tracer.Core.DataModels;
using Tracer.Serialization.Abstractions;
using Tracer.Serialization.Xml.DataModels;

namespace Tracer.Serialization.Xml;

public class XmlTraceResultSerializer : ITraceResultSerializer
{
    public void Serialize(TraceResult traceResult, Stream to)
    {
        var serializer = new XmlSerializer(typeof(TraceResultInfo));
        var model = new TraceResultInfo(traceResult);
        serializer.Serialize(to, model);
    }
}