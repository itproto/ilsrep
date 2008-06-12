using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SharpTeam.PollClient
{
    public class Poll : ICloneable
    {
            public object Clone() 
            {        
                Poll mc = this.MemberwiseClone() as Poll;        
                mc.id = this.id;
                mc.name = this.name.Clone() as string;
                mc.description = this.description.Clone() as string;
                mc.choice = this.choice.Clone() as ArrayList;
                return mc;    
            }

        public int id;
        public string name;
        public string description;
        public ArrayList choice = new ArrayList();
    }

    public class Choice : ICloneable
    {
        public object Clone()
        {
            Choice mc = this.MemberwiseClone() as Choice;
            mc.id = this.id;
            mc.choice = this.choice.Clone() as string;
            return mc;
        }

        public int id;
        public string choice;
    }

    public class Program
    {
        static void Main(string[] args)
        {
            int i, j;
            Choice tempChoice = new Choice();
            Poll tempPoll = new Poll();
            ArrayList pollDoc = new ArrayList();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load("Polls.xml");
            XmlNodeList xmlPoll = xmldoc.GetElementsByTagName("poll");
            
            for (i = 0; i < xmlPoll.Count; i++)
            {
                tempPoll.id = i;
                XmlAttributeCollection xmlAttr = xmlPoll[i].Attributes;
                tempPoll.name = xmlAttr[1].Value;
                tempPoll.description = xmlPoll[i].FirstChild.InnerText;

                XmlNodeList xmlChoices = xmlPoll[i].ChildNodes;
                XmlNodeList xmlChoice = xmlChoices[1].ChildNodes;
                for (j = 0; j < xmlChoice.Count; j++)
                {
                    XmlAttributeCollection xmlAttrCh = xmlChoice[j].Attributes;
                    tempChoice.id = j+1;
                    tempChoice.choice = xmlAttrCh[1].Value;
                    tempPoll.choice.Add(tempChoice.Clone());
                }
                pollDoc.Add(tempPoll.Clone());
                tempPoll.choice.Clear();
            }

            foreach(Poll myPoll in pollDoc)
            {
                Console.WriteLine(myPoll.id+1 + ") Name: " + myPoll.name);
                Console.WriteLine("   Description:" + myPoll.description);
                foreach (Choice myChoice in myPoll.choice)
                {
                    Console.WriteLine("      " + myChoice.id + ". " + myChoice.choice);
                }
            }
            Console.ReadKey(true);
        }
    }
}
