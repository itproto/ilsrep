package ilsrep.poll.server;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.Socket;

import org.apache.log4j.Logger;

/**
 * This class handles communication with poll client.
 * 
 * @author TKOST
 * @author DRC
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
        logger.info("Client connected from IP:port: "
                + socket.getInetAddress().toString() + ":" + socket.getPort());
        try {

            BufferedReader inputReader = new BufferedReader(
                    new InputStreamReader(socket.getInputStream()));
            DataOutputStream outToServer = new DataOutputStream(socket
                    .getOutputStream());
            String buffer = "";
            buffer = inputReader.readLine();
            if (buffer.indexOf("<getPollSession><pollSessionId>") != -1) {
                int indexString = buffer
                        .indexOf("<getPollSession><pollSessionId>");
                indexString = buffer.indexOf(">", indexString + 20);
                int indexStringEnd = buffer.indexOf("<", indexString);
                String pollId = buffer.substring(indexString + 1,
                        indexStringEnd);
                // Pollsession
                // pollSession=this.serverInstance.getPollsessionById(pollId);

                logger.info(pollId);
                // outToServer.writeUTF(pollId);
                if (this.serverInstance.pollFiles.containsKey(pollId)) {
                    File file = this.serverInstance.pollFiles.get(pollId);

                    FileInputStream fis = new FileInputStream(file);
                    int x = fis.available();
                    byte b[] = new byte[x];
                    fis.read(b);
                    String content = new String(b);
                    outToServer.writeUTF(content);

                    outToServer.writeUTF("\n");
                }
                else {
                    outToServer.writeUTF("-1\n");
                    logger.warn("invalid id");

                }

                socket.close();
            }
            else {
                logger.warn("RECEIVING XML");
                String xmlItself = buffer;
                boolean eternal = true;
                while (eternal) {
                    buffer = inputReader.readLine();

                    xmlItself = xmlItself + "\n" + buffer;
                    if (buffer.indexOf("/pollses") != -1)
                        break;
                }

                serverInstance.addPollXML(xmlItself);

                logger.warn(xmlItself);
                logger.warn("XML RECEIVED");

            }
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
