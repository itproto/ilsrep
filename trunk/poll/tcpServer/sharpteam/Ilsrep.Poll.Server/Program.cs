using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;

namespace Ilsrep.PollApplication.Server
{
    public class PollServer
    {
        public const int PORT = 3320;
        public const int DATA_SIZE = 65536;
        public const string PATH_TO_POLLS = "Polls.xml";

        public static void Main()
        {
            // Get local ip
            string localHost = System.Net.Dns.GetHostName();
            string localIp = System.Net.Dns.GetHostEntry(localHost).AddressList[1].ToString();
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Start server
            Console.WriteLine("Server started on host: {0}:{1}", localIp, PORT);
            IPEndPoint clientAddress = new IPEndPoint(IPAddress.Any, PORT);
            client.Bind(clientAddress);
            Console.WriteLine("Waiting for clients...");
            while (true)
            {
                client.Listen(10);
                PollHandler newConnection = new PollHandler();
                newConnection.currentClient.Client = client.Accept();

                // Create a new thread if client is connected else wait
                if (newConnection.currentClient.Client.Connected)
                {
                    Thread newThread = new Thread(new ThreadStart(newConnection.HandleConnection));
                    newThread.Start();
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}

