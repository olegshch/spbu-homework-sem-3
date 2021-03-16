using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool.Tests
{
    [TestClass()]
    public class TPoolTests
    {
        private TPool pool;

        [TestMethod]
        public void CompletingTask()
        {
            pool = new TPool(1);
            var task = pool.Add(() => 2 * 2);
            pool.Shutdown();
            Assert.AreEqual(4, task.Result);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(pool.ClosedPool);
        }

        [TestMethod]
        public void OneThreadSeveralTasksTest()
        {
            pool = new TPool(1);
            var task1 = pool.Add(() => 2 + 3);
            var task2 = pool.Add(() => 8 * 8);
            var task3 = pool.Add(() => "123" + "456");
            Assert.AreEqual(5, task1.Result);
            Assert.AreEqual(64, task2.Result);
            Assert.AreEqual("123456", task3.Result);
            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
            Assert.IsTrue(task3.IsCompleted);
        }
        
        [TestMethod]
        public void UnexistingResultTest()
        {
            pool = new TPool(1);
            var list = new List<IMyTask<int>>();            
            list.Add(pool.Add(() =>
            {
                Thread.Sleep(1000);
                return 4;
            }));
            
            pool.Shutdown();
            Assert.AreEqual(4, list[0].Result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TaskAfterShutdownTest()
        {
            pool = new TPool(1);
            pool.Shutdown();
            var task1 = pool.Add(() => 2 + 3);           
        }

        [TestMethod]
        public void NumberOfThreads()
        {
            var threadPool = new TPool(5);
            int numberOfEvaluatedTasks = 0;
            for (var i = 0; i < 10; ++i)
            {
                threadPool.Add(() =>
                {
                    Interlocked.Increment(ref numberOfEvaluatedTasks);
                    Thread.Sleep(2000);
                    return 5;
                });
            }

            Thread.Sleep(500);
            Assert.IsTrue(numberOfEvaluatedTasks == threadPool.ThreadNumber);
            threadPool.Shutdown();
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void NotAddToThreadPoolAfterShutdown()
        {
            var threadPool = new TPool(5);
            threadPool.Shutdown();
            threadPool.Add(() => 1);
        }

        [TestMethod]
        public void ShutdownDoesNotStopEvaluatingTasks()
        {
            var threadPool = new TPool(5);
            int numberOfEvaluatedTasks = 0;

            for (var i = 0; i < 5; ++i)
            {
                threadPool.Add(() =>
                {
                    Interlocked.Increment(ref numberOfEvaluatedTasks);
                    Thread.Sleep(500);
                    return 5;
                });
            }

            Thread.Sleep(100);
            threadPool.Shutdown();
            Thread.Sleep(400);
            Assert.AreEqual(threadPool.ThreadNumber, numberOfEvaluatedTasks);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ResultThrowRightExceptionOnError()
        {
            var threadPool = new TPool(5);
            var result = threadPool.Add<object>(() => throw new NullReferenceException()).Result;
        }

        [TestMethod]
        public void ContinueWithWork()
        {
            var threadPool = new TPool(5);
            var task = threadPool.Add(() => true);
            var flag = false;
            task.ContinueWith((x) =>
            {
                flag = x;
                return x;
            });

            Thread.Sleep(500);
            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void ContinueWithTaskCalculatesAfterMainTask()
        {
            var threadPool = new TPool(5);
            var flag = false;
            var task = threadPool.Add(() =>
            {
                flag = false;
                return 2;
            });

            task.ContinueWith((x) =>
            {
                flag = true;
                return x;
            });
            Thread.Sleep(200);
            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void ContinueWithNotBlockThread()
        {
            var threadPool = new TPool(5);
            var isThreadBlock = true;
            var task = threadPool.Add(() =>
            {
                Thread.Sleep(200);
                return 1;
            });

            task.ContinueWith((x) => 2);

            if (!task.IsCompleted)
            {
                isThreadBlock = false;
            }
            Assert.IsFalse(isThreadBlock);
        }

        [TestMethod]
        public void ContinueWithQueuesNewTasks()
        {
            var threadPool = new TPool(5);
            var task = threadPool.Add(() => 5);
            var list = new List<int> { 1, 2, 3, 4, 5 };
            for (var i = 0; i < list.Count; i++)
            {
                task.ContinueWith((x) =>
                {
                    list[i] += x;
                    Assert.AreEqual(i + 6, list[i]);
                    return list[i];
                });
            }
            Thread.Sleep(200);
            threadPool.Shutdown();
        }

        [TestMethod]
        public void ParallelTest()
        {
            var threadPool = new TPool(15);
            var result = 0;
            for (var i = 0; i < 15; i++)
            {
                threadPool.Add(() =>
                {
                    result++;
                    return 0;
                });
            }

            threadPool.Shutdown();
            Assert.AreEqual(15, result);
        }
    }
}