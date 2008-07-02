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
        public const int DATA_SIZE = 65536;
        static public string pathToPolls;
        static public int port;
        static public IPAddress host;

        private static void ParseArgs(string[] args)
        {
            int curIndex = 0;
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-port":
                        try
                        {
                            port = Convert.ToInt32(args[curIndex+1]);
                        }
                        catch (Exception)
                        { 
                        }
                        break;
                    case "-host":
                        try
                        {
                            host = IPAddress.Parse(args[curIndex+1]);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case "-polls":
                        pathToPolls = args[curIndex+1];
                        try
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(pathToPolls);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Error: Invalid path to polls");
                            Console.ReadKey(true);
                            Environment.Exit(-1);
                        }
                        break;
                }
                curIndex++;
            }
        }

        public static void Main(string[] args)
        {
            // Set default host
            host = IPAddress.Any;
            // Set default port
            port = 3320;
            //Set default PathToPolls
            pathToPolls = "Polls.xml";

            ParseArgs(args);

            // Start server
            Console.WriteLine("Server started on host: {0}:{1}", host.ToString(), port);
            IPEndPoint clientAddress = new IPEndPoint(host, port);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

