package ilsrep.poll.irc;

import java.io.IOException;
import java.util.List;
import java.util.Vector;

import org.jibble.pircbot.IrcException;
import org.jibble.pircbot.NickAlreadyInUseException;
import org.jibble.pircbot.PircBot;

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

        if (message.equals("~help"))
            sendMessage(channel, "This is ILS Poll Survey Bot!");
        else if (message.equals("~quit"))
            dispose();
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
