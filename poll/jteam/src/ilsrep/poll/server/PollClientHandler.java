package ilsrep.poll.server;

import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.Pollpacket;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.common.protocol.Request;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.StringReader;
import java.io.StringWriter;
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

        String exceptionMessage = null;

        try {
            while (true) {
                if (socket.isClosed())
                    break;

                Pollpacket receivedPacket = null;
                receivedPacket = receivePacket();

                if (receivedPacket.getRequest() != null) {
                    if (receivedPacket.getRequest().getType().compareTo(
                            Request.TYPE_SAVE_RESULT) == 0) {
                        if (receivedPacket.getResultsList() != null)
                            serverInstance.getDB().saveResults(
                                    receivedPacket.getResultsList());
                    }
                    else
                        if (receivedPacket.getRequest().getType().compareTo(
                                Request.TYPE_LIST) == 0) {
                            Pollsessionlist list = serverInstance.getDB()
                                    .getPollsessionlist();

                            Pollpacket packetForSending = new Pollpacket();
                            packetForSending.setPollsessionList(list);
                            sendPacket(packetForSending);
                            logger.info("Sent pollsession list("
                                    + list.getItems().size()
                                    + " elements) to client("
                                    + generateHostPortAsText(socket) + ").");
                        }
                        else
                            if (receivedPacket.getRequest().getType()
                                    .compareTo(Request.TYPE_POLLXML) == 0) {
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

                                // Sending empty packet if pollsession not found
                                // in
                                // DB.
                                if (session != null) {
                                    packetForSending.setPollsession(session);
                                    logger.info("Sent pollsession(id = "
                                            + session.getId() + ") to client("
                                            + generateHostPortAsText(socket)
                                            + ").");
                                }
                                else {
                                    logger.warn("Client asked for invalid id.");
                                }

                                sendPacket(packetForSending);
                            }
                            else
                                if (receivedPacket
                                        .getRequest()
                                        .getType()
                                        .compareTo(
                                                Request.TYPE_CREATE_POLLSESSION) == 0) {
                                    if (receivedPacket.getPollsession() != null) {
                                        int storedId = serverInstance
                                                .getDB()
                                                .storePollsession(
                                                        receivedPacket
                                                                .getPollsession());
                                        if (storedId != -1)
                                            logger
                                                    .info("Editors("
                                                            + generateHostPortAsText(socket)
                                                            + ") xml stored with id = "
                                                            + storedId + ".");
                                        else
                                            logger
                                                    .warn("Error while saving xml from editor("
                                                            + generateHostPortAsText(socket)
                                                            + ").");
                                    }
                                }
                                else
                                    if (receivedPacket
                                            .getRequest()
                                            .getType()
                                            .compareTo(
                                                    Request.TYPE_REMOVE_POLLSESSION) == 0) {
                                        if (receivedPacket.getRequest().getId() != null) {
                                            serverInstance
                                                    .getDB()
                                                    .removePollsession(
                                                            receivedPacket
                                                                    .getRequest()
                                                                    .getId());
                                            logger.info("Pollsession id = "
                                                    + receivedPacket
                                                            .getRequest()
                                                            .getId()
                                                    + " deleted.");
                                        }
                                    }
                                    else
                                        if (receivedPacket.getRequest()
                                                .getType().compareTo(
                                                        Request.TYPE_USER) == 0) {
                                            if ((receivedPacket.getUser()
                                                    .getAction().equals("new"))) {
                                                serverInstance
                                                        .getDB()
                                                        .createUser(
                                                                receivedPacket
                                                                        .getUser()
                                                                        .getUserName(),
                                                                receivedPacket
                                                                        .getUser()
                                                                        .getPass());
                                                Pollpacket packetForSending = new Pollpacket();
                                                packetForSending
                                                        .setUser(receivedPacket
                                                                .getUser());
                                                                                                                sendPacket(packetForSending);
                                                logger
                                                        .info("User created. Name = "
                                                                + receivedPacket
                                                                        .getUser()
                                                                        .getUserName());
                                            }
                                            else {
                                                if ((receivedPacket.getUser()
                                                        .getAction()
                                                        .equals("exist"))) {
receivedPacket.getUser().setAction(serverInstance.getDB().checkUser(receivedPacket.getUser().getUserName()).equals("true")? "exist" :"new" );
                                                    Pollpacket packetForSending = new Pollpacket();
                                                    packetForSending
                                                            .setUser(receivedPacket
                                                                    .getUser());
                                                    sendPacket(packetForSending);
                                                    logger
                                                            .info("Client asked if user(name = "
                                                                    + packetForSending
                                                                            .getUser()
                                                                            .getUserName()
                                                                    + ") exist. Result sent: "
                                                                    + packetForSending
                                                                            .getUser()
                                                                            .getExist());
                                                }
                                                else {
        receivedPacket.getUser().setAction(serverInstance.getDB().authUser(receivedPacket.getUser().getUserName(),receivedPacket.getUser().getPass()).equals("true")? "auth" : "exist");
                                                    Pollpacket packetForSending = new Pollpacket();
                                                    packetForSending
                                                            .setUser(receivedPacket
                                                                    .getUser());
                                                    sendPacket(packetForSending);
                                                    logger
                                                            .info("User(name = "
                                                                    + packetForSending
                                                                            .getUser()
                                                                            .getUserName()
                                                                    + ") login "
                                                                    + ((packetForSending
                                                                            .getUser()
                                                                            .getAuth()
                                                                            .compareTo(
                                                                                    "true") == 0) ? "successed"
                                                                            : "failed")
                                                                    + ".");
                                                }
                                            }
                                        }
                                        else
                                            if (receivedPacket
                                                    .getRequest()
                                                    .getType()
                                                    .compareTo(
                                                            Request.TYPE_UPDATE_POLLSESSION) == 0) {
                                                if (receivedPacket.getRequest()
                                                        .getId() != null
                                                        && receivedPacket
                                                                .getPollsession() != null) {
                                                    serverInstance
                                                            .getDB()
                                                            .updatePollsession(
                                                                    receivedPacket
                                                                            .getRequest()
                                                                            .getId(),
                                                                    receivedPacket
                                                                            .getPollsession());
                                                }
                                            }

                }
            }
        }
        catch (JAXBException e) {
            // Serialising problems or JAXB init problems(no Pollpacket class in
            // classpath or corrupted).
            exceptionMessage = e.getMessage();
        }
        catch (IOException e) {
            // On network I/O problems.
            exceptionMessage = e.getMessage();
        }
        catch (SQLException e) {
            // On DB problems.
            exceptionMessage = e.getMessage();
        }
        catch (Exception e) {
            // On other exceptions.
            exceptionMessage = e.getMessage();
        }
        finally {
            if (exceptionMessage != null)
                logger.warn("Exception occured: " + exceptionMessage);

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
    public static synchronized Pollpacket receivePacket(InputStream inStream)
            throws JAXBException, IOException {
        Pollpacket receivedPacket = null;

        JAXBContext pollPacketContext = JAXBContext
                .newInstance(Pollpacket.class);

        StringBuffer inputBuffer = new StringBuffer();

        final int bufferSize = 64 * 1024;
        byte[] buffer = new byte[bufferSize];

        while (true) {
            int bytesRead = inStream.read(buffer, 0, bufferSize);

            if (bytesRead > 0) {
                String s = new String(buffer, 0, bytesRead);

                inputBuffer.append(s);
            }

            if (inStream.available() == 0)
                break;
        }

        Unmarshaller um = pollPacketContext.createUnmarshaller();
        StringReader reader = new StringReader(inputBuffer.toString().trim());
logger.warn(inputBuffer.toString());
        receivedPacket = (Pollpacket) um.unmarshal(reader);

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
    public static synchronized void sendPacket(OutputStream outStream,
            Pollpacket packet) throws JAXBException, IOException {
        JAXBContext pollPacketContext = JAXBContext
                .newInstance(Pollpacket.class);
        Marshaller mr = pollPacketContext.createMarshaller();

        StringWriter wr = new StringWriter();

        mr.marshal(packet, wr);
        logger.warn("SENDING "+wr.toString());
        outStream.write(wr.toString().getBytes());
        // JAXBContext pollPacketContext = JAXBContext
        // .newInstance(Pollpacket.class);
        //
        // Marshaller mr = pollPacketContext.createMarshaller();
        //
        // mr.marshal(packet, outStream);
        //
        // BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(
        // outStream));
        // writer.newLine();
        //
        // outStream.flush();
    }

}
