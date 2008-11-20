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
        /// Deserializes survey from XML string into Survey object
        /// </summary>
        /// <param name="xmlString">XML string, which holds serialized Survey object</param>
        /// <returns>Deserialized Survey object</returns>
        public static Survey DeserializeSurvey(string xmlString)
        {
            Survey survey = new Survey();
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Survey));
                MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                survey = (Survey)xmlSerializer.Deserialize(memoryStream);

                foreach (Poll poll in survey.Polls)
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
            return survey;
        }

        /// <summary>
        /// Serializes survey from Survey object into XML string
        /// </summary>
        /// <param name="survey">Survey oject</param>
        /// <returns>XML string, which holds serialized Survey object</returns>
        public static String SerializeSurvey(Survey survey)
        {
            String XmlizedString = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(Survey));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                xs.Serialize(xmlTextWriter, survey);
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
            Survey.isSerialized = true;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PollPacket));
                MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                pollPacket = (PollPacket)xmlSerializer.Deserialize(memoryStream);

                if (pollPacket.survey != null)
                {
                    foreach (Poll poll in pollPacket.survey.Polls)
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
                Survey.isSerialized = false;
                return null;
            }

            Survey.isSerialized = false;
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
