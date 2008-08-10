package ilsrep.poll.client;

import ilsrep.poll.common.Item;
import ilsrep.poll.common.Pollpacket;
import ilsrep.poll.common.Pollsession;
import ilsrep.poll.common.Request;
import ilsrep.poll.server.PollClientHandler;
import ilsrep.poll.common.Answers;
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
    Socket clientSocket;

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
        this("127.0.0.1", 3320);
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

        System.out.println("\nConnecting to " + serverIp + " on "
                + Integer.toString(port));
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
    protected void finalize() {
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
     * Retreives XML from server. Asks for the poll id, sends XML request,
     * receives byte data from server and converts it to a Reader object.
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

            Pollpacket packetToSend = new Pollpacket();
            Request saveRequest = new Request();
            saveRequest.setType(Request.TYPE_CREATE_POLLSESSION);
            packetToSend.setRequest(saveRequest);
            packetToSend.setPollsession(sessionToSend);

            PollClientHandler.sendPacket(clientSocket.getOutputStream(),
                    packetToSend);
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
     * Retrieves and outputs XML IDs and names
     */
    public void listXml() {
        System.out.println("\nGetting List of pollsessions. Please wait.");
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
            if (response.getPollsessionList() != null
                    && response.getPollsessionList().getItems() != null) {
                System.out.println("\nList of pollsessions stored on server:");

                for (Item i : response.getPollsessionList().getItems()) {
                    System.out.println(i.getId() + ") " + i.getName());
                }
            }
            else {
                System.out.println("\nList is empty or server sent no list.");
            }
        }
        catch (JAXBException e) { System.out.println(e.getMessage());
            return;
        }
        catch (IOException e) { System.out.println(e.getMessage());
            return;
        }
    }
    public void sendResult(Answers ans){
	try {Pollpacket packet=new Pollpacket();
	packet.setResultsList(ans);
	  PollClientHandler.sendPacket(clientSocket.getOutputStream(),
                    packet);
} catch(Exception e){e.printStackTrace();};
	
	
	}

}
