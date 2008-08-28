﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;

namespace Ilsrep.PollApplication.DAL
{
    /// <summary>
    /// PollDAL is Poll Database Abstraction Layer for Poll Application. It handles all database connection
    /// </summary>
    public class PollDAL
    {
        /// <summary>
        /// Connection string specifying path to database file among other connection parameters
        /// </summary>
        static public string connectionString = "Data Source=\"AppData/pollserver.db\"";
        /// <summary>
        /// Database connection
        /// </summary>
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
        /// Name of Users table in database
        /// </summary>
        public const string USERS_TABLE = "users";

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

        /// <summary>
        /// Close database connection
        /// </summary>
        static public void Close()
        {
            if (isConnected)
                dbConnection.Close();
        }

        /// <summary>
        /// Open database connection
        /// </summary>
        static private void Init()
        {
            dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
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

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT * FROM " + POLLSESSIONS_TABLE;
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
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

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT * FROM " + POLLSESSIONS_TABLE + " WHERE id=:id";
            sqliteCommand.Parameters.AddWithValue(":id", pollSessionID.ToString());
            SQLiteDataReader sqlitePollSession = sqliteCommand.ExecuteReader();
            PollSession pollSession = new PollSession();

            if (!sqlitePollSession.HasRows)
            {
                return pollSession;
            }

            pollSession.id = Convert.ToInt32(sqlitePollSession["id"]);
            pollSession.name = sqlitePollSession["name"].ToString();
            pollSession.testMode = Convert.ToBoolean(sqlitePollSession["testmode"]);
            pollSession.minScore = Convert.ToDouble(sqlitePollSession["minscore"]);
            pollSession.polls = new List<Poll>();
            sqlitePollSession.Close();

            sqliteCommand.CommandText = "SELECT p.* FROM " + POLLSESSION_POLLS_TABLE + " pxp LEFT JOIN " + POLLS_TABLE + " p ON (pxp.poll_id=p.id) WHERE pxp.pollsession_id=:id";
            SQLiteDataReader sqlPolls = sqliteCommand.ExecuteReader();

            SQLiteCommand sqliteCommand2 = dbConnection.CreateCommand();
            sqliteCommand2.CommandText = "SELECT c.* FROM " + POLL_CHOICES_TABLE + " pxc LEFT JOIN " + CHOICES_TABLE + " c ON (pxc.choice_id=c.id) WHERE pxc.poll_id=:id";
            sqliteCommand2.Parameters.Add(new SQLiteParameter(":id"));

            while (sqlPolls.Read())
            {
                Poll newPoll = new Poll();
                newPoll.id = Convert.ToInt32(sqlPolls["id"]);
                newPoll.name = sqlPolls["name"].ToString();
                newPoll.description = sqlPolls["description"].ToString();
                newPoll.correctChoiceID = Convert.ToInt32(sqlPolls["correctchoice"]);
                newPoll.customChoiceEnabled = Convert.ToBoolean(sqlPolls["customenabled"]);

                sqliteCommand2.Parameters[":id"].Value = newPoll.id;
                SQLiteDataReader sqlChoices = sqliteCommand2.ExecuteReader();

                while (sqlChoices.Read())
                {
                    Choice newChoice = new Choice();
                    newChoice.id = Convert.ToInt32(sqlChoices["id"]);
                    newChoice.choice = sqlChoices["name"].ToString();

                    newPoll.choices.Add(newChoice);
                }
                sqlChoices.Close();

                pollSession.polls.Add(newPoll);
            }
            sqlPolls.Close();

            return pollSession;
        }

        /// <summary>
        /// Creates new PollSession in database
        /// </summary>
        /// <param name="newPollSession">object of PollSession that is to be created in database</param>
        /// <returns></returns>
        static public int CreatePollSession(PollSession newPollSession)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();

