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
        private static String username = String.Empty;
        private static PollSession pollSession = new PollSession();
        private const String HOST = "localhost";
        private const int PORT = 3320;
        private static TcpServer server;

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
        /// Converts string to boolean
        /// </summary>
        /// <param name="boolString">string containing either "y" or "n"</param>
        /// <returns>true - when string equals to "y", false - otherwise</returns>
        public static bool ToBoolean(String boolString)
        {
            if (boolString == "y")
                return true;
            else
                return false;
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
                    server.Send(sendString);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                string receivedString = server.Receive();
                PollPacket receivedPacket = new PollPacket();
                receivedPacket = PollSerializator.DeserializePacket(receivedString);

                // Check if received data is correct
                if (receivedPacket == null)
                {
                    Console.WriteLine("Wrong data received");
                    Console.WriteLine("Would you like to retry?[y/n]:");
                    while (true)
                    {
                        string userInput;
                        userInput = Console.ReadLine();
                        if (userInput == "y")
                        {
                            server.Disconnect();
                            ConnectToServer();
                            break;
                        }
                        else if (userInput == "n")
                        {
                            Environment.Exit(-1);
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice");
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
        /// Run conversation with user
        /// </summary>
        public static void RunUserDialog()
        {
            // Get list of pollsessions from server and write they in console
            PollPacket sendPacket = new PollPacket();
            sendPacket.request.type = Request.GET_LIST;

            PollPacket receivedPacket = new PollPacket();
            receivedPacket = ReceivePollPacket(sendPacket);

            // Check if list is not empty
            if (receivedPacket.pollSessionList.items.Count == 0)
            {
                Console.WriteLine("Sorry, but data base is is empty, no pollsessions...");
                if (AskQuestion("Would you create new pollsession[y/n]?", new String[] { "y", "n" }) == "y")
                {
                    CreatePollsession();
                }
                else
                {
                    Environment.Exit(0);
                }
                return;
            }

            // Output list of poll sessions
            Console.WriteLine("Here is pollsessions list:");
            int index = 0;
            foreach (Item curItem in receivedPacket.pollSessionList.items)
            {
                index++;
                Console.WriteLine(index + ". " + curItem.name);
            }

            Console.WriteLine("Please, select action:");
            Console.WriteLine("1. Create new pollsession");
            Console.WriteLine("2. Remove pollsession");
            Console.WriteLine("3. Edit pollsession - NOT AVAILABLE");

            // Read action
            int action = 0;
            while (true)
            {
                try
                {
                    action = Convert.ToInt32(Console.ReadLine());
                    if (action < 1 || action > 3)
                        throw new Exception("Invalid choice");
                    break;
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }

            switch (action)
            {
                case 1:
                    CreatePollsession();
                    break;
                case 2:
                    RemovePollsession(receivedPacket);
                    break;
                case 3:
                    Console.WriteLine("IN FUTURE HERE WILL BE EDITING POLLSESSION");
                    break;
                default:
                    Console.WriteLine("Invalid action!");
                    break;
            }
        }

        /// <summary>
        /// Remove pollsession
        /// </summary>
        public static void RemovePollsession(PollPacket receivedPacket)
        {
            // Let user input poll session id
            while (true)
            {
                Console.Write("Please, input id of pollsession that you want to remove:");
                string userAnswer = Console.ReadLine();

                try
                {
                    int index = Convert.ToInt32(userAnswer);

                    if (index > 0 && index <= receivedPacket.pollSessionList.items.Count)
                    {
                        string pollSessionID = receivedPacket.pollSessionList.items[index - 1].id;
                        Console.Clear();

                        // Ask if user is sure
                        if (AskQuestion("Do you really want to remove pollsession \"" + receivedPacket.pollSessionList.items[index - 1].name + "\" [y/n]?", new String[] {"y", "n"}) == "n")
                        {
                            break;
                        }

                        PollPacket sendPacket = new PollPacket();
                        sendPacket.request.type = Request.REMOVE_POLLSESSION;
                        sendPacket.request.id = pollSessionID;
                        string sendString = PollSerializator.SerializePacket(sendPacket);
                        server.Send(sendString);
                        break;
                    }
                    else
                    {
                        throw new Exception("Invalid poll session id!");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        /// <summary>
        /// Create new pollsession and save it to server
        /// </summary>
        public static void CreatePollsession()
        {
            pollSession.name = AskQuestion("Enter pollsession name:", null);
            pollSession.testMode = ToBoolean(AskQuestion("Test mode[y/n]?", new String[] { "y", "n" }));

            if (pollSession.testMode == true)
            {
                while (true)
                {
                    try
                    {
                        pollSession.minScore = Convert.ToDouble(AskQuestion("Min score to pass test:", null), cultureInfo);
                        if (pollSession.minScore > 1 || pollSession.minScore < 0)
                            throw new Exception("minScore must be greater 0 and lesser 1");
                        break;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Error: {0}", exception.Message);
                        Console.WriteLine("Please, input correct minScore(example: 0.57)");
                    }
                }
            }

            while (true)
            {
                if (pollSession.polls.Count > 0)
                {
                    bool addNewPoll = ToBoolean(AskQuestion("Add new poll[y/n]?", new String[] { "y", "n" }));
                    if (addNewPoll == false)
                        break;
                }

                Poll newPoll = new Poll();

                newPoll.id = pollSession.polls.Count + 1;
                newPoll.correctChoiceID = 0;
                newPoll.name = AskQuestion("Poll name:", null);
                newPoll.description = AskQuestion("Poll description:", null);

                while (true)
                {
                    bool isTestMode = pollSession.testMode;
                    if (newPoll.choices.Count >= (isTestMode ? 2 : 0))
                    {
                        bool addNewChoice = ToBoolean(AskQuestion("Add new choice[y/n]?", new String[] { "y", "n" }));
                        if (addNewChoice == false)
                            break;
                    }

                    Choice newChoice = new Choice();

                    newChoice.id = newPoll.choices.Count + 1;
                    newChoice.parent = newPoll;
                    newChoice.choice = AskQuestion("Choice name:", null);

                    newPoll.choices.Add(newChoice);
                    Console.WriteLine("Choice added!");
                }

                if (pollSession.testMode == false)
                {
                    // Enable CustomChoice automaticly if user doesn't inputed any choice
                    if (newPoll.choices.Count == 0)
                    {
                        newPoll.customChoiceEnabled = true;
                    }
                    else
                    {
                        newPoll.customChoiceEnabled = ToBoolean(AskQuestion("Enable custom choice for this poll[y/n]?", new String[] { "y", "n" }));
                    }
                }
                else
                {
                    // Ask correct choice
                    Console.WriteLine("Please, enter id of correct choice:");
                    foreach (Choice choice in newPoll.choices)
                        Console.WriteLine("\t" + choice.id + ". " + choice.choice);
                    while (true)
                    {
                        newPoll.correctChoiceID = -1;
                        try
                        {
                            int correctId = Convert.ToInt32(Console.ReadLine());
                            foreach (Choice choice in newPoll.choices)
                            {
                                if (choice.id == correctId)
                                {
                                    newPoll.correctChoiceID = correctId;
                                    break;
                                }
                            }
                            if (newPoll.correctChoiceID != -1)
                                break;
                            Console.WriteLine("Invalid id!");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Invalid id!");
                        }
                    }
                }

                pollSession.polls.Add(newPoll);
                Console.WriteLine("Poll added!");
            }

            // Save pollsession to server
            bool savePollsession = ToBoolean(AskQuestion("Do you want to save pollsesion to server[y/n]?", new String[] { "y", "n" }));
            if (savePollsession)
            {
                PollPacket sendPacket = new PollPacket();
                sendPacket.request.type = Request.CREATE_POLLSESSION;
                sendPacket.pollSession = pollSession;
                string sendString = PollSerializator.SerializePacket(sendPacket);
                server.Send(sendString);
                Console.WriteLine("New pollsession successfully sent to server");
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
                    Console.WriteLine("Please wait, connection to server...");
                    server = new TcpServer();
                    server.Connect(HOST, PORT);
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
        /// Main method
        /// </summary>
        public static void Main()
        {
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            username = AskQuestion("Enter your name:", null);
            ConnectToServer();

            while (true)
            {
                RunUserDialog();
                if (AskQuestion("Do you want to execute another action[y/n]?", new String[] { "y", "n" }) == "n")                
                    break;
            }

            // Disconnect from server
            server.Disconnect();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
