using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CheckSum
{
    class Program
    {
        /// <summary>
        /// кодирование файла
        /// </summary>
        /// <param name="md5Hash">тип кодирования</param>
        /// <param name="filename">имя файла</param>
        /// <returns>зашифрованное содержимое</returns>
        static string Hash(MD5 md5Hash, string filename)
        {
            byte[] data = md5Hash.ComputeHash(File.ReadAllBytes(filename));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// рекурсивное кодироване папки
        /// </summary>
        /// <param name="dir">имя папки</param>
        /// <returns>зашифрованное содержимое</returns>
        static string Coding(string dir)
        {
            string result = "";

            //получение подкаталогов и рекурсивный спуск
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string dirname in dirs)
            {
                Coding(dirname);
            }

            //получение файлов и их кодирование
            string[] files = Directory.GetFiles(dir);
            using (MD5 md5Hash = MD5.Create())
            {
                result += Hash(md5Hash, dir);
                foreach (string filename in files)
                {
                    string hash = Hash(md5Hash, filename);
                    result += hash;
                }
            }           
            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path");
            string dirName = Console.ReadLine();
            Console.WriteLine(Coding(dirName));
            Console.ReadKey();
        }
    }
}