            sqliteCommand.Parameters.Add(new SQLiteParameter(":name", newPollSession.name));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":testmode", newPollSession.testMode));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":minscore", newPollSession.minScore));
            sqliteCommand.CommandText = "INSERT INTO " + POLLSESSIONS_TABLE + "(name, testmode, minscore) VALUES(:name, :testmode, :minscore)";
            sqliteCommand.ExecuteNonQuery();

            sqliteCommand.CommandText = "SELECT last_insert_rowid()";
            newPollSession.id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

            foreach (Poll curPoll in newPollSession.polls)
            {
                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.Add(new SQLiteParameter(":name"));

                int index = 0;
                foreach (Choice curChoice in curPoll.choices)
                {
                    index++;
                    sqliteCommand.Parameters[":name"].Value = curChoice.choice;
                    sqliteCommand.CommandText = "INSERT INTO " + CHOICES_TABLE + "(name) VALUES(:name)";
                    sqliteCommand.ExecuteNonQuery();

                    sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                    curChoice.id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

                    if (index == curPoll.correctChoiceID)
                    {
                        curPoll.correctChoiceID = curChoice.id;
                    }
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":name", curPoll.name);
                sqliteCommand.Parameters.AddWithValue(":description", curPoll.description);
                sqliteCommand.Parameters.AddWithValue(":correctchoice", curPoll.correctChoiceID);
                sqliteCommand.Parameters.AddWithValue(":customenabled", curPoll.customChoiceEnabled);
                sqliteCommand.CommandText = "INSERT INTO " + POLLS_TABLE + "(name, description, correctchoice, customenabled) VALUES(:name, :description, :correctchoice, :customenabled)";
                sqliteCommand.ExecuteNonQuery();
                
                sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                curPoll.id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

                sqliteCommand.Parameters.Clear();
                sqliteCommand.CommandText = "INSERT INTO " + POLL_CHOICES_TABLE + "(poll_id, choice_id) VALUES(:poll_id, :choice_id)";
                sqliteCommand.Parameters.AddWithValue(":poll_id", curPoll.id);
                sqliteCommand.Parameters.Add(new SQLiteParameter(":choice_id"));

                foreach (Choice curChoice in curPoll.choices)
                {
                    sqliteCommand.Parameters[":choice_id"].Value = curChoice.id;
                    sqliteCommand.ExecuteNonQuery();
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":pollsession_id", newPollSession.id);
                sqliteCommand.Parameters.AddWithValue(":poll_id", curPoll.id);
                sqliteCommand.CommandText = "INSERT INTO " + POLLSESSION_POLLS_TABLE + "(pollsession_id, poll_id) VALUES(:pollsession_id, :poll_id)";
                sqliteCommand.ExecuteNonQuery();
            }

            return Convert.ToInt32(newPollSession.id);
        }

        /// <summary>
        /// Save changed pollsession to database
        /// </summary>
        /// <param name="pollSession">PollSession object that is to be changed in database</param>
        static public void EditPollSession(PollSession pollSession)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();

            sqliteCommand.Parameters.AddWithValue(":pollsession_id", pollSession.id);
            sqliteCommand.CommandText = "DELETE FROM " + POLLSESSION_POLLS_TABLE + " WHERE pollsession_id=:pollsession_id";
            sqliteCommand.ExecuteNonQuery();

            sqliteCommand.Parameters.Clear();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":id", pollSession.id));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":name", pollSession.name));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":testmode", pollSession.testMode));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":minscore", pollSession.minScore));
            sqliteCommand.CommandText = "UPDATE " + POLLSESSIONS_TABLE + " SET name=:name, testmode=:testmode, minscore=:minscore WHERE id=:id";
            sqliteCommand.ExecuteNonQuery();
            
            foreach (Poll curPoll in pollSession.polls)
            {
                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.Add(new SQLiteParameter(":name"));
                sqliteCommand.Parameters.Add(new SQLiteParameter(":id"));

                int index = 0;
                foreach (Choice curChoice in curPoll.choices)
                {
                    index++;
                    sqliteCommand.Parameters[":name"].Value = curChoice.choice;
                    sqliteCommand.Parameters[":id"].Value = curChoice.id;
                    if (curChoice.id == 0)
                    {
                        sqliteCommand.CommandText = "INSERT INTO " + CHOICES_TABLE + "(name) VALUES(:name)";
                        sqliteCommand.ExecuteNonQuery();

                        sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                        curChoice.id = Convert.ToInt32(sqliteCommand.ExecuteScalar());
                    }
                    else
                    {
                        sqliteCommand.CommandText = "UPDATE " + CHOICES_TABLE + " SET name=:name WHERE id=:id";
                        sqliteCommand.ExecuteNonQuery();
                    }

                    if (index == curPoll.correctChoiceID)
                    {
                        curPoll.correctChoiceID = curChoice.id;
                    }
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":id", curPoll.id);
                sqliteCommand.Parameters.AddWithValue(":name", curPoll.name);
                sqliteCommand.Parameters.AddWithValue(":description", curPoll.description);
                sqliteCommand.Parameters.AddWithValue(":correctchoice", curPoll.correctChoiceID);
                sqliteCommand.Parameters.AddWithValue(":customenabled", curPoll.customChoiceEnabled);
                if (curPoll.id == 0)
                {
                    sqliteCommand.CommandText = "INSERT INTO " + POLLS_TABLE + "(name, description, correctchoice, customenabled) VALUES(:name, :description, :correctchoice, :customenabled)";
                    sqliteCommand.ExecuteNonQuery();

                    sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                    curPoll.id = Convert.ToInt32(sqliteCommand.ExecuteScalar());
                }
                else
                {
                    sqliteCommand.CommandText = "UPDATE " + POLLS_TABLE + " SET name=:name, description=:description, correctchoice=:correctchoice, customenabled=:customenabled WHERE id=:id";
                    sqliteCommand.ExecuteNonQuery();
                }
                
                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":poll_id", curPoll.id);
                sqliteCommand.Parameters.Add(new SQLiteParameter(":choice_id"));

                sqliteCommand.CommandText = "DELETE FROM " + POLL_CHOICES_TABLE + " WHERE poll_id=:poll_id";
                sqliteCommand.ExecuteNonQuery();

                sqliteCommand.CommandText = "INSERT INTO " + POLL_CHOICES_TABLE + "(poll_id, choice_id) VALUES(:poll_id, :choice_id)";
                foreach (Choice curChoice in curPoll.choices)
                {
                    sqliteCommand.Parameters[":choice_id"].Value = curChoice.id;
                    sqliteCommand.ExecuteNonQuery();
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":pollsession_id", pollSession.id);
                sqliteCommand.Parameters.AddWithValue(":poll_id", curPoll.id);

                sqliteCommand.CommandText = "INSERT INTO " + POLLSESSION_POLLS_TABLE + "(pollsession_id, poll_id) VALUES(:pollsession_id, :poll_id)";
                sqliteCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Receive from Client pollsession result and save it in database
        /// </summary>
        /// <param name="resultsList">list of results</param>
        static public void SavePollSessionResult(ResultsList resultsList)
        {
            if (!isConnected)
            {
                Init();
            }
            
            // Get current date
            string currentDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            
            sqliteCommand.CommandText = "INSERT INTO " + RESULTS_TABLE + "(user_name, pollsession_id, question_id, answer_id, custom_choice, date) VALUES(:username, :pollsession, :question, :answer, :custom, :date)";
            sqliteCommand.Parameters.Add(new SQLiteParameter(":username", resultsList.userName));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":pollsession", resultsList.pollsessionId.ToString()));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":date", currentDate));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":question"));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":answer"));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":custom"));

            // Insert new pollSessionResult to database
            foreach (PollResult result in resultsList.results)
            {
                sqliteCommand.Parameters[":question"].Value = result.questionId.ToString();
                sqliteCommand.Parameters[":answer"].Value = result.answerId.ToString();
                sqliteCommand.Parameters[":custom"].Value = result.customChoice;
                sqliteCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Removes PollSession from database
        /// </summary>
        /// <param name="pollSessionID">PollSession ID that is to be removed</param>
        static public void RemovePollSession(int pollSessionID)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":pollsession", pollSessionID));

            sqliteCommand.CommandText = "DELETE FROM " + POLLSESSIONS_TABLE + " WHERE id=:pollsession";
            sqliteCommand.ExecuteNonQuery();
            sqliteCommand.CommandText = "DELETE FROM " + POLLSESSION_POLLS_TABLE + " WHERE pollsession_id=:pollsession";
            sqliteCommand.ExecuteNonQuery();
        }

        static public User AuthorizeUser(User user)
        {
            if (!isConnected)
            {
                Init();
            }

            if (!user.isNew)
            {
                SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
                sqliteCommand.Parameters.Add(new SQLiteParameter(":userName", user.username));
                sqliteCommand.CommandText = "SELECT * FROM " + USERS_TABLE + " WHERE userName=:userName";
                SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
                if (sqliteReader.HasRows)
                {
                    if (sqliteReader["password"].ToString() == user.password)
                    {
                        user.auth = true;
                    }
                    else
                    {
                        user.auth = false;
                    }
                }
                else
                {
                    user.isNew = true;
                }
            }
            else
            {
                SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
                sqliteCommand.Parameters.Add(new SQLiteParameter(":userName", user.username));
                sqliteCommand.Parameters.Add(new SQLiteParameter(":password", user.password));
                sqliteCommand.CommandText = "INSERT INTO " + USERS_TABLE + "(userName, password) VALUES (:userName, :password)";
                SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
                user.auth = true;
            }

            return user;
        }
    }
}