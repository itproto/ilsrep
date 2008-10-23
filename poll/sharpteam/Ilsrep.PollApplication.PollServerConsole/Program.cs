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
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.DAL;
using System.Data.SQLite;

namespace Ilsrep.PollApplication.PollServer
{
    /// <summary>
    /// PollServer handles connections and creates new threads to work with them.
    /// </summary>
    public class PollServer
    {
        private class StateObject
        {
            /// <summary>
            /// Client  socket
            /// </summary>
            public Socket workSocket = null;
            /// <summary>
            /// Size of receive buffer
            /// </summary>
            public const int BUFFERSIZE = 65536; //1024;
            /// <summary>
            /// Receive buffer
            /// </summary>
            public byte[] buffer = new byte[BUFFERSIZE];
            /// <summary>
            /// Received data string
            /// </summary>
            public StringBuilder sb = new StringBuilder();
            /// <summary>
            /// Shows is user authorized or not
            /// </summary>
            public bool isAuthorized = false;

            /// <summary>
            /// get ip address
            /// </summary>
            public string ipAddress
            {
                get { return workSocket.RemoteEndPoint.ToString(); }
            }
        }

        /// <summary>
        /// path to Logger configuration file
        /// </summary>
        public const string PATH_TO_LOG_CONFIG = "LogConfig.xml";
        /// <summary>
        /// IP Address to which bind the server
        /// </summary>
        static public IPAddress host = IPAddress.Any;
        /// <summary>
        /// Port to which bind the server
        /// </summary>
        static public int port = 3320;
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(PollServer));
        /// <summary>
        /// Main socket that will accept connections
        /// </summary>
        private static Socket server = null;
        /// <summary>
        /// Worker sockets that will handle clients individually
        /// </summary>
        private static List<StateObject> workers = new List<StateObject>();
        /// <summary>
        /// Thread signal
        /// </summary>
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">arguments</param>
        public static void Main(string[] args)
        {
            // Configure logger
            XmlConfigurator.Configure(new System.IO.FileInfo(PATH_TO_LOG_CONFIG));

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
            if (commandLineParameters["port"] != null && commandLineParameters["port"] != String.Empty)
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
            /*if (commandLineParameters["data"] != null && commandLineParameters["data"] != String.Empty)
                pathToDatabase = commandLineParameters["data"];*/

            // Start server
            PollServer pollServer = new PollServer();
            pollServer.Start();
            pollServer.Stop();
        }

        /// <summary>
        /// Start Server. Inits socket and starts listening. Sets async accept callback.
        /// </summary>
        public void Start()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += delegate
            {
                Stop();
            };

