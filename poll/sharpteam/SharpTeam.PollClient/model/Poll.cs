using System;
using System.Collections.Generic;
using System.Text;

namespace Ilsrep.PollApplication.Model
{
    public class Poll
    {
        public int id;
        public string name;
        public string description;
        public bool customChoice;
        public int correctChoice;
        public List<Choice> choice = new List<Choice>();
    }
}
