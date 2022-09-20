using System.Reflection;
using Tracer.Core.DataModels;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization;

public class Service
{
    public void Upload(TraceResult result)
    {
        var files = Directory.EnumerateFiles("SerializerPlugins", "*.dll");
        foreach (var file in files)
        {
            var serializerAssembly = Assembly.LoadFrom(file);
            var types = serializerAssembly.GetTypes();
            foreach (var type in types)
            {
                if (type.GetInterface(nameof(ITraceResultSerializer)) == null) 
                    continue;
                var serializer = (ITraceResultSerializer?)Activator.CreateInstance(type);
                if (serializer == null)
                {
                    throw new Exception($"Serializer {type.ToString()} has not been created");
                }
                var serializerTypeName = type.ToString().Split('.')[2];
                using var fileStream = new FileStream($"rez.{serializerTypeName.ToLower()}", FileMode.Create);
                serializer.Serialize(result, fileStream);
            }
        }
    }
}