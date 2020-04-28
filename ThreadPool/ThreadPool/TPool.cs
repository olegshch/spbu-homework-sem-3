﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadPool
{
    /// <summary>
    /// MyThreadPool class
    /// </summary>
    public class TPool
    {
        public int ThreadNumber { get; }
        private BlockingCollection<Action> taskqueue = new BlockingCollection<Action>();
        private CancellationTokenSource token = new CancellationTokenSource();
        
        public TPool(int numberthreads)
        {
            ThreadNumber = numberthreads;
            StartThread();
        }

        /// <summary>
        /// Stopping threadpool work
        /// </summary>
        public void Shutdown()
        {
            token.Cancel();
            taskqueue.CompleteAdding();
        }

        /// <summary>
        /// Adding task to ThreadPool queue
        /// </summary>
        public IMyTask<TResult> Add<TResult>(Func<TResult> func)
        {
            if (!token.Token.IsCancellationRequested)
            {
                var task = new MyTask<TResult>(func, this);
                taskqueue.Add(task.Calculate);
                return task;
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Starting with "ThreadNumber" number of threads
        /// </summary>
        private void StartThread()
        {
            for(int i = 0; i < ThreadNumber; i++)
            {
                new Thread(()=> 
                {
                    while (true)
                    {
                        taskqueue.Take().Invoke();
                    }
                }).Start();
            }
        }

        /// <summary>
        /// MyTask class
        /// </summary>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private TPool pool;
            private object locker = new object();
            private Func<TResult> function;
            public bool IsCompleted { get; set; }
            public TResult Result { get; private set; }

            public MyTask(Func<TResult> task, TPool threadpool)
            {
                function = task;
                pool = threadpool;
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult,TNewResult> func)
            {
                var task = new MyTask<TNewResult>(() => func(Result), pool);
                if (IsCompleted)
                {
                    return pool.Add(() => func(Result));
                }
                return task;
            }

            public void Calculate()
            {
                Result = function();
                IsCompleted = true;
                function = null;
            }
        }

    }
}