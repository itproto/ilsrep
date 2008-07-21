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
using System.Data.SQLite;

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
        public const string POLLS_TABLE_NAME = "polls";
        public const string RESULTS_TABLE_NAME = "results";

        /// <summary>
        /// The function execute query to data base
        /// </summary>
        /// <param name="query">query in string</param>
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        /// <returns>result of query</returns>
        public static SQLiteDataReader Query(string query, SQLiteConnection curDataBaseCon)
        {
            SQLiteCommand command = new SQLiteCommand(query, curDataBaseCon);
            SQLiteDataReader result = command.ExecuteReader();
            return result;
        }

        /// <summary>
        /// The function search poll session by id
        /// </summary>
        /// <param name="pollSessionIdStr">Id of needed poll session</param>
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        /// <returns>Poll session</returns>
        /// <exception>null</exception>
        private static PollSession GetPollSessionById(string pollSessionIdStr, SQLiteConnection curDataBaseCon)
        {
            try
            {
                // Search patch to needed file by id
                int pollSessionId = Convert.ToInt32(pollSessionIdStr);
                string pollSessionXml = Query("SELECT * from " + POLLS_TABLE_NAME + " where id='" + pollSessionId + "'", curDataBaseCon)["xml"].ToString();
                PollSession pollSession = PollSerializator.DeSerialize(pollSessionXml);
                return pollSession;
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
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
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        public void RunClientSession(NetworkStream client, SQLiteConnection curDataBaseCon)
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
                        SendPollSessionsList(client, curDataBaseCon);
                        break;
                    case Request.GET_POLLSESSION:
                        SendPollSession(client, curDataBaseCon);
                        break; 
                    case Request.CREATE_POLLSESSION:
                        CreatePollSession(client, curDataBaseCon);
                        break;
                    case Request.SAVE_RESULT:
                        SavePollSessionResult(client, curDataBaseCon);
                        break;
                    default:
                        log.Error("Invalid option sent by client");
                        break;
                }
            }
        }

        /// <summary>
        /// Receive from Client pollsession result and save it in database
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        public void SavePollSessionResult(NetworkStream client, SQLiteConnection curDataBaseCon)
        {
            // Generate new id
            int newId;
            try
            {
                SQLiteDataReader sqliteReader = Query("SELECT * from " + RESULTS_TABLE_NAME + " order by id DESC", curDataBaseCon);
                newId = Convert.ToInt32(sqliteReader["id"]) + 1;
            }
            catch
            {
                newId = 1;
            }

            // Insert new pollSessionResult to database
            foreach(PollSessionResult result in receivedPacket.resultsList.results)
            {
                try
                {
                    Query("INSERT into " + RESULTS_TABLE_NAME + " values('" + newId + "', '" + receivedPacket.resultsList.userName + "', '" + receivedPacket.resultsList.pollsessionId + "', '" + result.questionId + "', '" + result.answerId + "', '" + result.customChoice + "', DATETIME('NOW'))", curDataBaseCon);

                    // Check DB record
                    SQLiteDataReader r = Query("SELECT * from " + RESULTS_TABLE_NAME + " WHERE id='" + newId + "'", curDataBaseCon);
                    Console.WriteLine("id={0} name={1} pollsession_id={2} question_id={3} answer_id={4} custom_choice={5} date={6}", r["id"], r["user_name"], r["pollsession_id"], r["question_id"], r["answer_id"], r["custom_choice"], r["date"]);
                }
                catch (Exception exception)
                {
                    log.Error(exception.Message);
                }
                newId++;
            }
        }

        /// <summary>
        /// Receive new pollsession from PollEditor and save it to database
        /// </summary>
        /// <param name="client">NetworkStream client</param>
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        public void CreatePollSession(NetworkStream client, SQLiteConnection curDataBaseCon)
        {
            PollSession newPollSession = new PollSession();
            newPollSession = receivedPacket.pollSession;

            // Generate new id
            try
            {
                SQLiteDataReader sqliteReader = Query("SELECT * from " + POLLS_TABLE_NAME + " order by id DESC", curDataBaseCon);
                newPollSession.id = Convert.ToInt32(sqliteReader["id"]) + 1;
            }
            catch
            {
                newPollSession.id = 1;
            }
            
            try
            {
                // Save newPollSession in data base
                string newPollSessionString = PollSerializator.Serialize(newPollSession);
                Query("INSERT into " + POLLS_TABLE_NAME + " values ('" + newPollSession.id + "','" + newPollSession.name + "','" + newPollSessionString + "')", curDataBaseCon);
                log.Info("New PollSession has been added(name=\"" + newPollSession.name + "\" id=\"" + newPollSession.id + "\")");
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
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        public void SendPollSessionsList(NetworkStream client, SQLiteConnection curDataBaseCon)
        {
            string sendString;
            PollPacket sendPacket = new PollPacket();

            try
            {
                SQLiteDataReader sqliteReader = Query("SELECT * from " + POLLS_TABLE_NAME, curDataBaseCon);
                while(sqliteReader.Read())
                {
                    Item curItem = new Item();
                    curItem.id = sqliteReader["id"].ToString();
                    curItem.name = sqliteReader["name"].ToString();
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
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        public void SendPollSession(NetworkStream client, SQLiteConnection curDataBaseCon)
        {
            string pollSessionId = receivedPacket.request.id;
            
            // Send PollSession
            sendPacket.pollSession = GetPollSessionById(pollSessionId, curDataBaseCon);
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
            
            //Connect to data base
            SQLiteConnection curDataBaseCon = new SQLiteConnection();
            try
            {
                curDataBaseCon = new SQLiteConnection("data source=\"" + PollServer.pathToPolls + "\"");
                curDataBaseCon.Open();
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
            }

            // Run dialog with client
            RunClientSession(currentStream, curDataBaseCon);

            // Close clients connection
            curDataBaseCon.Close();
            currentStream.Close();
            currentClient.Close();
            activeConnCount--;
            log.Info("Client disconnected: " + clientAddress + " (" + activeConnCount + " active connections left)");
        }
    }
}