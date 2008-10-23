using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common;

/*
 Developed by SharpTeam: vlad & ksi
*/

namespace Ilsrep.PollApplication.PollClient
{
    /// <summary>
    /// Poll Client that interacts with user
    /// </summary>
    public class PollClient
    {
        /// <summary>
        /// String that states yes
        /// </summary>
        private const string CONSOLE_YES = "y";
        /// <summary>
        /// String that states no
        /// </summary>
        private const string CONSOLE_NO = "n";
        /// <summary>
        /// host to which connect
        /// </summary>
        private const string HOST = "localhost";
        /// <summary>
        /// port to which connect
        /// </summary>
        private const int PORT = 3320;
        /// <summary>
        /// holds current user's username
        /// </summary>
        static string userName = "";
        /// <summary>
        /// handles connection to server
        /// </summary>
        static TcpClient client;
        /// <summary>
        /// holds pollsession that is read from server
        /// </summary>
        static PollSession pollSession = new PollSession();
        /// <summary>
        /// holds user choices that he selects during poll session
        /// </summary>
        static List<Choice> userChoices = new List<Choice>();

        /// <summary>
        /// Helper method to get answers on questions, with possibility of choosing what user can answer
        /// Empty answer is not allowed
        /// </summary>
        /// <param name="question">Question to be asked</param>
        /// <param name="allowedAnswers">Allowed answers that user can input. Set to null if any</param>
        /// <returns>Answer from user</returns>
        public static string AskQuestion(String question, String[] allowedAnswers)
        {
            String inputLine = String.Empty;

            while (true)
            {
                Console.Write(question);
                inputLine = Console.ReadLine();

                if (inputLine != String.Empty)
                {
                    if (allowedAnswers == null || allowedAnswers.Length == 0)
                        return inputLine;

                    foreach (String allowedAnswer in allowedAnswers)
                        if (inputLine == allowedAnswer)
                            return inputLine;
                }

                Console.WriteLine("Wrong input!");
            }
        }

