using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common;

namespace Ilsrep.PollApplication.PollEditor
{
    /// <summary>
    /// Editor of pollsessions
    /// </summary>
    public class PollEditor
    {
        private static System.Globalization.CultureInfo cultureInfo = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
        private static String userName = String.Empty;
        private static String userPassword = String.Empty;
        private static PollSession pollSession = new PollSession();
        private const String HOST = "localhost";
        private const int PORT = 3320;
        private static TcpClient client;

        /// <summary>
        /// Run conversation with user
        /// </summary>
        public static void RunUserDialog()
        {
            // Get list of pollsessions from server and write they in console
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.GET_LIST;

            PollPacket receivedPacket = new PollPacket();
            receivedPacket = ReceivePollPacket(sendPacket);

            // Check if list is not empty
            if (receivedPacket.pollSessionList.items.Count == 0)
            {
                Console.WriteLine("Sorry, but data base is empty, no pollsessions...");
                if (InputHelper.AskQuestion("Would you create new pollsession[y/n]?", new string[] { "y", "n" }) == "y")
                {
                    EditPollsession(0);
                }
                else
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                return;
            }

            Console.WriteLine( "1. Create new pollsession" );
            Console.WriteLine( "2. Edit pollsession" );
            Console.WriteLine( "3. Remove pollsession" );
            int action = InputHelper.AskQuestion( "Choose action [1-3]:", 1, 3 );

            int pollSessionIndex;
            switch (action)
            {
                case 1:
                    EditPollsession( 0 );
                    break;
                case 2:
                    pollSessionIndex = receivedPacket.pollSessionList.GetPollSession();
                    RemovePollsession( receivedPacket.pollSessionList[pollSessionIndex] );
                    break;
                case 3:
                    pollSessionIndex = receivedPacket.pollSessionList.GetPollSession();
                    EditPollsession( Convert.ToInt32( receivedPacket.pollSessionList[pollSessionIndex].id ) );
                    break;
                default:
                    Console.WriteLine("Invalid action!");
                    break;
            }
        }

        /// <summary>
        /// Remove pollsession
        /// </summary>
        public static void RemovePollsession( Item item )
        {
            // Ask if user is sure
            if ( InputHelper.AskQuestion( "Do you really want to remove pollsession \"" + item.name + "\" [y/n]?", new string[] { "y", "n" } ) == "n" )
            {
                return;
            }

            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.REMOVE_POLLSESSION;
            sendPacket.request.id = item.id.ToString();
            string sendString = PollSerializator.SerializePacket(sendPacket);
            client.Send(sendString);
        }

        /// <summary>
        /// Adds or edits poll session and saves it to server
        /// </summary>
        /// <param name="pollSessionID">id of session to edit</param>
        public static void EditPollsession( int pollSessionID )
        {
            PollSession pollSession;

            if ( pollSessionID == 0 )
            {
                pollSession = new PollSession();
            }
            else
            {
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.type = Request.GET_POLLSESSION;
                pollPacket.request.id = pollSessionID.ToString();
                pollPacket = ReceivePollPacket( pollPacket );
                pollSession = pollPacket.pollSession;
            }

            pollSession.name = InputHelper.AskQuestion("Enter pollsession name:", pollSession.name, null);
            pollSession.testMode = InputHelper.ToBoolean(InputHelper.AskQuestion("Test mode[y/n]?", InputHelper.ToString(pollSession.testMode), new string[] { "y", "n" }));

            if (pollSession.testMode == true)
            {
                while (true)
                {
                    try
                    {
                        pollSession.minScore = Convert.ToDouble(InputHelper.AskQuestion("Min score to pass test:", pollSession.minScore.ToString(), null), cultureInfo);
                        if (pollSession.minScore > 1 || pollSession.minScore < 0)
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
                Console.WriteLine("Poll session contains {0} polls.", pollSession.polls.Count);
                
                if (pollSession.polls.Count > 0)
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
                            EditPoll( ref pollSession, -1 );
                        break;
                        case 2:
                            index = 1;
                            Console.WriteLine("List of polls:");
                            foreach (Poll poll in pollSession.polls)
                            {
                                Console.WriteLine("{0}. {1}", index, poll.name);
                            }

                            userChoice = InputHelper.AskQuestion(String.Format("Choose which one to edit [1-{0}]:", index), 1, index);
                            
                            EditPoll( ref pollSession, userChoice-1 );
                        break;
                        case 3:
                            index = 1;
                            Console.WriteLine("List of polls:");
                            foreach (Poll poll in pollSession.polls)
                            {
                                Console.WriteLine("{0}. {1}", index, poll.name);
                            }

                            pollSession.polls.RemoveAt(InputHelper.AskQuestion(String.Format("Choose which one to delete [1-{0}]:", index), 1, index));
                        break;
                    }
                }
                else
                {
                    EditPoll( ref pollSession, -1 );
                }
            }
        }

