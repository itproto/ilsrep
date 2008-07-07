using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using log4net;
using log4net.Config;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common;

namespace Ilsrep.PollApplication.PollServer
{
    public class PollServer
    {
        // Initial declarations
        private static readonly ILog log = LogManager.GetLogger(typeof(PollServer));
        public const string PATH_TO_LOG_CONFIG = "LogConfig.xml";
        public const string PATH_TO_POLL_ID = "Poll_id.xml";
        public const int DATA_SIZE = 65536;
        static public string pathToPolls;
        static public int port;
        static public IPAddress host;

        public static void Main(string[] args)
        {
            // Configure logger
            XmlConfigurator.Configure(new System.IO.FileInfo(PATH_TO_LOG_CONFIG));

            // Set default host
            host = IPAddress.Any;
            // Set default port
            port = 3320;
            //Set default PathToPolls
            pathToPolls = "";

            // Parse command line
            NameValueCollection commandLineParameters = CommandLineParametersHelper.Parse(args);
            if (commandLineParameters["host"] != null && commandLineParameters["host"] != String.Empty)
            {
                try
                {
                    host = IPAddress.Parse(commandLineParameters["host"]);
                }
                catch (Exception exception)
                {
                    log.Error("Invalid host. " + exception.Message);
                }
            }

            if (commandLineParameters["port"] != null && commandLineParameters["host"] != String.Empty)   
            {
                try
                {
                    port = Convert.ToInt32(commandLineParameters["port"]);
                }
                catch (Exception exception)
                {
                    log.Error("Invalid port. " + exception.Message);
                }
            }

            if (commandLineParameters["polls"] != null && commandLineParameters["polls"] != String.Empty)
                pathToPolls = commandLineParameters["polls"] + "/";

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

