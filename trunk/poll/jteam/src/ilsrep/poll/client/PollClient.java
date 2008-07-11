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
        System.out.println("Welcome to polls client program!\n");
        // System.out.print("Please enter your name: ");
        String name = readFromConsole("Please enter your name");
        JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller um = cont.createUnmarshaller();
        Pollsession polls = null;
        boolean repeater = true;
        String[] answerSet12 = { "1", "2" };
        String yesNoChoice = readFromConsole(
                "Use server?(1) or local file?(2)", answerSet12);
        // while (repeater) {
        // System.out.print("Use server?(1) or local file?(2): ");
        // yesNoChoice = consoleInputReader.readLine();
        // if (((yesNoChoice.equals("1")) || (yesNoChoice.equals("2"))))
        // repeater = false;
        // }
        if (!(yesNoChoice.compareTo("1") == 0)) {
            // System.out.print("Please enter filename to read poll xml "
            // + "from\n[press enter for default \"xml/Polls.xml\"]: ");
            String fileName = readFromConsole("Please enter filename to read"
                    + " poll xml from\n[press enter for default"
                    + " \"xml/Polls.xml\"]");
            if (fileName.compareTo("") == 0)
                fileName = "xml/Polls.xml";

            // Serialising xml file into object model.
            File pollFile = new File(fileName);
            polls = (Pollsession) um.unmarshal(pollFile);
        }
        else {
            // System.out.print("Please enter server:port to read poll xml "
            // + "from\n[press enter for default \"127.0.0.1:3320\"]: ");
            String serverPortString = readFromConsole("Please enter server:port"
                    + " to read poll xml from\n[press enter for"
                    + " default \"127.0.0.1:3320\"]");

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
                    // TODO: BUG: This cause client hang when it can't connect
                    // to server.
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
        String resultingOutput = "Results\n\n"; // here we will store everything
        // we will need to output
        repeater = true;
        while (repeater) {
            yesNoChoice = readFromConsole("\nOk, " + name
                    + ", are you ready for poll?", Y_N_ANSWER_SET);
            if (((yesNoChoice.equals("y")) || (yesNoChoice.equals("n"))))
                repeater = false;
        }
        if (!(yesNoChoice.compareTo("y") == 0))

            return;

        String choice = null;
        float i = 0, n = 0;
        for (Poll cur : polls.getPolls()) {
            while (choice == null)
                choice = cur.queryUser();
            // saveElement.pushAnswer(cur.getName(), choice, cur.pass);
            if (testMode.compareTo("true") == 0) {
                resultingOutput += cur.getName() + " => " + choice + " ("
                        + cur.pass + ")" + "\n";
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
            // BUG: May happen too long number after comma.
            resultingOutput += "\nYour score " + Float.toString(i / n) + "\n";

            if ((i / n) >= minScore) {
                resultingOutput += "You pass!";
            }
            else {
                resultingOutput += "You fail.";
            }
        }

        consoleClearScreen();
        System.out.println(resultingOutput);

        // Making program wait till user press enter.
        BufferedReader consoleInputReader = new BufferedReader(
                new InputStreamReader(System.in));
        consoleInputReader.readLine();
    }

    /**
     * Clears console(prints 40 blank lines).
     */
    public static void consoleClearScreen() {
        for (int i = 0; i < 40; i++)
            System.out.println();
    }

    /**
     * Indicates that just text should be read from System.in.
     */
    public static final int ANSWER_TYPE_TEXT = 1;

    /**
     * Indicates that integer as string should be read from System.in.
     */
    public static final int ANSWER_TYPE_INTEGER = 2;

    /**
     * Indicates that double as string should be read from System.in.
     */
    public static final int ANSWER_TYPE_DOUBLE = 3;

    /**
     * "Y" and "n" answer set.
     */
    public static final String[] Y_N_ANSWER_SET = { "y", "n" };

    /**
     * Reads answer from console, using specified question and if present list
     * of correct answers or type.
     * 
     * @param question
     *            Question to promt user.
     * @param correctAnswers
     *            List if correct answers.
     * @param type
     *            Type of answer(integer, double etc).
     * @return "Polished" answer.
     * 
     * @see #ANSWER_TYPE_TEXT
     * @see #ANSWER_TYPE_INTEGER
     * @see #ANSWER_TYPE_DOUBLE
     * @see #Y_N_ANSWER_SET
     */
    public static String readFromConsole(String question,
            String[] correctAnswers, int type) {
        // Asking question on console.
        if (correctAnswers == null)
            System.out.print(question + ": ");
        else {
            System.out.print(question + " [");
            for (int i = 0; i < correctAnswers.length; i++) {
                if (i != (correctAnswers.length - 1))
                    System.out.print(correctAnswers[i] + ",");
                else
                    System.out.print(correctAnswers[i]);
            }
            System.out.print("]: ");
        }

        BufferedReader consoleReader = new BufferedReader(
                new InputStreamReader(System.in));
        String answer = null;
        try {
            answer = consoleReader.readLine();
        }
        catch (IOException e) {
            // Shouldn't be for console program. Using console utilities for
            // daemon?
            return null;
        }

        if (correctAnswers != null) {
            while (true) {
                for (String cur : correctAnswers) {
                    if (answer.compareTo(cur) == 0)
                        return answer;
                }
                System.out
                        .print("Wrong answer. Please select answer from list again: ");
                try {
                    answer = consoleReader.readLine();
                }
                catch (IOException e) {
                    // Read comment for this before.
                    return null;
                }
            }
        }

        if ((type == 2) || (type == 3)) {
            while (true) {
                try {
                    if (type == 2)
                        Integer.parseInt(answer);
                    else
                        if (type == 3)
                            Double.parseDouble(answer);
                    return answer;
                }
                catch (NumberFormatException e) {
                    try {
                        answer = consoleReader.readLine();
                    }
                    catch (IOException e1) {
                        // Read comment for this before.
                        return null;
                    }
                    continue;
                }
            }
        }

        return answer;
    }

    /**
     * @see #readFromConsole(String, String[], int)
     */
    public static String readFromConsole(String question,
            String[] correctAnswers) {
        return readFromConsole(question, correctAnswers, ANSWER_TYPE_TEXT);
    }

    /**
     * @see #readFromConsole(String, String[], int)
     */
    public static String readFromConsole(String question) {
        return readFromConsole(question, null, ANSWER_TYPE_TEXT);
    }

    /**
     * @see #readFromConsole(String, String[], int)
     */
    public static String readFromConsole(String question, int type) {
        return readFromConsole(question, null, type);
    }

}
