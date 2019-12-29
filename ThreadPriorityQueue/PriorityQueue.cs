using System.Threading;

namespace ThreadPriorityQueue
{
    /// <summary>
    /// очередь с приоритетом
    /// </summary>
    public class PriorityQueue
    {
        /// <summary>
        /// узел со значением и приоритетом
        /// </summary>
        private class Node
        {
            public int Data { get; set; }
            public int Priority { get; set; }
            public Node Next { get; set; }
            public Node(int value, int priority)
            {
                Data = value;
                Priority = priority;
            }
        }

        private Node head;

        public int Count { get; private set; }

        public int Size() => Count;

        private object locker = new object();

        /// <summary>
        /// получение узла
        /// </summary>
        /// <param name="priority">ближайший приоритет</param>
        /// <returns>нужный узел</returns>
        private Node Get(int priority)
        {
            lock (locker)
            {
                Node current = head;
                if (Count < 2) return current;
                for (int i = 0; i < Count - 1; i++)
                {
                    if (current.Next.Priority < priority)
                    {
                        return current;
                    }
                    current = current.Next;
                }
                return current.Next;
            }
        }

        /// <summary>
        /// добавление с блокированием
        /// </summary>
        /// <param name="value">значение</param>
        /// <param name="priority">приоритет</param>
        /// <returns>true при успешном добавлении</returns>
        public bool Enqueue(int value, int priority)
        {
            lock (locker)
            {
                Node newNode = new Node(value, priority);
                if (Count == 0)
                {
                    head = newNode;
                }
                else
                {
                    var current = Get(priority);
                    newNode.Next = current.Next;
                    current.Next = newNode;
                }
                Count++;
                if (Count == 1)
                {
                    Monitor.PulseAll(locker);
                }

                return true;
            }
        }

        /// <summary>
        /// удаление с ожиданием в пустой очереди
        /// </summary>
        /// <returns></returns>
        public int Deque()
        {
            lock (locker)
            {
                while (Count == 0)
                {
                    Monitor.Wait(locker);
                }
                int result = head.Data;
                head = head.Next;
                Count--;
                return result;
            }
        }
    }
}
