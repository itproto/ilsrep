using System;
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
        /// Gets names and IDs of Surveys from database
        /// </summary>
        /// <returns>List of Surveys</returns>
        static public List<Item> GetSurveys()
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
                curItem.id = Convert.ToInt32(sqliteReader["id"]);
                curItem.name = sqliteReader["name"].ToString();
                items.Add(curItem);
            }

            return items;
        }

        /// <summary>
        /// Gets users from database
        /// </summary>
        /// <returns>List of Surveys</returns>
        static public List<String> GetUsers()
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT * FROM " + USERS_TABLE;
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
            List<String> users = new List<String>();

            while (sqliteReader.Read())
            {
                users.Add(sqliteReader["userName"].ToString());
            }

            return users;
        }

        /// <summary>
        /// Gets names and IDs of test Surveys from database
        /// </summary>
        /// <returns>List of Surveys</returns>
        static public List<Item> GetTestSurveys()
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.AddWithValue(":testmode", "1");
            sqliteCommand.CommandText = "SELECT * FROM " + SURVEYS_TABLE + " WHERE testmode=:testmode";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
            List<Item> items = new List<Item>();

            while (sqliteReader.Read())
            {
                Item curItem = new Item();
                curItem.id = Convert.ToInt32(sqliteReader["id"]);
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
        static public Survey GetSurvey(int surveyID)
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
        /// Gets Poll from database by poll ID
        /// </summary>
        /// <param name="surveyID">Poll ID that tells which poll to get</param>
        /// <returns></returns>
        static public Poll GetPoll(int pollID)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqlitePollCommand = dbConnection.CreateCommand();
            sqlitePollCommand.Parameters.AddWithValue(":id", pollID.ToString());
            sqlitePollCommand.CommandText = "SELECT * FROM " + POLLS_TABLE + " WHERE id=:id";
            
            SQLiteDataReader sqlitePoll = sqlitePollCommand.ExecuteReader();
            Poll poll = new Poll();

            if (!sqlitePoll.HasRows)
            {
                return poll;
            }

            poll.Id = pollID;
            poll.Description = sqlitePoll["description"].ToString();

            SQLiteCommand sqliteChoicesCommand = dbConnection.CreateCommand();
            sqliteChoicesCommand.CommandText = "SELECT c.* FROM " + POLL_CHOICES_TABLE + " pxc LEFT JOIN " + CHOICES_TABLE + " c ON (pxc.choice_id=c.id) WHERE pxc.poll_id=:id";
            sqliteChoicesCommand.Parameters.AddWithValue(":id", poll.Id);
            SQLiteDataReader sqliteChoices = sqliteChoicesCommand.ExecuteReader();

            while (sqliteChoices.Read())
            {
                Choice newChoice = new Choice(sqliteChoices["name"].ToString());
                newChoice.Id = Convert.ToInt32(sqliteChoices["id"]);

                poll.Choices.Add(newChoice);
            }
            sqliteChoices.Close();

            return poll;
        }

        /// <summary>
        /// Gets the choice name
        /// </summary>
        /// <param name="choiceID">Choice ID</param>
        /// <returns>Choice name</returns>
        static public string GetChoice(int choiceID)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.AddWithValue(":id", choiceID);
            sqliteCommand.CommandText = "SELECT * FROM " + CHOICES_TABLE + " WHERE id=:id";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
            return sqliteReader["name"].ToString();
        }

        /// <summary>
        /// Creates new Survey in database
        /// </summary>
        /// <param name="newSurvey">object of Survey that is to be created in database</param>
        /// <returns></returns>
        static public int CreateSurvey(Survey newSurvey)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();

            sqliteCommand.Parameters.Add(new SQLiteParameter(":name", newSurvey.Name));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":testmode", newSurvey.TestMode));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":minscore", newSurvey.MinScore));
            sqliteCommand.Parameters.Add( new SQLiteParameter( ":date", DateTime.Now.ToString() ) );
            sqliteCommand.CommandText = "INSERT INTO " + SURVEYS_TABLE + "(name, testmode, minscore, date_created) VALUES(:name, :testmode, :minscore, :date)";
            sqliteCommand.ExecuteNonQuery();

            sqliteCommand.CommandText = "SELECT last_insert_rowid()";
            newSurvey.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

            foreach (Poll curPoll in newSurvey.Polls)
            {
                sqliteCommand.Parameters.Clear();
                sqliteCommand.Parameters.Add(new SQLiteParameter(":name"));

                foreach (Choice curChoice in curPoll.Choices)
                {
                    int oldId = curChoice.Id;
                    sqliteCommand.Parameters[":name"].Value = curChoice.choice;
                    sqliteCommand.CommandText = "INSERT INTO " + CHOICES_TABLE + "(name) VALUES(:name)";
                    sqliteCommand.ExecuteNonQuery();

                    sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                    curChoice.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

                    if (oldId == curPoll.CorrectChoiceID)
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
        /// Creates new Poll in database
        /// </summary>
        /// <param name="poll">object of Poll that is to be created in database</param>
        /// <returns>id of created item</returns>
        static public int CreatePoll(Poll poll)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":name"));
            foreach (Choice curChoice in poll.Choices)
            {
                int oldId = curChoice.Id;
                sqliteCommand.Parameters[":name"].Value = curChoice.choice;
                sqliteCommand.CommandText = "INSERT INTO " + CHOICES_TABLE + "(name) VALUES(:name)";
                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.CommandText = "SELECT last_insert_rowid()";
                curChoice.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());
            }

            sqliteCommand.Parameters.Clear();
            sqliteCommand.Parameters.AddWithValue(":name", poll.Name);
            sqliteCommand.Parameters.AddWithValue(":description", poll.Description);
            sqliteCommand.Parameters.AddWithValue(":correctchoice", -1);
            sqliteCommand.Parameters.AddWithValue(":customenabled", false);
            sqliteCommand.CommandText = "INSERT INTO " + POLLS_TABLE + "(name, description, correctchoice, customenabled) VALUES(:name, :description, :correctchoice, :customenabled)";
            sqliteCommand.ExecuteNonQuery();
            sqliteCommand.CommandText = "SELECT last_insert_rowid()";
            poll.Id = Convert.ToInt32(sqliteCommand.ExecuteScalar());

            sqliteCommand.Parameters.Clear();
            sqliteCommand.CommandText = "INSERT INTO " + POLL_CHOICES_TABLE + "(poll_id, choice_id) VALUES(:poll_id, :choice_id)";
            sqliteCommand.Parameters.AddWithValue(":poll_id", poll.Id);
            sqliteCommand.Parameters.Add(new SQLiteParameter(":choice_id"));
            foreach (Choice curChoice in poll.Choices)
            {
                sqliteCommand.Parameters[":choice_id"].Value = curChoice.Id;
                sqliteCommand.ExecuteNonQuery();
            }

            return Convert.ToInt32(poll.Id);
        }

        /// <summary>
        /// Save changed survey to database
        /// </summary>
        /// <param name="survey">Survey object that is to be changed in database</param>
        static public void EditSurvey(Survey survey)
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

                foreach (Choice curChoice in curPoll.Choices)
                {
                    int oldId = curChoice.Id;
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

                    if (oldId == curPoll.CorrectChoiceID)
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
        static public void SaveSurveyResult(SurveyResults resultsList)
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
            sqliteCommand.Parameters.Add(new SQLiteParameter(":date", DateTime.Now));
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
        /// Receive from Client poll result and save it in database
        /// </summary>
        /// <param name="resultsList">PollResult</param>
        static public void SavePollResult(PollResult pollResult)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();

            sqliteCommand.CommandText = "INSERT INTO " + RESULTS_TABLE + "(user_name, survey_id, question_id, answer_id, custom_choice) VALUES(:username, :survey, :question, :answer, :custom)";
            sqliteCommand.Parameters.AddWithValue(":username", pollResult.userName);
            sqliteCommand.Parameters.AddWithValue(":survey", -1);
            sqliteCommand.Parameters.AddWithValue(":question", pollResult.questionId);
            sqliteCommand.Parameters.AddWithValue(":answer", pollResult.answerId);
            sqliteCommand.Parameters.AddWithValue(":custom", pollResult.customChoice);
            sqliteCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Select from DB all results of needed Survey
        /// </summary>
        /// <param name="surveyId">Id of Survey which results we need</param>
        /// <returns>List of results of needed Survey</returns>
        static public SurveyResults GetSurveyResults(int surveyId)
        {
            if (!isConnected)
            {
                Init();
            }

            SurveyResults resultsList = new SurveyResults();
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
                curResult.date = sqliteReader["date"].ToString();
                curResult.questionId = Convert.ToInt32(sqliteReader["question_id"].ToString());
                curResult.userName = sqliteReader["user_name"].ToString();
                resultsList.results.Add(curResult);
            }

            return resultsList;
        }

        /// <summary>
        /// Select from DB all results of needed Poll
        /// </summary>
        /// <param name="surveyId">Id of Poll which results we need</param>
        /// <returns>List of results of needed Poll</returns>
        static public List<PollResult> GetPollResults(int pollId)
        {
            if (!isConnected)
            {
                Init();
            }

            List<PollResult> pollResults = new List<PollResult>();
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.AddWithValue(":question_id", pollId);
            sqliteCommand.Parameters.AddWithValue(":survey_id", -1);
            sqliteCommand.CommandText = "SELECT * FROM " + RESULTS_TABLE + " WHERE question_id=:question_id AND survey_id=:survey_id";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            // Save each result to resultsList
            while (sqliteReader.Read())
            {
                PollResult curResult = new PollResult();
                curResult.answerId = Convert.ToInt32(sqliteReader["answer_id"].ToString());
                curResult.customChoice = sqliteReader["custom_choice"].ToString();
                curResult.questionId = Convert.ToInt32(sqliteReader["question_id"].ToString());
                curResult.userName = sqliteReader["user_name"].ToString();
                pollResults.Add(curResult);
            }

            return pollResults;
        }

        static public SurveyResults GetSurveyResults(int surveyId, String user, DateTime date)
        {
            if (!isConnected)
            {
                Init();
            }

            SurveyResults resultsList = new SurveyResults();
            resultsList.surveyId = surveyId;
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":survey_id", surveyId));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":user_name", user));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":date", date));
            sqliteCommand.CommandText = "SELECT * FROM " + RESULTS_TABLE + " WHERE ((survey_id=:survey_id) AND (user_name=:user_name) AND (date=:date))";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            // Save each result to resultsList
            while (sqliteReader.Read())
            {
                PollResult curResult = new PollResult();
                curResult.answerId = Convert.ToInt32(sqliteReader["answer_id"].ToString());
                curResult.customChoice = sqliteReader["custom_choice"].ToString();
                curResult.date = sqliteReader["date"].ToString();
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
        static public void RemoveSurvey(int surveyID)
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

        static public User RegisterUser(User user)
        {
            if (!isConnected)
            {
                Init();
            }

            try
            {
                SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
                sqliteCommand.Parameters.Add( new SQLiteParameter( ":userName", user.username ) );
                sqliteCommand.Parameters.Add( new SQLiteParameter( ":password", user.password ) );
                sqliteCommand.CommandText = "INSERT INTO " + USERS_TABLE + "(userName, password) VALUES (:userName, :password)";
                SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
                user.action = User.AUTH;
            }
            catch (Exception)
            {
                user.action = User.EXIST;
            }

            return user;
        }

        static public User ExistUser(User user)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add( new SQLiteParameter( ":userName", user.username ) );
            sqliteCommand.CommandText = "SELECT * FROM " + USERS_TABLE + " WHERE userName=:userName";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            // if user already exist
            if ( sqliteReader.HasRows )
            {
                user.action = User.EXIST;                
            }
            else
            {
                user.action = User.NEW_USER;
            }

            return user;
        }

        static public User AuthorizeUser(User user)
        {
            if (!isConnected)
            {
                Init();
            }

            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add( new SQLiteParameter( ":userName", user.username ) );
            sqliteCommand.CommandText = "SELECT * FROM " + USERS_TABLE + " WHERE userName=:userName";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

            // if user already exist
            if ( sqliteReader.HasRows )
            {
                if ( sqliteReader["password"].ToString() == user.password )
                {
                    user.action = User.AUTH;
                }
                else
                {
                    user.action = User.EXIST;
                }
            }
            else
            {
                user.action = User.EXIST;
            }
            return user;
        }

        static public List<DateTime> GetDatesOfAttempts(String userName, int surveyID)
        {
            List<DateTime> datesOfAttempts = new List<DateTime>();
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":userName", userName));
            sqliteCommand.Parameters.Add(new SQLiteParameter(":surveyID", surveyID));
            sqliteCommand.CommandText = "SELECT * FROM " + RESULTS_TABLE + " WHERE (survey_id=:surveyID) and (user_name=:userName) GROUP BY date";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
            while (sqliteReader.Read())
            {
                datesOfAttempts.Add((DateTime)sqliteReader["date"]);
            }
            return datesOfAttempts;
        }

        static public bool HasResults(int surveyID)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":surveyID", surveyID));
            sqliteCommand.CommandText = "SELECT * FROM " + RESULTS_TABLE + " WHERE survey_id=:surveyID GROUP BY date";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
            return sqliteReader.HasRows;
        }

        static public bool HasResults(string userName)
        {
            SQLiteCommand sqliteCommand = dbConnection.CreateCommand();
            sqliteCommand.Parameters.Add(new SQLiteParameter(":userName", userName));
            sqliteCommand.CommandText = "SELECT * FROM " + RESULTS_TABLE + " WHERE user_name=:userName GROUP BY date";
            SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();
            return sqliteReader.HasRows;
        }
    }
}
