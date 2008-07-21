using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common;

/*
 Developed by SharpTeam: vlad & ksi
*/

namespace Ilsrep.PollApplication.PollClient
{
    public class PollClient
    {
        private const string POLL_SESSION_ELEMENT = "pollsession";
        private const string POLL_ELEMENT = "poll";
        private const string CONSOLE_YES = "y";
        private const string CONSOLE_NO = "n";
        private const string HOST = "localhost";
        private const int PORT = 3320;
        static string userName = "";
        static TcpServer server;
        static PollSession pollSession = new PollSession();
        static List<Choice> userChoices = new List<Choice>();

        public static void RunUserDialog()
        {
            // Read user name
            Console.WriteLine("Welcome to polls client program.");
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
                    Console.WriteLine("You didn't enter your name.");
                }
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
                Console.WriteLine("Wrong choice, please, choose [y/n]:");
            }
        }

        public static void RunUserPoll()
        {
            foreach (Poll curentPoll in pollSession.polls)
            {
                // Clear screen and write this poll's question
                Console.Clear();
                Console.WriteLine(curentPoll.name + "\n" + curentPoll.description);
                
                // List choices for this poll
                int index = 1;
                foreach (Choice currentChoice in curentPoll.choices)
                {
                    Console.WriteLine("\t" + index + ". " + currentChoice.choice);
                    ++ index;
                }

                // add option for customer choice
                if ( curentPoll.customChoice )
                {
                    Console.WriteLine("\t" + index + ". Custom Choice");
                }

                // accept only correct choices
                while ( true )
                {
                    int choiceCount = (curentPoll.customChoice ? curentPoll.choices.Count + 1 : curentPoll.choices.Count);
                    Console.Write("Pick your choice: [1-" + choiceCount + "]:");

                    // Get user choice
                    try
                    {
                        index = Convert.ToInt32(Console.ReadLine());
                        --index;

                        // check if input correct
                        bool choiceInputIsAcceptable = (index >= 0 && index <= curentPoll.choices.Count - (curentPoll.customChoice ? 0 : 1));
                        if (choiceInputIsAcceptable)
                            break;
                    }
                    catch( Exception )
                    {
                        
                    }
                    Console.WriteLine("Invalid choice!");
                }

                // check if custom choice
                bool ifCustomChoice = (index == curentPoll.choices.Count);
                if (ifCustomChoice)
                {
                    Console.Write("Enter your choice:" );

                    // create custom choice
                    Choice userChoice = new Choice();
                    userChoice.choice = Console.ReadLine();
                    userChoice.id = 0;
                    userChoice.parent = curentPoll;

                    // add custom choice to list
                    userChoices.Add(userChoice);
                }
                else
                {
                    // add one of the choices that already exist to list
                    userChoices.Add(curentPoll.choices[index]);
                }
            }

            // go through choices and display them
            Console.Clear();
            Console.WriteLine(userName + ", here is your PollSession results:");
            bool isTestMode = pollSession.testMode;
            foreach(Choice userChoice in userChoices)
            {
                Console.WriteLine(userChoice.parent.id + ". " + userChoice.parent.name + ": " + userChoice.choice);
                if (isTestMode)
                {
                    // Get correctChoice by id
                    string correctChoice = "";
                    foreach (Choice curChoice in userChoice.parent.choices)
                    {
                        if (curChoice.id == userChoice.parent.correctChoiceId)
                            correctChoice = curChoice.choice;
                    }

                    string isChoicePassed = (userChoice.parent.correctChoiceId == userChoice.id ? "+ Correct" : "- Wrong");
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
                    if (userChoice.id == userChoice.parent.correctChoiceId)
                        correctAnswersCount++;
                }

                // Check if is passed
                double userScore = Convert.ToDouble(correctAnswersCount) / Convert.ToDouble(userChoices.Count);
                bool isPassed = (userScore >= pollSession.minScore);
                if (isPassed)
                {
                    Console.WriteLine("PASSED");
                }
                else
                {
                    Console.WriteLine("Sorry, try again...");
                }
                Console.WriteLine("Your scores: " + userScore);
                Console.WriteLine("Minimal needed scores: " + pollSession.minScore);
            }
        }

        /// <summary>
        /// Connect to Poll Server to begin transaction. If connection fails quit application as it is essential.
        /// </summary>
        public static void ConnectToServer()
        {
            try
            {
                // connect to server
                Console.WriteLine("Please wait. Connecting to poll server...");
                server = new TcpServer();
                server.Connect(HOST, PORT);
                
                // if all ok inform
                if (server.isConnected)
                {
                    Console.WriteLine("Connection established.");
                }
                else
                    throw new Exception("Not connected");
            }
            catch (Exception)
            {
                // if any error occurs - exit
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                Environment.Exit(-1);
            }
        }

        public static void DisconnectFromServer()
        {
            server.Disconnect();
            Console.WriteLine("Disconnected from server");
        }

        /// <summary>
        /// Server sends list of available poll sessions. User picks one and client gets that poll session from server.
        /// </summary>
        public static void GetPollSession()
        {
            PollPacket sendPacket = new PollPacket();
            sendPacket.request.type = Request.GET_LIST;
            string sendString = PollSerializator.SerializePacket(sendPacket);
            server.Send(sendString);

            string receivedString = server.Receive();
            PollPacket receivedPacket = new PollPacket();
            receivedPacket = PollSerializator.DeSerializePacket(receivedString);

            //Check if list is not empty
            if (receivedPacket.pollSessionList.items.Count == 0)
            {
                Console.WriteLine("Sorry, but data base is is empty, no pollsessions...");
                server.Disconnect();
                Console.WriteLine("Disconnected from server");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                Environment.Exit(-1);
            }

            // Output list of poll sessions
            Console.WriteLine("Please select poll session:");
            int index = 0;
            foreach (Item curItem in receivedPacket.pollSessionList.items)
            {
                index++;
                Console.WriteLine(index + ". " + curItem.name);
            }
            
            while (true)
            {
                // Let used input poll session id
                string userAnswer = Console.ReadLine();

                try
                {
                    index = Convert.ToInt32(userAnswer);

                    if (index > 0 && index <= receivedPacket.pollSessionList.items.Count)
                    {
                        string pollSessionID = receivedPacket.pollSessionList.items[index - 1].id;
                        sendPacket.request.type = Request.GET_POLLSESSION;
                        sendPacket.request.id = pollSessionID;
                        sendString = PollSerializator.SerializePacket(sendPacket);
                        server.Send(sendString);
                        break;
                    }
                }
                catch (Exception exception)
                {

                }

                // show that wrong id was inputed
                Console.WriteLine("Invalid poll session id! Please, input correct id");
            }

            // receive poll
            receivedString = server.Receive();
            Console.WriteLine("Data received");
            receivedPacket = PollSerializator.DeSerializePacket(receivedString);
            pollSession = receivedPacket.pollSession;
            Console.WriteLine("Data parsed");
        }

        /// <summary>
        /// Send to server results of pollsession
        /// </summary>
        public static void SavePollSessionResults()
        {
            PollPacket sendPacket = new PollPacket();
            sendPacket.request.type = Request.SAVE_RESULT;
            sendPacket.resultsList.userName = userName;
            sendPacket.resultsList.pollsessionId = pollSession.id;

            foreach (Choice userChoice in userChoices)
            {
                PollResult curPollSessionResult = new PollResult();
                curPollSessionResult.questionId = userChoice.parent.id;
                if (userChoice.id == 0)
                {
                    curPollSessionResult.answerId = 0;
                    curPollSessionResult.customChoice = userChoice.choice;
                }
                else
                {
                    curPollSessionResult.answerId = userChoice.id;
                }
                sendPacket.resultsList.results.Add(curPollSessionResult);
            }
            string sendString = PollSerializator.SerializePacket(sendPacket);
            server.Send(sendString);
            Console.WriteLine("Result of pollsession successfully sent to server");
        }

        public static void Main()
        {
            ConnectToServer();
            GetPollSession();
            DisconnectFromServer();

            RunUserDialog();
            RunUserPoll();

            ConnectToServer();
            SavePollSessionResults();
            DisconnectFromServer();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
