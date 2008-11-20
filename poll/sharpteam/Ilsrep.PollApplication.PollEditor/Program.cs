using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common;

namespace Ilsrep.PollApplication.PollEditor
{
    /// <summary>
    /// Editor of surveys
    /// </summary>
    public class PollEditor
    {
        private static System.Globalization.CultureInfo cultureInfo = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
        private static string userName = String.Empty;
        private static string userPassword = String.Empty;
        private static Survey survey = new Survey();
        private const string HOST = "localhost";
        private const int PORT = 3320;
        private static TcpClient client;
        private const string STRING_YES = "y";
        private const string STRING_NO = "n";

        /// <summary>
        /// Run conversation with user
        /// </summary>
        public static void RunUserDialog()
        {
            // Get list of surveys from server and write they in console
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.GET_LIST;

            PollPacket receivedPacket = new PollPacket();
            receivedPacket = ReceivePollPacket(sendPacket);

            // Check if list is not empty
            if (receivedPacket.surveyList.items.Count == 0)
            {
                Console.WriteLine("Sorry, but data base is empty, no surveys...");
                if (InputHelper.AskQuestion(String.Format("Would you create new survey[{0}/{1}]?", STRING_YES, STRING_NO), new string[] { STRING_YES, STRING_NO }) == STRING_YES)
                {
                    EditSurvey(0);
                }
                else
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                return;
            }

            Console.WriteLine( "1. Create new survey" );
            Console.WriteLine( "2. Edit survey" );
            Console.WriteLine( "3. Remove survey" );
            int action = InputHelper.AskQuestion( "Choose action [1-3]:", 1, 3 );

            int surveyIndex;
            switch (action)
            {
                case 1:
                    EditSurvey( 0 );
                    break;
                case 2:
                    surveyIndex = receivedPacket.surveyList.GetSurveyId();
                    EditSurvey( Convert.ToInt32( receivedPacket.surveyList[surveyIndex-1].id ) );                    
                    break;
                case 3:
                    surveyIndex = receivedPacket.surveyList.GetSurveyId();
                    RemoveSurvey( receivedPacket.surveyList[surveyIndex-1] );                    
                    break;
                default:
                    Console.WriteLine("Invalid action!");
                    break;
            }
        }

        /// <summary>
        /// Remove survey
        /// </summary>
        public static void RemoveSurvey( Item item )
        {
            // Ask if user is sure
            if ( InputHelper.AskQuestion( "Do you really want to remove survey \"" + item.name + "\" [y/n]?", new string[] { STRING_YES, STRING_NO } ) == "n" )
            {
                return;
            }

            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.REMOVE_SURVEY;
            sendPacket.request.id = item.id.ToString();
            string sendString = PollSerializator.SerializePacket(sendPacket);
            client.Send(sendString);
        }

        /// <summary>
        /// Adds or edits survey and saves it to server
        /// </summary>
        /// <param name="surveyID">id of session to edit</param>
        public static void EditSurvey( int surveyID )
        {
            Survey survey;

            if ( surveyID == 0 )
            {
                survey = new Survey();
            }
            else
            {
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.type = Request.GET_SURVEY;
                pollPacket.request.id = surveyID.ToString();
                pollPacket = ReceivePollPacket( pollPacket );
                survey = pollPacket.survey;
            }

            survey.Name = InputHelper.AskQuestion("Enter survey name:", survey.Name, null);
            survey.TestMode = InputHelper.ToBoolean(InputHelper.AskQuestion("Test mode[y/n]?", InputHelper.ToString(survey.TestMode), new string[] { STRING_YES, STRING_NO }));

            if (survey.TestMode == true)
            {
                while (true)
                {
                    try
                    {
                        survey.MinScore = Convert.ToDouble(InputHelper.AskQuestion("Min score to pass test:", Convert.ToString(survey.MinScore, cultureInfo), null), cultureInfo);
                        if (survey.MinScore > 1 || survey.MinScore < 0)
                            throw new Exception("min score must be between 0 and 1");
                        break;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Error: {0}", exception.Message);
                        //Console.WriteLine("Please, input correct min score (example: 0.5)");
                    }
                }
            }
            
            while(true)
            {
                Console.WriteLine("Survey contains {0} polls.", survey.Polls.Count);
                
                if (survey.Polls.Count > 0)
                {
                    Console.WriteLine( "Actions:" );
                    Console.WriteLine( "1. Add new poll" );
                    Console.WriteLine( "2. Edit poll" );
                    Console.WriteLine( "3. Remove poll" );
                    Console.WriteLine( "4. End" );
                    int userChoice = InputHelper.AskQuestion("Select action [1-4]:", 1, 4);

                    if ( userChoice == 4 )
                        break;

                    int index;
                    switch (userChoice)
                    {
                        case 1:
                            EditPoll( ref survey, -1 );
                        break;
                        case 2:
                            index = 0;
                            Console.WriteLine("List of polls:");
                            foreach (Poll poll in survey.Polls)
                            {
                                index++;
                                Console.WriteLine("{0}. {1}", index, poll.Name);
                            }

                            userChoice = InputHelper.AskQuestion(String.Format("Choose which one to edit [1-{0}]:", index), 1, index);
                            
                            EditPoll( ref survey, userChoice-1 );
                        break;
                        case 3:
                            index = 0;
                            Console.WriteLine("List of polls:");
                            foreach (Poll poll in survey.Polls)
                            {
                                index++;
                                Console.WriteLine("{0}. {1}", index, poll.Name);
                            }

                            survey.Polls.RemoveAt(InputHelper.AskQuestion(String.Format("Choose which one to delete [1-{0}]:", index), 1, index)-1);
                        break;
                    }
                }
                else
                {
                    EditPoll( ref survey, -1 );
                }
            }

            // Save survey to server
            if (InputHelper.AskQuestion("Do you want to save this survey to server [y/n]?", new string[] { STRING_YES, STRING_NO }) == STRING_YES)
            {
                PollPacket sendPacket = new PollPacket();
                sendPacket.request = new Request();
                if (surveyID == 0)
                {
                    sendPacket.request.type = Request.CREATE_SURVEY;
                }
                else
                {
                    sendPacket.request.type = Request.EDIT_SURVEY;
                }
                sendPacket.request.id = surveyID.ToString();
                sendPacket.survey = survey;
                string sendString = PollSerializator.SerializePacket(sendPacket);
                client.Send(sendString);
            }
        }

