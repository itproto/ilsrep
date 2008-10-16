using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilsrep.PollApplication.Helpers
{
    public class HistoryHelper
    {
        public List<int> edited = new List<int>();
        public List<int> deleted = new List<int>();

        public void AddToEdited(int id)
        {
            foreach (int curId in edited)
            {
                if (curId == id)
                    return;
            }
            edited.Add(id);
        }

        public void AddToDeleted(int id)
        {
            foreach (int curId in deleted)
            {
                if (curId == id)
                    return;
            }
            deleted.Add(id);
        }

        public void Exclude()
        {
            foreach (int idDeleted in deleted)
            {
                edited.Remove(idDeleted);
            }
        }

        public void Clear()
        {
            edited.Clear();
            deleted.Clear();
        }
    }
}