        /// <summary>
        /// Interaction with user. Gets answers to pollsession
        /// </summary>
        public static void RunUserPoll()
        {
            foreach (Poll curentPoll in pollSession.Polls)
            {
                // Clear screen and write this poll's question
                Console.Clear();
                Console.WriteLine(curentPoll.Name + "\n" + curentPoll.Description);
                
                // List choices for this poll
                int index = 1;
                foreach (Choice currentChoice in curentPoll.Choices)
                {
                    Console.WriteLine("\t" + index + ". " + currentChoice.choice);
                    ++ index;
                }

                // add option for customer choice
                if ( curentPoll.CustomChoiceEnabled )
                {
                    Console.WriteLine("\t" + index + ". Custom Choice");
                }

                // accept only correct choices
                while ( true )
                {
                    int choiceCount = (curentPoll.CustomChoiceEnabled ? curentPoll.Choices.Count + 1 : curentPoll.Choices.Count);
                    Console.Write("Pick your choice: [1-" + choiceCount + "]:");

                    // Get user choice
                    try
                    {
                        index = Convert.ToInt32(Console.ReadLine());
                        --index;

                        // check if input correct
                        bool choiceInputIsAcceptable = (index >= 0 && index <= curentPoll.Choices.Count - (curentPoll.CustomChoiceEnabled ? 0 : 1));
                        if (choiceInputIsAcceptable)
                            break;
                    }
                    catch( Exception )
                    {
                        
                    }
                    Console.WriteLine("! Invalid choice");
                }

                // check if custom choice
                bool isCustomChoice = (index == curentPoll.Choices.Count);
                if (isCustomChoice)
                {
                    Console.Write("Enter your choice:" );

                    string userInput = String.Empty;
                    while (true)
                    {
                        userInput = Console.ReadLine();
                        if (userInput != String.Empty)
                            break;
                        Console.WriteLine("! Custom choice can't be empty");
                    }

                    // create custom choice
                    Choice userChoice = new Choice();
                    userChoice.choice = userInput;
                    userChoice.Id = 0;
                    userChoice.parent = curentPoll;

                    // add custom choice to list
                    userChoices.Add(userChoice);
                }
                else
                {
                    // add one of the choices that already exist to list
                    userChoices.Add(curentPoll.Choices[index]);
                }
            }

            // go through choices and display them
            Console.Clear();
            Console.WriteLine(userName + ", here is your PollSession results:");
            bool isTestMode = pollSession.TestMode;

            int index1 = 0;
            foreach(Choice userChoice in userChoices)
            {
                index1++;
                Console.WriteLine(index1 + ". " + userChoice.parent.Name + ": " + userChoice.choice);
                if (isTestMode)
                {
                    // Get correctChoice by id
                    string correctChoice = String.Empty;
                    foreach (Choice curChoice in userChoice.parent.Choices)
                    {
                        if (curChoice.Id == userChoice.parent.CorrectChoiceID)
                            correctChoice = curChoice.choice;
                    }

                    string isChoicePassed = (userChoice.parent.CorrectChoiceID == userChoice.Id ? "+ Correct" : "- Wrong");
                    Console.WriteLine("Correct choice: " + correctChoice);
                    Console.WriteLine(isChoicePassed + "\n");
                }
            }

            if (isTestMode)
            {
                // Get correct answers count
                int correctAnswersCount = 0;
                foreach (Choice userChoice in userChoices)
                {
                    if (userChoice.Id == userChoice.parent.CorrectChoiceID)
                        correctAnswersCount++;
                }

                // Check if is passed
                double userScore = Convert.ToDouble(correctAnswersCount) / Convert.ToDouble(userChoices.Count);
                bool isPassed = (userScore >= pollSession.MinScore);
                if (isPassed)
                {
                    Console.WriteLine("PASSED");
                }
                else
                {
                    Console.WriteLine("Sorry, try again...");
                }
                Console.WriteLine("Your scores: " + userScore);
                Console.WriteLine("Minimal needed scores: " + pollSession.MinScore);
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
                    if (client.isConnected)
                    {
                        Console.WriteLine("+ Connection established");
                    }
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    if (AskQuestion("Would you like to retry[y/n]?", new String[] { "y", "n" }) == "n")
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
        public static void DisconnectFromServer()
        {
            client.Disconnect();
            Console.WriteLine("- Disconnected from server");
        }

        /// <summary>
        /// Function sends request, receive PollPacket and check if receivedPacket == null. If true, user can retry to receive Packet, else function returns receivedPacket
        /// </summary>
        /// <param name="sendPacket">PollPacket with request to send</param>
        /// <returns>PollPacket receivedPacket</returns>
        public static PollPacket ReceivePollPacket(PollPacket sendPacket)
        {
            while (true)
            {
                try
                {
                    string sendString = PollSerializator.SerializePacket(sendPacket);
                    client.Send(sendString);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("! " + exception.Message);
                }

                string receivedString = client.Receive();
                PollPacket receivedPacket = new PollPacket();
                receivedPacket = PollSerializator.DeserializePacket(receivedString);

                // Check if received data is correct
                if (receivedPacket == null)
                {
                    Console.WriteLine("! Wrong data received");
                    Console.WriteLine("Would you like to retry?[y/n]:");
                    while (true)
                    {
                        string userInput;
                        userInput = Console.ReadLine();
                        if (userInput == "y")
                        {
                            DisconnectFromServer();
                            ConnectToServer();
                            break;
                        }
                        else if (userInput == "n")
                        {
                            Environment.Exit(-1);
                        }
                        else
                        {
                            Console.WriteLine("! Invalid choice");
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
        /// Server sends list of available poll sessions. User picks one and client gets that poll session from server.
        /// </summary>
        public static void GetPollSession()
        {
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.GET_LIST;

            PollPacket receivedPacket = new PollPacket();
            receivedPacket = ReceivePollPacket(sendPacket);

            // Check if list is not empty
            if (receivedPacket.pollSessionList.items.Count == 0)
            {
                Console.WriteLine("Sorry, but data base is empty, no pollsessions...");
                client.Disconnect();
                Console.WriteLine("- Disconnected from server");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                Environment.Exit(-1);
            }

            // Output list of poll sessions
            Console.WriteLine("List of pollsessions:");
            int index = 0;
            foreach (Item curItem in receivedPacket.pollSessionList.items)
            {
                index++;
                Console.WriteLine(index + ". " + curItem.name);
            }
            
            while (true)
            {
                Console.WriteLine("Please select poll session[1-{0}]:", receivedPacket.pollSessionList.items.Count);
                // Let user input poll session id
                string userAnswer = Console.ReadLine();

                try
                {
                    index = Convert.ToInt32(userAnswer);

                    if (index > 0 && index <= receivedPacket.pollSessionList.items.Count)
                    {
                        string pollSessionID = receivedPacket.pollSessionList.items[index - 1].id;
                        sendPacket.request.type = Request.GET_POLLSESSION;
                        sendPacket.request.id = pollSessionID;
                        break;
                    }
                    else
                    {
                        throw new Exception("Invalid poll session id! Please, input correct id");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("! " + exception.Message);
                }                
            }

            // Receive poll
            receivedPacket = ReceivePollPacket(sendPacket);
            pollSession = receivedPacket.pollSession;
            Console.WriteLine("<- PollSession received");
        }

        /// <summary>
        /// Send results of pollsession to server
        /// </summary>
        public static void SavePollSessionResults()
        {
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.SAVE_RESULT;
            sendPacket.resultsList = new ResultsList();
            sendPacket.resultsList.userName = userName;
            sendPacket.resultsList.pollsessionId = pollSession.Id;

            foreach (Choice userChoice in userChoices)
            {
                PollResult curPollSessionResult = new PollResult();
                curPollSessionResult.questionId = userChoice.parent.Id;
                if (userChoice.Id == 0)
                {
                    curPollSessionResult.answerId = 0;
                    curPollSessionResult.customChoice = userChoice.choice;
                }
                else
                {
                    curPollSessionResult.answerId = userChoice.Id;
                }
                sendPacket.resultsList.results.Add(curPollSessionResult);
            }
            string sendString = PollSerializator.SerializePacket(sendPacket);
            client.Send(sendString);
            Console.WriteLine("-> Result of pollsession successfully sent to server");
        }

        public static void AuthorizeUser()
        {
           
            Console.WriteLine("Welcome to polls client program.");
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
                    pollPacket.user.password = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("{0}, please, enter your password:", userName);
                    pollPacket.user.password = Console.ReadLine();
                    pollPacket.user.action = User.LOGIN;
                }

                pollPacket = ReceivePollPacket(pollPacket);
                if (pollPacket.user.action == User.ACCEPTED)
                    break;
                if (pollPacket.user.action == User.DENIED)
                    Console.WriteLine("Invalid password!");
            }

            Console.Clear();
            Console.WriteLine("Glad to meet you, " + userName);
            Console.WriteLine(userName + ", let's start the poll session?[y/n]");

            // Ask user if he want to start the poll session
            while (true)
            {
                string userInput = Console.ReadLine();
                if (userInput == CONSOLE_YES) break;
                if (userInput == CONSOLE_NO) Environment.Exit(0);
                Console.WriteLine("! Wrong choice, please, choose [y/n]:");
            }
        }

        /// <summary>
        /// Main method of Poll Client
        /// </summary>
        public static void Main()
        {
            ConnectToServer();
            AuthorizeUser();

            GetPollSession();
            RunUserPoll();

            SavePollSessionResults();
            DisconnectFromServer();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
