using Tracer.Core.DataModels;

namespace Tracer.Serialization.Xml.DataModels;

public class ThreadInfo
{
    public int Id { get; set; }

    public string Time { get; set; }

    public List<MethodInfo> Methods { get; set; }

    public ThreadInfo()
    {
    }

    public ThreadInfo(ThreadInformation thread)
    {
        Id = thread.Id;
        Time = $"{thread.Time.ToString().Substring(9, thread.Time.ToString().Length - 13)}ms";
        Methods = new(thread.Methods.Count);
        foreach (var method in thread.Methods)
        {
            var model = new MethodInfo(method);
            Methods.Add(model);
        }
    }
}