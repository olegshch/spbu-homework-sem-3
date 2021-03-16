using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool
{
    /// <summary>
    /// MyThreadPool class
    /// </summary>
    public class TPool
    {
        public int ThreadNumber { get; }
        private BlockingCollection<Action> taskQueue = new BlockingCollection<Action>();
        private CancellationTokenSource token = new CancellationTokenSource();
        private int finishedThreads = 0;
        private object locker = new object();
        private AutoResetEvent shutdownSignal = new AutoResetEvent(false);

        public TPool(int numberThreads)
        {
            ThreadNumber = numberThreads;
            StartThread(numberThreads);
        }

        /// <summary>
        /// Stopping threadpool work
        /// </summary>
        public void Shutdown()
        {            
            token.Cancel();
            lock (locker)
            {
                taskQueue?.CompleteAdding();
                while (!ClosedPool)
                {
                    shutdownSignal.WaitOne();        
                }
                taskQueue = null;
            }            
        }

        /// <summary>
        /// true if all threads have been closed
        /// </summary>
        public bool ClosedPool => ThreadNumber == finishedThreads;

        /// <summary>
        /// Adding task to ThreadPool queue
        /// </summary>
        public IMyTask<TResult> Add<TResult>(Func<TResult> func)
        {            
            if (token.Token.IsCancellationRequested)
            {
                throw new InvalidOperationException();
            }
            var task = new MyTask<TResult>(func, this);
            try
            {                    
                taskQueue.Add(task.Calculate, token.Token);                    
            }
            catch 
            {
                throw new InvalidOperationException();
            }
            return task;           
        }

        /// <summary>
        /// Starting with "ThreadNumber" number of threads
        /// </summary>
        private void StartThread(int number)
        {
            for (int i = 0; i < number; ++i)
            {
                new Thread(()=> 
                {
                    while (true)
                    {
                        if (token.Token.IsCancellationRequested)
                        {
                            Interlocked.Increment(ref finishedThreads);
                            shutdownSignal.Set();
                            break;
                        }
                        if (taskQueue.TryTake(out Action action))
                        {
                            action.Invoke();
                        }
                        //taskQueue?.Take().Invoke();
                    }                                                                                                                          
                }).Start();
            }
        }

        private Action ActionAdd(Action action)
        {
            taskQueue.Add(action, token.Token);
            return action;
        }

        /// <summary>
        /// MyTask class
        /// </summary>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private TPool pool;
            private ManualResetEvent flag = new ManualResetEvent(false);
            private object locker = new object();
            private Func<TResult> function;
            private Queue<Action> local;
            private TResult result;
            private AggregateException exception;
            public bool IsCompleted { get; set; } = false;
            public TResult Result
            {
                get
                {
                    flag.WaitOne();
                    if (exception == null)
                    {
                        return result;
                    }
                    throw exception;
                }
            }
            
            public MyTask(Func<TResult> task, TPool threadpool)
            {
                function = task;
                pool = threadpool;
                local = new Queue<Action>();
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult,TNewResult> func)
            {
                var task = new MyTask<TNewResult>(() => func(Result), pool);
                lock (locker)
                {
                    if (IsCompleted)
                    {
                        return pool.Add(() => func(Result));
                    }
                    local.Enqueue(task.Calculate);
                    return task;
                }
                
            }

            public void Calculate()
            {
                try
                {
                    result = function();
                }
                catch (Exception calcException)
                {
                    exception = new AggregateException(calcException);
                }
                finally
                {
                    lock (locker)
                    {
                        IsCompleted = true;
                        function = null;
                        flag.Set();
                        while (local.Count != 0)
                        {
                            pool.ActionAdd(local.Dequeue());
                        }
                    }
                }
            }
        }

    }
}
