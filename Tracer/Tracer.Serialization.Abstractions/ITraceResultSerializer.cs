using Tracer.Core.DataModels;

namespace Tracer.Serialization.Abstractions;

public interface ITraceResultSerializer
{
    public void Serialize(TraceResult traceResult, Stream to);
}