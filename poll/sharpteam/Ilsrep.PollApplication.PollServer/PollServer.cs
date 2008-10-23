using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.DAL;

namespace Ilsrep.PollApplication.PollServer
{
    partial class PollServer : ServiceBase
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
        //public const string PATH_TO_LOG_CONFIG = "LogConfig.xml";
        /// <summary>
        /// Event Log
        /// </summary>
        EventLog log = null;
        /// <summary>
        /// IP Address to which bind the server
        /// </summary>
        static public IPAddress host = IPAddress.Any;
        /// <summary>
        /// Port to which bind the server
        /// </summary>
        static public int port = 3320;
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
        private static ManualResetEvent allDone = new ManualResetEvent( false );
        /// <summary>
        /// Main thread that server will run on
        /// </summary>
        private Thread serverThread = null;
        /// <summary>
        /// Flag to see if service is still running
        /// </summary>
        private bool serviceRunning = true;

        public PollServer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">arguments</param>
        public static void Main( string[] args )
        {
            if ( args.Length != 0 && args[0] == "debug" )
            {
                PollServer pollServer = new PollServer();
                pollServer.Run();
            }
            else
            {
                ServiceBase.Run( new PollServer() );
            }
        }

        /// <summary>
        /// Start server
        /// </summary>
        /// <param name="args">passed arguments</param>
        protected override void OnStart( string[] args )
        {
            System.IO.Directory.SetCurrentDirectory( System.AppDomain.CurrentDomain.BaseDirectory );

            // Create the source, if it does not already exist.
            if ( !EventLog.SourceExists( "PollServer" ) )
            {
                //An event log source should not be created and immediately used.
                //There is a latency time to enable the source, it should be created
                //prior to executing the application that uses the source.
                //Execute this sample a second time to use the new source.
                EventLog.CreateEventSource( "PollServer", "Application" );

                return;
            }

            log = this.EventLog;
            log.Source = "PollServer";
            log.Log = "Application";

            serverThread = new Thread( new ThreadStart( Run ) );
            serverThread.Start();
        }

        /// <summary>
        /// Stop server.
        /// </summary>
        protected override void OnStop()
        {
            serviceRunning = false;
            allDone.Set();
            serverThread.Join( new TimeSpan( 0, 2, 0 ) );
        }

        /// <summary>
        /// Start Server. Inits socket and starts listening. Sets async accept callback.
        /// </summary>
        public void Run()
        {
            if ( ConfigurationManager.AppSettings.Get( "host" ) == "any" )
                host = IPAddress.Any;
            else
                host = IPAddress.Parse( ConfigurationManager.AppSettings.Get( "host" ) );
            port = Convert.ToInt32( ConfigurationManager.AppSettings.Get( "port" ) );
            PollDAL.connectionString = ConfigurationManager.AppSettings.Get( "connectionString" );

            server = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

            try
            {
                server.Bind( new IPEndPoint( host, port ) );
                server.Listen( 5 );
                
                EventLog.WriteEntry( "Server started on " + host.ToString() + ":" + port, EventLogEntryType.Information );

                while ( serviceRunning )
                {
                    allDone.Reset();

                    // Main loop
                    //EventLog.WriteEntry( "Waiting for a connection...", EventLogEntryType.Information );
                    server.BeginAccept( new AsyncCallback( OnClientConnect ), null );

                    allDone.WaitOne();
                }
            }
            catch ( Exception e )
            {
                EventLog.WriteEntry( e.Message, EventLogEntryType.Error );
            }

            EventLog.WriteEntry( "Stopping server...", EventLogEntryType.Information );

            // Close all connections
            server.Close();
            foreach ( StateObject worker in workers )
                worker.workSocket.Close();
            
            // Disconnect from DB
            PollDAL.Close();

            // Close the thread
            Thread.CurrentThread.Abort();
        }

        /// <summary>
        /// Method is exetuted when new client connects
        /// </summary>
        /// <param name="iAsyncResult"></param>
        public void OnClientConnect( IAsyncResult iAsyncResult )
        {
            try
            {
                StateObject newClient = new StateObject();
                newClient.workSocket = server.EndAccept( iAsyncResult );
                workers.Add( newClient );

                EventLog.WriteEntry( String.Format( "Client {0} connected", newClient.ipAddress ), EventLogEntryType.Information );

                newClient.workSocket.BeginReceive( newClient.buffer, 0, StateObject.BUFFERSIZE, SocketFlags.None, new AsyncCallback( ProcessClient ), workers.Count - 1 );

                allDone.Set();
            }
            catch ( ObjectDisposedException )
            {
                //EventLog.WriteEntry( "OnClientConnection: Socket has been closed", EventLogEntryType.Information );
            }
            catch ( SocketException se )
            {
                EventLog.WriteEntry( se.Message, EventLogEntryType.Error );
            }
        }

