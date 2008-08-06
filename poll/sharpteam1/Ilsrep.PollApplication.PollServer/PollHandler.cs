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
        public const string POLLSESSIONS_TABLE = "pollsessions";
        public const string POLLS_TABLE = "polls";
        public const string CHOICES_TABLE = "choices";
        public const string POLLSESSION_POLLS_TABLE = "pollsessions_polls";
        public const string POLL_CHOICES_TABLE = "polls_choices";
        public const string RESULTS_TABLE = "results";


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
        /// <param name="strPollSessionID">Id of needed poll session</param>
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        /// <returns>Poll session</returns>
        /// <exception>null</exception>
        private static PollSession GetPollSessionById(string strPollSessionID, SQLiteConnection curDataBaseCon)
        {
            try
            {
                int pollSessionID = Convert.ToInt32(strPollSessionID);
                SQLiteDataReader sqlPollSession = Query("SELECT * FROM " + POLLSESSIONS_TABLE + " WHERE id=" + pollSessionID, curDataBaseCon);

                PollSession pollSession = new PollSession();
                pollSession.id = Convert.ToInt32(sqlPollSession["id"].ToString());
                pollSession.name = sqlPollSession["name"].ToString();
                pollSession.testMode = Convert.ToBoolean(sqlPollSession["test_mode"].ToString());
                pollSession.minScore = Convert.ToDouble(sqlPollSession["min_score"].ToString());
                pollSession.polls = new List<Poll>();

                SQLiteDataReader sqlPolls = Query("SELECT p.* FROM " + POLLSESSION_POLLS_TABLE + " pxp LEFT JOIN " + POLLS_TABLE + " p ON (pxp.poll_id=p.id) WHERE pxp.pollsession_id=" + pollSessionID, curDataBaseCon);

                while (sqlPolls.Read())
                {
                    Poll newPoll = new Poll();
                    newPoll.id = Convert.ToInt32(sqlPolls["id"].ToString());
                    newPoll.name = sqlPolls["name"].ToString();
                    newPoll.description = sqlPolls["description"].ToString();
                    newPoll.correctChoiceId = Convert.ToInt32(sqlPolls["correct_choice_id"].ToString());
                    newPoll.customChoice = Convert.ToBoolean(sqlPolls["custom_choice_enabled"].ToString());

                    SQLiteDataReader sqlChoices = Query("SELECT c.* FROM " + POLL_CHOICES_TABLE + " pxc LEFT JOIN " + CHOICES_TABLE + " c ON (pxc.choice_id=c.id) WHERE pxc.poll_id=" + newPoll.id, curDataBaseCon);

                    while (sqlChoices.Read())
                    {
                        Choice newChoice = new Choice();
                        newChoice.id = Convert.ToInt32(sqlChoices["id"].ToString());
                        newChoice.choice = sqlChoices["name"].ToString();

                        newPoll.choices.Add(newChoice);
                    }

                    pollSession.polls.Add(newPoll);
                }

                //string pollSessionXml = Query("SELECT * from " + POLLS_TABLE_NAME + " where id='" + pollSessionId + "'", curDataBaseCon)["xml"].ToString();
                //PollSession pollSession = PollSerializator.DeserializePollSession(pollSessionXml);
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
                    case Request.REMOVE_POLLSESSION:
                        RemovePollSession(client, curDataBaseCon);
                        break;
                    default:
                        log.Error("Invalid option sent by client");
                        break;
                }
            }
        }

        public void RemovePollSession(NetworkStream client, SQLiteConnection curDataBaseCon)
        {
            try
            {
                int pollSessionID = Convert.ToInt32(receivedPacket.request.id);
                SQLiteDataReader sqliteReader = Query("DELETE FROM " + POLLSESSIONS_TABLE + " WHERE id='" + pollSessionID + "'", curDataBaseCon);
                sqliteReader = Query("DELETE FROM " + POLLSESSION_POLLS_TABLE + " WHERE pollsession_id='" + pollSessionID + "'", curDataBaseCon);
                log.Info("Pollsession deleted - " + pollSessionID);
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
        /// <param name="curDataBaseCon">connection of current client to data base</param>
        public void SavePollSessionResult(NetworkStream client, SQLiteConnection curDataBaseCon)
        {
            // Generate new id
            int newId;
            try
            {
                SQLiteDataReader sqliteReader = Query("SELECT * FROM " + RESULTS_TABLE + " ORDER BY ID DESC LIMIT 1", curDataBaseCon);
                newId = Convert.ToInt32(sqliteReader["id"]) + 1;
            }
            catch
            {
                newId = 1;
            }

            // Get current date
            string currentDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

            // Insert new pollSessionResult to database
            foreach(PollResult result in receivedPacket.resultsList.results)
            {
                try
                {
                    Query("INSERT INTO " + RESULTS_TABLE + " values('" + newId + "', '" + receivedPacket.resultsList.userName + "', '" + receivedPacket.resultsList.pollsessionId + "', '" + result.questionId + "', '" + result.answerId + "', '" + result.customChoice + "', '" + currentDate + "')", curDataBaseCon);

                    // Check pollSessionResult record by selecting him from DB and writing to console
                    SQLiteDataReader r = Query("SELECT * FROM " + RESULTS_TABLE + " WHERE id='" + newId + "'", curDataBaseCon);
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

            /*
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
            */

            // Save newPollSession in data base
            try
            {
                //string newPollSessionString = PollSerializator.SerializePollSession(newPollSession);
                Query("INSERT INTO " + POLLSESSIONS_TABLE + "(name, test_mode, min_score) VALUES('"+newPollSession.name+"', '"+newPollSession.testMode.ToString()+"', '"+newPollSession.minScore.ToString()+"')", curDataBaseCon);
                String newPollSessionID = Query("SELECT last_insert_rowid()", curDataBaseCon)[0].ToString();
                newPollSession.id = Convert.ToInt32(newPollSessionID);

                foreach (Poll curPoll in newPollSession.polls)
                {
                    String newChoiceID = String.Empty;
                    String newPollID = String.Empty;
                    
                    foreach (Choice curChoice in curPoll.choices)
                    {
                        Query("INSERT INTO " + CHOICES_TABLE + "(name) VALUES('" + curChoice.choice + "')", curDataBaseCon);

                        /* Maybe better way? equivalent of mysql_last_id */
                        if (curChoice.id == curPoll.correctChoiceId)
                        {
                            newChoiceID = Query("SELECT last_insert_rowid()", curDataBaseCon)[0].ToString();
                            curChoice.id = Convert.ToInt32(newChoiceID);
                        }
                        else
                            curChoice.id = Convert.ToInt32(Query("SELECT last_insert_rowid()", curDataBaseCon)[0].ToString());
                            //newCorrectID = Query("SELECT id FROM " + CHOICES_TABLE + " ORDER BY id DESC LIMIT 1", curDataBaseCon)["id"].ToString();
                    }

                    Query("INSERT INTO " + POLLS_TABLE + "(name, description, correct_choice_id, custom_choice_enabled) VALUES('" + curPoll.name + "', '" + curPoll.description + "', '" + newChoiceID + "', '" + curPoll.customChoice.ToString() + "')", curDataBaseCon);
                    newPollID = Query("SELECT last_insert_rowid()", curDataBaseCon)[0].ToString();

                    foreach (Choice curChoice in curPoll.choices)
                        Query("INSERT INTO " + POLL_CHOICES_TABLE + "(poll_id, choice_id) VALUES('" + newPollID + "', '" + curChoice.id + "')", curDataBaseCon);

                    Query("INSERT INTO " + POLLSESSION_POLLS_TABLE + "(pollsession_id, poll_id) VALUES('" + newPollSessionID + "', '" + newPollID + "')", curDataBaseCon);
                }
                

                //Query("INSERT into " + POLLS_TABLE_NAME + " values ('" + newPollSession.id + "','" + newPollSession.name + "','" + newPollSessionString + "')", curDataBaseCon);
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
                SQLiteDataReader sqliteReader = Query("SELECT * FROM " + POLLSESSIONS_TABLE, curDataBaseCon);
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
            
            //Create new connection to database
            SQLiteConnection curDataBaseCon = new SQLiteConnection();
            try
            {
                curDataBaseCon = new SQLiteConnection("data source=\"" + PollServer.pathToDatabase + "\"");
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