using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.Helpers
{
    public class PollSerializator
    {
        public static PollSession DeSerialize(string xmlString)
        {
            PollSession pollSession = new PollSession();
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PollSession));
            MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

            pollSession = (PollSession)xmlSerializer.Deserialize(memoryStream);

            foreach (Poll poll in pollSession.polls)
            {
                foreach (Choice choice in poll.choices)
                {
                    choice.parent = poll;
                }
            }

            return pollSession;
        }

        public static String Serialize(PollSession pollSession)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(PollSession));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                xs.Serialize(xmlTextWriter, pollSession);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = Encoding.ASCII.GetString(memoryStream.ToArray());

                return XmlizedString;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
