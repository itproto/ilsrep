package ilsrep.poll.client;
import ilsrep.poll.common.Pollsession;
import javax.swing.JRadioButton;
import javax.swing.ButtonGroup;
import javax.swing.JLabel;
import ilsrep.poll.common.Item;
import ilsrep.poll.common.Poll;
import ilsrep.poll.common.Pollsessionlist;
import ilsrep.poll.common.AnswerItem;
import ilsrep.poll.common.Answers;
import ilsrep.poll.common.User;
import ilsrep.poll.common.MainWindow;
import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.UnknownHostException;
import java.util.List;
import java.util.ArrayList;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;
import java.util.jar.Manifest;
import java.util.jar.JarFile;
import java.util.jar.Attributes;
import java.awt.*;
import java.awt.event.*;
import javax.swing.JFileChooser;
/**
 * Main class for task 7 - Poll.
 * 
 * @author TKOST
 * @author DCR
 * 
 */
public class PollClientGUI {

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



 public static void main(String[] args)  throws JAXBException, IOException {
List<AnswerItem> answers = new ArrayList<AnswerItem>();
String serverPortString = null;
MainWindow win=new MainWindow();
Pollsession polls=null;
String name=win.askUser("Enter Name");
Object[] options = {"Use Server",
                    "Use local file"};
int reply=win.askUserChoice("Please choose poll source",options);

 if (reply==1) {
File pollFile=null;
JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller um = cont.createUnmarshaller();
       JFileChooser chooser = new JFileChooser();




    int returnVal = chooser.showOpenDialog(win);
    if(returnVal == JFileChooser.APPROVE_OPTION) {
       pollFile=chooser.getSelectedFile();
    }

            // Serialising xml file into object model.
            
            polls = (Pollsession) um.unmarshal(pollFile);
        }
if (reply==0){
      serverPortString = win.askUser("\nPlease enter server:port"
                    + " to connect to\n[press enter for"
                    + " default \"127.0.0.1:3320\"]");
            TcpCommunicator communicator = null;

            communicator = initTcpCommunicator(serverPortString);

            User user = new User();
            user.setUserName(name);

           user = communicator.sendUser(user);

            // comm2.finalize();
            if (user.getExist().equals("true")) {
                boolean notLogged = true;
                while (notLogged) {
                    String password = win.askUser("Enter password");
                    user.setPass(password);
                    user = communicator.sendUser(user);
                    if (user.getAuth().equals("true")) {
                        win.alert("\nLogged in. Welcome!");
                        notLogged = false;
                    }
                    else
                        win.askUser("\nWrong password. Try again.");
                }
            }
            else {
                String password = "";
                boolean notSame = true;
                win.alert("\nUser doesn't exist. Creating user.");
                while (notSame) {
                    password = win.askUser("Enter new password");
                    if (password.equals(win.askUser("Confirm password"))) {
                        notSame = false;
                    }
                    else {
                        win.alert("\nPasswords don't match. Try again.");
                    }

                }
                user.setPass(password);
                user.setNew("true");
                user = communicator.sendUser(user);
                win.alert("\nUser created. Welcome!");
            }
Pollsessionlist lst=communicator.listXml();
 ButtonGroup group = new ButtonGroup();
 if (lst!= null
                    && lst.getItems() != null) {
             
                for (Item i : lst.getItems()) {
JRadioButton jrb= new JRadioButton(i.getName());
System.out.println(i.getName()+"\n");
jrb.setActionCommand(i.getId());
                   group.add(jrb);
                }
            }
            else {
                win.alert("\nList is empty or server sent no list.");
            }

    
polls=communicator.getPollsession(win.getChoice(group,"Choose pollsession"));
}
if (polls!=null) win.alert("Pollsession aquired");
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
        
Boolean res=null;
        res = win.askYesNo("\nOk, " + name + ", are you ready for poll?");
        if (!res)return;
    String choice = null;
   
     float i = 0, n = 0;
        for (Poll cur : polls.getPolls()) {
            while (choice == null)
                choice = cur.queryUserGUI(win);
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

        win.setVisible(false);
win.alert(resultingOutput);        
if(reply==1){
        

        TcpCommunicator communicator = initTcpCommunicator(serverPortString);

        Answers ans = new Answers();
        ans.setUsername(name);
        ans.setPollSesionId(polls.getId());
        ans.setAnswers(answers);

        communicator.sendResult(ans);
        communicator.finalize();

        win.alert("\nResults sent");
}
return;



















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
            }
            catch (UnknownHostException e) {
                communicator = null;
            }
            catch (IOException e) {
                communicator = null;
            }

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
