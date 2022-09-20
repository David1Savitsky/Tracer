using Tracer.Core.Interfaces;

namespace Tracer.Core.DataModels
{
    public class TraceResult : IGetData
    {
        public Dictionary<int, ThreadInformation> Threads { get; set; }
        public object GetData()
        {
            return this;
        }
    }
} 