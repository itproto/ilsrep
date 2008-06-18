using System;
using System.Collections.Generic;
using System.Text;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.Model
{
    class PollSession
    {
        public int id;
        public string name;
        public bool testMode;
        public double minScore;
        public List<Poll> polls = new List<Poll>();
    }
}
