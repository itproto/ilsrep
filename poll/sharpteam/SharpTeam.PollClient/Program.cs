using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Ilsrep.PollApplication.Model;

/*
 ParseXml by ksi
 DoUserDialog by ksi
 RunUserPoll by vlad
*/

namespace Ilsrep.PollApplication.Client
{
    public class Program
    {
        private const string PATH_TO_POLLS = "Polls.xml";
        private const string POLL_SESSION_ELEMENT = "pollsession";
        private const string POLL_ELEMENT = "poll";
        private const string CONSOLE_YES = "y";
        private const string CONSOLE_NO = "n";
        static string userName = "";

        static PollSession pollSession = new PollSession();
        static List<Choice> userChoices = new List<Choice>();

        public static void ParseXml()
        {
            //---------------Init---------------
            List<Poll> pollDoc = new List<Poll>();
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(PATH_TO_POLLS);
            }
            catch(Exception)
            {
                System.Console.WriteLine("Couldn't find xml file: " + PATH_TO_POLLS);
                Console.ReadKey(true);
                Environment.Exit(-1);
            }

            System.Globalization.CultureInfo cultureInfo = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";

            XmlNodeList xmlPollSessionList = xmlDoc.GetElementsByTagName(POLL_SESSION_ELEMENT);

            foreach (XmlNode xmlPollSession in xmlPollSessionList)
            {
                //XmlNodeList xmlPollList = ; /* xmlDoc.GetElementsByTagName(POLL_ELEMENT); */
                //---------------Fill pollDoc---------------
                pollSession.id = Convert.ToInt32(xmlPollSession.Attributes["id"].Value);
                pollSession.name = xmlPollSession.Attributes["name"].Value;
                pollSession.testMode = Convert.ToBoolean(xmlPollSession.Attributes["testMode"].Value);
                pollSession.minScore = Convert.ToDouble(xmlPollSession.Attributes["minScore"].Value, cultureInfo);

                foreach (XmlNode xmlPoll in xmlPollSession.ChildNodes)
                {
                    Poll newPoll = new Poll();
                    XmlAttributeCollection xmlAttr = xmlPoll.Attributes;
                    // Get current Poll id
                    newPoll.id = Convert.ToInt32(xmlAttr["id"].Value);
                    // Get current Poll name
                    newPoll.name = xmlAttr["name"].Value;
                    // Get current Poll customChoiceEnabled option
                    if (xmlAttr["customChoiceEnabled"] != null)
                        newPoll.customChoice = Convert.ToBoolean(xmlAttr["customChoiceEnabled"].Value);
                    // Get current Poll description
                    //newPoll.description = xmlPoll.FirstChild.InnerText;
                    // Get correct choice in current Poll
                    newPoll.correctChoiceId = Convert.ToInt32(xmlAttr["correctChoice"].Value);

                    foreach( XmlNode node in xmlPoll.ChildNodes )
                    {
                        if (node.Name == "choices")
                        {
                            // Get list of choices of current Poll
                            foreach (XmlNode xmlChoice in node.ChildNodes)
                            {
                                Choice newChoice = new Choice();
                                XmlAttributeCollection xmlAttrChoice = xmlChoice.Attributes;
                                // Get current choice id
                                newChoice.id = Convert.ToInt32(xmlAttrChoice["id"].Value);
                                // Get current choice name
                                newChoice.choice = xmlAttrChoice["name"].Value;
                                // Save current poll in current choice for future convenience
                                newChoice.parent = newPoll;
                                newPoll.choice.Add(newChoice);
                            }
                        }
                        else if (node.Name == "description")
                        {
                            newPoll.description = node.InnerText;
                        }
                    }

                    pollSession.polls.Add(newPoll);
                }
            }
        }

        public static void DoUserDialog()
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
                    Console.Clear();
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
                foreach (Choice currentChoice in curentPoll.choice)
                {
                    Console.WriteLine("\t" + index + ". " + currentChoice.choice);
                    ++ index;
                }

                // add option for customer choice
                if ( curentPoll.customChoice == true )
                {
                    Console.WriteLine("\t" + index + ". Custom Choice");
                }

                // accept only correct choices
                while ( true )
                {
                    Console.Write("Pick your choice: [1-" + (curentPoll.customChoice ? curentPoll.choice.Count + 1 : curentPoll.choice.Count) + "]:");

                    // Get user choice
                    try
                    {
                        index = Convert.ToInt32(Console.ReadLine());
                        --index;

                        // check if input correct
                        if (index >= 0 && index <= curentPoll.choice.Count - (curentPoll.customChoice ? 0 : 1))
                            break;

                    }
                    catch( Exception )
                    {
                        
                    }
                    Console.WriteLine("Invalid choice!");
                }

                // check if custom choice
                if ( index == curentPoll.choice.Count )
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
                    userChoices.Add(curentPoll.choice[index]);
                }
            }

            // go through choices and display them
            Console.Clear();
            Console.WriteLine(userName + ", here is your PollSession results:");
            foreach(Choice userChoice in userChoices)
            {
                Console.WriteLine(userChoice.parent.name + ": " + (userChoice.id == 0 ? "Custom Choice: " : userChoice.id + ". ") + userChoice.choice + ( pollSession.testMode ? (userChoice.parent.correctChoiceId == userChoice.id ? " +" : " -")  : String.Empty ) );
            }
        }

        public static void Main()
        {
            ParseXml();
            DoUserDialog();
            RunUserPoll();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
