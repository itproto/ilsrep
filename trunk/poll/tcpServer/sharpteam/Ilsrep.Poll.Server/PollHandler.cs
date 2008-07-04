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

        private static bool idExists(int pollSessionID)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(PollServer.pathToPolls);
                XmlNodeList xmlPollSessionList = xmlDoc.GetElementsByTagName("pollsession");

                foreach (XmlNode xmlPollSession in xmlPollSessionList)
                {
                    bool isRightPollSession = (Convert.ToInt32(xmlPollSession.Attributes["id"].Value) == pollSessionID);
                    if (isRightPollSession)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message.ToString());
                return false;
            }
            return false;
        }

        private static string GetPollSessionById(int pollSessionID)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(PollServer.pathToPolls);
                XmlNodeList xmlPollSessionList = xmlDoc.GetElementsByTagName("pollsession");
                foreach (XmlNode xmlPollSession in xmlPollSessionList)
                {
                    bool isRightPollSession = (Convert.ToInt32(xmlPollSession.Attributes["id"].Value) == pollSessionID);
                    if (isRightPollSession)
                    {
                        return xmlPollSession.OuterXml;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message.ToString());
                return String.Empty;
            }

            return String.Empty;
        }

        public void RunClientSession(NetworkStream client)
        {
            string sendString = String.Empty;
            int pollSessionID = 0;
            byte[] data = new byte[PollServer.DATA_SIZE];

            while (true)
            {
                // Receive from client pollSessionId
                int receivedBytesCount = client.Read(data, 0, PollServer.DATA_SIZE);
                string recievedString = Encoding.ASCII.GetString(data, 0, receivedBytesCount);

                // Get poll sesssion id sent by server
                try
                {
                    pollSessionID = Convert.ToInt32(recievedString);

                    // Check if exists such id
                    if (idExists(pollSessionID))
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
                catch (Exception ex)
                {
                    log.Error(ex.Message.ToString());
                    log.Info(clientAddress + ": Client asked for non-existant ID: " + recievedString);
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