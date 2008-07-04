using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using log4net;
using log4net.Config;

namespace Ilsrep.PollApplication.Server
{
    public class PollServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PollServer));
        public const string PATH_TO_LOG_CONFIG = "LogConfig.xml";
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
                        catch (Exception exception)
                        {
                            log.Error("Invalid port used:" + args[curIndex + 1] + Environment.NewLine + "\t\tDefault port loaded:" + port,exception);
                        }
                        break;
                    case "-host":
                        try
                        {
                            host = IPAddress.Parse(args[curIndex+1]);
                        }
                        catch (Exception exception)
                        {
                            log.Error("Invalid host used:" + args[curIndex + 1] + Environment.NewLine + "\t\tDefault host loaded:" + IPAddress.Any.ToString(),exception);
                        }
                        break;
                    case "-polls":
                        try
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(args[curIndex + 1]);
                            pathToPolls = args[curIndex + 1];
                        }
                        catch (Exception exception)
                        {
                            log.Error("Invalid path to polls:" + args[curIndex + 1] + Environment.NewLine + "\t\tDefault path to polls loaded:" + pathToPolls,exception);
                        }
                        break;
                }
                curIndex++;
            }
        }

        public static void Main(string[] args)
        {
            // Configure logger
            XmlConfigurator.Configure(new System.IO.FileInfo(PATH_TO_LOG_CONFIG));

            // Set default host
            host = IPAddress.Any;
            // Set default port
            port = 3320;
            //Set default PathToPolls
            pathToPolls = "Polls.xml";

            ParseArgs(args);

            // Start server
            log.Info("Server started on host: " + host.ToString() + ":" + port);
            IPEndPoint clientAddress = new IPEndPoint(host, port);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Bind(clientAddress);
            log.Info("Waiting for clients...");
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

