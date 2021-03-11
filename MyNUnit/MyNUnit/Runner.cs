using Attributes;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    public static class Runner
    {
        public static BlockingCollection<TestInfo> TestInformation { get; private set; }

        /// <summary>
        /// starts execution
        /// </summary>
        public static void Run(string path)
        {
            var types = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories).Select(Assembly.LoadFrom).ToHashSet().SelectMany(a => a.ExportedTypes);
            TestInformation = new BlockingCollection<TestInfo>();
            Parallel.ForEach(types, TryAll);
        }

        /// <summary>
        /// printing results
        /// </summary>
        public static void Print()
        {
            foreach (var test in TestInformation)
            {
                Console.WriteLine($"{test.Assembly}.{test.Name} " +
                    $"{(test.Ignore == null ? ((test.IsPassed ? "Passed" : "Failed") + ". Time:" + test.Time.ToString()) : " ignored due" + test.Ignore)}");
            }
        }

        /// <summary>
        /// executing all methods
        /// </summary>
        private static void TryAll(Type type)
        {
            ExecuteAllWith<BeforeClassAttribute>(type);
            ExecuteAllWith<TestAttribute>(type);
            ExecuteAllWith<AfterClassAttribute>(type);
        }

        /// <summary>
        /// executing specific attribute methods
        /// </summary>
        private static void ExecuteAllWith<AttributeType>(Type type, object obj = null) where AttributeType : Attribute
        {
            var methodsWithAttribute = type.GetTypeInfo().DeclaredMethods.Where(meti => Attribute.IsDefined(meti, typeof(AttributeType)));
            
            Action<MethodInfo> RunMethod;

            switch (typeof(AttributeType))
            {
                case Type attribute when attribute == typeof(TestAttribute):
                    RunMethod = ExecuteTestMethod;
                    break;

                case Type attribute when attribute == typeof(BeforeClassAttribute) || attribute == typeof(AfterClassAttribute)
                    || attribute == typeof(BeforeAttribute) || attribute == typeof(AfterAttribute):
                    RunMethod = meti => ExecuteOtherMethod(meti, obj, attribute);
                    break;
                default:
                    throw new InvalidProgramException("Unpredictable attribute");
            }

            Parallel.ForEach(methodsWithAttribute, RunMethod);

        }

        /// <summary>
        /// executions test method
        /// </summary>
        private static void ExecuteTestMethod(MethodInfo meti)
        {
            CheckMethod(meti);

            var attributes = Attribute.GetCustomAttribute(meti, typeof(TestAttribute)) as TestAttribute;

            if (attributes.Ignore != null)
            {
                TestInformation.Add(new TestInfo(meti.Name, meti.DeclaringType.FullName, 0, false, ignore: attributes.Ignore));
                return;
            }

            var constructor = meti.DeclaringType.GetConstructor(Type.EmptyTypes);

            var obj = constructor.Invoke(null);

            ExecuteAllWith<BeforeAttribute>(meti.DeclaringType, obj);

            var clock = Stopwatch.StartNew();
            bool isCrashed = true;
            try
            {
                meti.Invoke(obj, null);
                if (attributes.Expected == null)
                {
                    isCrashed = false;
                }
            }
            catch(Exception exception)
            {
                if(attributes.Expected == exception.InnerException.GetType())
                {
                    isCrashed = false;
                }
            }
            finally
            {
                clock.Stop();
                TestInformation.Add(new TestInfo(meti.Name, meti.DeclaringType.FullName, clock.ElapsedMilliseconds, !isCrashed, attributes.Expected, attributes.Ignore));
            }

            ExecuteAllWith<AfterAttribute>(meti.DeclaringType, obj);
        }

        /// <summary>
        /// executions non-test method
        /// </summary>
        private static void ExecuteOtherMethod(MethodInfo meti, object obj, Type attribute)
        {
            CheckMethod(meti);
            if ((attribute == typeof(BeforeClassAttribute) || attribute == typeof(AfterClassAttribute)) && !meti.IsStatic)
            {
                throw new InvalidOperationException($"{meti.Name} is not static");
            }
            meti.Invoke(obj, null);
        }

        /// <summary>
        /// checking methods for problems 
        /// </summary>
        private static void CheckMethod(MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().Length > 0)
            {
                throw new InvalidOperationException($"{methodInfo.Name} with InvalidOperationException (no params)");
            }
            else if (methodInfo.ReturnType != typeof(void))
            {
                throw new InvalidOperationException($"{methodInfo.Name} with InvalidOperationException (no value)");
            }
        }
    }
}
