package ilsrep.poll.server;
import java.io.* ;
import java.net.*;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;

import org.apache.log4j.Logger;

/**
 * This class handles communication with poll client.
 * 
 * @author TKOST
 * 
 */
public class PollClientHandler implements ClientHandler, Runnable {

    /**
     * Log4j Logger for this class.
     */
    private static Logger logger = Logger.getLogger(PollClientHandler.class);

    /**
     * Socket with connected client.
     */
    protected Socket socket = null;

    /**
     * Server instance, where poll xml's are kept.
     */
    protected PollServer serverInstance = null;

    /**
     * @see ClientHandler#handle(Socket, PollServer)
     */
    @Override
    public void handle(Socket socket, PollServer serverInstance) {
        this.socket = socket;
        this.serverInstance = serverInstance;

        (new Thread(this)).start();
    }

    /**
     * @see java.lang.Runnable#run()
     */
    @Override
    public void run() {
        logger.info("Client connected from IP / port: "
                + socket.getInetAddress().toString() + " / " + socket.getPort()
                + ". Using local port " + socket.getLocalPort());
        try {
             BufferedReader inputReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            DataOutputStream outToServer = new DataOutputStream(socket.getOutputStream());
            String buffer;
 	buffer=inputReader.readLine();
 	
 	int indexString=buffer.indexOf("<getPollSession><pollSessionId>");
 	indexString=buffer.indexOf(">",indexString+20);
 String pollId=buffer.substring(indexString+1,indexString+2);
 Pollsession pollSession=this.serverInstance.getPollsessionById(pollId);
 
  // logger.info(buffer);
            		//outToServer.writeUTF(pollId);
            		buffer=inputReader.readLine();
            		buffer=inputReader.readLine();
            		buffer=inputReader.readLine();
        }
        catch (IOException e) {
            logger.warn("I/O exception. Closing connection.");
        }
        finally {
            try {
                socket.close();
            }
            catch (IOException e) {
                logger.warn("I/O exception. Closing connection.");
            }
            serverInstance.removeConnection(socket);
        }
    }

}
