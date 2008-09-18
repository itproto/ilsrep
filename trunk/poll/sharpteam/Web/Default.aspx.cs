using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common;

public partial class _Default : System.Web.UI.Page
{
    /// <summary>
    /// host to which connect
    /// </summary>
    public const string HOST = "localhost";
    /// <summary>
    /// port to which connect
    /// </summary>
    public const int PORT = 3320;
    /// <summary>
    /// handles connection to server
    /// </summary>
    private TcpClient client = new TcpClient();
    public static List<Item> pollSessionsList = new List<Item>();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            client.Connect(HOST, PORT);
        }
        catch (Exception exception)
        {

        }

        if (client.isConnected)
        {
            pollSessionsList = GetPollSessionsList();
            client.Disconnect();
        }
        else
        {
            pollSessionsList = null;
        }       
    }

    /// <summary>
    /// Function sends request, receive PollPacket and check if receivedPacket == null. If true, user can retry to receive Packet, else function returns receivedPacket
    /// </summary>
    /// <param name="sendPacket">PollPacket with request to send</param>
    /// <returns>PollPacket receivedPacket</returns>
    private PollPacket ReceivePollPacket(PollPacket sendPacket)
    {
        try
        {
            string sendString = PollSerializator.SerializePacket(sendPacket);
            client.Send(sendString);
        }
        catch (Exception)
        {
            return null;
        }

        string receivedString = client.Receive();
        PollPacket receivedPacket = new PollPacket();
        receivedPacket = PollSerializator.DeserializePacket(receivedString);

        // Check if received data is correct
        if (receivedPacket == null)
        {
            return null;
        }
        return receivedPacket;
    }

    private List<Item> GetPollSessionsList()
    {
        // Get list of pollsessions
        PollPacket pollPacket = new PollPacket();
        pollPacket.user = new User();
        pollPacket.user.username = "test";
        pollPacket.user.password = "123456";
        pollPacket = ReceivePollPacket(pollPacket);

        if (pollPacket.user.auth)
        {
            pollPacket.request = new Request();
            pollPacket.request.type = Ilsrep.PollApplication.Communication.Request.GET_LIST;
            pollPacket = ReceivePollPacket(pollPacket);

            if (pollPacket != null)
            {
                return pollPacket.pollSessionList.items;
            }
        }

        return null;
    }
}
