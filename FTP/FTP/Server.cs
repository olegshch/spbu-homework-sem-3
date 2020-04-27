using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MyFTP
{
    /// <summary>
    /// Server class
    /// </summary>
    public class Server
    {
        private TcpListener listener;
        private CancellationTokenSource token = new CancellationTokenSource();

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        /// <summary>
        /// starting server
        /// </summary>
        public async Task Start()
        {
            listener.Start();
            while (!token.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                Task.Run(() => Work(client));
            }
            listener.Stop();
        }

        /// <summary>
        /// Stopping server
        /// </summary>
        public void Stop()
        {
            token.Cancel();
        }

        /// <summary>
        /// Getting command and path
        /// </summary>
        /// <param name="message">pair of command and path in string format</param>
        private (string, string) Parse(string message)
        {
            var pair = message.Split();
            if (pair.Length < 2)
            {
                return (null, null);
            }
            return (pair[0], pair[1]);
        }

        /// <summary>
        /// Making a list of file and directories in string format
        /// </summary>
        /// <param name="path">current path</param>
        private (int, string) List(string path)
        {
            try
            {
                var dirInfo = new DirectoryInfo(path);
                var files = dirInfo.GetFiles();
                var dirNames = dirInfo.GetDirectories();
                return (files.Length + dirNames.Length, 
                    string.Join("", files.Select(name => $"{name.Name} False ")) 
                    + string.Join("", dirNames.Select(name => $"{name.Name} True ")));
            }
            catch (Exception exception) when (exception is IOException)
            {
                return (-1, null);
            }
        }

        /// <summary>
        /// Main process
        /// </summary>
        /// <param name="client">current client</param>
        private async Task Work(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                var reader = new StreamReader(stream);
                var message = await reader.ReadLineAsync();
                var (command, path) = Parse(message);
                switch (command)
                {
                    case "1":
                        await writer.WriteLineAsync($"{ List(path).Item1} {List(path).Item2}");
                        break;
                    case "2":
                        try
                        {
                            var data = File.OpenRead(path);
                            await writer.WriteLineAsync($"{data.Length}");
                            data.CopyTo(writer.BaseStream);
                            data.Close();
                            break;
                        }
                        catch (Exception exception) when (exception is IOException)
                        {
                            await writer.WriteLineAsync($"{-1}");
                        }
                        break;
                }
            }
        }
    }
}
