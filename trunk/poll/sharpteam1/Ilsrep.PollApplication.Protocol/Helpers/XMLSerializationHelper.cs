using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.Helpers
{
    public class XMLSerializationHelper
    {
        public static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);

            return (constructedString);
        }

        public static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);

            return byteArray;
        }

        public static PollSession DeSerialize(string xmlString)
        {
            PollSession pollSession = new PollSession();
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PollSession));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xmlString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

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
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

                xs.Serialize(xmlTextWriter, pollSession);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());

                return XmlizedString;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
