using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SharpTeam.PollClient
{
    class Poll
    {
        public string name;
        public string description;
        public string[] choise = new string[3];
    }

    class Program
    {
        static void Main(string[] args)
        {
            Poll myDoc = new Poll();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load("Polls.xml");
            XmlNodeList xmlPoll = xmldoc.GetElementsByTagName("poll");
            for (int i = 0; i < xmlPoll.Count; i++)
            {
                XmlAttributeCollection xmlAttr = xmlPoll[i].Attributes;
                myDoc.name = Convert.ToString(xmlAttr[1].Value);
                Console.WriteLine(xmlAttr[1].Value); //name of poll

                myDoc.description = xmlPoll[i].FirstChild.InnerText;
                Console.WriteLine(xmlPoll[i].FirstChild.InnerText); //description

                XmlNodeList xmlChoices = xmlPoll[i].ChildNodes;
                XmlNodeList xmlChoice = xmlChoices[1].ChildNodes;
                for (int j = 0; j < xmlChoice.Count; j++)
                {
                    XmlAttributeCollection xmlAttrCh = xmlChoice[j].Attributes;
                    myDoc.choise[j] = xmlAttrCh[1].Value;
                    Console.WriteLine(xmlAttrCh[1].Value); //choise
                }

                Console.ReadKey(true);
            }
        }
    }
}
