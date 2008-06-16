using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

/*
 ParseXml author: ksi
 UserDialog author: vlad
*/

namespace Ilsrep.Poll.Client
{
    public class Program
    {
        private const string PATH_TO_POLLS = "Polls.xml";
        private const string POLL_ELEMENT = "poll";

        public static List<Poll> ParseXml()
        {
            //---------------Init---------------
            bool customChoiceExists;
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

            XmlNodeList xmlPollList = xmlDoc.GetElementsByTagName(POLL_ELEMENT);
            //---------------Fill pollDoc---------------
            foreach (XmlNode xmlPoll in xmlPollList)
            {
                Poll currentPoll = new Poll();
                XmlAttributeCollection xmlAttr = xmlPoll.Attributes;
                // Get current Poll id
                currentPoll.id = Convert.ToInt32(xmlAttr["id"].Value);
                // Get current Poll name
                currentPoll.name = xmlAttr["name"].Value;
                // Get current Poll customChoiceEnabled option
                customChoiceExists = (xmlAttr["customChoiceEnabled"] != null);
                if (customChoiceExists)
                    currentPoll.customChoice = Convert.ToBoolean(xmlAttr["customChoiceEnabled"].Value);
                // Get current Poll description
                currentPoll.description = xmlPoll.FirstChild.InnerText;
                // Get correct choice in current Poll
                currentPoll.correctChoice = Convert.ToInt32(xmlAttr["correctChoice"].Value);

                // Get list of chioces of current Poll
                XmlNodeList xmlChoices = xmlPoll.ChildNodes;
                XmlNodeList xmlChoicesList = xmlChoices[1].ChildNodes;
                foreach (XmlNode xmlChoice in xmlChoicesList)
                {
                    Choice currentChoice = new Choice();
                    XmlAttributeCollection xmlAttrChoice = xmlChoice.Attributes;
                    // Get current choice id
                    currentChoice.id = Convert.ToInt32(xmlAttrChoice["id"].Value);
                    // Get current choice name
                    currentChoice.choice = xmlAttrChoice["name"].Value;
                    // Save current poll in current choice for future convenience
                    currentChoice.parent = currentPoll;
                    currentPoll.choice.Add(currentChoice);
                }
                pollDoc.Add(currentPoll);
            }
            return pollDoc;
        }

        public static void UserDialog(List<Poll> pollDoc)
        {
            List<Choice> userChoices = new List<Choice>();

            foreach (Poll curPoll in pollDoc)
            {
                // Clear screen and write this poll's question
                Console.Clear();
                Console.WriteLine(curPoll.name + "\n" + curPoll.description);
                
                // List choices for this poll
                int index = 1;
                foreach (Choice curChoice in curPoll.choice)
                {
                    Console.WriteLine("\t" + index + ". " + curChoice.choice);
                    ++ index;
                }

                // add option for customer choice
                if ( curPoll.customChoice == true )
                {
                    Console.WriteLine("\t" + index + ". Custom Choice");
                }

                // accept only correct choices
                while ( true )
                {
                    Console.Write("Pick your choice: [1-" + (curPoll.customChoice ? curPoll.choice.Count + 1 : curPoll.choice.Count) + "]:");

                    // Get user choice
                    try
                    {
                        index = Convert.ToInt32(Console.ReadLine());
                        --index;

                        // check if input correct
                        if (index >= 0 && index <= curPoll.choice.Count - (curPoll.customChoice ? 0 : 1))
                            break;

                    }
                    catch( Exception )
                    {
                        //continue;
                    }
                    Console.WriteLine("Invalid choice!");
                }

                // check if custom choice
                if ( index == curPoll.choice.Count )
                {
                    Console.Write("Enter your choice:" );

                    // create custom choice
                    Choice userChoice = new Choice();
                    userChoice.choice = Console.ReadLine();
                    userChoice.id = 0;
                    userChoice.parent = curPoll;

                    // add custom choice to list
                    userChoices.Add(userChoice);
                }
                else
                {
                    // add one of the choices that already exist to list
                    userChoices.Add(curPoll.choice[index]);
                }

            // end of loop of polls
            }

            // list choices
            Console.Clear();
            Console.WriteLine("Your choices:");

            // go through choices and display them
            foreach(Choice choice in userChoices)
            {
                Console.WriteLine(choice.parent.name + ": " + (choice.id == 0 ? "Custom Choice: " : choice.id + ". ") + choice.choice);
            }
        }

        public static void Main()
        {
            List<Poll> pollDoc = ParseXml();
            UserDialog(pollDoc);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