        /// <summary>
        /// Adds or edits a poll
        /// </summary>
        /// <param name="editedPoll">Poll which is edited</param>
        /// <returns>returns added or edited poll</returns>
        public static void EditPoll( ref PollSession pollSession, int index )
        {
            Poll newPoll;

            if ( index == -1 )
            {
                pollSession.polls.Add( new Poll() );
                index = pollSession.polls.Count - 1;

                newPoll = pollSession.polls[index];

                newPoll.id = 0;
                newPoll.correctChoiceID = 0;
            }
            else
            {
                newPoll = pollSession.polls[index];
            }


            newPoll.name = InputHelper.AskQuestion( "Poll name:", newPoll.name, null );
            newPoll.description = InputHelper.AskQuestion( "Poll description:", newPoll.description, null );

            while ( true )
            {
                int choiceIndex;
                Console.WriteLine( "Poll contains {0} choices.", newPoll.choices.Count );

                if ( newPoll.choices.Count > 0 )
                {
                    Console.WriteLine( "Actions:" );
                    Console.WriteLine( "1. Add new choice" );
                    Console.WriteLine( "2. Edit choice" );
                    Console.WriteLine( "3. Remove choice" );
                    Console.WriteLine( "4. End" );
                    int userChoice = InputHelper.AskQuestion( "Select action [1-4]:", 1, 4 );

                    if ( userChoice == 4 )
                        break;

                    switch ( userChoice )
                    {
                        case 1:
                            choiceIndex = newPoll.choices.Count + 1;
                            newPoll.choices.Add( new Choice() );
                            newPoll.choices[choiceIndex].id = 0;
                            newPoll.choices[choiceIndex].choice = InputHelper.AskQuestion( "Choice name:", null );
                            newPoll.choices[choiceIndex].parent = newPoll;

                            Console.WriteLine( "Choice added!" );
                            break;
                        case 2:
                            choiceIndex = newPoll.GetChoice();
                            newPoll.choices[choiceIndex].choice = InputHelper.AskQuestion( "Choice name:", newPoll.choices[choiceIndex].choice, null );

                            Console.WriteLine( "Choice edited!" );
                            break;
                        case 3:
                            choiceIndex = newPoll.GetChoice();
                            newPoll.choices.RemoveAt( choiceIndex );

                            Console.WriteLine( "Choice deleted!" );
                            break;
                    }
                }
                else
                {
                    if ( newPoll.choices.Count >= (pollSession.testMode ? 2 : 0) )
                    {
                        bool addNewChoice = InputHelper.ToBoolean( InputHelper.AskQuestion( "Add new choice[y/n]?", new string[] { "y", "n" } ) );
                        if ( addNewChoice == false )
                            break;
                    }

                    choiceIndex = newPoll.choices.Count + 1;
                    newPoll.choices.Add( new Choice() );
                    newPoll.choices[choiceIndex].id = 0;
                    newPoll.choices[choiceIndex].choice = InputHelper.AskQuestion( "Choice name:", null );
                    newPoll.choices[choiceIndex].parent = newPoll;

                    Console.WriteLine( "Choice added!" );
                }
            }

            if ( pollSession.testMode == false )
            {
                // Enable CustomChoice automaticly if user doesn't inputed any choice
                if ( newPoll.choices.Count == 0 )
                {
                    newPoll.customChoiceEnabled = true;
                }
                else
                {
                    newPoll.customChoiceEnabled = InputHelper.ToBoolean( InputHelper.AskQuestion( "Enable custom choice for this poll[y/n]?", InputHelper.ToString( newPoll.customChoiceEnabled ), new string[] { "y", "n" } ) );
                }
            }
            else
            {
                // Ask correct choice
                Console.WriteLine("Which one is correct?");
                foreach ( Choice choice in newPoll.choices )
                    Console.WriteLine( "\t" + choice.id + ". " + choice.choice );
                newPoll.correctChoiceID = newPoll.GetChoice();
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
                    if ( InputHelper.AskQuestion( "Would you like to retry[y/n]?", new string[] { "y", "n" } ) == "n" )
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

        /// <summary>
        /// Authorize user
        /// </summary>
        public static void AuthorizeUser()
        {
            // Read user name
            Console.WriteLine("Welcome to polls editor program.");
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
            while (true)
            {
                pollPacket = ReceivePollPacket(pollPacket);
                if (pollPacket.user.auth)
                    break;
                if (pollPacket.user.exist)
                {
                    Console.WriteLine("{0}, please, enter your password:", userName);
                    userPassword = Console.ReadLine();
                    pollPacket.user.password = userPassword;
                }
                else
                {
                    Console.WriteLine("{0}, please, set your password:", userName);
                    userPassword = Console.ReadLine();
                    pollPacket.user.password = userPassword;
                    pollPacket.user.isNew = true;
                }
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
                        if ( userInput == "y" )
                        {
                            client.Disconnect();
                            ConnectToServer();
                            break;
                        }
                        else if ( userInput == "n" )
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
                if ( InputHelper.AskQuestion( "Do you want to execute another action[y/n]?", new string[] { "y", "n" } ) == "n" )                
                    break;
                ConnectToServer();
                PollPacket pollPacket = new PollPacket();
                pollPacket.user = new User();
                pollPacket.user.username = userName;
                pollPacket.user.password = userPassword;
                pollPacket.user.auth = true;
                pollPacket = ReceivePollPacket(pollPacket);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
