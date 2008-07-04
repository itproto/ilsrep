package ilsrep.poll.server;

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
            InputStream is = socket.getInputStream();
            OutputStream os = socket.getOutputStream();

            // TODO: Fix :)

            is.close();
            os.close();
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
