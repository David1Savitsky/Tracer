using System.Collections.Concurrent;
using System.Diagnostics;
using Tracer.Core.DataModels;
using Tracer.Core.Interfaces;

namespace Tracer.Core
{
    public class Tracer : ITracer
    {
        //private readonly Mutex _mutex = new();
        private class TrackedMethod
        {
            public Stopwatch Stopwatch;
            public MethodInformation MethodInformation;
        }
        
        private TraceResult _trace = new() {Threads = new Dictionary<int, ThreadInformation>()};
        private readonly ConcurrentDictionary<int, Stack<TrackedMethod>> _threads = new(); 

        public void StartTrace()
        {
            var trackedMethod = new TrackedMethod {Stopwatch = new(), MethodInformation = new()};

            var stackFrame = new StackFrame(1);
            trackedMethod.MethodInformation.MethodName = stackFrame.GetMethod()?.Name;
            trackedMethod.MethodInformation.ClassName = stackFrame.GetMethod()?.ReflectedType?.Name;

            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_threads.TryGetValue(threadId, out var trackedStack))
            {
                trackedStack = new Stack<TrackedMethod>();
                trackedStack.Push(trackedMethod);
                _threads.TryAdd(threadId, trackedStack);
            }
            else
            {
                if (trackedStack.Count != 0) 
                    trackedStack.Peek().MethodInformation.Methods.Add(trackedMethod.MethodInformation);
                _threads[threadId].Push(trackedMethod);
            }
            
            trackedMethod.Stopwatch.Start();
        }

        public void StopTrace()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var trackedMethod = _threads[threadId].Pop();
            
            trackedMethod.Stopwatch.Stop();
            trackedMethod.MethodInformation.Time = trackedMethod.Stopwatch.Elapsed;

            //_mutex.WaitOne();
            if (!_trace.Threads.TryGetValue(threadId, out var threadInformation))
            {
                _trace.Threads[threadId] = new ThreadInformation() {
                        Id = threadId, 
                        Time = TimeSpan.Zero, 
                        Methods = new List<MethodInformation>()
                };
            }

            if (_threads[threadId].Count == 0)
            {
                _trace.Threads[threadId].Methods.Add(trackedMethod.MethodInformation);
            }

            _trace.Threads[threadId].Time = TimeSpan.Zero;
            foreach (var method in _trace.Threads[threadId].Methods)
            {
                _trace.Threads[threadId].Time = _trace.Threads[threadId].Time.Add(method.Time);
            }
            
            //_mutex.ReleaseMutex();
        }

        public TraceResult GetTraceResult()
        {
           // _mutex.WaitOne();
            var result = (TraceResult)_trace.GetData();
            //_mutex.ReleaseMutex();
            return result;
        }
    }
}