        /// <summary>
        /// Adds or edits a poll
        /// </summary>
        /// <param name="survey">Survey conected to poll</param>
        /// <param name="index">index of poll in survey poll list</param>
        public static void EditPoll( ref Survey survey, int index )
        {
            Poll newPoll;

            if ( index == -1 )
            {
                survey.Polls.Add( new Poll() );
                index = survey.Polls.Count - 1;

                newPoll = survey.Polls[index];

                newPoll.Id = 0;
                newPoll.CorrectChoiceID = 0;
            }
            else
            {
                newPoll = survey.Polls[index];
            }


            newPoll.Name = InputHelper.AskQuestion( "Poll name:", newPoll.Name, null );
            newPoll.Description = InputHelper.AskQuestion( "Poll description:", newPoll.Description, null );

            while ( true )
            {
                int choiceIndex;
                Console.WriteLine( "Poll contains {0} choices.", newPoll.Choices.Count );

                if ( newPoll.Choices.Count >= (survey.TestMode ? 2 : 0) )
                {
                    Console.WriteLine( "Actions:" );
                    Console.WriteLine( "1. Add new choice" );
                    Console.WriteLine( "2. Edit choice" );
                    Console.WriteLine( "3. Remove choice" );
                    Console.WriteLine( "4. End" );
                    int userChoice = InputHelper.AskQuestion( "Select action [1-4]:", 1, 4 );

                    if (userChoice == 4)
                        break;

                    switch ( userChoice )
                    {
                        case 1:
                            choiceIndex = newPoll.Choices.Count;
                            newPoll.Choices.Add( new Choice() );
                            newPoll.Choices[choiceIndex].Id = 0;
                            newPoll.Choices[choiceIndex].choice = InputHelper.AskQuestion( "Choice name:", null );
                            newPoll.Choices[choiceIndex].parent = newPoll;

                            Console.WriteLine( "Choice added!" );
                            break;
                        case 2:
                            choiceIndex = newPoll.GetChoiceId();
                            newPoll.Choices[choiceIndex-1].choice = InputHelper.AskQuestion( "Choice name:", newPoll.Choices[choiceIndex-1].choice, null );

                            Console.WriteLine( "Choice edited!" );
                            break;
                        case 3:
                            choiceIndex = newPoll.GetChoiceId();
                            newPoll.Choices.RemoveAt( choiceIndex-1 );

                            Console.WriteLine( "Choice deleted!" );
                            break;
                    }
                }
                else
                {
/*                    bool addNewChoice = InputHelper.ToBoolean( InputHelper.AskQuestion( "Add new choice[y/n]?", new string[] { STRING_YES, STRING_NO } ) );
                    if ( addNewChoice == false )
                        break;*/

                    choiceIndex = newPoll.Choices.Count;
                    newPoll.Choices.Add( new Choice() );
                    newPoll.Choices[choiceIndex].Id = 0;
                    newPoll.Choices[choiceIndex].choice = InputHelper.AskQuestion( "Choice name:", null );
                    newPoll.Choices[choiceIndex].parent = newPoll;

                    Console.WriteLine( "Choice added!" );
                }
            }

            if ( survey.TestMode == false )
            {
                // Enable CustomChoice automaticly if user doesn't inputed any choice
                if ( newPoll.Choices.Count == 0 )
                {
                    newPoll.CustomChoiceEnabled = true;
                }
                else
                {
                    newPoll.CustomChoiceEnabled = InputHelper.ToBoolean( InputHelper.AskQuestion( "Enable custom choice for this poll[y/n]?", InputHelper.ToString( newPoll.CustomChoiceEnabled ), new string[] { STRING_YES, STRING_NO } ) );
                }
            }
            else
            {
                /*
                foreach ( Choice choice in newPoll.choices )
                    Console.WriteLine( "\t" + choice.id + ". " + choice.choice );
                */

                // Ask correct choice
                Console.WriteLine("Which one is correct?");
                newPoll.CorrectChoiceID = newPoll.GetChoiceId();
            }
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        public static void ConnectToServer()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Please wait, connecting to server...");
                    client = new TcpClient();
                    client.Connect(HOST, PORT);
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    if ( InputHelper.AskQuestion( "Would you like to retry[y/n]?", new string[] { STRING_YES, STRING_NO } ) == "n" )
                    {
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                    }
                }
            }
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        private static void DisconnectFromServer()
        {
            client.Disconnect();
        }

