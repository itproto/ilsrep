package ilsrep.poll.server;

import ilsrep.poll.common.Pollpacket;
import ilsrep.poll.common.Pollsession;
import ilsrep.poll.common.Pollsessionlist;
import ilsrep.poll.common.Request;

import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.StringReader;
import java.net.Socket;
import java.sql.SQLException;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;

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
                + generateHostPortAsText(socket));

        try {
            while (true) {
                if (socket.isClosed())
                    break;

                Pollpacket receivedPacket = null;
                receivedPacket = receivePacket();

                if (receivedPacket.getRequest() != null) {
                    if (receivedPacket.getRequest().getType().compareTo(
                            Request.TYPE_LIST) == 0) {
                        Pollsessionlist list = serverInstance.getDB()
                                .getPollsessionlist();

                        Pollpacket packetForSending = new Pollpacket();
                        packetForSending.setPollsessionList(list);
                        sendPacket(packetForSending);
                    }
                    else
                        if (receivedPacket.getRequest().getType().compareTo(
                                Request.TYPE_POLLXML) == 0) {
                            Pollsession session = null;
                            try {
                                session = serverInstance
                                        .getPollsessionById(receivedPacket
                                                .getRequest().getId());
                            }
                            catch (NullPointerException e) {
                                // This should be fixed in protocol.
                                session = null;
                            }

                            Pollpacket packetForSending = new Pollpacket();

                            // Sending empty packet if pollsession not found in
                            // DB.
                            if (session != null)
                                packetForSending.setPollsession(session);

                            sendPacket(packetForSending);
                        }
                        else
                            if (receivedPacket.getRequest().getType()
                                    .compareTo(Request.TYPE_CREATE_POLLSESSION) == 0) {
                                if (receivedPacket.getPollsession() != null) {
                                    serverInstance.getDB().storePollsession(
                                            receivedPacket.getPollsession());
                                }
                            }
                }
            }
        }
        catch (JAXBException e) {
            // Serialising problems or JAXB init problems(no Pollpacket class in
            // classpath or corrupted).
        }
        catch (IOException e) {
            // On network I/O problems.
        }
        catch (SQLException e) {
            // On DB problems.
        }
        catch (Exception e) {
            // On other exceptions.
        }
        finally {
            try {
                socket.close();
            }
            catch (IOException e) {
            }
            serverInstance.removeConnection(socket);
        }
    }

    /**
     * Generates string for output from socket's IP and port.
     * 
     * @param socket
     *            Socket to get info from.
     * @return Generated string.
     */
    public static String generateHostPortAsText(Socket socket) {
        return socket.getInetAddress().toString() + ":" + socket.getPort();
    }

    /**
     * Receives packet from this handler's connection.
     * 
     * @return Received packet as object.
     * @throws JAXBException
     *             On errors in stream(broken client input).
     * @throws IOException
     *             On errors in stream(I/O errors).
     */
    private Pollpacket receivePacket() throws JAXBException, IOException {
        return receivePacket(socket.getInputStream());
    }

    /**
     * Sends packet to client.
     * 
     * @param packet
     *            Packet to send.
     * @throws JAXBException
     *             On errors in stream(broken client input).
     * @throws IOException
     *             On errors in stream(I/O errors).
     */
    private void sendPacket(Pollpacket packet) throws JAXBException,
            IOException {
        sendPacket(socket.getOutputStream(), packet);
    }

    /**
     * Serialises packet from given stream.
     * 
     * @param inStream
     *            Stream to serialise to.
     * @return Received packet as object.
     * @throws JAXBException
     *             On errors in stream(broken client input).
     * @throws IOException
     *             On errors in stream(I/O errors).
     */
    public static Pollpacket receivePacket(InputStream inStream)
            throws JAXBException, IOException {
        JAXBContext pollPacketContext = JAXBContext
                .newInstance(Pollpacket.class);

        StringBuffer inputBuffer = new StringBuffer();

        final int bufferSize = 64 * 1024;
        byte[] buffer = new byte[bufferSize];

        while (true) {
            int bytesRead = inStream.read(buffer, 0, bufferSize);

            String s = new String(buffer, 0, bytesRead);

            inputBuffer.append(s);

            if (inStream.available() == 0)
                break;
        }

        Unmarshaller um = pollPacketContext.createUnmarshaller();
        StringReader reader = new StringReader(inputBuffer.toString().trim());

        Pollpacket receivedPacket = (Pollpacket) um.unmarshal(reader);

        return receivedPacket;
    }

    /**
     * De-serialises packet from given stream.
     * 
     * @param outStream
     *            Stream to de-serialise from.
     * @param packet
     *            Packet to send.
     * @throws JAXBException
     *             On errors in stream(broken client input).
     * @throws IOException
     *             On errors in stream(I/O errors).
     */
    public static void sendPacket(OutputStream outStream, Pollpacket packet)
            throws JAXBException, IOException {
        JAXBContext pollPacketContext = JAXBContext
                .newInstance(Pollpacket.class);

        Marshaller mr = pollPacketContext.createMarshaller();

        mr.marshal(packet, outStream);

        BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(
                outStream));
        writer.newLine();

        outStream.flush();
    }

}
