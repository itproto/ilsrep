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
        private const string POLL_SESSION_ELEMENT = "pollsession";
        public const string POLL_SESSIONS_LIST_FILE = "PollSessionsList.xml";

        /// <summary>
        /// The function search poll session by id
        /// </summary>
        /// <param name="pollSessionID">Id of needed poll session</param>
        /// <returns>Poll session in XmlStirng</returns>
        private static PollSession GetPollSessionById(string pollSessionId)
        {
            try
            {
                // Search patch to needed file by id
                string pathToPollSession = string.Empty;
                XmlDocument pollSessionsListDoc = new XmlDocument();
                pollSessionsListDoc.Load(PollServer.pollsFolder + POLL_SESSIONS_LIST_FILE);
                XmlNodeList pollSessionsList = pollSessionsListDoc.GetElementsByTagName(POLL_SESSION_ELEMENT);
                foreach (XmlNode curPollSession in pollSessionsList)
                {
                    bool isRightPollSession = (curPollSession.Attributes["id"].Value == pollSessionId);
                    if (isRightPollSession)
                    {
                        pathToPollSession = curPollSession.Attributes["file"].Value;
                        break;
                    }
                }

                XmlDocument pollSessionDoc = new XmlDocument();
                pollSessionDoc.Load(pathToPollSession);
                PollSession pollSession = PollSerializator.DeSerialize(pollSessionDoc.OuterXml);
                return pollSession;
            }
            catch (Exception exception)
            {
                log.Error("Exception in GetPollSessionById. " + exception.Message);
                return null;
            }
        }

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
                    return;

                receivedPacket = PollSerializator.DeSerializePacket(receivedString);

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
                    default:
                        log.Error("Invalid option sent by client");
                        break;
                }
            }
        }

        /// <summary>
        /// Receive XmlString from PollEditor, save it to new XmlFile and append PollSessionsList.xml file
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void CreatePollSession(NetworkStream client)
        {
            // Receive new poll session
            string receivedString = ReceiveFromClient(client);
            if (receivedString == String.Empty)
                return;

            // Check if received xml data is correct
            XmlDocument pollSessionDoc = new XmlDocument();
            try
            {
                pollSessionDoc.LoadXml(receivedString);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return;
            }

            // Get name of poll session
            string pollSessionName = pollSessionDoc.GetElementsByTagName(POLL_SESSION_ELEMENT)[0].Attributes["name"].Value;

            // Get idList from PollSessionsList.xml
            int pollSessionId;
            List<int> idList = new List<int>();
            XmlDocument PollSessionsListDoc = new XmlDocument();
            try
            {
                PollSessionsListDoc.Load(PollServer.pollsFolder + POLL_SESSIONS_LIST_FILE);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return;
            }
            XmlNodeList pollSessionsList = PollSessionsListDoc.GetElementsByTagName(POLL_SESSION_ELEMENT);
            foreach (XmlNode pollSession in pollSessionsList)
            {
                pollSessionId = Convert.ToInt32(pollSession.Attributes["id"].Value);
                idList.Add(pollSessionId);
            }
            idList.Sort();

            // Generate new id
            int newPollSessionId = idList[idList.Count-1] + 1;
            
            // Add id attribute to pollSession
            XmlAttribute pollSessionIdAttribute = pollSessionDoc.CreateAttribute("id");
            pollSessionIdAttribute.Value = newPollSessionId.ToString();
            pollSessionDoc.GetElementsByTagName(POLL_SESSION_ELEMENT)[0].Attributes.Append(pollSessionIdAttribute);

            // Save pollSession in new file
            string newPathToPollSession = PollServer.pollsFolder + "PollSession_" + newPollSessionId + ".xml";
            pollSessionDoc.Save(newPathToPollSession);

            // Add information about current poll session to PollSessionsList.xml
            XmlElement newPollSessionInf = PollSessionsListDoc.CreateElement(POLL_SESSION_ELEMENT);
            XmlAttribute idAttribute = PollSessionsListDoc.CreateAttribute("id");
            idAttribute.Value = newPollSessionId.ToString();
            XmlAttribute nameAttribute = PollSessionsListDoc.CreateAttribute("name");
            nameAttribute.Value = pollSessionName;
            XmlAttribute fileAttribute = PollSessionsListDoc.CreateAttribute("file");
            fileAttribute.Value = newPathToPollSession;
            newPollSessionInf.Attributes.Append(idAttribute);
            newPollSessionInf.Attributes.Append(nameAttribute);
            newPollSessionInf.Attributes.Append(fileAttribute);
            PollSessionsListDoc.GetElementsByTagName("pollsessions")[0].AppendChild(newPollSessionInf);
            PollSessionsListDoc.Save(PollServer.pollsFolder + POLL_SESSIONS_LIST_FILE);
            log.Info("New PollSession has been added(name=\"" + pollSessionName + "\" id=\""+ newPollSessionId + "\")");
        }

        /// <summary>
        /// Send to client list of PollSessions
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void SendPollSessionsList(NetworkStream client)
        {
            string sendString;
            XmlDocument pollSessionsListDoc = new XmlDocument();
            try
            {
                pollSessionsListDoc.Load(PollServer.pollsFolder + POLL_SESSIONS_LIST_FILE);
                XmlNodeList pollSessionsList = pollSessionsListDoc.GetElementsByTagName(POLL_SESSION_ELEMENT);
                
                foreach(XmlNode curPollSession in pollSessionsList)
                {
                    Item curItem = new Item();
                    curItem.id = curPollSession.Attributes["id"].Value;
                    curItem.name = curPollSession.Attributes["name"].Value;
                    sendPacket.pollSessionList.items.Add(curItem);
                }

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
            sendPacket.pollSession = GetPollSessionById(pollSessionId);
            string sendString = PollSerializator.SerializePacket(sendPacket);
            data = Encoding.ASCII.GetBytes(sendString);
            client.Write(data, 0, sendString.Length);
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