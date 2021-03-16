using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadPool
{
    /// <summary>
    /// Interface for MyTask
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// true if task is finished
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// rresult of task
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// new task which depends on result of current task
        /// </summary>
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
    }
}
