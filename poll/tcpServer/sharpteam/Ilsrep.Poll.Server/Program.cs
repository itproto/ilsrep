using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.Server
{
    public class PollServer
    {
        public const int PORT = 3320;
        public const int DATA_SIZE = 65536;
        public const string PATH_TO_POLLS = "Polls.xml";
        public const string WELCOME = "Welcome to poll server.";

        public static void Main()
        {
            // Get local ip
            string localHost = System.Net.Dns.GetHostName();
            string localIp = System.Net.Dns.GetHostEntry(localHost).AddressList[0].ToString();

            // Start server
            Console.WriteLine("Server started on host: *:{1}", localIp, PORT);
            IPEndPoint clientAddress = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 3320);
            tcpListener.Start();
            Console.WriteLine("Waiting for clients...");
            while (true)
            {
                // Wait until client connect
                while (!tcpListener.Pending())
                {
                    Thread.Sleep(1000);
                }

                // Create a new thread for each client
                ConnectionThread newConnection = new ConnectionThread();
                newConnection.threadListener = tcpListener;
                Thread newThread = new Thread(new ThreadStart(newConnection.HandleConnection));
                newThread.Start();
            }
        }
    }
}

