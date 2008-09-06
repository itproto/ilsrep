package ilsrep.poll.client;

import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.Answers;
import ilsrep.poll.common.protocol.Pollpacket;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.common.protocol.Request;
import ilsrep.poll.common.protocol.User;
import ilsrep.poll.server.PollClientHandler;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StringReader;
import java.net.Socket;
import java.net.UnknownHostException;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;

/**
 * Handles TCP socket connection with the server
 * 
 * @author DRC
 * 
 */
public class TcpCommunicator {

    /**
     * Default server name to connect to.
     */
    public static final String DEFAULT_SERVER = "127.0.0.1";

    /**
     * Default port to connect to.
     */
    public static final int DEFAULT_PORT = 3320;

    /**
     * Stores received XML data
     * 
     * @see TcpCommunicator
     * 
     */
    protected String receivedXml;

    /**
     * Stores the ID of selected poll
     * 
     * @see TcpCommunicator
     * 
     */
    protected String pollId;

    /**
     * Stores data that may be sent to server. For future versions.
     * 
     * @see TcpCommunicator
     * 
     */
    protected String answers;

    /**
     * Stores POLL server IP
     * 
     * @see TcpCommunicator
     * 
     */
    protected String serverIp = null;

    /**
     * Stores POLL server port
     * 
     * @see TcpCommunicator
     * 
     */
    int port = -1;

    /**
     * Data connection socket.
     * 
     * @see TcpCommunicator
     * 
     */
    Socket clientSocket = null;

    /**
     * Constructs <code>TcpCommunicator</code>, that connects to
     * localhost(127.0.0.1). port 3320.
     * 
     * @throws IOException
     *             If I/O exception occurs.
     * @throws UnknownHostException
     *             If host is unknown(shoundn't be).
     * 
     * @see TcpCommunicator
     * 
     */
    public TcpCommunicator() throws UnknownHostException, IOException {
        this(DEFAULT_SERVER, DEFAULT_PORT);
    }

    /**
     * Constructs <code>TcpCommunicator</code>, that connects to specified
     * server and port.
     * 
     * @param serverIP
     *            Server to connect.
     * @param port
     *            Port to connect.
     * @throws IOException
     *             If I/O exception occurs.
     * @throws UnknownHostException
     *             If host is unknown.
     */
    public TcpCommunicator(String serverIP, int port)
            throws UnknownHostException, IOException {
        this.serverIp = serverIP;
        this.port = port;

        clientSocket = new Socket(serverIp, port);
        System.out.println("Connected!");
    }

    /**
     * Making sure that the socket is closed before the object is gced.
     * Virtually a destructor.
     * 
     * @see TcpCommunicator
     * 
     */
    public void finalize() {
        try {
            clientSocket.close();
        }
        catch (Exception e) {
        }
        try {
            super.finalize();
        }
        catch (Throwable e) {
        }
    }

    /**
     * Retreives XML from server(for console usage). Asks for the poll id, sends
     * XML request, receives byte data from server and converts it to a Reader
     * object.
     * 
     * @return xmlBuffered received XML from server
     * @throws IOException
     *             On network I/O errors.
     * @throws JAXBException
     *             On corrupted output from server.
     * 
     * @see TcpCommunicator
     */
    public Pollsession getXML() throws JAXBException, IOException {
        String id = PollClient
                .readFromConsole("\nEnter ID number of the desired poll");

        return getPollsession(id);
    }

    /**
     * Retrieves pollsession from server by id.
     * 
     * @param id
     *            Id of pollsession to retrieve.
     * 
     * @return Received XML from server.
     * @throws IOException
     *             On network I/O errors.
     * @throws JAXBException
     *             On corrupted output from server.
     */
    public Pollsession getPollsession(String id) throws JAXBException,
            IOException {
        // Forming request packet.
        Request pollxmlRequest = new Request();
        pollxmlRequest.setType(Request.TYPE_POLLXML);
        pollxmlRequest.setId(id);
        Pollpacket requestPacket = new Pollpacket();
        requestPacket.setRequest(pollxmlRequest);

        // Sending request and getting response.
        Pollpacket responsePacket = null;
        PollClientHandler.sendPacket(clientSocket.getOutputStream(),
                requestPacket);
        responsePacket = PollClientHandler.receivePacket(clientSocket
                .getInputStream());

        return (responsePacket != null) ? responsePacket.getPollsession()
                : null;
    }

