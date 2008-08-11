package ilsrep.poll.server;

import java.net.Socket;

/**
 * This is base interface for class that can handle communication with client in
 * concurrent thread.
 * 
 * @author TKOST
 * 
 */
public interface ClientHandler {

    /**
     * Starts communication with client, connected to socket(in concurrent
     * thread).
     * 
     * @param socket
     *            Socket, to process.
     * @param serverInstance
     *            Instance of PollServer that holds poll xml's.
     */
    public void handle(Socket socket, PollServer serverInstance);

}
