using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common;

namespace Ilsrep.PollApplication.PollEditor
{
    public class PollEditor
    {
        private static System.Globalization.CultureInfo cultureInfo = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
        private static String username = String.Empty;
        private static PollSession pollSession = new PollSession();
        private const String HOST = "localhost";
        private const int PORT = 3320;

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
        /// Makes conversation with user
        /// </summary>
        public static void RunUserDialog()
        {
            username = AskQuestion("Enter your username:", null);
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
                newPoll.correctChoiceId = 0;
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
                    
                    newPoll.choices.Add( newChoice );
                    Console.WriteLine("Choice added!");
                }

                if (pollSession.testMode == false)
                {
                    // Enable CustomChoice automaticly if user doesn't inputed any choice
                    if (newPoll.choices.Count == 0)
                    {
                        newPoll.customChoice = true;
                    }
                    else
                    {
                        newPoll.customChoice = ToBoolean(AskQuestion("Enable custom choice for this poll[y/n]?", new String[] { "y", "n" }));
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
                        newPoll.correctChoiceId = -1;
                        try
                        {
                            int correctId = Convert.ToInt32(Console.ReadLine());
                            foreach (Choice choice in newPoll.choices)
                            {
                                if (choice.id == correctId)
                                {
                                    newPoll.correctChoiceId = correctId;
                                    break;
                                }
                            }
                            if (newPoll.correctChoiceId != -1)
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
        }

        /// <summary>
        /// Main method
        /// </summary>
        public static void Main()
        {
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            RunUserDialog();

            // Save pollsession to server
            bool connectToServer = ToBoolean(AskQuestion("Do you want to save pollsesion to server[y/n]?", new String[] { "y", "n" }));
            if (connectToServer == true)
            {
                TcpServer client = new TcpServer();
                client.Connect(HOST, PORT);
                PollPacket sendPacket = new PollPacket();
                sendPacket.request.type = Request.CREATE_POLLSESSION;
                sendPacket.pollSession = pollSession;
                string sendString = PollSerializator.SerializePacket(sendPacket);
                client.Send(sendString);
                client.Disconnect();
                Console.WriteLine("Pollsession successfully sent to server");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
