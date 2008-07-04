package ilsrep.poll.server;

import java.io.BufferedWriter;
import java.io.IOException;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.Socket;

/**
 * This class handles communication with poll client.
 * 
 * @author TKOST
 * 
 */
public class PollClientHandler implements ClientHandler, Runnable {

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
        OutputStream os = null;
        try {
            os = socket.getOutputStream();
            BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(
                    os));
            writer.write("Test!\n");
            writer.flush();
            os.close();
        }
        catch (IOException e) {
            System.out.println("ERROR! I/O exception. Closing connection.");
            return;
        }
        finally {
            try {
                socket.close();
            }
            catch (IOException e) {
                System.out.println("ERROR! I/O exception. Closing connection.");
                return;
            }
        }
    }

}
