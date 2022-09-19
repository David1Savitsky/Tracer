using System.Xml.Serialization;
using Tracer.Core.Interfaces;

namespace Tracer.Core.DataModels
{
    public class MethodInformation : IGetData
    {
        public string ClassName { get; set; }
        
        public string MethodName { get; set; }
        
        public TimeSpan Time { get; set; }
        
        public List<MethodInformation> Methods { get; set; } = new List<MethodInformation>();
        
        public object GetData()
        {
            return this;
        }
    }
}