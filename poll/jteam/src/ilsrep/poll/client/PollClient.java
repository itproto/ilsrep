package ilsrep.poll.client;

import ilsrep.poll.common.Versioning;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.AnswerItem;
import ilsrep.poll.common.protocol.Answers;
import ilsrep.poll.common.protocol.Item;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.common.protocol.User;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.List;

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
        List<AnswerItem> answers = new ArrayList<AnswerItem>();
TcpCommunicator communicator = null;
        String serverPortString = null;

        System.out.println("Poll Client\nVersion: "
                + Versioning.getVersion(Versioning.COMPONENT_CLIENT_CONSOLE) + '\n');

        // Greeting user and asking his name and filename of poll xml file.
        System.out.println("Welcome to polls client program!\n");
        // System.out.print("Please enter your name: ");
        String name = readFromConsole("Please enter your name");

        JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller um = cont.createUnmarshaller();
        Pollsession polls = null;
        boolean repeater = true;
        String yesNoChoice = readFromConsole("\nUse \n (1) server \n (2) local file \n[press enter for default (1)]");
Boolean onServer=true;
        if ((yesNoChoice.compareTo("2") == 0)) {
	        onServer=false;
            String fileName = readFromConsole("\nPlease enter filename to read"
                    + " poll xml from\n[press enter for default"
                    + " \"xml/Polls.xml\"]");
            if (fileName.compareTo("") == 0)
                fileName = "xml/Polls.xml";

            // Serialising xml file into object model.
            File pollFile = new File(fileName);
            polls = (Pollsession) um.unmarshal(pollFile);
        }
        else {
            serverPortString = readFromConsole("\nPlease enter server:port"
                    + " to connect to\n[press enter for"
                    + " default \"127.0.0.1:3320\"]");
            

            communicator = initTcpCommunicator(serverPortString);

            User user = new User();
            user.setUserName(name);

            System.out.println("Please wait.");

            user = communicator.sendUser(user);

            // comm2.finalize();
            if (user.getAction().equals("exist")) {
                boolean notLogged = true;
                while (notLogged) {
                    System.out.println("\nUser Exists.");
                    String password = readFromConsole("Enter password");
                    user.setPass(password);
                    user.setAction("auth");
                    user = communicator.sendUser(user);
                    if (user.getAction().equals("auth")) {
                        System.out.println("\nLogged in. Welcome!");
                        notLogged = false;
                    }
                    else
                        System.out.println("\nWrong password. Try again.");
                }
            }
            else {
                String password = "";
                boolean notSame = true;
                System.out.println("\nUser doesn't exist. Creating user.");
                while (notSame) {
                    password = readFromConsole("Enter new password");
                    if (password.equals(readFromConsole("Confirm password"))) {
                        notSame = false;
                    }
                    else {
                        System.out
                                .println("\nPasswords don't match. Try again.");
                    }

                }
                user.setPass(password);
                user.setNew("true");
                user = communicator.sendUser(user);
                System.out.println("\nUser created. Welcome!");
            }

            // communicator.listXml();
            System.out.println("\nGetting list of pollsessions. Please wait.");
            Pollsessionlist lst = communicator.listXml();
            if (lst != null && lst.getItems() != null) {
                System.out.println("\nList of pollsessions stored on server:");

                for (Item i : lst.getItems()) {
                    System.out.println(i.getId() + ") " + i.getName());
                }
            }
            else {
                System.out.println("\nList is empty or server sent no list.");
            }
            boolean done = false;
            while (!done) {
                try {
                    polls = communicator.getXML();

                    if (polls != null)
                        done = true;
                    else {
                        System.out
                                .println("\nNo such id on server. Enter id again.");
                    }
                }
                catch (Exception m) {
                    // TODO: BUG: This cause client hang when it can't connect
                    // to server.
                    System.out
                            .println("\nCorrupted output from server.\nPossibly no such id or corrupted XML.\nTry again.\n");
                }
            }
          //  communicator.finalize();
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
            AnswerItem itm = new AnswerItem();
            if (cur.selectedId != 0) {
                answers.add(itm.setItem(Integer.parseInt(cur.getId()),
                        cur.selectedId));
            }
            else
                answers.add(itm.setItem(Integer.parseInt(cur.getId()), choice));

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

        System.out.println("\nSending results to server.");

       // TcpCommunicator communicator = initTcpCommunicator(serverPortString);

        Answers ans = new Answers();
        ans.setUsername(name);
        ans.setPollSesionId(polls.getId());
        ans.setAnswers(answers);
if (onServer){
        communicator.sendResult(ans);
        communicator.finalize();

        System.out.println("\nResults sent.\nPress ENTER key to exit.");
}
        // Making program wait till user press enter.
        BufferedReader consoleInputReader = new BufferedReader(
                new InputStreamReader(System.in));
        consoleInputReader.readLine();
    }

    private static TcpCommunicator initTcpCommunicator(String serverPortString) {
        TcpCommunicator communicator = null;
        if (serverPortString.compareTo("") != 0) {
            int separatorIndex = serverPortString.indexOf(':');
            if (separatorIndex != 1) {
                try {
                    String serverName = serverPortString.substring(0,
                            separatorIndex);
                    int port = Integer.parseInt(serverPortString
                            .substring(separatorIndex + 1));

                    System.out.println("\nConnecting to " + serverName + " on "
                            + Integer.toString(port));

                    communicator = new TcpCommunicator(serverName, port);
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
            try {
                communicator = new TcpCommunicator();

                System.out.println("\nConnecting to "
                        + TcpCommunicator.DEFAULT_SERVER + " on "
                        + Integer.toString(TcpCommunicator.DEFAULT_PORT));
            }
            catch (UnknownHostException e) {
                communicator = null;
            }
            catch (IOException e) {
                communicator = null;
            }

        if (communicator != null)
            System.out.println("Connected!");

        return communicator;
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
                        String numberName = "";
                        if (type == 2)
                            numberName = "integer";
                        else
                            if (type == 3)
                                numberName = "double";

                        System.out.print("Wrong answer. Please write "
                                + numberName + ": ");
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
