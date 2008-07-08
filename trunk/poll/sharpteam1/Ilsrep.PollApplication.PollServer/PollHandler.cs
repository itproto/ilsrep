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
        public const string PATH_TO_POLL_ID = "Poll_id.xml";

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
                XmlDocument Poll_id = new XmlDocument();
                Poll_id.Load(PollServer.pollsFolder + PATH_TO_POLL_ID);
                XmlNodeList pollSessionList = Poll_id.GetElementsByTagName(POLL_SESSION_ELEMENT);
                foreach (XmlNode curPollSession in pollSessionList)
                {
                    bool isRightPollSession = (Convert.ToInt32(curPollSession.Attributes["id"].Value) == pollSessionID);
                    if (isRightPollSession)
                    {
                        pathToPollSession = curPollSession.Attributes["file"].Value;
                        break;
                    }
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(pathToPollSession);
                return xmlDoc.OuterXml;
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
        /// Receive option from client and select one: GetPollSession or CreatePollSession
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void RunClientSession(NetworkStream client)
        {
            // Receive option from client
            string receivedString = ReceiveFromClient(client);
            if (receivedString == String.Empty)
                return;
            
            // Select option {GetPollSession or CreatePollSession}
            switch (receivedString)
            {
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
        /// Receive XmlString from PollEditor, save it to new XmlFile and append Poll_id.xml file
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        public void CreatePollSession(NetworkStream client)
        {
            // Receive new poll session
            string receivedString = ReceiveFromClient(client);
            if (receivedString == String.Empty)
                return;

            // Check if received xml data is correct
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(receivedString);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return;
            }

            // Get name of poll session
            //XmlNode xmlPollSession = xmlDoc.GetElementsByTagName("pollsession")[0];
            //string pollSessionName = xmlPollSession.Attributes["name"].Value;
            string pollSessionName = xmlDoc.GetElementsByTagName("pollsession")[0].Attributes["name"].Value;

            // Get idList from Poll_id.xml
            int pollSessionId;
            List<int> idList = new List<int>();
            XmlDocument Poll_id = new XmlDocument();
            try
            {
                Poll_id.Load(PollServer.pollsFolder + PATH_TO_POLL_ID);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return;
            }
            XmlNodeList pollSessionList = Poll_id.GetElementsByTagName(POLL_SESSION_ELEMENT);
            foreach (XmlNode pollSession in pollSessionList)
            {
                pollSessionId = Convert.ToInt32(pollSession.Attributes["id"].Value);
                idList.Add(pollSessionId);
            }
            idList.Sort();

            // Generate new id
            int newPollSessionId = idList[idList.Count-1] + 1;
            
            // Add id attribute to pollSession
            XmlAttribute pollSessionIdAttribute = xmlDoc.CreateAttribute("id");
            pollSessionIdAttribute.Value = newPollSessionId.ToString();
            xmlDoc.GetElementsByTagName(POLL_SESSION_ELEMENT)[0].Attributes.Append(pollSessionIdAttribute);

            // Save pollSession in new file
            string newPathToPollSession = PollServer.pollsFolder + "PollSession_" + newPollSessionId + ".xml";
            xmlDoc.Save(newPathToPollSession);

            // Add information about current poll session to Poll_id.xml
            XmlElement newPollInformation = Poll_id.CreateElement(POLL_SESSION_ELEMENT);
            XmlAttribute idAttribute = Poll_id.CreateAttribute("id");
            idAttribute.Value = newPollSessionId.ToString();
            XmlAttribute nameAttribute = Poll_id.CreateAttribute("name");
            nameAttribute.Value = pollSessionName;
            XmlAttribute fileAttribute = Poll_id.CreateAttribute("file");
            fileAttribute.Value = newPathToPollSession;
            newPollInformation.Attributes.Append(idAttribute);
            newPollInformation.Attributes.Append(nameAttribute);
            newPollInformation.Attributes.Append(fileAttribute);
            Poll_id.GetElementsByTagName("pollsessions")[0].AppendChild(newPollInformation);
            Poll_id.Save(PollServer.pollsFolder + PATH_TO_POLL_ID);
            log.Info("New PollSession has been added(name=\"" + pollSessionName + "\" id=\""+ newPollSessionId + "\")");
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