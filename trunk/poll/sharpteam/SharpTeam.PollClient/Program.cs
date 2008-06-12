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
                mc.customChoice = this.customChoice;
                mc.choice = this.choice.Clone() as ArrayList;
                return mc;    
            }

        public int id;
        public string name;
        public string description;
        public bool customChoice;
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
            int i, j, k;
            Choice tempChoice = new Choice();
            Poll tempPoll = new Poll();
            ArrayList pollDoc = new ArrayList();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load("Polls.xml");
            XmlNodeList xmlPoll = xmldoc.GetElementsByTagName("poll");
            
            for (i = 0; i < xmlPoll.Count; i++)
            {
                XmlAttributeCollection xmlAttr = xmlPoll[i].Attributes;
                for (k = 0; k < xmlAttr.Count; k++)
                {
                    switch (k)
                    {
                    case 0:
                        tempPoll.id = Convert.ToInt32(xmlAttr[k].Value);
                        break;
                    case 1:
                        tempPoll.name = xmlAttr[k].Value;
                        break;
                    case 2:
                        tempPoll.customChoice = Convert.ToBoolean(xmlAttr[k].Value);
                        break;
                    }
                }
                tempPoll.description = xmlPoll[i].FirstChild.InnerText;

                XmlNodeList xmlChoices = xmlPoll[i].ChildNodes;
                XmlNodeList xmlChoice = xmlChoices[1].ChildNodes;
                for (j = 0; j < xmlChoice.Count; j++)
                {
                    XmlAttributeCollection xmlAttrCh = xmlChoice[j].Attributes;
                    for (k = 0; k < xmlAttr.Count; k++)
                    {
                        switch (k)
                        {
                            case 0:
                                tempChoice.id = Convert.ToInt32(xmlAttrCh[k].Value);
                                break;
                            case 1:
                                tempChoice.choice = xmlAttrCh[k].Value;
                                break;
                        }
                    }
                    tempPoll.choice.Add(tempChoice.Clone());
                }
                pollDoc.Add(tempPoll.Clone());
                tempPoll.choice.Clear();
            }

            foreach(Poll myPoll in pollDoc)
            {
                Console.WriteLine(myPoll.id + ") Name: " + myPoll.name);
                Console.WriteLine("   Description: " + myPoll.description);
                Console.WriteLine("   CustomChoiceEnabled: " + myPoll.customChoice);
                foreach (Choice myChoice in myPoll.choice)
                {
                    Console.WriteLine("      " + myChoice.id + ". " + myChoice.choice);
                }
            }
            Console.ReadKey(true);
        }
    }
}
