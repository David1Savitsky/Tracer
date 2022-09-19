using System.Reflection;
using Tracer.Core.DataModels;

namespace Tracer.Serialization.Json.DataModels;

public class MethodInfo
{
    public string Name { get; set; }

    public string Class { get; set; }

    public string Time { get; set; }

    public List<MethodInfo> Methods { get; set; }

    public MethodInfo(MethodInformation method)
    {
        Name = method.MethodName;
        Class = method.ClassName;
        Time = $"{method.Time.ToString().Substring(9, method.Time.ToString().Length - 13)}ms";
        Methods = new(method.Methods.Count);
        foreach (var m in method.Methods)
        {
            var model = new MethodInfo(m);
            Methods.Add(model);
        }
    }
}