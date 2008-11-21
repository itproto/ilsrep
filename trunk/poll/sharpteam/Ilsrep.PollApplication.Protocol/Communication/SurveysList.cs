using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Helpers;

namespace Ilsrep.PollApplication.Communication
{
    /// <summary>
    /// Holds a list of survey ids and names
    /// </summary>
    [XmlRoot("pollsessionlist"), Serializable] // <-- surveylist
    public class SurveyList
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
        /// List surveys
        /// </summary>
        public int GetSurveyId()
        {
            int index = 0;
            Console.WriteLine( "Surveys:" );
            foreach (Item curItem in items)
            {
                index++;
                Console.WriteLine(index + ". " + curItem.name);
            }
            int surveyIndex = InputHelper.AskQuestion( String.Format( "Choose survey [1-{0}]:", index ), 1, index );
            return surveyIndex;
            //return Convert.ToInt32( items[surveyIndex-1].id );
        }
    }
}
