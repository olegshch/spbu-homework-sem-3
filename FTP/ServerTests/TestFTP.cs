using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFTP;
using System.Threading.Tasks;

namespace ServerTests
{
    [TestClass]
    public class TestFTP
    {
        
        private Server server;
        private Client client;
        private string path= "../../../ServerTests";

        [TestInitialize]
        public void Initialize()
        {
            
            server = new Server(1234);
            server.Start();
            client = new Client("localhost", 1234);
            client.Connect();
        }

        [TestMethod]
        public async Task GetCommand()
        {
            var (size, data) = await client.Get(path + "/TestDir/example.txt");
            Assert.AreEqual("EF-BB-BF-74-65-73-74-69-6E-67-20-74-65-78-74", BitConverter.ToString(data));
            server.Stop();
            client.Dispose();
        }

        [TestMethod]
        public async Task FailureGetCommand()
        {
            var (size, data) = await client.Get(path + "/example.dll");
            Assert.AreEqual(-1, size);
            server.Stop();
            client.Dispose();
        }

        [TestMethod]
        public async Task ListCommandTestAsync()
        {
            var answer = await client.List(path);
            Assert.AreEqual(7, answer.Item1);
            Assert.AreEqual("TestDir", answer.Item2[6].Item1);
            Assert.AreEqual(true, answer.Item2[6].Item2);
            server.Stop();
            client.Dispose();
        }

        [TestMethod]
        public async Task FailureListCommandTestAsync()
        {
            var answer = await client.List(path + "abc.txt");
            Assert.AreEqual(-1, answer.Item1);
            client.Dispose();
        }

    }
}
