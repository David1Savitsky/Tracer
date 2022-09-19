using Tracer.Core.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace Tracer.Core.Tests;

public class TracerTests
{
   public class TracerServiceTests
    {
        
        internal class FooSingleThread
        {
            private readonly BarSingleThread _bar;
            private readonly ITracer _tracer;

            internal FooSingleThread(ITracer tracer)
            {
                _tracer = tracer;
                _bar = new BarSingleThread(_tracer);
            }

            public void CustomMethod()
            {
                _tracer.StartTrace();
                Thread.Sleep(100);
                _bar.M1();
                _bar.M2();
                _tracer.StopTrace();
            }
        }
        
        internal class BarSingleThread
        {
            private readonly ITracer _tracer;

            public BarSingleThread(ITracer tracer)
            {
                _tracer = tracer;
            }

            public void M1()
            {
                _tracer.StartTrace();
                Thread.Sleep(100);
                _tracer.StopTrace();
            }

            public void M2()
            {
                _tracer.StartTrace();
                Thread.Sleep(70);
                _tracer.StopTrace();
            }
        }
        
        internal class FooMultipleThreads
        {
            private readonly BarMultipleThreads _bar;
            private readonly ITracer _tracer;

            internal FooMultipleThreads(ITracer tracer)
            {
                _tracer = tracer;
                _bar = new BarMultipleThreads(_tracer);
            }

            public void CustomMethod()
            {
                _tracer.StartTrace();
                Thread.Sleep(100);
                var task = Task.Run(_bar.M1);
                _bar.M2();
                task.Wait();
                _tracer.StopTrace();
            }
        }
        
        internal class BarMultipleThreads
        {
            private readonly ITracer _tracer;

            public BarMultipleThreads(ITracer tracer)
            {
                _tracer = tracer;
            }

            public void M1()
            {
                _tracer.StartTrace();
                Thread.Sleep(100);
                _tracer.StopTrace();
            }

            public void M2()
            {
                _tracer.StartTrace();
                Thread.Sleep(70);
                _tracer.StopTrace();
            }
        }
        
        [Fact]
        public void SingleThread()
        {
            var tracer = new Tracer();
            var foo = new FooSingleThread(tracer);
            
            foo.CustomMethod();
            var result = tracer.GetTraceResult();
            Console.WriteLine(result);

            Assert.NotNull(result);
            Assert.NotNull(result.Threads);
            Assert.Equal(1, result.Threads.Count);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods);
            Assert.Equal(1, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods.Count);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods);
            Assert.Equal(2, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods.Count);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].Methods);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[1].Methods);
            Assert.Equal(0, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].Methods.Count);
            Assert.Equal(0, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[1].Methods.Count);
            Assert.Equal("CustomMethod", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].MethodName);
            Assert.Equal("FooSingleThread", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].ClassName);
            Assert.Equal("M1", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].MethodName);
            Assert.Equal("BarSingleThread", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].ClassName);
            Assert.Equal("M2", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[1].MethodName);
            Assert.Equal("BarSingleThread", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[1].ClassName);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Time.TotalMilliseconds >= 270);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Time.TotalMilliseconds >= 270);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].Time.TotalMilliseconds >= 100);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[1].Time.TotalMilliseconds >= 70);
        }

        [Fact]
        public void MultipleThreads()
        {
            var tracer = new Tracer();
            var foo = new FooMultipleThreads(tracer);

            foo.CustomMethod();
            var result = tracer.GetTraceResult();

            Assert.NotNull(result);
            Assert.NotNull(result.Threads);
            Assert.Equal(2, result.Threads.Count);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods);
            Assert.Equal(1, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods.Count);
            Assert.Equal(1, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods.Count);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods);
            Assert.Equal(1, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods.Count);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods);
            Assert.Equal(1, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods.Count);
            Assert.NotNull(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].Methods);
            Assert.Equal(0, result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].Methods.Count);
            Assert.Equal("CustomMethod", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].MethodName);
            Assert.Equal("FooMultipleThreads", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].ClassName);
            Assert.Equal("CustomMethod", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].MethodName);
            Assert.Equal("FooMultipleThreads", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].ClassName);
            Assert.Equal("M2", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].MethodName);
            Assert.Equal("BarMultipleThreads", result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].ClassName);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Time.TotalMilliseconds >= 170);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Time.TotalMilliseconds >= 170);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Methods[0].Time.TotalMilliseconds >= 70);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Time.TotalMilliseconds >= 100);
            Assert.True(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0].Time.TotalMilliseconds >= 100);
        }
    }
}