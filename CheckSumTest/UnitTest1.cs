using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CheckSumTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestingHashFunction()
        {

            MD5 md5Hash = MD5.Create();
            string filename = "testfile.txt";
                byte[] data = md5Hash.ComputeHash(File.ReadAllBytes(filename));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                Assert.AreNotEqual(null, sBuilder.ToString());           
        }

        [TestMethod]
        public void TestingCodingFunction()
        {
            string Hash(MD5 md5Hash, string filename)
            {
                byte[] data = md5Hash.ComputeHash(File.ReadAllBytes(filename));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }

            string Coding(string dir)
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
            Assert.AreNotEqual(null, Coding("testfile.txt"));
        }
    }
}
