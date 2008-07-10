package ilsrep.poll.client;

import ilsrep.poll.common.Poll;
import ilsrep.poll.common.Pollsession;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;
import java.net.UnknownHostException;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;

/**
 * Main class for task 7 - Poll.
 * 
 * @author TKOST
 * @author DCR
 * 
 */
public class PollClient {

    /**
     * Main method for task 7.
     * 
     * @param args
     *            Command line arguments.
     * @throws JAXBException
     *             When xml document had failed to parse by JAXB.
     * @throws IOException
     *             When I/O exception occurs.
     */
    public static void main(String[] args) throws JAXBException, IOException {
        // Greeting user and asking his name and filename of poll xml file.
        BufferedReader consoleInputReader = new BufferedReader(
                new InputStreamReader(System.in));
        System.out.println("Welcome to polls client program!\n");
        System.out.print("Please enter your name: ");
        String name = consoleInputReader.readLine();
        JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller um = cont.createUnmarshaller();
        Pollsession polls = null;
        System.out.print("Use server?(1) or local file?(2): ");

        String yesNoChoice = consoleInputReader.readLine();
        if (!(yesNoChoice.compareTo("1") == 0)) {
            System.out.print("Please enter filename to read poll xml "
                    + "from\n[press enter for default \"xml/Polls.xml\"]: ");
            String fileName = consoleInputReader.readLine();
            if (fileName.compareTo("") == 0)
                fileName = "xml/Polls.xml";

            // Serialising xml file into object model.

            File pollFile = new File(fileName);
            polls = (Pollsession) um.unmarshal(pollFile);
        }
        else {
            System.out.print("Please enter server:port to read poll xml "
                    + "from\n[press enter for default \"127.0.0.1:3320\"]: ");
            String serverPortString = consoleInputReader.readLine();

            boolean done = false;
            while (!done) {
                try {
                    TcpCommunicator communicator = null;

                    if (serverPortString.compareTo("") != 0) {
                        int separatorIndex = serverPortString.indexOf(':');
                        if (separatorIndex != 1) {
                            try {
                                String serverName = serverPortString.substring(
                                        0, separatorIndex);
                                int port = Integer.parseInt(serverPortString
                                        .substring(separatorIndex + 1));

                                communicator = new TcpCommunicator(serverName,
                                        port);
                            }
                            catch (NumberFormatException e) {
                                communicator = null;
                            }
                            catch (UnknownHostException e) {
                                communicator = null;
                            }
                            catch (IOException e) {
                                communicator = null;
                            }
                        }
                    }

                    if (communicator == null)
                        communicator = new TcpCommunicator();
                    communicator.listXml();
                    Reader pollFile = communicator.getXML();
                    communicator.finalize();
                    polls = (Pollsession) um.unmarshal(pollFile);
                    done = true;
                }
                catch (Exception m) {
                    // TODO: This cause client hang when it can't connect to
                    // server.
                    System.out
                            .println("Corrupted output from server. Possibly no such id or corrupted XML. RETRYING...");
                }
            }
        }

        // Showing xml, generated from already read object model.
        Marshaller mr = cont.createMarshaller();
        System.out.println();
        mr.setProperty("jaxb.formatted.output", true);
        mr.marshal(polls, System.out);

        // Processing polls.
        /*
         * AnswerSaver saveElement = new AnswerSaver();
         * saveElement.minScore=Float.parseFloat(polls.getMinScore());
         * saveElement.testMode=polls.getTestMode();
         */
        // New way of saving answers
        String testMode = "false";
        if (polls.getTestMode() != null)
            testMode = polls.getTestMode();
        float minScore = -1;
        try {
            if (polls.getMinScore() != null)
                minScore = Float.parseFloat(polls.getMinScore());
        }
        catch (NumberFormatException e) {
            // If it is wrong in xml - it stays "-1".
        }
        String resultingOutput = "Results \n"; // here we will store everything
        // we will need to output
        System.out.print("\nOk, " + name + ", are you ready for poll? [y/n]");
        yesNoChoice = consoleInputReader.readLine();
        if (!(yesNoChoice.compareTo("y") == 0))
            return;

        String choice = null;
        float i = 0, n = 0;
        for (Poll cur : polls.getPolls()) {
            while (choice == null)
                choice = cur.queryUser();
            // saveElement.pushAnswer(cur.getName(), choice, cur.pass);
            if (testMode.compareTo("true") == 0) {
                resultingOutput += cur.getName() + " => " + choice + "=>"
                        + cur.pass + "\n";
            }
            else {
                resultingOutput += cur.getName() + " => " + choice + "\n";
            }
            if (cur.pass == "PASS")
                i++;
            n++;

            choice = null;
        }
        if (testMode.compareTo("true") == 0) {
            resultingOutput += "Your score " + Float.toString(i / n) + "\n";

            if ((i / n) >= minScore) {
                resultingOutput += "You pass";
            }
            else {
                resultingOutput += "You fail";
            }
        }
        // Showing poll results.
        consoleClearScreen();
        // saveElement.popAnswer();
        System.out.println(resultingOutput);
        consoleInputReader.readLine();
    }

    /**
     * Clears console(prints 40 blank lines).
     */
    public static void consoleClearScreen() {
        for (int i = 0; i < 40; i++)
            System.out.println();
    }

}
