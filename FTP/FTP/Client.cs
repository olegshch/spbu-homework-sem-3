using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyFTP
{
    /// <summary>
    /// Client class
    /// </summary>
    public class Client : IDisposable
    {
        private TcpClient client;
        private StreamWriter writer;
        private StreamReader reader;
        private string host;
        private int port;

        public Client(string host, int port)
        {
            client = new TcpClient();
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Connecting
        /// </summary>
        public void Connect()
        {
            client.Connect(host, port);
            var stream = client.GetStream();
            writer = new StreamWriter(stream) { AutoFlush = true };
            reader = new StreamReader(stream);           
        }

        /// <summary>
        /// Sending message
        /// </summary>
        /// <param name="message">message text</param>
        private async Task Write(string message) => await writer.WriteLineAsync(message);
        
        /// <summary>
        /// Getting message
        /// </summary>
        private async Task<string> Read() => await reader.ReadLineAsync();

        /// <summary>
        /// Closing client
        /// </summary>
        public void Dispose() => client.Dispose();

        /// <summary>
        /// Get command
        /// </summary>
        /// <param name="path">file path</param>
        public async Task<(int, byte[])> Get(string path)
        {
            await Write("2 " + path);
            int size = int.Parse(await Read());
            if (size == -1)
            {
                return (-1, null);
            }
            var data = new byte[size];
            await reader.BaseStream.ReadAsync(data, 0, size);
            return (size, data);
        }

        /// <summary>
        /// Listing command
        /// </summary>
        /// <param name="path">current path</param>
        public async Task<(int, List<(string, bool)>)> List(string path)
        {
            await Write("1 " + path);
            var message = await Read();
            if (message == "-1")
            {
                return (-1, null);
            }
            var split = message.Split();
            var list = new List<(string, bool)>();
            for (int i = 1; i < split.Length - 1; i += 2)
            {
                list.Add((split[i], Convert.ToBoolean(split[i + 1])));
            }
            return (Convert.ToInt32(split[0]), list);
        }
    }
}
