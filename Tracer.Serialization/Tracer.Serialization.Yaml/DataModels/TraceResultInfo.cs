using Tracer.Core.DataModels;

namespace Tracer.Serialization.Yaml.DataModels;

public class TraceResultInfo
{
    public List<ThreadInfo> Threads { get; set; }

    public TraceResultInfo(TraceResult result)
    {
        Threads = new(result.Threads.Count);
        foreach (var thread in result.Threads)
        {
            var model = new ThreadInfo(thread.Value);
            Threads.Add(model);
        }
    }
}