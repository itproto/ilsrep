using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace KioskBrowser
{
    public partial class ClientPage : System.Web.UI.Page
    {
        public string selectedPath = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            if (Request["address"] != null)
            {
                Response.Write(getAddress(Request["address"]));
                //Response.Write(addressString.Text);
                Response.End();
                //Response.Close();
            }
            else
            {
                mainFrame.Attributes["src"] = "ClientPage.aspx?address=" + addressString.Text;
            }
            */

            // Load iframe page
            string httpString = addHTTP(addressString.Text);
            addressString.Text = httpString;
            mainFrame.Attributes["src"] = httpString;

            selectedPath = programsList.SelectedValue;
            RefreshProgramsList();
        }

        public void RefreshProgramsList()
        {
            // Manually refresh programs list
            programsList.Items.Clear();
            ListItem listItem1 = new ListItem("-----Select program-----", null);
            programsList.Items.Add(listItem1);
            ListItem listItem2 = new ListItem("Calculator", "C:\\WINDOWS\\system32\\calc.exe");
            programsList.Items.Add(listItem2);
            ListItem listItem3 = new ListItem("Paint", "C:\\WINDOWS\\system32\\mspaint.exe");
            programsList.Items.Add(listItem3);
        }

        /// <summary>
        /// Add "http://" to beginning of the string if it's necessary
        /// </summary>
        /// <param name="address">Address string</param>
        /// <returns>String with "http://" in the beginning</returns>
        public string addHTTP(string address)
        {
            if (String.Compare("http://", 0, address, 0, 7, true) == 0)
            {
                return address;
            }
            else
            {
                return "http://" + address;
            }
        }

        public void RunButtonClick(object sender, EventArgs e)
        {
            try
            {
                Process newProcess = new Process();
                string newDocumentsPath = selectedPath;
                newProcess.StartInfo.FileName = newDocumentsPath;
                newProcess.Start();
            }
            catch (Exception exception)
            {

            }
        }

        public void LogoutButtonClick(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        /*
        public string getAddress(string address)
        {
            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create(address);
            // If required by the server, set the credentials.
            //request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            //Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);
            Regex.Replace(responseFromServer, "href=\"(.+?)\"", "href=\"ClientPage.aspx?address=$1\"");
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }
        */
    }
}
