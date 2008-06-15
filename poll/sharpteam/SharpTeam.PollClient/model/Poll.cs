using System;
using System.Collections.Generic;
using System.Text;

namespace Ilsrep.Poll.Client
{
    public class Poll
    {
        public int id;
        public string name;
        public string description;
        public bool customChoice;
        public List<Choice> choice = new List<Choice>();
    }
}
