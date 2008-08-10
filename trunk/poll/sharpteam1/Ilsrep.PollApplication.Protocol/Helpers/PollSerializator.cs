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
        public static PollSession DeserializePollSession(string xmlString)
        {
            PollSession pollSession = new PollSession();
            try
            {
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
            }
            catch (Exception exception)
            {
                return null;
            }
            return pollSession;
        }

        public static String SerializePollSession(PollSession pollSession)
        {
            String XmlizedString = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(PollSession));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                xs.Serialize(xmlTextWriter, pollSession);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = Encoding.ASCII.GetString(memoryStream.ToArray());
            }
            catch (Exception exception)
            {
                return null;
            }
            return XmlizedString;
        }
        
        public static PollPacket DeserializePacket(string xmlString)
        {
            PollPacket pollPacket = new PollPacket();
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PollPacket));
                MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                pollPacket = (PollPacket)xmlSerializer.Deserialize(memoryStream);

                foreach (Poll poll in pollPacket.pollSession.polls)
                {
                    foreach (Choice choice in poll.choices)
                    {
                        choice.parent = poll;
                    }
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            
            return pollPacket;
        }

        public static String SerializePacket(PollPacket pollPacket)
        {
            String XmlizedString = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(PollPacket));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                xs.Serialize(xmlTextWriter, pollPacket);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = Encoding.ASCII.GetString(memoryStream.ToArray());   
            }
            catch (Exception exception)
            {
                return null;
            }
            return XmlizedString;
        }
        
    }
}