            try
            {
                server.Bind(new IPEndPoint(host, port));
                server.Listen(5);

                log.Info("Server started on host: " + host.ToString() + ":" + port);

                while (true)
                {
                    allDone.Reset();

                    // Main loop
                    log.Info("Waiting for a connection...");
                    server.BeginAccept(new AsyncCallback(OnClientConnect), null);

                    allDone.WaitOne();
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
        }

        /// <summary>
        /// Stop server. Close main listening socket and all the clients' sockets.
        /// </summary>
        public void Stop()
        {
            log.Info("Stopping server...");
            // Close all connections
            server.Close();
            foreach (StateObject worker in workers)
                worker.workSocket.Close();

            PollDAL.Close();
        }

        /// <summary>
        /// Method is exetuted when new client connects
        /// </summary>
        /// <param name="iAsyncResult"></param>
        public void OnClientConnect(IAsyncResult iAsyncResult)
        {
            try
            {
                StateObject newClient = new StateObject();
                newClient.workSocket = server.EndAccept(iAsyncResult);
                workers.Add(newClient);

                log.Info(String.Format("Client {0} connected", newClient.ipAddress));

                newClient.workSocket.BeginReceive(newClient.buffer, 0, StateObject.BUFFERSIZE, SocketFlags.None, new AsyncCallback(ProcessClient), workers.Count - 1);
                //ProcessClient(workerSockets.Count - 1);

                //server.BeginAccept(new AsyncCallback(OnClientConnect), null);
                allDone.Set();
            }
            catch(ObjectDisposedException)
            {
              log.Info("OnClientConnection: Socket has been closed");
            }
            catch(SocketException se)
            {
              log.Error( se.Message );
            }
        }
        
        /// <summary>
        /// Method is executed when data is received
        /// </summary>
        /// <param name="iAsyncResult"></param>
        public void ProcessClient(IAsyncResult iAsyncResult)
        {
            int clientID = (int)iAsyncResult.AsyncState;
            StateObject client = workers[clientID];
            int countReceived = 0;

            try
            {
                countReceived = client.workSocket.EndReceive(iAsyncResult);
                if (countReceived == 0)
                {
                    log.Info(String.Format("Client {0} disconnected", client.ipAddress));
                    client.workSocket.Close();
                    workers.RemoveAt(clientID);

                    return;
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    log.Info(String.Format("Client {0} disconnected", client.ipAddress));
                    workers.RemoveAt(clientID);
                }
                else
                {
                    log.Error(String.Format("Socket Exception on client {0}: {1}", client.ipAddress, e.Message));
                }

                return;
            }

            string receivedData = Encoding.ASCII.GetString(client.buffer, 0, countReceived);
            PollPacket receivedPacket = PollSerializator.DeserializePacket(receivedData);
            if (receivedPacket == null)
            {
                log.Error("Invalid data received");
                client.workSocket.BeginReceive(client.buffer, 0, StateObject.BUFFERSIZE, SocketFlags.None, new AsyncCallback(ProcessClient), clientID);
                
                return;
            }
            
            // Packet to be sent back
            PollPacket sendPacket = new PollPacket();

            if (!client.isAuthorized)
            {
                switch (receivedPacket.user.action)
                {
                    case User.LOGIN:
                        sendPacket.user = PollDAL.AuthorizeUser(receivedPacket.user);
                        if (sendPacket.user.action == User.ACCEPTED)
                        {
                            client.isAuthorized = true;
                            log.Info("User accepted: " + sendPacket.user.username);
                        }
                        break;
                    case User.NEW_USER:
                        sendPacket.user = PollDAL.RegisterUser(receivedPacket.user);
                        if (sendPacket.user.action == User.ACCEPTED)
                        {
                            client.isAuthorized = true;
                            log.Info("New user created: " + sendPacket.user.username);
                        }
                        break;
                }
            }
            else
            {
                // Select option
                switch (receivedPacket.request.type)
                {
                    case Request.GET_LIST:
                        sendPacket.pollSessionList = new PollSessionList();
                        sendPacket.pollSessionList.items = PollDAL.GetPollSessions();
                        log.Info(String.Format("Pollsession List sent to {0}", client.ipAddress));
                        break;
                    case Request.GET_POLLSESSION:
                        sendPacket.pollSession = PollDAL.GetPollSession(Convert.ToInt32(receivedPacket.request.id));
                        log.Info(String.Format("Pollsession {0} sent to {1}", sendPacket.pollSession.Id, client.ipAddress));
                        break;
                    case Request.EDIT_POLLSESSION:
                        PollDAL.EditPollSession(receivedPacket.pollSession);
                        log.Info(String.Format("Pollsession {0} changed by {1}", receivedPacket.pollSession.Id, client.ipAddress));
                        break;
                    case Request.CREATE_POLLSESSION:
                        receivedPacket.pollSession.Id = PollDAL.CreatePollSession(receivedPacket.pollSession);
                        log.Info(String.Format("Pollsession {0} created by {1}", receivedPacket.pollSession.Id, client.ipAddress));
                        break;
                    case Request.REMOVE_POLLSESSION:
                        PollDAL.RemovePollSession(Convert.ToInt32(receivedPacket.request.id));
                        log.Info(String.Format("Pollsession {0} removed by {1}", receivedPacket.request.id, client.ipAddress));
                        break;
                    case Request.SAVE_RESULT:
                        PollDAL.SavePollSessionResult(receivedPacket.resultsList);
                        log.Info(String.Format("Pollsession {0} results of user {1} sent by {2}", receivedPacket.resultsList.pollsessionId, receivedPacket.resultsList.userName, client.ipAddress));
                        break;
                    case Request.GET_RESULTS:
                        sendPacket.resultsList = PollDAL.GetPollSessionResults(Convert.ToInt32(receivedPacket.request.id));
                        log.Info(String.Format("Pollsession {0} results sent to {1}", receivedPacket.request.id, client.ipAddress));
                        break;
                    default:
                        log.Error("Invalid option sent by client");
                        break;
                }
            }

            string sendString = PollSerializator.SerializePacket(sendPacket);
            byte[] sendData = Encoding.ASCII.GetBytes(sendString);
            try
            {
                client.workSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientID);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    log.Info(String.Format("Client {0} disconnected", client.ipAddress));
                    workers.RemoveAt(clientID);
                }
                else
                {
                    log.Error(String.Format("Socket Exception on client {0}: {1}", client.ipAddress, e.Message));
                }

                return;
            }
        }

        /// <summary>
        /// Method is executed when data has been sent
        /// </summary>
        /// <param name="iAsyncResult"></param>
        public void SendCallback(IAsyncResult iAsyncResult)
        {
            int clientID = (int)iAsyncResult.AsyncState;
            StateObject client = workers[clientID];

            try
            {
                int bytesSent = client.workSocket.EndSend(iAsyncResult);
                //log.Info(String.Format("Sent {0} bytes to client {1}.", bytesSent, client.ipAddress));

                client.workSocket.BeginReceive(client.buffer, 0, StateObject.BUFFERSIZE, SocketFlags.None, new AsyncCallback(ProcessClient), clientID);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    log.Info(String.Format("Client {0} disconnected", client.ipAddress));
                    workers.RemoveAt(clientID);
                }
                else
                {
                    log.Error(String.Format("Socket Exception on client {0}: {1}", client.ipAddress, e.Message));
                }

                return;
            }
        }
    }
}

