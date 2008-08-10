using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.DAL
{
    /// <summary>
    /// PollDAL is Poll Database Abstraction Layer for Poll Application. It handles all database connection
    /// </summary>
    public class PollDAL
    {
        private const string connectionString = "Data Source=\"AppData/pollserver.db\"";
        static private SQLiteConnection dbConnection = null;
        /// <summary>
        /// Name of PollSessions table in database
        /// </summary>
        public const string POLLSESSIONS_TABLE = "pollsessions";
        /// <summary>
        /// Name of Polls table in database
        /// </summary>
        public const string POLLS_TABLE = "polls";
        /// <summary>
        /// Name of Choices table in database
        /// </summary>
        public const string CHOICES_TABLE = "choices";
        /// <summary>
        /// Name of PollSessions and Polls table in database
        /// </summary>
        public const string POLLSESSION_POLLS_TABLE = "pollsession_polls";
        /// <summary>
        /// Name of Polls and Choices table in database
        /// </summary>
        public const string POLL_CHOICES_TABLE = "poll_choices";
        /// <summary>
        /// Name of Results table in database
        /// </summary>
        public const string RESULTS_TABLE = "results";

        /// <summary>
        /// Property that tells if database connection is active
        /// </summary>
        static public bool isConnected
        {
            get
            {
                return (dbConnection != null && dbConnection.State == System.Data.ConnectionState.Open);
            }
        }
        
        static private void Init()
        {
            dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
        }

        static private SQLiteDataReader Query(string strQuery)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.CommandText = strQuery;
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            return sqliteReader;
        }

        static private SQLiteDataReader Query(string strQuery, Dictionary<string, SQLiteParameter> paramCollection)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.CommandText = strQuery;
            sqliteCommand.Parameters.AddRange(paramCollection.Values.ToArray());
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            return sqliteReader;
        }

        /// <summary>
        /// Gets names and IDs of Poll Sessions from database
        /// </summary>
        /// <returns>List of Poll Sessions</returns>
        static public List<Item> GetPollSessions()
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteDataReader sqliteReader = Query("SELECT * FROM " + POLLSESSIONS_TABLE);
            List<Item> items = new List<Item>();
            
            while (sqliteReader.Read())
            {
                Item curItem = new Item();
                curItem.id = sqliteReader["id"].ToString();
                curItem.name = sqliteReader["name"].ToString();
                items.Add(curItem);
            }

            return items;
        }

        /// <summary>
        /// Gets PollSession from database by PollSession ID
        /// </summary>
        /// <param name="pollSessionID">PollSession ID that tells which PollSession to get</param>
        /// <returns></returns>
        static public PollSession GetPollSession(int pollSessionID)
        {
            if (!isConnected)
            {
                Init();
            }

            Dictionary<string, SQLiteParameter> paramCollection = new Dictionary<string, SQLiteParameter>();
            //NameValueCollection paramCollection = new NameValueCollection();
            //List<SQLiteParameter> paramCollection = new List<SQLiteParameter>();
            paramCollection.Add("@ID", new SQLiteParameter("@ID", pollSessionID.ToString()));
            SQLiteDataReader sqlitePollSession = Query("SELECT * FROM " + POLLSESSIONS_TABLE + " WHERE id=@ID", paramCollection);

            PollSession pollSession = new PollSession();
            pollSession.id = Convert.ToInt32(sqlitePollSession["id"].ToString());
            pollSession.name = sqlitePollSession["name"].ToString();
            pollSession.testMode = Convert.ToBoolean(sqlitePollSession["test_mode"].ToString());
            pollSession.minScore = Convert.ToDouble(sqlitePollSession["min_score"].ToString());
            pollSession.polls = new List<Poll>();

            SQLiteDataReader sqlPolls = Query("SELECT p.* FROM " + POLLSESSION_POLLS_TABLE + " pxp LEFT JOIN " + POLLS_TABLE + " p ON (pxp.poll_id=p.id) WHERE pxp.pollsession_id=@ID", paramCollection);

            while (sqlPolls.Read())
            {
                Poll newPoll = new Poll();
                newPoll.id = Convert.ToInt32(sqlPolls["id"].ToString());
                newPoll.name = sqlPolls["name"].ToString();
                newPoll.description = sqlPolls["description"].ToString();
                newPoll.correctChoiceID = Convert.ToInt32(sqlPolls["correct_choice_id"].ToString());
                newPoll.customChoiceEnabled = Convert.ToBoolean(sqlPolls["custom_choice_enabled"].ToString());

                //paramCollection[0].Value = newPoll.id;
                SQLiteDataReader sqlChoices = Query("SELECT c.* FROM " + POLL_CHOICES_TABLE + " pxc LEFT JOIN " + CHOICES_TABLE + " c ON (pxc.choice_id=c.id) WHERE pxc.poll_id=@ID", paramCollection);

                while (sqlChoices.Read())
                {
                    Choice newChoice = new Choice();
                    newChoice.id = Convert.ToInt32(sqlChoices["id"].ToString());
                    newChoice.choice = sqlChoices["name"].ToString();

                    newPoll.choices.Add(newChoice);
                }

                pollSession.polls.Add(newPoll);
            }

            return pollSession;
        }

        /// <summary>
        /// Creates new PollSession in database
        /// </summary>
        /// <param name="newPollSession">object of PollSession that is to be created in database</param>
        /// <returns></returns>
        static public int CreatePollSession(PollSession newPollSession)
        {
            Dictionary<string, SQLiteParameter> paramCollection = new Dictionary<string, SQLiteParameter>();
            paramCollection.Add("@NAME", new SQLiteParameter("@NAME", newPollSession.name));
            paramCollection.Add("@TESTMODE", new SQLiteParameter("@TESTMODE", newPollSession.testMode.ToString()));
            paramCollection.Add("@MINSCORE", new SQLiteParameter("@MINSCORE", newPollSession.minScore.ToString()));
            Query("INSERT INTO " + POLLSESSIONS_TABLE + "(name, test_mode, min_score) VALUES('@NAME', '@TESTMODE', '@MINSCORE')", paramCollection);
            
            string newPollSessionID = Query("SELECT last_insert_rowid()")[0].ToString();
            newPollSession.id = Convert.ToInt32(newPollSessionID);

            foreach (Poll curPoll in newPollSession.polls)
            {
                string newChoiceID = String.Empty;
                string newPollID = String.Empty;

                paramCollection.Clear();
                paramCollection.Add("@NAME", new SQLiteParameter("@NAME"));

                foreach (Choice curChoice in curPoll.choices)
                {
                    paramCollection["@NAME"].Value = curChoice.choice;
                    Query("INSERT INTO " + CHOICES_TABLE + "(name) VALUES(@NAME)", paramCollection);

                    if (curChoice.id == curPoll.correctChoiceID)
                    {
                        newChoiceID = Query("SELECT last_insert_rowid()")[0].ToString();
                        curChoice.id = Convert.ToInt32(newChoiceID);
                    }
                    else
                        curChoice.id = Convert.ToInt32(Query("SELECT last_insert_rowid()")[0].ToString());
                }

                paramCollection.Clear();
                paramCollection.Add("@NAME", new SQLiteParameter("@NAME", curPoll.name));
                paramCollection.Add("@DESC", new SQLiteParameter("@DESC", curPoll.description));
                paramCollection.Add("@CHOICE", new SQLiteParameter("@CHOICE", newChoiceID));
                paramCollection.Add("@CUSTOM", new SQLiteParameter("@CUSTOM", curPoll.customChoiceEnabled.ToString()));
                Query("INSERT INTO " + POLLS_TABLE + "(name, description, correct_choice_id, custom_choice_enabled) VALUES('@NAME', '@DESC', '@CHOICE', '@CUSTOM')", paramCollection);
                newPollID = Query("SELECT last_insert_rowid()")[0].ToString();

                paramCollection.Clear();
                paramCollection.Add("@POLLID", new SQLiteParameter("@POLLID", newPollID));
                paramCollection.Add("@CHOICEID", new SQLiteParameter("@CHOICEID"));

                foreach (Choice curChoice in curPoll.choices)
                {
                    paramCollection["@CHOICEID"].Value = curChoice.id;
                    Query("INSERT INTO " + POLL_CHOICES_TABLE + "(poll_id, choice_id) VALUES('@POLLID', '@CHOICEID')", paramCollection);
                }

                paramCollection.Clear();
                paramCollection.Add("@POLLSESSID", new SQLiteParameter("@POLLSESSID", newPollSessionID));
                paramCollection.Add("@POLLID", new SQLiteParameter("@POLLID", newPollID));

                Query("INSERT INTO " + POLLSESSION_POLLS_TABLE + "(pollsession_id, poll_id) VALUES('@POLLSESSID', '@POLLID')", paramCollection);
            }

            return Convert.ToInt32(newPollSessionID);
        }

        /// <summary>
        /// Receive from Client pollsession result and save it in database
        /// </summary>
        /// <param name="resultsList">list of results</param>
        static public void SavePollSessionResult(ResultsList resultsList)
        {
            // Get current date
            string currentDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

            Dictionary<string, SQLiteParameter> paramCollection = new Dictionary<string, SQLiteParameter>();
            paramCollection.Add("@USERNAME", new SQLiteParameter("@USERNAME", resultsList.userName));
            paramCollection.Add("@POLLSESSION", new SQLiteParameter("@POLLSESSION", resultsList.pollsessionId.ToString()));
            paramCollection.Add("@QUESTION", new SQLiteParameter("@QUESTION"));
            paramCollection.Add("@ANSWER", new SQLiteParameter("@ANSWER"));
            paramCollection.Add("@CUSTOM", new SQLiteParameter("@CUSTOM"));
            paramCollection.Add("@DATE", new SQLiteParameter("@DATE", currentDate));

            // Insert new pollSessionResult to database
            foreach (PollResult result in resultsList.results)
            {
                paramCollection["@QUESTION"].Value = result.questionId.ToString();
                paramCollection["@ANSWER"].Value = result.answerId.ToString();
                paramCollection["@CUSTOM"].Value = result.customChoice;

                Query("INSERT INTO " + RESULTS_TABLE + "(user_name, pollsession_id, question_id, answer_id, custom_choice, date) VALUES('@USERNAME', '@POLLSESSION', '@QUESTION', '@ANSWER', '@CUSTOM', '@DATE')", paramCollection);
            }
        }

        /// <summary>
        /// Removes PollSession from database
        /// </summary>
        /// <param name="pollSessionID">PollSession ID that is to be removed</param>
        static public void RemovePollSession(string pollSessionID)
        {
            Dictionary<string, SQLiteParameter> paramCollection = new Dictionary<string, SQLiteParameter>();
            paramCollection.Add("@POLLSESSION", new SQLiteParameter("@POLLSESSION", pollSessionID));

            Query("DELETE FROM " + POLLSESSIONS_TABLE + " WHERE id='@POLLSESSION'", paramCollection);
            Query("DELETE FROM " + POLLSESSION_POLLS_TABLE + " WHERE pollsession_id='@POLLSESSION'", paramCollection);
        }
    }
}
