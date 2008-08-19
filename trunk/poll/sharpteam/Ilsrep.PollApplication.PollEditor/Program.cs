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
        private static TcpClient client;

        /// <summary>
        /// Helper method to get answers on questions, with possibility of choosing what user can answer
        /// Empty answer is not allowed
        /// </summary>
        /// <param name="question">Question to be asked</param>
        /// <param name="allowedAnswers">Allowed answers that user can input. Set to null if any</param>
        /// <returns>Answer from user</returns>
        public static string AskQuestion(string question, string[] allowedAnswers)
        {
            string inputLine = String.Empty;

            while (true)
            {
                Console.Write(question);
                inputLine = Console.ReadLine();

                if (inputLine != String.Empty)
                {
                    if (allowedAnswers == null || allowedAnswers.Length == 0)
                        return inputLine;
                    
                    foreach (string allowedAnswer in allowedAnswers)
                        if (inputLine == allowedAnswer)
                            return inputLine;
                }
                
                Console.WriteLine("Wrong input!");
            }
        }

        /// <summary>
        /// Helper method to get answers on questions with default answer, with possibility of choosing what user can answer
        /// Empty answer is not allowed
        /// </summary>
        /// <param name="question">Question to be asked</param>
        /// <param name="defaultAnswer">Default answer that gets filled in</param>
        /// <param name="allowedAnswers">Allowed answers that user can input. Set to null if any</param>
        /// <returns>Answer from user</returns>
        public static string AskQuestion(string question, string defaultAnswer, string[] allowedAnswers)
        {
            string inputLine = defaultAnswer;

            while (true)
            {
                Console.Write(question + defaultAnswer);

                for (ConsoleKeyInfo cki = Console.ReadKey(); cki.Key != ConsoleKey.Enter; cki = Console.ReadKey())
                {
                    //rollback the cursor and write a space so it looks backspaced to the user
                    if (cki.Key == ConsoleKey.Backspace)
                    {
                        if (inputLine.Length == 0)
                            continue;

                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        inputLine = inputLine.Substring(0, inputLine.Length - 1);
                    }
                    else
                    {
                        inputLine += cki.KeyChar;
                    }
                }

                if (inputLine != String.Empty)
                {
                    if (allowedAnswers == null || allowedAnswers.Length == 0)
                        return inputLine;

                    foreach (string allowedAnswer in allowedAnswers)
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
        public static bool ToBoolean(string boolString)
        {
            if (boolString == "y")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Convery boolean to string
        /// </summary>
        /// <param name="boolValue">boolean value</param>
        /// <returns>string "y" if boolValue is true, else "n"</returns>
        public static string ToString(bool boolValue)
        {
            if (boolValue)
                return "y";
            else
                return "n";
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
                    Console.WriteLine(exception.Message);
                }

                string receivedString = client.Receive();
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
                            client.Disconnect();
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
            sendPacket.request = new Request();
            sendPacket.request.type = Request.GET_LIST;

            PollPacket receivedPacket = new PollPacket();
            receivedPacket = ReceivePollPacket(sendPacket);

            // Check if list is not empty
            if (receivedPacket.pollSessionList.items.Count == 0)
            {
                Console.WriteLine("Sorry, but data base is empty, no pollsessions...");
                if (AskQuestion("Would you create new pollsession[y/n]?", new String[] { "y", "n" }) == "y")
                {
                    CreatePollsession();
                }
                else
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(true);
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
                        throw new Exception("Invalid action!");
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
                    int pollSessionIndex = 0;
                    while(true)
                    {
                        try
                        {
                            Console.Write("Choose pollsesion [1-{0}]:", index);
                            pollSessionIndex = Convert.ToInt32(Console.ReadLine());

                            if (pollSessionIndex > 0 && pollSessionIndex <= index)
                            {
                                --pollSessionIndex;
                                break;
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Wrong choice!");
                        }
                    }

                    EditPollsession(Convert.ToInt32(receivedPacket.pollSessionList.items[pollSessionIndex].id));
                    //Console.WriteLine("IN FUTURE HERE WILL BE EDITING POLLSESSION");
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
                        sendPacket.request = new Request();
                        sendPacket.request.type = Request.REMOVE_POLLSESSION;
                        sendPacket.request.id = pollSessionID;
                        string sendString = PollSerializator.SerializePacket(sendPacket);
                        client.Send(sendString);
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
        /// Edits current session and saves it to server
        /// </summary>
        /// <param name="pollSessionID">id of session to edit</param>
        public static void EditPollsession(int pollSessionID)
        {
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.GET_POLLSESSION;
            sendPacket.request.id = pollSessionID.ToString();
            PollPacket receivedPacket = ReceivePollPacket(sendPacket);
            string sendString = PollSerializator.SerializePacket(sendPacket);
            pollSession = receivedPacket.pollSession;

            pollSession.name = AskQuestion("Enter pollsession name:", pollSession.name, null);
            pollSession.testMode = ToBoolean(AskQuestion("Test mode[y/n]?", ToString(pollSession.testMode), new String[] { "y", "n" }));


            if (pollSession.testMode == true)
            {
                while (true)
                {
                    try
                    {
                        pollSession.minScore = Convert.ToDouble(AskQuestion("Min score to pass test:", pollSession.minScore.ToString(), null), cultureInfo);
                        if (pollSession.minScore > 1 || pollSession.minScore < 0)
                            throw new Exception("minScore must be between 0 and 1");
                        break;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Error: {0}", exception.Message);
                        Console.WriteLine("Please, input correct minScore(example: 0.5)");
                    }
                }
            }

            /*
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
             * */

        }

        /// <summary>
        /// Create new pollsession and save it to server
        /// </summary>
        public static void CreatePollsession()
        {
            pollSession = new PollSession();
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
                sendPacket.request = new Request();
                sendPacket.request.type = Request.CREATE_POLLSESSION;
                sendPacket.pollSession = pollSession;
                string sendString = PollSerializator.SerializePacket(sendPacket);
                client.Send(sendString);
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
                    Console.WriteLine("Please wait, connecting to server...");
                    client = new TcpClient();
                    client.Connect(HOST, PORT);
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
        private static void DisconnectFromServer()
        {
            client.Disconnect();
        }

        /// <summary>
        /// Main method
        /// </summary>
        public static void Main()
        {
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            username = AskQuestion("Enter your name:", null);

            while (true)
            {
                ConnectToServer();
                RunUserDialog();
                DisconnectFromServer();
                if (AskQuestion("Do you want to execute another action[y/n]?", new String[] { "y", "n" }) == "n")                
                    break;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
