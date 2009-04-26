using System;
using System.Collections;
using System.Configuration;
using System.Data;
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
        protected void Page_Load(object sender, EventArgs e)
        {
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
        }

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
    }
}
