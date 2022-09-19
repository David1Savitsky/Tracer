using Tracer.Core.Interfaces;
using Tracer.Serialization;

namespace Tracer.Example;

class Program
{
    private static ITracer _tracer;
        
    static void MethodA()
    {
        _tracer.StartTrace();
        MethodB();
        Thread.Sleep(500);
        _tracer.StopTrace();
    }

    static void MethodB()
    {
        _tracer.StartTrace();
        Thread.Sleep(300);
        _tracer.StopTrace();
    }

    static void MethodC()
    {
        MethodB();
        MethodA();
    }
        
        
    static void Main(string[] args)
    {
        /*_tracer = new Core.Tracer();
        MethodC();
        MethodB();
        var thread = new Thread(MethodC);
        thread.Start();
        Thread.Sleep(1000);
        using var stream = new FileStream("rez.xml", FileMode.Create);
        new XmlSerializer().Serialize(_tracer.GetTraceResult(), stream);*/

        /*_tracer = new Core.Tracer();
        MethodC();
        MethodB();
        var thread = new Thread(MethodC);
        thread.Start();
        Thread.Sleep(5000);*/
        var tracer = new Core.Tracer();
        var template = new FirstThread(tracer); 
        template.CustomMethod();
        var result = tracer.GetTraceResult();
        Service service = new Service();
        service.Upload(result);
    }
}

internal class SecondThread
{
    private readonly ITracer _tracer;

    public SecondThread(ITracer tracer)
    {
        _tracer = tracer;
    }

    public void FirstMethod()
    {
        _tracer.StartTrace();
        Thread.Sleep(100);
        _tracer.StopTrace();
    }

    public void SecondMethod()
    {
        _tracer.StartTrace();
        Thread.Sleep(70);
        _tracer.StopTrace();
    }
}

internal class FirstThread
{
    private readonly SecondThread _secondThread;
    private readonly ITracer _tracer;

    internal FirstThread(ITracer tracer)
    {
        _tracer = tracer;
        _secondThread = new SecondThread(_tracer);
    }

    public void CustomMethod()
    {
        _tracer.StartTrace();
        Thread.Sleep(100);
        var task = Task.Run(_secondThread.FirstMethod);
        _secondThread.SecondMethod();
        _secondThread.FirstMethod();
        task.Wait();
        _tracer.StopTrace();
    }
}