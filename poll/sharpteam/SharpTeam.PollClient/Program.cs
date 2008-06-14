using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

/*
 140608 Fixed all bugs by ksi
*/

namespace SharpTeam.PollClient
{
    public class Program
    {
        public static List<Poll> ParseXml(string PATH_TO_POLLS)
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
                Environment.Exit(-1);
            }
            
            XmlNodeList xmlPollList = xmlDoc.GetElementsByTagName("poll");

            //---------------Fill pollDoc---------------
            foreach (XmlNode xmlPoll in xmlPollList)
            {
                Poll currentPoll = new Poll();
                XmlAttributeCollection xmlAttr = xmlPoll.Attributes;
                currentPoll.id = Convert.ToInt32(xmlAttr["id"].Value);
                currentPoll.name = xmlAttr["name"].Value;
                customChoiceExists = (xmlAttr["customChoiceEnabled"] != null);
                if (customChoiceExists)
                    currentPoll.customChoice = Convert.ToBoolean(xmlAttr["customChoiceEnabled"].Value);
                currentPoll.description = xmlPoll.FirstChild.InnerText;

                XmlNodeList xmlChoices = xmlPoll.ChildNodes;
                XmlNodeList xmlChoicesList = xmlChoices[1].ChildNodes;
                foreach (XmlNode xmlChoice in xmlChoicesList)
                {
                    Choice currentChoice = new Choice();
                    XmlAttributeCollection xmlAttrChoice = xmlChoice.Attributes;
                    currentChoice.id = Convert.ToInt32(xmlAttrChoice["id"].Value);
                    currentChoice.choice = xmlAttrChoice["name"].Value;
                    currentPoll.choice.Add(currentChoice);
                }
                pollDoc.Add(currentPoll);
            }
            return pollDoc;
        }

        public static void DisplayPollDoc(List<Poll> pollDoc)
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

                Console.Write("Pick your choice: [1-" + (curPoll.customChoice ? curPoll.choice.Count + 1 : curPoll.choice.Count) + "]:");
                
                // accept only correct choices
                while ( true )
                {
                    // Get user choice but don't show ( not sure if it's correct )
                    try
                    {
                        index = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                        --index;
                    }
                    catch( Exception )
                    {
                        continue;
                    }
                    
                    if ( index > 0 && index <= curPoll.choice.Count - ( curPoll.customChoice ? 0 : 1 ) )
                    {
                        // show user choice
                        break;
                    } 
                }

                // check if custom choice
                if ( index == curPoll.choice.Count )
                {
                    Console.Write("\nEnter your choice:" );

                    // create custom choice
                    Choice userChoice = new Choice();
                    userChoice.choice = Console.ReadLine();
                    userChoice.id = 0;

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

            // don't know how to do this with foreach... ((
            for (int i = 0; i < pollDoc.Count; ++ i )
            {
                Console.WriteLine(pollDoc[i].name + ": " + (userChoices[i].id == 0 ? "Custom Choice: " : userChoices[i].id + ". ") + userChoices[i].choice);
            }
        }

        public static void Main()
        {
            const string PATH_TO_POLLS = "Polls.xml";
            List<Poll> pollDoc = ParseXml(PATH_TO_POLLS);
            DisplayPollDoc(pollDoc);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