        public static void AuthorizeUser()
        {
            Console.WriteLine("Welcome to polls editor program.");
            while (true)
            {
                // Read user name
                while (true)
                {
                    Console.Write("Please enter your name:");
                    userName = Console.ReadLine();
                    if (userName != String.Empty)
                    {
                        break;
                    }
                    else
                    {
                        //Console.Clear();
                        Console.WriteLine("! You didn't enter your name.");
                    }
                }

                PollPacket pollPacket = new PollPacket();
                pollPacket.user = new User();
                pollPacket.user.username = userName;
                pollPacket.user.action = User.LOGIN;
                pollPacket = ReceivePollPacket(pollPacket);

                if (pollPacket.user.action == User.NEW_USER)
                {
                    Console.WriteLine("{0}, please, set your password:", userName);
                    userPassword = Console.ReadLine();
                    pollPacket.user.password = userPassword;
                }
                else
                {
                    Console.WriteLine("{0}, please, enter your password:", userName);
                    userPassword = Console.ReadLine();
                    pollPacket.user.password = userPassword;
                    pollPacket.user.action = User.LOGIN;
                }

                pollPacket = ReceivePollPacket(pollPacket);
                if (pollPacket.user.action == User.ACCEPTED)
                    break;
                if (pollPacket.user.action == User.DENIED)
                    Console.WriteLine("Invalid password!");
            }
        }

        /// <summary>
        /// Function sends request, receive PollPacket and check if receivedPacket == null. If true, user can retry to receive Packet, else function returns receivedPacket
        /// </summary>
        /// <param name="sendPacket">PollPacket with request to send</param>
        /// <returns>PollPacket receivedPacket</returns>
        public static PollPacket ReceivePollPacket( PollPacket sendPacket )
        {
            while ( true )
            {
                try
                {
                    string sendString = PollSerializator.SerializePacket( sendPacket );
                    client.Send( sendString );
                }
                catch ( Exception exception )
                {
                    Console.WriteLine( exception.Message );
                }

                string receivedString = client.Receive();
                PollPacket receivedPacket = new PollPacket();
                receivedPacket = PollSerializator.DeserializePacket( receivedString );

                // Check if received data is correct
                if ( receivedPacket == null )
                {
                    Console.WriteLine( "Wrong data received" );
                    Console.WriteLine( "Would you like to retry?[y/n]:" );
                    while ( true )
                    {
                        string userInput;
                        userInput = Console.ReadLine();
                        if ( userInput == STRING_YES )
                        {
                            client.Disconnect();
                            ConnectToServer();
                            break;
                        }
                        else if ( userInput == STRING_NO )
                        {
                            Environment.Exit( -1 );
                        }
                        else
                        {
                            Console.WriteLine( "Invalid choice" );
                        }
                    }
                }
                else
                {
                    return receivedPacket;
                }
            }
        }

        /// <summary>
        /// Main method
        /// </summary>
        public static void Main()
        {
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";

            ConnectToServer();
            AuthorizeUser();
            while (true)
            {
                RunUserDialog();
                DisconnectFromServer();
                if ( InputHelper.AskQuestion( "Do you want to execute another action[y/n]?", new string[] { STRING_YES, STRING_NO } ) == STRING_NO )                
                    break;
                ConnectToServer();
                PollPacket pollPacket = new PollPacket();
                pollPacket.user = new User();
                pollPacket.user.username = userName;
                pollPacket.user.password = userPassword;
                pollPacket.user.action = User.LOGIN;
                pollPacket = ReceivePollPacket(pollPacket);
                if (pollPacket.user.action == User.DENIED)
                {
                    Console.WriteLine("Sorry, access denied");
                    break;
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
