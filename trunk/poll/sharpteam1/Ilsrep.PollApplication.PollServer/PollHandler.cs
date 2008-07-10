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

namespace Ilsrep.PollApplication.PollServer
{
    class PollHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PollHandler));
        public TcpClient currentClient = new TcpClient();
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
        private static string GetPollSessionById(int pollSessionID)
        {
            try
            {
                // Search patch to needed file by id
                string pathToPollSession = string.Empty;
                XmlDocument PollSessionsListDoc = new XmlDocument();
                PollSessionsListDoc.Load(PollServer.pollsFolder + POLL_SESSIONS_LIST_FILE);
                XmlNodeList pollSessionsList = PollSessionsListDoc.GetElementsByTagName(POLL_SESSION_ELEMENT);
                foreach (XmlNode curPollSession in pollSessionsList)
                {
                    bool isRightPollSession = (Convert.ToInt32(curPollSession.Attributes["id"].Value) == pollSessionID);
                    if (isRightPollSession)
                    {
                        pathToPollSession = curPollSession.Attributes["file"].Value;
                        break;
                    }
                }

                XmlDocument pollSessionDoc = new XmlDocument();
                pollSessionDoc.Load(pathToPollSession);
                return pollSessionDoc.OuterXml;
            }
            catch (Exception exception)
            {
                log.Error("Exception in GetPollSessionById. " + exception.Message);
                return String.Empty;
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
        /// Receive option from client and select one: GetPollSessionsList, GetPollSession or CreatePollSession
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void RunClientSession(NetworkStream client)
        {
            // Receive option from client
            string receivedString = ReceiveFromClient(client);
            if (receivedString == String.Empty)
                return;
            
            // Select option
            switch (receivedString)
            {
                case "GetPollSessionsList":
                    SendPollSessionsList(client);
                    break;
                case "GetPollSession":
                    SendPollSession(client);
                    break;
                case "CreatePollSession":
                    CreatePollSession(client);
                    break;
                default:
                    log.Error("Invalid option sent by client");
                    break;
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
            XmlDocument PollSessionsListDoc = new XmlDocument();
            try
            {
                PollSessionsListDoc.Load(PollServer.pollsFolder + POLL_SESSIONS_LIST_FILE);
                sendString = PollSessionsListDoc.OuterXml;
                data = Encoding.ASCII.GetBytes(sendString);
                client.Write(data, 0, sendString.Length);
                log.Info("PollSessionList successfully sent to client");
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
            string sendString;
            int pollSessionID = 0;

            // Receive from client pollSessionId
            while (true)
            {
                string receivedString = ReceiveFromClient(client);
                if (receivedString == String.Empty)
                    return;

                // Get poll sesssion id sent by client
                try
                {
                    pollSessionID = Convert.ToInt32(receivedString);

                    // Check if exists such id
                    bool idExist = (GetPollSessionById(pollSessionID) != String.Empty);
                    if (idExist)
                    {
                        log.Info(clientAddress + ": ID validated: " + pollSessionID);
                        sendString = "1";
                        client.Write(Encoding.ASCII.GetBytes(sendString), 0, sendString.Length);
                        break;
                    }
                    else
                    {
                        log.Info(clientAddress + ": Client asked for non-existant ID: " + pollSessionID);
                        sendString = "-1";
                        client.Write(Encoding.ASCII.GetBytes(sendString), 0, sendString.Length);
                    }
                }
                catch (Exception)
                {
                    log.Info(clientAddress + ": Client asked for non-existant ID: " + receivedString);
                    sendString = "-1";
                    client.Write(Encoding.ASCII.GetBytes(sendString), 0, sendString.Length);
                }
            }

            // Send PollSession
            sendString = GetPollSessionById(pollSessionID);
            if (sendString == String.Empty)
            {
                log.Error("Can not get poll by id");
                return;
            }
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