    /**
     * Sends xml to server.
     * 
     * @param genXml
     *            Xml, as <code>String</code>.
     */
    public void sendXml(String genXml) {
        try {
            StringReader xmlReader = new StringReader(genXml);

            JAXBContext pollPacketCont = JAXBContext
                    .newInstance(Pollsession.class);

            Unmarshaller um = pollPacketCont.createUnmarshaller();

            Pollsession sessionToSend = (Pollsession) um.unmarshal(xmlReader);

            sendPollsession(sessionToSend);
        }
        catch (Exception e) {
            System.out.println("Error while sending xml to server.");
            e.printStackTrace();
            try {
                BufferedReader consoleInputReader = new BufferedReader(
                        new InputStreamReader(System.in));
                consoleInputReader.readLine();
            }
            catch (Exception exception) {
            }
        }

    }

    /**
     * Sends pollsession to server.
     * 
     * @param sessToSend
     *            Pollsession as object to send.
     * @throws IOException
     * @throws JAXBException
     */
    public void sendPollsession(Pollsession sessToSend) throws JAXBException,
            IOException {
        Pollpacket packetToSend = new Pollpacket();
        Request saveRequest = new Request();
        saveRequest.setType(Request.TYPE_CREATE_POLLSESSION);
        packetToSend.setRequest(saveRequest);
        packetToSend.setPollsession(sessToSend);

        PollClientHandler.sendPacket(clientSocket.getOutputStream(),
                packetToSend);
    }

    /**
     * Retrieves and outputs XML IDs and names
     */
    public Pollsessionlist listXml() {
        // Forming request packet.
        Pollpacket requestPacket = new Pollpacket();
        Request request = new Request();
        request.setType(Request.TYPE_LIST);
        requestPacket.setRequest(request);

        try {
            // Sending list request.
            PollClientHandler.sendPacket(clientSocket.getOutputStream(),
                    requestPacket);

            // Receiving response.
            Pollpacket response = PollClientHandler.receivePacket(clientSocket
                    .getInputStream());

            // Processing.
            return response.getPollsessionList();
        }
        catch (JAXBException e) {
            System.out.println(e.getMessage());
            return null;
        }
        catch (IOException e) {
            System.out.println(e.getMessage());
            return null;
        }
    }

    public void sendResult(Answers ans) {
        try {
            Pollpacket packet = new Pollpacket();

            Request saveRequest = new Request();
            saveRequest.setType(Request.TYPE_SAVE_RESULT);

            packet.setRequest(saveRequest);
            packet.setResultsList(ans);

            PollClientHandler
                    .sendPacket(clientSocket.getOutputStream(), packet);
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void deleteXml(String id) {
        Request deleteRequest = new Request();
        deleteRequest.setType(Request.TYPE_REMOVE_POLLSESSION);
        deleteRequest.setId(id);

        Pollpacket deletePacket = new Pollpacket();
        deletePacket.setRequest(deleteRequest);

        try {
            PollClientHandler.sendPacket(clientSocket.getOutputStream(),
                    deletePacket);

            clientSocket.getOutputStream().flush();
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

    public User sendUser(User user) {
        try {
            Pollpacket packet = new Pollpacket();

            Request saveRequest = new Request();
            saveRequest.setType(Request.TYPE_USER);

            packet.setRequest(saveRequest);
            packet.setUser(user);
            PollClientHandler
                    .sendPacket(clientSocket.getOutputStream(), packet);
            Pollpacket response = PollClientHandler.receivePacket(clientSocket
                    .getInputStream());
            user = response.getUser();

        }
        catch (Exception e) {
            e.printStackTrace();
        }

        return user;
    }

    /**
     * Edits pollsession on server.
     * 
     * @param id
     *            Id of pollsession to edit.
     * @param session
     *            New session data.
     */
    public void editPollsession(String id, Pollsession session) {
        try {
            Pollpacket packet = new Pollpacket();

            Request editRequest = new Request();
            editRequest.setType(Request.TYPE_UPDATE_POLLSESSION);
            editRequest.setId(id);

            packet.setRequest(editRequest);
            packet.setPollsession(session);

            PollClientHandler
                    .sendPacket(clientSocket.getOutputStream(), packet);
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

}