        /// <summary>
        /// Method is executed when data is received
        /// </summary>
        /// <param name="iAsyncResult"></param>
        public void ProcessClient( IAsyncResult iAsyncResult )
        {
            int clientID = (int)iAsyncResult.AsyncState;
            StateObject client = workers[clientID];
            int countReceived = 0;

            try
            {
                countReceived = client.workSocket.EndReceive( iAsyncResult );
                if ( countReceived == 0 )
                {
                    EventLog.WriteEntry( String.Format( "Client {0} disconnected", client.ipAddress ), EventLogEntryType.Information );
                    client.workSocket.Close();
                    workers.RemoveAt( clientID );

                    return;
                }
            }
            catch ( SocketException e )
            {
                if ( e.ErrorCode == 10054 )
                {
                    EventLog.WriteEntry( String.Format( "Client {0} disconnected", client.ipAddress ), EventLogEntryType.Information );
                    workers.RemoveAt( clientID );
                }
                else
                {
                    EventLog.WriteEntry( String.Format( "Socket Exception on client {0}: {1}", client.ipAddress, e.Message ), EventLogEntryType.Error );
                }

                return;
            }

            string receivedData = Encoding.ASCII.GetString( client.buffer, 0, countReceived );
            PollPacket receivedPacket = PollSerializator.DeserializePacket( receivedData );
            if ( receivedPacket == null )
            {
                EventLog.WriteEntry( "Invalid data received", EventLogEntryType.Error );
                client.workSocket.BeginReceive( client.buffer, 0, StateObject.BUFFERSIZE, SocketFlags.None, new AsyncCallback( ProcessClient ), clientID );

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
                            EventLog.WriteEntry("User accepted: " + sendPacket.user.username, EventLogEntryType.Information);
                        }
                        break;
                    case User.NEW_USER:
                        sendPacket.user = PollDAL.RegisterUser(receivedPacket.user);
                        if (sendPacket.user.action == User.ACCEPTED)
                        {
                            EventLog.WriteEntry("New user created: " + sendPacket.user.username, EventLogEntryType.Information);
                        }
                        break;
                }
            }
            else
            {
                // Select option
                switch ( receivedPacket.request.type )
                {
                    case Request.GET_LIST:
                        sendPacket.pollSessionList = new PollSessionList();
                        sendPacket.pollSessionList.items = PollDAL.GetPollSessions();
                        EventLog.WriteEntry( String.Format( "Pollsession List sent to {0}", client.ipAddress ), EventLogEntryType.Information );
                        break;
                    case Request.GET_POLLSESSION:
                        sendPacket.pollSession = PollDAL.GetPollSession( Convert.ToInt32( receivedPacket.request.id ) );
                        EventLog.WriteEntry( String.Format( "Pollsession {0} sent to {1}", sendPacket.pollSession.Id, client.ipAddress ), EventLogEntryType.Information );
                        break;
					case Request.EDIT_POLLSESSION:
						PollDAL.EditPollSession(receivedPacket.pollSession);
                        EventLog.WriteEntry( String.Format( "Pollsession {0} changed by {1}", receivedPacket.pollSession.Id, client.ipAddress ), EventLogEntryType.Information );
						break;
                    case Request.CREATE_POLLSESSION:
                        receivedPacket.pollSession.Id = PollDAL.CreatePollSession( receivedPacket.pollSession );
                        EventLog.WriteEntry( String.Format( "Pollsession {0} created by {1}", receivedPacket.pollSession.Id, client.ipAddress ), EventLogEntryType.Information );
                        break;
                    case Request.REMOVE_POLLSESSION:
                        PollDAL.RemovePollSession( Convert.ToInt32( receivedPacket.request.id ) );
                        EventLog.WriteEntry( String.Format( "Pollsession {0} removed by {1}", receivedPacket.request.id, client.ipAddress ), EventLogEntryType.Information );
                        break;
                    case Request.SAVE_RESULT:
                        PollDAL.SavePollSessionResult( receivedPacket.resultsList );
                        EventLog.WriteEntry( String.Format( "Pollsession {0} results of user {1} sent by {2}", receivedPacket.resultsList.pollsessionId, receivedPacket.resultsList.userName, client.ipAddress ), EventLogEntryType.Information );
                        break;
                    case Request.GET_RESULTS:
                        sendPacket.resultsList = PollDAL.GetPollSessionResults(Convert.ToInt32(receivedPacket.request.id));
                        EventLog.WriteEntry( String.Format( "Pollsession {0} results sent to {1}", receivedPacket.request.id, client.ipAddress ), EventLogEntryType.Information );
                        break;
                    default:
                        EventLog.WriteEntry( String.Format( "Invalid option sent by {0}", client.ipAddress ), EventLogEntryType.Error );
                        break;
                }
            }

            string sendString = PollSerializator.SerializePacket( sendPacket );
            byte[] sendData = Encoding.ASCII.GetBytes( sendString );
            try
            {
                client.workSocket.BeginSend( sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback( SendCallback ), clientID );
            }
            catch ( SocketException e )
            {
                if ( e.ErrorCode == 10054 )
                {
                    EventLog.WriteEntry( String.Format( "Client {0} disconnected", client.ipAddress ), EventLogEntryType.Information );
                    workers.RemoveAt( clientID );
                }
                else
                {
                    EventLog.WriteEntry( String.Format( "Socket Exception on client {0}: {1}", client.ipAddress, e.Message ), EventLogEntryType.Error );
                }

                return;
            }
        }

        /// <summary>
        /// Method is executed when data has been sent
        /// </summary>
        /// <param name="iAsyncResult"></param>
        public void SendCallback( IAsyncResult iAsyncResult )
        {
            int clientID = (int)iAsyncResult.AsyncState;
            StateObject client = workers[clientID];

            try
            {
                int bytesSent = client.workSocket.EndSend( iAsyncResult );
                //log.Info(String.Format("Sent {0} bytes to client {1}.", bytesSent, client.ipAddress));

                client.workSocket.BeginReceive( client.buffer, 0, StateObject.BUFFERSIZE, SocketFlags.None, new AsyncCallback( ProcessClient ), clientID );
            }
            catch ( SocketException e )
            {
                if ( e.ErrorCode == 10054 )
                {
                    EventLog.WriteEntry( String.Format( "Client {0} disconnected", client.ipAddress ), EventLogEntryType.Information );
                    workers.RemoveAt( clientID );
                }
                else
                {
                    EventLog.WriteEntry( String.Format( "Socket Exception on client {0}: {1}", client.ipAddress, e.Message ), EventLogEntryType.Error );
                }

                return;
            }
        }
    }
}
