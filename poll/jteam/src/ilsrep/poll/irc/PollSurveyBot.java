package ilsrep.poll.irc;

import ilsrep.poll.common.protocol.Pollsessionlist;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.List;
import java.util.Vector;

import javax.xml.namespace.QName;

import org.jibble.pircbot.IrcException;
import org.jibble.pircbot.NickAlreadyInUseException;
import org.jibble.pircbot.PircBot;

import webservice.endpoint.WebJPoll_Service;

/**
 * IRC bot for passing poll surveys on IRC channels.
 * 
 * @author TKOST
 * 
 */
public class PollSurveyBot extends PircBot {

    // Connect settings.
    /**
     * Server to connect to.
     */
    protected String server = null;

    /**
     * Port to connect to.
     */
    protected int port = -1;

    /**
     * Nick to connect with.
     */
    protected String nick = null;

    /**
     * List of channels to connect to.
     */
    protected List<String> channels = null;

    // Runtime settings.
    /**
     * "Command prefix".<br>
     * Default is <code>~</code>.
     */
    protected String commandPrefix = "~";

    /**
     * Webservice for Poll Survey operations.
     */
    protected String webserviceURL = null;

    // Other options.
    /**
     * Webservice to work with poll survey data.
     */
    protected WebJPoll_Service webservice = null;

    /**
     * @param server
     *            Server to connect to.
     * @param port
     *            Port to connect to.
     * @param nick
     *            Nick to connect with.
     * @param channels
     *            List of channels to connect to.
     * @throws IrcException
     *             On IrcException.
     * @throws IOException
     *             On I/O errors.
     * @throws NickAlreadyInUseException
     *             When current nick is already in use.
     */
    public PollSurveyBot(String server, int port, String nick,
            List<String> channels) throws NickAlreadyInUseException,
            IOException, IrcException {
        this.server = server;
        this.port = port;
        this.nick = nick;
        this.channels = channels;

        connect();
    }

    /**
     * Connects to server.
     * 
     * @throws IrcException
     *             On IrcException.
     * @throws IOException
     *             On I/O errors.
     * @throws NickAlreadyInUseException
     *             When current nick is already in use.
     */
    protected void connect() throws NickAlreadyInUseException, IOException,
            IrcException {
        setLogin(nick);
        setFinger(nick);
        changeNick(nick);

        connect(server, 6667);

        changeNick(nick);

        for (String channel : channels)
            joinChannel(channel);
    }

    /**
     * @see org.jibble.pircbot.PircBot#onMessage(java.lang.String,
     *      java.lang.String, java.lang.String, java.lang.String,
     *      java.lang.String)
     */
    @Override
    protected void onMessage(String channel, String sender, String login,
            String hostname, String message) {
        if (sender.equals(nick))
            return;

        if (message.charAt(0) == commandPrefix.charAt(0))
            message = message.substring(1);
        else
            return;

        String[] messageParts = message.split(" ");

        try {
            if (messageParts[0].equals("help"))
                sendMessage(
                        channel,
                        "This is ILS Poll Survey Bot! Go to http://code.google.com/p/ilsrep/ for project details.");
            else if (messageParts[0].equals("list")) {
                Pollsessionlist list = getService().getWebJPollPort()
                        .getPollsessionlist();

                sendMessage(channel, "Poll Surveys list:");
                for (int i = 0; i < list.getItems().size(); i++) {
                    sendMessage(channel, (i + 1) + ") "
                            + list.getItems().get(i).getName());
                }
            }
            else if (messageParts[0].equals("set") && messageParts.length > 2) {
                if (messageParts[1].equals("webserviceURL")) {
                    webserviceURL = messageParts[2];
                    getService();
                }
                else if (messageParts[1].equals("commandPrefix")) {
                    commandPrefix = "" + messageParts[2].charAt(0);
                }
            }
            else if (messageParts[0].equals("quit")) {
                quitServer("ILS Poll Survey Bot stopped.");
                dispose();
            }
        }
        catch (Exception e) {
            sendMessage(channel, e.getClass().toString() + ": "
                    + e.getMessage());
        }
    }

    /**
     * Creates webservice conncetion, if not created and returns it.
     * 
     * @return Connected webservice.
     * @throws MalformedURLException
     *             When URL is malformed.
     */
    protected WebJPoll_Service getService() throws MalformedURLException {
        if (webservice == null) {
            if (webserviceURL == null)
                throw new MalformedURLException("Webservice URL is null!");

            QName serviceName = new QName("http://endpoint.webservice/",
                    "WebJPoll");
            URL url = new URL(webserviceURL);
            webservice = new WebJPoll_Service(url, serviceName);
        }

        return webservice;
    }

    /**
     * Launches <code>PollSurveyBot</code> and for now connects to
     * <code>irc.freenode.org:6667</code> with name <code>PollSurveyBot</code>
     * to channel <code>#ils</code>.
     * 
     * @param args
     */
    public static void main(String[] args) throws NickAlreadyInUseException,
            IOException, IrcException {
        List<String> channels = new Vector<String>();
        channels.add("#ils");

        new PollSurveyBot("irc.freenode.org", 6667, "PollSurveyBot", channels);
    }

}
