using System;
using System.Collections.Generic;
using System.Text;

namespace SharpTeam.PollClient
{
    public class Choice
    {
        public int id;
        public string choice;
        public Poll parent;
    }

    public class Poll
    {
        public int id;
        public string name;
        public string description;
        public bool customChoice;
        public List<Choice> choice = new List<Choice>();
    }
}
