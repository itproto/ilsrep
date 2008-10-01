using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Helpers
{
    /// <summary>
    /// Help to work with settings
    /// </summary>
    [XmlRoot("settings"), Serializable]
    public class SettingsHelper
    {
        /// <summary>
        /// Host
        /// </summary>
        [XmlElement("host")] public string host;
        /// <summary>
        /// Port
        /// </summary>
        [XmlElement("port")] public int port;

        /// <summary>
        /// Load settings from xml file
        /// </summary>
        /// <param name="pathToConfigFile">Path to xml file</param>
        public void LoadSettings(string pathToConfigFile)
        {
            SettingsHelper settings = new SettingsHelper();
            try
            {
                // Load from file
                XmlDocument settingsDoc = new XmlDocument();
                settingsDoc.Load(pathToConfigFile);
                String xmlString = settingsDoc.OuterXml;

                // Deserialize
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsHelper));
                MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);
                settings = (SettingsHelper)xmlSerializer.Deserialize(memoryStream);
            }
            catch (Exception exception)
            {
                host = "localhost";
                port = 3320;
                throw new Exception(exception.Message + Environment.NewLine + "Default settings loaded(localhost:3320)");
            }

            host = settings.host;
            port = settings.port;
        }

        /// <summary>
        /// Save settings to xml file
        /// </summary>
        /// <param name="pathToConfigFile">Path to xml file</param>
        public void SaveSettings(string pathToConfigFile)
        {
            String xmlString = null;
            try
            {
                SettingsHelper settings = new SettingsHelper();
                settings.host = host;
                settings.port = port;

                // Serialize
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(SettingsHelper));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);
                xs.Serialize(xmlTextWriter, settings);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                xmlString = Encoding.ASCII.GetString(memoryStream.ToArray());

                // Save to file
                XmlDocument settingsDoc = new XmlDocument();
                settingsDoc.LoadXml(xmlString); 
                settingsDoc.Save(pathToConfigFile);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to save settings" + Environment.NewLine + exception.Message);
            }
        }
    }
}
