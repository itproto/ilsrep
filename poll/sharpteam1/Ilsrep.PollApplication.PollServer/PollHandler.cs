using System;
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
    class PollHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PollHandler));
        public TcpClient currentClient = new TcpClient();
        private string clientAddress;
        private static int activeConnCount = 0;
        private static byte[] data = new byte[PollServer.DATA_SIZE];
        private const string POLL_SESSION_ELEMENT = "pollsession";

        private static string GetPollSessionById(int pollSessionID)
        {
            try
            {
                // Search patch to needed file by id
                string patchToPollSession = "";
                XmlDocument Poll_id = new XmlDocument();
                Poll_id.Load(PollServer.pathToPolls + PollServer.PATH_TO_POLL_ID);
                XmlNodeList pollSessionList = Poll_id.GetElementsByTagName(POLL_SESSION_ELEMENT);
                foreach (XmlNode curPollSession in pollSessionList)
                {
                    bool isRightPollSession = (Convert.ToInt32(curPollSession.Attributes["id"].Value) == pollSessionID);
                    if (isRightPollSession)
                    {
                        patchToPollSession = curPollSession.Attributes["file"].Value;
                        break;
                    }
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(patchToPollSession);
                return xmlDoc.FirstChild.OuterXml;
            }
            catch (Exception exception)
            {
                log.Error("Exception in GetPollSessionById. " + exception.Message);
                return String.Empty;
            }
        }

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

        public void RunClientSession(NetworkStream client)
        {
            // Receive option {GetPollSession or CreatePollSession} from client
            string receivedString = ReceiveFromClient(client);
            if (receivedString == String.Empty)
                return;

            // Choose one option {GetPollSession or CreatePollSession}
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

            XmlNode xmlPollSession = xmlDoc.FirstChild;
            int pollSessionId = Convert.ToInt32(xmlPollSession.Attributes["id"].Value);
            string pollSessionName = xmlPollSession.Attributes["name"].Value;

            // Save current poll session in file
            string curPathToPollSession = PollServer.pathToPolls + "PollSession_" + pollSessionId + ".xml";
            xmlDoc.Save(curPathToPollSession);
            
            // Add information about current poll session to Poll_id.xml
            XmlDocument Poll_id = new XmlDocument();
            XmlDocument curPollInformation = new XmlDocument();
            Poll_id.Load(PollServer.pathToPolls + "Poll_id.xml");
            curPollInformation.LoadXml("<" + POLL_SESSION_ELEMENT + " id=" + pollSessionId + " name=" + pollSessionName + " file=" + curPathToPollSession + " />");
            Poll_id.FirstChild.AppendChild(curPollInformation);            
        }

        public void SendPollSession(NetworkStream client)
        {
            string sendString;
            int pollSessionID = 0;
            while (true)
            {
                // Receive from client pollSessionId
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
                log.Error("Couldn't get poll by id");
                return;
            }

            data = Encoding.ASCII.GetBytes(sendString);
            client.Write(data, 0, sendString.Length);
        }

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