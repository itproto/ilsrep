using System;
using System.Collections.Generic;
using System.Text;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common.TcpServer;

namespace Ilsrep.PollApplication.PollEditor
{
    public class PollEditor
    {
        private static System.Globalization.CultureInfo cultureInfo = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();

        private static String username = String.Empty;
        private static PollSession pollSession = new PollSession();

        private const String HOST = "localhost";
        private const int PORT = 3120;

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
                pollSession.minScore = Convert.ToDouble(AskQuestion("Min score to pass test:", null), cultureInfo);

            while (true)
            {
                bool addNewPoll = ToBoolean(AskQuestion("Add new poll[y/n]?", new String[] { "y", "n" }));

                if (addNewPoll == false)
                    break;

                Poll newPoll = new Poll();

                newPoll.id = pollSession.polls.Count + 1;
                newPoll.correctChoiceId = 0;
                newPoll.name = AskQuestion("Poll name:", null);
                newPoll.description = AskQuestion("Poll description:", null);

                while (true)
                {
                    bool addNewChoice = ToBoolean(AskQuestion("Add new choice[y/n]?", new String[] { "y", "n" }));

                    if (addNewChoice == false)
                        break;

                    Choice newChoice = new Choice();

                    newChoice.id = newPoll.choices.Count + 1;
                    newChoice.parent = newPoll;
                    newChoice.choice = AskQuestion("Choice name:", null);
                    
                    newPoll.choices.Add( newChoice );

                    if (newPoll.correctChoiceId == 0)
                    {
                        bool isChoiceCorrect = ToBoolean(AskQuestion("Is this choice correct[y/n]?", new String[] { "y", "n" }));

                        if (isChoiceCorrect == true)
                            newPoll.correctChoiceId = newChoice.id;
                    }

                    Console.WriteLine("Choice added!");
                }

                if (pollSession.testMode == false)
                {
                    newPoll.customChoice = ToBoolean(AskQuestion("Enable custom choice for this poll[y/n]?", new String[] { "y", "n" }));
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

            bool connectToServer = ToBoolean(AskQuestion("Do you want to save pollsesion to server[y/n]?", new String[] { "y", "n" }));

            if (connectToServer == true)
            {
                TcpServer client = new TcpServer();
                client.Connect(HOST, PORT);
                client.Send("CreatePollSession");
                client.Send(XMLSerializationHelper.Serialize(pollSession));
                client.Disconnect();
            }
        }
    }
}
