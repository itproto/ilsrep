using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.PollApplication.DAL;

namespace Ilsrep.PollApplication.PollServer
{
    class PollHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PollHandler));
        public TcpClient currentClient = new TcpClient();
        private static PollPacket sendPacket = new PollPacket();
        private static PollPacket receivedPacket = new PollPacket();
        private string clientAddress;
        private static int activeConnCount = 0;
        private static byte[] data = new byte[PollServer.DATA_SIZE];


        /// <summary>
        /// Receive data from client application
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        /// <returns>Received string if received or empty string if exception</returns>
        public string ReceiveFromClient(NetworkStream client)
        {
            byte[] data = new byte[PollServer.DATA_SIZE];
            int receivedBytesCount = 0;
            string recievedString = String.Empty;
            try
            {
                receivedBytesCount = client.Read(data, 0, PollServer.DATA_SIZE);
                recievedString = Encoding.ASCII.GetString(data, 0, receivedBytesCount);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return String.Empty;
            }
            return recievedString;
        }

        /// <summary>
        /// Receive PollPacket from client and response for a query
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void RunClientSession(NetworkStream client)
        {
            while (true)
            {
                // Receive PollPacket from client
                string receivedString = ReceiveFromClient(client);
                if (receivedString == String.Empty)
                {
                    return;
                }
                receivedPacket = PollSerializator.DeserializePacket(receivedString);
                if (receivedPacket == null)
                {
                    log.Error("Invalid data received");
                    return;
                }

                // Select option
                switch (receivedPacket.request.type)
                {
                    case Request.GET_LIST:
                        SendPollSessionsList(client);
                        break;
                    case Request.GET_POLLSESSION:
                        SendPollSession(client);
                        break; 
                    case Request.CREATE_POLLSESSION:
                        CreatePollSession(client);
                        break;
                    case Request.SAVE_RESULT:
                        SavePollSessionResult(client);
                        break;
                    case Request.REMOVE_POLLSESSION:
                        RemovePollSession(client);
                        break;
                    default:
                        log.Error("Invalid option sent by client");
                        break;
                }
            }
        }

        public void RemovePollSession(NetworkStream client)
        {
            try
            {
                PollDAL.RemovePollSession(receivedPacket.request.id);
                log.Info("Pollsession deleted - " + receivedPacket.request.id);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
            }
        }

        /// <summary>
        /// Receive from Client pollsession result and save it in database
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void SavePollSessionResult(NetworkStream client)
        {
            PollDAL.SavePollSessionResult(receivedPacket.resultsList);

            log.Info("Poll results added. PollSession ID: " + receivedPacket.resultsList.pollsessionId + ", Username: " + receivedPacket.resultsList.userName);
        }

        /// <summary>
        /// Receive new pollsession from PollEditor and save it to database
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void CreatePollSession(NetworkStream client)
        {
            // Save newPollSession
            try
            {
                receivedPacket.pollSession.id = PollDAL.CreatePollSession(receivedPacket.pollSession);
                log.Info("New PollSession has been added(name=\"" + receivedPacket.pollSession.name + "\" id=\"" + receivedPacket.pollSession.id + "\")");
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
            }
        }

        /// <summary>
        /// Send to client list of PollSessions
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void SendPollSessionsList(NetworkStream client)
        {
            string sendString;
            PollPacket sendPacket = new PollPacket();

            try
            {
                sendPacket.pollSessionList.items = PollDAL.GetPollSessions();
                sendString = PollSerializator.SerializePacket(sendPacket);
                data = Encoding.ASCII.GetBytes(sendString);
                client.Write(data, 0, sendString.Length);
                log.Info("List of PollSessions successfully sent to client");
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
            }
        }

        /// <summary>
        /// Receive pollSessionId from PollClient and send PollSession with corresponding id
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void SendPollSession(NetworkStream client)
        {
            string pollSessionId = receivedPacket.request.id;
            
            // Send PollSession
            sendPacket.pollSession = PollDAL.GetPollSession(Convert.ToInt32(pollSessionId));
            string sendString = PollSerializator.SerializePacket(sendPacket);
            data = Encoding.ASCII.GetBytes(sendString);
            client.Write(data, 0, sendString.Length);
            log.Info("Id of poll session, sent to client - " + pollSessionId);
        }

        /// <summary>
        /// Handle client connection
        /// </summary>
        public void HandleConnection()
        {
            // Establish connection with new client
            NetworkStream currentStream = currentClient.GetStream();
            activeConnCount++;
            clientAddress = currentClient.Client.RemoteEndPoint.ToString();
            log.Info("New client accepted: " + clientAddress + " (" + activeConnCount + " active connections)");
            
            // Run dialog with client
            RunClientSession(currentStream);

            // Close clients connection
            currentStream.Close();
            currentClient.Close();
            activeConnCount--;
            log.Info("Client disconnected: " + clientAddress + " (" + activeConnCount + " active connections left)");
        }
    }
}