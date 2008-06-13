using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

/*
 130608 Initial codereview by okl
 */
namespace SharpTeam.PollClient
{
    public class Program
    {
        //try always to split complexity, use more smaller functions instead of big one. Split tasks to smallers
        static void Main(string[] args)
        {
            //---------------Init---------------
            int i, j;
            List<Poll> pollDoc = new List<Poll>();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load("Polls.xml"); //NO string literals, numbers in functions. use private const PATH_TO_POLLS = ""; 
            XmlNodeList xmlPoll = xmldoc.GetElementsByTagName("poll");

            //---------------Fill pollDoc---------------
            /*
              don't be boring use )
                  foreach(XmlNode pollNode in pollNodeList){
                    ...
                    currentPoll.name = pollNode.Attributes[""].Value..
                  }
             instead of 
             
             "for (i = 0; i < xmlPoll.Count; i++)"
             as we discussed
             */

            for (i = 0; i < xmlPoll.Count; i++)
            {
                Poll currentPoll = new Poll();
                XmlAttributeCollection xmlAttr = xmlPoll[i].Attributes;
                currentPoll.id = Convert.ToInt32(xmlAttr["id"].Value); //okl: xmlAttr["id"] looks better then long xmlAttr.GetNamedItem("id")
                currentPoll.name = xmlAttr.GetNamedItem("name").Value;
                bool customChoiceExists = (xmlAttr["customChoiceEnabled"] != null); //looks simple? yes it is
                //if (xmlAttr.GetNamedItem("customChoiceEnabled"))
                    //currentPoll.customChoice = Convert.ToBoolean(xmlAttr.GetNamedItem("customChoiceEnabled").Value);
                currentPoll.description = xmlPoll[i].FirstChild.InnerText;

                XmlNodeList xmlChoices = xmlPoll[i].ChildNodes;
                XmlNodeList xmlChoice = xmlChoices[1].ChildNodes;
                for (j = 0; j < xmlChoice.Count; j++)
                {
                    Choice currentChoice = new Choice();
                    XmlAttributeCollection xmlAttrChoice = xmlChoice[j].Attributes;
                    currentChoice.id = Convert.ToInt32(xmlAttrChoice.GetNamedItem("id").Value);
                    currentChoice.choice = xmlAttrChoice.GetNamedItem("name").Value;
                    currentPoll.choice.Add(currentChoice);
                }
                pollDoc.Add(currentPoll);
            }

            //---------------Display contains of pollDoc---------------
            foreach(Poll tempPoll in pollDoc) //nothing temporary, better curPoll or currentPoll
            {
                Console.WriteLine(tempPoll.id + ") Name: " + tempPoll.name);
                Console.WriteLine("   Description: " + tempPoll.description);
                Console.WriteLine("   CustomChoiceEnabled: " + tempPoll.customChoice);
                foreach (Choice tempChoice in tempPoll.choice)
                {
                    Console.WriteLine("      " + tempChoice.id + ". " + tempChoice.choice);
                }
            }
            Console.ReadKey(true);
        }
    }
}
