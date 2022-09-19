using System.Reflection;
using Tracer.Core.DataModels;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization;

public class Service
{
    public void Upload(TraceResult result)
    {
        /*foreach (var filePath in Directory.EnumerateFiles("SerializerPlugins", "*.dll"))
        {
            var assembly = Assembly.LoadFrom(filePath);
            var serializerNamespace = filePath.Split('\\')[1];
            serializerNamespace = serializerNamespace[..serializerNamespace.LastIndexOf('.')];
            var serializerTypeName = serializerNamespace.Split('.')[2];
            var fullName = $"{serializerNamespace}.{serializerTypeName}TraceResultSerializer";
            var serializerType = assembly.GetType(fullName);

            var serializer = (ITraceResultSerializer)(Activator.CreateInstance(serializerType));
            using var stream = new FileStream($"rez.{serializerTypeName.ToLower()}", FileMode.Create);
            serializer.Serialize(result, stream);
        }*/
        
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