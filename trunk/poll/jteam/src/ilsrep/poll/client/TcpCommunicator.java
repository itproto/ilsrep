package ilsrep.poll.client;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;
import java.io.StringReader;
import java.net.Socket;
import java.net.UnknownHostException;

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

        System.out.println("Connecting to " + serverIp + " on "
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
        ;
        try {
            super.finalize();
        }
        catch (Throwable e) {
        }
        ;
    }

    /**
     * Retreives XML from server. Asks for the poll id, sends XML request,
     * receives byte data from server and converts it to a Reader object.
     * 
     * @return xmlBuffered received XML from server
     * 
     * @see TcpCommunicator
     */
    public Reader getXML() {
        InputStream inFromServer = null;
        String xmlItself = "";
        Reader xmlBuffered = null;
        try {
            // Generating input and output streams

            BufferedReader consoleInputReader = new BufferedReader(
                    new InputStreamReader(System.in));
            DataOutputStream outToServer = new DataOutputStream(clientSocket
                    .getOutputStream());
            inFromServer = clientSocket.getInputStream();

            xmlItself = "";
            System.out.println("Enter ID number of the desired poll:");
            String id = consoleInputReader.readLine();
            // sending request
            outToServer.writeUTF("<getPollSession><pollSessionId>" + id
                    + "</pollSessionId></getPollSession> \n");
            System.out.println("Receiving XML...");

            String buffer;

            BufferedReader inputReader = new BufferedReader(
                    new InputStreamReader(inFromServer));
            // Getting and parsing request. Reading line because
            // for some reason m test server returned first line empty, and the
            // output started from second line.
            inputReader.readLine();
            boolean eternal = true;
            try {
                while (eternal) {
                    buffer = inputReader.readLine();
                    if (buffer.indexOf("-1") != -1)
                        break;

                    xmlItself = xmlItself + "\n" + buffer;
                    System.out.println(buffer);
                    if (buffer.indexOf("/pollses") != -1)
                        break;
                }
            }
            catch (Exception m) {

                System.out.println("XML Received.. preparing poll");
            }

            ;

            System.out.println(xmlItself);
            // Making Reader out of string (needed for marshaller)
            xmlBuffered = new StringReader(xmlItself);
        }
        catch (Exception e) {
            System.out.println("ExCePtIoN");
            e.printStackTrace();
            try {
                BufferedReader consoleInputReader = new BufferedReader(
                        new InputStreamReader(System.in));
                consoleInputReader.readLine();
            }
            catch (Exception exception) {
            }
            ;
            ;
        }
        ;
        // returning reader
        return xmlBuffered;

    }

    /**
     * Sends xml to server.
     * 
     * @param genXml
     *            Xml, as <code>String</code>.
     */
    public void sendXml(String genXml) {
        // TKOST: I commented out variables that are not used, to make it don't
        // generate warnings on compilation.

        // InputStream inFromServer = null;
        // String xmlItself = "";
        // Reader xmlBuffered = null;
        try {
            // Generating input and output streams

            // BufferedReader consoleInputReader = new BufferedReader(
            // new InputStreamReader(System.in));
            DataOutputStream outToServer = new DataOutputStream(clientSocket
                    .getOutputStream());
            // inFromServer = clientSocket.getInputStream();

            outToServer.writeUTF(genXml);
            System.out.println("XML sent to server.");
        }
        catch (Exception e) {
            System.out.println("ExCePtIoN");
            e.printStackTrace();
            try {
                BufferedReader consoleInputReader = new BufferedReader(
                        new InputStreamReader(System.in));
                consoleInputReader.readLine();
            }
            catch (Exception exception) {
            }
            ;
        }

    }

    /**
     * Retrieves and outputs XML IDs and names
     * 
     * 
     * 
     */
    public void listXml() {
        System.out.println("Getting list of polls...");
        boolean allOk = false;
        while (!allOk) {
            try {
                allOk = true;
                DataOutputStream outToServer = new DataOutputStream(
                        clientSocket.getOutputStream());
                BufferedReader inputReader = new BufferedReader(
                        new InputStreamReader(clientSocket.getInputStream()));
                outToServer.writeUTF("LIST\n");
                inputReader.readLine();
                String buffer = "";
                String output = "";
                while (!((buffer = inputReader.readLine()).equals("END")))
                    output += buffer;
                System.out.println(output);
            }
            catch (Exception e) {
                System.out
                        .println("Wrong response from server...Press ENTER to retry");
                try {
                    BufferedReader consoleInputReader = new BufferedReader(
                            new InputStreamReader(System.in));
                    consoleInputReader.readLine();
                }
                catch (Exception exception) {
                }
                allOk = false;
            }

        }

    }

}
