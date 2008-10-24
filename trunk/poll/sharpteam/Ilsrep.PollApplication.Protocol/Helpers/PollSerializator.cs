using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;

namespace Ilsrep.PollApplication.Helpers
{
    /// <summary>
    /// Class that serializes objects concering poll
    /// </summary>
    public class PollSerializator
    {
        /// <summary>
        /// Deserializes poll session from XML string into PollSession object
        /// </summary>
        /// <param name="xmlString">XML string, which holds serialized PollSession object</param>
        /// <returns>Deserialized PollSession object</returns>
        public static PollSession DeserializePollSession(string xmlString)
        {
            PollSession pollSession = new PollSession();
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PollSession));
                MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                pollSession = (PollSession)xmlSerializer.Deserialize(memoryStream);

                foreach (Poll poll in pollSession.Polls)
                {
                    foreach (Choice choice in poll.Choices)
                    {
                        choice.parent = poll;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return pollSession;
        }

        /// <summary>
        /// Serializes poll session from PollSession object into XML string
        /// </summary>
        /// <param name="pollSession">Poll Session oject</param>
        /// <returns>XML string, which holds serialized PollSession object</returns>
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
            catch (Exception)
            {
                return null;
            }
            return XmlizedString;
        }
        
        /// <summary>
        /// Deserializes poll packet from XML string into PollPacket object 
        /// </summary>
        /// <param name="xmlString">XML string, which holds serialized PollPacket object</param>
        /// <returns>Deserialized PollPacket object</returns>
        public static PollPacket DeserializePacket(string xmlString)
        {
            PollPacket pollPacket = new PollPacket();
            PollSession.isSerialized = true;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PollPacket));
                MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                pollPacket = (PollPacket)xmlSerializer.Deserialize(memoryStream);

                if (pollPacket.pollSession != null)
                {
                    foreach (Poll poll in pollPacket.pollSession.Polls)
                    {
                        foreach (Choice choice in poll.Choices)
                        {
                            choice.parent = poll;
                        }
                    }
                }
            }
            catch (Exception)
            {
                PollSession.isSerialized = false;
                return null;
            }

            PollSession.isSerialized = false;
            return pollPacket;
        }

        /// <summary>
        /// Serializes poll packet from PollPacket object into XML string
        /// </summary>
        /// <param name="pollPacket">PollPacket object</param>
        /// <returns>XML string, which holds serialized PollPacket object</returns>
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
            catch (Exception)
            {
                return null;
            }
            return XmlizedString;
        }
        
    }
}
