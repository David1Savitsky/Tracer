using System.Xml.Serialization;
using Tracer.Core.Interfaces;

namespace Tracer.Core.DataModels
{
    public class ThreadInformation : IGetData
    {
        public int Id { get; set; }

        public TimeSpan Time { get; set; }
        
        public List<MethodInformation> Methods { get; set; }
        
        public object GetData()
        {
            return this;
        }
    }
}