using System;
using System.IO;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No path");
                return;
            }

            var path = args[0];

            try
            {
                Console.WriteLine("Execution started");
                MyNUnit.Runner.Run(path);
                MyNUnit.Runner.Print();
                Console.WriteLine("Execution finished");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("No directory");
            }
        }
    }
}
