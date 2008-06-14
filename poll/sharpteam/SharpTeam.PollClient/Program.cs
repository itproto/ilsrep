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
            xmlDoc.Load(PATH_TO_POLLS);
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
            foreach (Poll curPoll in pollDoc)
            {
                Console.WriteLine(curPoll.id + ") Name: " + curPoll.name);
                Console.WriteLine("   Description: " + curPoll.description);
                Console.WriteLine("   CustomChoiceEnabled: " + curPoll.customChoice);
                foreach (Choice curChoice in curPoll.choice)
                {
                    Console.WriteLine("      " + curChoice.id + ". " + curChoice.choice);
                }
            }
        }

        public static void Main()
        {
            const string PATH_TO_POLLS = "Polls.xml";
            List<Poll> pollDoc = ParseXml(PATH_TO_POLLS);
            DisplayPollDoc(pollDoc);
            Console.ReadKey(true);
        }
    }
}
