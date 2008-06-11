using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SharpTeam.PollClient
{
    public class Poll
    {
        public Poll()
        {
            name = "";
            description = "";
            for (int i = 0; i < 3; i++)
            {
                choice[i] = "";
            }
        }

        public string name;
        public string description;
        public string[] choice = new string[3];
    }

    public class Program
    {
        static void Main(string[] args)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load("Polls.xml");
            XmlNodeList xmlPoll = xmldoc.GetElementsByTagName("poll");

            Poll[] myDoc;
            myDoc = new Poll[xmlPoll.Count];
            for (int i = 0; i < xmlPoll.Count; i++)
                myDoc[i] = new Poll();

            for (int i = 0; i < xmlPoll.Count; i++)
            {
                XmlAttributeCollection xmlAttr = xmlPoll[i].Attributes;
                myDoc[i].name = xmlAttr[1].Value;
                Console.WriteLine(xmlAttr[1].Value); //name of poll

                myDoc[i].description = xmlPoll[i].FirstChild.InnerText;
                Console.WriteLine(xmlPoll[i].FirstChild.InnerText); //description

                XmlNodeList xmlChoices = xmlPoll[i].ChildNodes;
                XmlNodeList xmlChoice = xmlChoices[1].ChildNodes;
                for (int j = 0; j < xmlChoice.Count; j++)
                {
                    XmlAttributeCollection xmlAttrCh = xmlChoice[j].Attributes;
                    myDoc[i].choice[j] = xmlAttrCh[1].Value;
                    Console.WriteLine(xmlAttrCh[1].Value); //choise
                }
                
                Console.ReadKey(true);
            }
        }
    }
}
