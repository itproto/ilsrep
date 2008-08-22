using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Helpers;

namespace Ilsrep.PollApplication.Communication
{
    /// <summary>
    /// Holds a list of poll session ids and names
    /// </summary>
    [XmlRoot("pollsessionlist"), Serializable]
    public class PollSessionList
    {
        /// <summary>
        /// a list of items that holds ids and names
        /// </summary>
        [XmlElement("item", typeof(Item))] public List<Item> items = new List<Item>();

        /// <summary>
        /// Allows quick access to items
        /// </summary>
        /// <param name="index">index of item</param>
        /// <returns>Item in specified index</returns>
        public Item this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        /// <summary>
        /// List poll sessions
        /// </summary>
        public int GetPollSession()
        {
            int index = 1;
            Console.WriteLine( "Poll Sessions:" );
            foreach (Item curItem in items)
            {
                ++ index;
                Console.WriteLine(index + ". " + curItem.name);
            }
            int pollSessionIndex = InputHelper.AskQuestion( String.Format( "Choose pollsession [1-{0}]:", index ), 1, index );
            return pollSessionIndex;
            //return Convert.ToInt32( items[pollSessionIndex-1].id );
        }
    }
}
