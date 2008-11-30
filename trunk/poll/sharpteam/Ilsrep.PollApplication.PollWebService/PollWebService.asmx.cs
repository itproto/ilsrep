﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using System.Data.SQLite;

namespace Ilsrep.PollApplication.WebService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "Ilsrep.PollApplication.PollWebService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PollWebService : System.Web.Services.WebService
    {
        /// <summary>
        /// Connection string specifying path to database file among other connection parameters
        /// </summary>
        public string connectionString = "Data Source=\"AppData/pollserver.db\"";
        /// <summary>
        /// Database connection
        /// </summary>
        private SQLiteConnection dbConnection = null;
        /// <summary>
        /// Name of Surveys table in database
        /// </summary>
        public const string SURVEYS_TABLE = "surveys";
        /// <summary>
        /// Name of Polls table in database
        /// </summary>
        public const string POLLS_TABLE = "polls";
        /// <summary>
        /// Name of Choices table in database
        /// </summary>
        public const string CHOICES_TABLE = "choices";
        /// <summary>
        /// Name of Surveys and Polls table in database
        /// </summary>
        public const string SURVEY_POLLS_TABLE = "survey_polls";
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
        public bool isConnected
        {
            get
            {
                return (dbConnection != null && dbConnection.State == System.Data.ConnectionState.Open);
            }
        }

        /// <summary>
        /// Close database connection
        /// </summary>
        [WebMethod]
        public void Close()
        {
            if (isConnected)
                dbConnection.Close();
        }

        /// <summary>
        /// Open database connection
        /// </summary>
        [WebMethod]
        private void Init()
        {
            dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
        }

        /// <summary>
        /// Gets names and IDs of Surveys from database
        /// </summary>
        /// <returns>List of Surveys</returns>
        [WebMethod]
        public List<Item> GetSurveys()
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT * FROM " + SURVEYS_TABLE;
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
        /// Gets Survey from database by Survey ID
        /// </summary>
        /// <param name="surveyID">Survey ID that tells which Survey to get</param>
        /// <returns></returns>
        [WebMethod]
        public Survey GetSurvey(int surveyID)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT * FROM " + SURVEYS_TABLE + " WHERE id=:id";
            sqliteCommand.Parameters.AddWithValue(":id", surveyID.ToString());
            SQLiteDataReader sqliteSurvey = sqliteCommand.ExecuteReader();
            Survey survey = new Survey();

            if (!sqliteSurvey.HasRows)
            {
                return survey;
            }

            survey.Id = Convert.ToInt32(sqliteSurvey["id"]);
            survey.Name = sqliteSurvey["name"].ToString();
            survey.TestMode = Convert.ToBoolean(sqliteSurvey["testmode"]);
            survey.MinScore = Convert.ToDouble(sqliteSurvey["minscore"]);
            survey.Polls = new List<Poll>();
            sqliteSurvey.Close();

            sqliteCommand.CommandText = "SELECT p.* FROM " + SURVEY_POLLS_TABLE + " pxp LEFT JOIN " + POLLS_TABLE + " p ON (pxp.poll_id=p.id) WHERE pxp.survey_id=:id";
            SQLiteDataReader sqlPolls = sqliteCommand.ExecuteReader();

            SQLiteCommand sqliteCommand2 = dbConnection.CreateCommand();
            sqliteCommand2.CommandText = "SELECT c.* FROM " + POLL_CHOICES_TABLE + " pxc LEFT JOIN " + CHOICES_TABLE + " c ON (pxc.choice_id=c.id) WHERE pxc.poll_id=:id";
            sqliteCommand2.Parameters.Add(new SQLiteParameter(":id"));

            while (sqlPolls.Read())
            {
                Poll newPoll = new Poll(sqlPolls["name"].ToString());
                newPoll.Id = Convert.ToInt32(sqlPolls["id"]);
                newPoll.Description = sqlPolls["description"].ToString();
                newPoll.CorrectChoiceID = Convert.ToInt32(sqlPolls["correctchoice"]);
                newPoll.CustomChoiceEnabled = Convert.ToBoolean(sqlPolls["customenabled"]);

                sqliteCommand2.Parameters[":id"].Value = newPoll.Id;
                SQLiteDataReader sqlChoices = sqliteCommand2.ExecuteReader();

                while (sqlChoices.Read())
                {
                    Choice newChoice = new Choice(sqlChoices["name"].ToString());
                    newChoice.Id = Convert.ToInt32(sqlChoices["id"]);

                    newPoll.Choices.Add(newChoice);
                }
                sqlChoices.Close();

                survey.Polls.Add(newPoll);
            }
            sqlPolls.Close();

            return survey;
        }

        /// <summary>
        /// Creates new Survey in database
        /// </summary>
        /// <param name="newSurvey">object of Survey that is to be created in database</param>
        /// <returns></returns>
        [WebMethod]
        public int CreateSurvey(Survey newSurvey)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();

            sqliteCommand.Parameters.Add(new SQLiteParameter(":name", newSurvey.Name));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":testmode", newSurvey.TestMode));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":minscore", newSurvey.MinScore));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":date", DateTime.Now.ToString()));
            sqliteCommand.CommandText = "INSERT INTO " + SURVEYS_TABLE + "(name, testmode, minscore, date_created) VALUES(:name, :testmode, :minscore, :date)";
            sqliteCommand.ExecuteNonQuery();

            sqliteCommand.CommandText = "SELECT last_insert_rowid()";
            newSurvey.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

            foreach (Poll curPoll in newSurvey.Polls)
            {
                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.Add(new SQLiteParameter(":name"));

                int index = 0;
                foreach (Choice curChoice in curPoll.Choices)
                {
                    index++;
                    sqliteCommand.Parameters[":name"].Value = curChoice.choice;
                    sqliteCommand.CommandText = "INSERT INTO " + CHOICES_TABLE + "(name) VALUES(:name)";
                    sqliteCommand.ExecuteNonQuery();

                    sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                    curChoice.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

                    if (index == curPoll.CorrectChoiceID)
                    {
                        curPoll.CorrectChoiceID = curChoice.Id;
                    }
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":name", curPoll.Name);
                sqliteCommand.Parameters.AddWithValue(":description", curPoll.Description);
                sqliteCommand.Parameters.AddWithValue(":correctchoice", curPoll.CorrectChoiceID);
                sqliteCommand.Parameters.AddWithValue(":customenabled", curPoll.CustomChoiceEnabled);
                sqliteCommand.CommandText = "INSERT INTO " + POLLS_TABLE + "(name, description, correctchoice, customenabled) VALUES(:name, :description, :correctchoice, :customenabled)";
                sqliteCommand.ExecuteNonQuery();

                sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                curPoll.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

                sqliteCommand.Parameters.Clear();
                sqliteCommand.CommandText = "INSERT INTO " + POLL_CHOICES_TABLE + "(poll_id, choice_id) VALUES(:poll_id, :choice_id)";
                sqliteCommand.Parameters.AddWithValue(":poll_id", curPoll.Id);
                sqliteCommand.Parameters.Add(new SQLiteParameter(":choice_id"));

                foreach (Choice curChoice in curPoll.Choices)
                {
                    sqliteCommand.Parameters[":choice_id"].Value = curChoice.Id;
                    sqliteCommand.ExecuteNonQuery();
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":survey_id", newSurvey.Id);
                sqliteCommand.Parameters.AddWithValue(":poll_id", curPoll.Id);
                sqliteCommand.CommandText = "INSERT INTO " + SURVEY_POLLS_TABLE + "(survey_id, poll_id) VALUES(:survey_id, :poll_id)";
                sqliteCommand.ExecuteNonQuery();
            }

            return Convert.ToInt32(newSurvey.Id);
        }

        /// <summary>
        /// Save changed survey to database
        /// </summary>
        /// <param name="survey">Survey object that is to be changed in database</param>
        [WebMethod]
        public void EditSurvey(Survey survey)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();

            sqliteCommand.Parameters.AddWithValue(":survey_id", survey.Id);
            sqliteCommand.CommandText = "DELETE FROM " + SURVEY_POLLS_TABLE + " WHERE survey_id=:survey_id";
            sqliteCommand.ExecuteNonQuery();

            sqliteCommand.Parameters.Clear();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":id", survey.Id));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":name", survey.Name));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":testmode", survey.TestMode));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":minscore", survey.MinScore));
            sqliteCommand.CommandText = "UPDATE " + SURVEYS_TABLE + " SET name=:name, testmode=:testmode, minscore=:minscore WHERE id=:id";
            sqliteCommand.ExecuteNonQuery();

            foreach (Poll curPoll in survey.Polls)
            {
                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.Add(new SQLiteParameter(":name"));
                sqliteCommand.Parameters.Add(new SQLiteParameter(":id"));

                int index = 0;
                foreach (Choice curChoice in curPoll.Choices)
                {
                    index++;
                    sqliteCommand.Parameters[":name"].Value = curChoice.choice;
                    sqliteCommand.Parameters[":id"].Value = curChoice.Id;
                    if (curChoice.Id <= 0)
                    {
                        sqliteCommand.CommandText = "INSERT INTO " + CHOICES_TABLE + "(name) VALUES(:name)";
                        sqliteCommand.ExecuteNonQuery();

                        sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                        curChoice.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());
                    }
                    else
                    {
                        sqliteCommand.CommandText = "UPDATE " + CHOICES_TABLE + " SET name=:name WHERE id=:id";
                        sqliteCommand.ExecuteNonQuery();
                    }

                    if (index == curPoll.CorrectChoiceID)
                    {
                        curPoll.CorrectChoiceID = curChoice.Id;
                    }
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":id", curPoll.Id);
                sqliteCommand.Parameters.AddWithValue(":name", curPoll.Name);
                sqliteCommand.Parameters.AddWithValue(":description", curPoll.Description);
                sqliteCommand.Parameters.AddWithValue(":correctchoice", curPoll.CorrectChoiceID);
                sqliteCommand.Parameters.AddWithValue(":customenabled", curPoll.CustomChoiceEnabled);
                if (curPoll.Id <= 0)
                {
                    sqliteCommand.CommandText = "INSERT INTO " + POLLS_TABLE + "(name, description, correctchoice, customenabled) VALUES(:name, :description, :correctchoice, :customenabled)";
                    sqliteCommand.ExecuteNonQuery();

                    sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                    curPoll.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());
                }
                else
                {
                    sqliteCommand.CommandText = "UPDATE " + POLLS_TABLE + " SET name=:name, description=:description, correctchoice=:correctchoice, customenabled=:customenabled WHERE id=:id";
                    sqliteCommand.ExecuteNonQuery();
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":poll_id", curPoll.Id);
                sqliteCommand.Parameters.Add(new SQLiteParameter(":choice_id"));

                sqliteCommand.CommandText = "DELETE FROM " + POLL_CHOICES_TABLE + " WHERE poll_id=:poll_id";
                sqliteCommand.ExecuteNonQuery();

                sqliteCommand.CommandText = "INSERT INTO " + POLL_CHOICES_TABLE + "(poll_id, choice_id) VALUES(:poll_id, :choice_id)";
                foreach (Choice curChoice in curPoll.Choices)
                {
                    sqliteCommand.Parameters[":choice_id"].Value = curChoice.Id;
                    sqliteCommand.ExecuteNonQuery();
                }

                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.AddWithValue(":survey_id", survey.Id);
                sqliteCommand.Parameters.AddWithValue(":poll_id", curPoll.Id);

                sqliteCommand.CommandText = "INSERT INTO " + SURVEY_POLLS_TABLE + "(survey_id, poll_id) VALUES(:survey_id, :poll_id)";
                sqliteCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Receive from Client survey result and save it in database
        /// </summary>
        /// <param name="resultsList">list of results</param>
        [WebMethod]
        public void SaveSurveyResult(ResultsList resultsList)
        {
            if (!isConnected)
            {
                Init();
            }

            // Get current date
            //string currentDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();

            sqliteCommand.CommandText = "INSERT INTO " + RESULTS_TABLE + "(user_name, survey_id, question_id, answer_id, custom_choice, date) VALUES(:username, :survey, :question, :answer, :custom, :date)";
            sqliteCommand.Parameters.Add(new SQLiteParameter(":username", resultsList.userName));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":survey", resultsList.surveyId.ToString()));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":date", DateTime.Now.ToString()));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":question"));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":answer"));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":custom"));

            // Insert new surveyResult to database
            foreach (PollResult result in resultsList.results)
            {
                sqliteCommand.Parameters[":question"].Value = result.questionId.ToString();
                sqliteCommand.Parameters[":answer"].Value = result.answerId.ToString();
                sqliteCommand.Parameters[":custom"].Value = result.customChoice;
                sqliteCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Select from DB all results of needed Survey
        /// </summary>
        /// <param name="surveyId">Id of Survey which results we need</param>
        /// <returns>List of results of needed Survey</returns>
        [WebMethod]
        public ResultsList GetSurveyResults(int surveyId)
        {
            if (!isConnected)
            {
                Init();
            }

            ResultsList resultsList = new ResultsList();
            resultsList.surveyId = surveyId;
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":survey_id", surveyId));
            sqliteCommand.CommandText = "SELECT * FROM " + RESULTS_TABLE + " WHERE survey_id=:survey_id";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            // Save each result to resultsList
            while (sqliteReader.Read())
            {
                PollResult curResult = new PollResult();
                curResult.answerId = Convert.ToInt32(sqliteReader["answer_id"].ToString());
                curResult.customChoice = sqliteReader["custom_choice"].ToString();
                // What's wrong? Why "Wrong DateTime" Exception?
                //curResult.date = sqliteReader["date"].ToString();
                curResult.questionId = Convert.ToInt32(sqliteReader["question_id"].ToString());
                curResult.userName = sqliteReader["user_name"].ToString();
                resultsList.results.Add(curResult);
            }

            return resultsList;
        }

        /// <summary>
        /// Removes Survey from database
        /// </summary>
        /// <param name="surveyID">Survey ID that is to be removed</param>
        [WebMethod]
        public void RemoveSurvey(int surveyID)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":survey", surveyID));

            sqliteCommand.CommandText = "DELETE FROM " + SURVEYS_TABLE + " WHERE id=:survey";
            sqliteCommand.ExecuteNonQuery();
            sqliteCommand.CommandText = "DELETE FROM " + SURVEY_POLLS_TABLE + " WHERE survey_id=:survey";
            sqliteCommand.ExecuteNonQuery();
        }

        [WebMethod]
        public User RegisterUser(User user)
        {
            if (!isConnected)
            {
                Init();
            }

            try
            {
                SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
                sqliteCommand.Parameters.Add(new SQLiteParameter(":userName", user.username));
                sqliteCommand.Parameters.Add(new SQLiteParameter(":password", user.password));
                sqliteCommand.CommandText = "INSERT INTO " + USERS_TABLE + "(userName, password) VALUES (:userName, :password)";
                SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
                user.action = Ilsrep.PollApplication.Model.User.AUTH;
            }
            catch (Exception)
            {
                user.action = Ilsrep.PollApplication.Model.User.EXIST;
            }

            return user;
        }

        [WebMethod]
        public User ExistUser(User user)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":userName", user.username));
            sqliteCommand.CommandText = "SELECT * FROM " + USERS_TABLE + " WHERE userName=:userName";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            // if user already exist
            if (sqliteReader.HasRows)
            {
                user.action = Ilsrep.PollApplication.Model.User.EXIST;
            }
            else
            {
                user.action = Ilsrep.PollApplication.Model.User.NEW_USER;
            }

            return user;
        }

        [WebMethod]
        public User AuthorizeUser(User user)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":userName", user.username));
            sqliteCommand.CommandText = "SELECT * FROM " + USERS_TABLE + " WHERE userName=:userName";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            // if user already exist
            if (sqliteReader.HasRows)
            {
                if (sqliteReader["password"].ToString() == user.password)
                {
                    user.action = Ilsrep.PollApplication.Model.User.AUTH;
                }
                else
                {
                    user.action = Ilsrep.PollApplication.Model.User.EXIST;
                }
            }
            else
            {
                user.action = Ilsrep.PollApplication.Model.User.EXIST;
            }
            return user;
        }
    }
}