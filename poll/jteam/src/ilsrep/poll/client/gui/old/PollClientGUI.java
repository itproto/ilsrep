package ilsrep.poll.client.gui.old;

import ilsrep.poll.client.TcpCommunicator;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.AnswerItem;
import ilsrep.poll.common.protocol.Answers;
import ilsrep.poll.common.protocol.Item;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.common.protocol.User;

import java.io.File;
import java.io.IOException;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.List;

import javax.swing.ButtonGroup;
import javax.swing.JFileChooser;
import javax.swing.JRadioButton;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;

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

    public static void main(String[] args) throws JAXBException, IOException {
        String serverPortString = null;
        GUIUtil win = new GUIUtil();
        Pollsession polls = null;
        String name = null;
        Object[] options = { "Use Server", "Use local file" };
        int reply = win.askUserChoice("Please choose poll source", options);

        if (reply == 1) {
            File pollFile = null;
            JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
            Unmarshaller um = cont.createUnmarshaller();
            JFileChooser chooser = new JFileChooser();

            int returnVal = chooser.showOpenDialog(win);
            if (returnVal == JFileChooser.APPROVE_OPTION) {
                pollFile = chooser.getSelectedFile();
            }

            // Serialising xml file into object model.

            polls = (Pollsession) um.unmarshal(pollFile);
        }
        if (reply == 0) {
            serverPortString = win.askUser("\nPlease enter server:port"
                    + " to connect to\n[press enter for"
                    + " default \"127.0.0.1:3320\"]");
            // if(serverPortString==null)serverPortString="127.0.0.1:3320";
            TcpCommunicator communicator = null;

            communicator = initTcpCommunicator(serverPortString);

            name = login(communicator, win);

            Pollsessionlist lst = communicator.listXml();
            ButtonGroup group = new ButtonGroup();
            if (lst != null && lst.getItems() != null) {

                for (Item i : lst.getItems()) {
                    JRadioButton jrb = new JRadioButton(i.getName());
                    // System.out.println(i.getName() + "\n");
                    jrb.setActionCommand(i.getId());
                    group.add(jrb);
                }
            }
            else {
                win.alert("\nList is empty or server sent no list.");
            }

            polls = communicator.getPollsession(win.getChoice(group,
                    "Choose pollsession"));
        }

        processPollsession(polls, win, name, (reply == 0), serverPortString);
    }

    /**
     * Processes pollsession with continual GUI dialogs.
     * 
     * @param polls
     *            Pollsession to process.
     * @param win
     *            The <code>GUIUtil</code> to use.
     * @param name
     *            User name
     * @param sendResults
     *            True, if send results to server.
     * @param serverPortString
     *            Server and port to send results to in format <code>server"port</code>.
     * @throws IOException
     *             On I/O error.
     */
    public static void processPollsession(Pollsession polls, GUIUtil win,
            String name, boolean sendResults, String serverPortString)
            throws IOException {
        List<AnswerItem> answers = new ArrayList<AnswerItem>();

        if (polls != null)
            win.infoWindow("Pollsession retrieved");
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

        Boolean res = null;
        res = win.askYesNo("\nOk, " + name + ", are you ready for poll?");
        if (!res)
            return;
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
        if (sendResults) {

            TcpCommunicator communicator = initTcpCommunicator(serverPortString);

            Answers ans = new Answers();
            ans.setUsername(name);
            ans.setPollSesionId(polls.getId());
            ans.setAnswers(answers);

            communicator.sendResult(ans);
            communicator.finalize();

            win.alert("\nResults sent");
        }
    }

    /**
     * Logins to server(promts for user name, creates new user on server if it
     * doesn't exist).
     * 
     * @param communicator
     *            Connected <code>TcpCommunicator</code>.
     * @param win
     *            The <code>GUIUtil</code> to use.
     * @return User name entered by user.
     */
    public static String login(TcpCommunicator communicator, GUIUtil win) {
        String name = win.askUser("Enter Name");

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
            // boolean notSame = true;
            win.alert("\nUser doesn't exist. Creating user.");
            password = win.createPass();
            user.setPass(password);
            user.setNew("true");
            user = communicator.sendUser(user);
            win.alert("\nUser created. Welcome!");
        }

        return name;
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

}
