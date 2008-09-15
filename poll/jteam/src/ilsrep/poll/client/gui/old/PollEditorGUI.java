package ilsrep.poll.client.gui.old;

import ilsrep.poll.client.TcpCommunicator;
import ilsrep.poll.common.Versioning;
import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Description;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.Item;
import ilsrep.poll.common.protocol.Pollsessionlist;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.swing.ButtonGroup;
import javax.swing.JRadioButton;

/**
 * Handles XML creation and uploading to server.
 * 
 * @author DRC
 * 
 */
public class PollEditorGUI {

    public static GUIUtil win = new GUIUtil();

    public static final String YES = "y";

    public static String getScore() {
        Boolean input = true;
        Double score = null;
        while (input) {
            score = Double.parseDouble(win.askUser("Enter minimum Score"));
            if (score < 1)
                input = false;
            else
                win.alert("Score has to be less then 1. Try again");
        }
        return Double.toString(score);

    }

    public static String getCorrect(int max) {
        String correct = "";
        Boolean input = true;
        while (input) {
            correct = win.askUser("Enter correct choice number");
            if (Integer.parseInt(correct) < max)
                input = false;
            else
                win
                        .alert("You entered a number outside choice scope. Try again");
        }
        return correct;

    }

    public static Choice createChoice(int id) {
        Choice choice = new Choice();
        choice.setId(Integer.toString(id));
        choice.setName(win.askUser(Integer.toString(id) + ")>"));
        return choice;
    }

    public static Poll createPoll(int id, Boolean testmode) {
        Poll poll = new Poll();
        poll.setId(Integer.toString(id));
        poll.setName(win.askUser("Enter poll name"));
        Description desc = new Description();
        desc.setValue(win.askUser("Enter poll description"));
        poll.setDescription(desc);
        poll.setCustomEnabled(win.askYesNo("Allow custom choice?") ? " true"
                : "false");
        int choiceNum = Integer.parseInt(win
                .askUser("How many options will the poll have?"));
        ArrayList<Choice> choices = new ArrayList<Choice>();
        for (int i = 1; i <= choiceNum; i++)
            choices.add(createChoice(i));
        poll.setChoices(choices);
        if (testmode)
            poll.setCorrectChoice(getCorrect(choiceNum));
        return poll;
    }

    public static void main(String[] args) throws IOException {
        try {
            win.alert("Poll Editor\nVersion: "
                    + Versioning
                            .getVersion(Versioning.COMPONENT_EDITOR_CONSOLE));
            Object[] a = { "Create new poll session",
                    "Edit existing oll session at server",
                    "Delete existing poll session from server" };
            int actionChoice = win.askUserChoice("Select action:", a);
            if (actionChoice == 1) {
                TcpCommunicator communicator = new TcpCommunicator();

                Pollsessionlist lst = communicator.listXml();
                ButtonGroup group = new ButtonGroup();
                if (lst != null && lst.getItems() != null) {

                    for (Item i : lst.getItems()) {
                        JRadioButton jrb = new JRadioButton(i.getName());
                        System.out.println(i.getName() + "\n");
                        jrb.setActionCommand(i.getId());
                        group.add(jrb);
                    }
                }
                else {
                    win.alert("\nList is empty or server sent no list.");
                }

                String pollsessToEditChoice = win.getChoice(group,
                        "Choose pollsession");

                Pollsession sessionToEdit = communicator
                        .getPollsession(pollsessToEditChoice);

                communicator.finalize();

                if (sessionToEdit != null) {
                    editPollsession(sessionToEdit);

                    communicator = new TcpCommunicator();

                    communicator.editPollsession(pollsessToEditChoice,
                            sessionToEdit);

                    communicator.finalize();
                }
                else {
                    win.alert("No such id on server.");
                }
            }
            else
                if (actionChoice == 2) {
                    TcpCommunicator communicator = new TcpCommunicator();

                    Pollsessionlist lst = communicator.listXml();
                    ButtonGroup group = new ButtonGroup();
                    if (lst != null && lst.getItems() != null) {

                        for (Item i : lst.getItems()) {
                            JRadioButton jrb = new JRadioButton(i.getName());
                            System.out.println(i.getName() + "\n");
                            jrb.setActionCommand(i.getId());
                            group.add(jrb);
                        }
                    }
                    else {
                        win.alert("\nList is empty or server sent no list.");
                    }

                    String pollsessToDeleteChoice = win.getChoice(group,
                            "Choose pollsession");

                    communicator.deleteXml(pollsessToDeleteChoice);

                    communicator.finalize();
                }
                else {
                    Pollsession pollSession = new Pollsession();
                    pollSession.setName(win.askUser("Enter pollsession name"));
                    pollSession.setTestMode((win
                            .askYesNo("will this run in testmode?") ? "true"
                            : "false"));
                    if (pollSession.getTestMode().equals("true")) {
                        pollSession.setMinScore(getScore());
                    }
                    int pollNum = Integer.parseInt(win
                            .askUser("How many polls will the session have?"));
                    ArrayList<Poll> polls = new ArrayList<Poll>();
                    for (int i = 1; i <= pollNum; i++)
                        polls.add(createPoll(i, pollSession.getTestMode()
                                .equals("true") ? true : false));
                    pollSession.setPolls(polls);
                    TcpCommunicator communicator = new TcpCommunicator();

                    communicator.sendPollsession(pollSession);

                    communicator.finalize();

                }
        }
        catch (Exception e) {
            win.alert("Invalid input or server is down");
        }

    }

    /**
     * Edits pollsession asking questions in console.
     * 
     * @param source
     *            Pollsession to edit.
     * @return Edited pollsession(reference to same object as source).
     */
    private static Pollsession editPollsession(Pollsession source) {
        win.alert("\nPollsession name: " + source.getName());
        if (win.askYesNo("Change pollsession name")) {
            source.setName(win.askUser("Enter new pollsession name"));
        }

        win.alert("\nTest mode: " + source.getTestMode());
        if (win.askYesNo("Change test mode value")) {
            source
                    .setTestMode((win.askYesNo("will this run in testmode?") ? "true"
                            : "false"));

        }

        if (source.getTestMode().compareTo("true") == 0) {
            System.out.println();

            if (source.getTestMode() == null) {
                source.setMinScore(getScore());
            }
            else {
                win.alert("Current minimum score: " + source.getMinScore());
                if (win.askYesNo("Change minimum score")) {
                    source.setMinScore(win.askUser("Enter minimum score"));
                }
            }
        }
        else
            source.setMinScore(null);

        for (Poll poll : source.getPolls()) {
            win.alert("\nPoll name: " + poll.getName());
            if (win.askYesNo("Change poll name")) {
                poll.setName(win.askUser("Enter new poll name"));
            }

            win
                    .alert("\nPoll description: "
                            + poll.getDescription().getValue());
            if (win.askYesNo("Change poll description")) {
                poll.getDescription().setValue(
                        (win.askUser("Enter new poll name")));
            }

            win.alert("\nCurrent custom choice enabled value: "
                    + poll.getCustomEnabled());
            if (win.askYesNo("Change")) {
                poll.setCustomEnabled(((win
                        .askYesNo("Allow custom choice for this poll") ? "true"
                        : "false")));
            }

            List<Choice> choicesToDelete = new ArrayList<Choice>();

            for (Choice choice : poll.getChoices()) {
                win.alert("\nChoice name: " + choice.getName());
                if (win.askYesNo("Remove this choice")) {
                    choicesToDelete.add(choice);
                }
                else
                    if (win.askYesNo("Change choice name")) {
                        choice.setName(win.askUser("Enter new choice name"));
                    }
            }

            poll.getChoices().removeAll(choicesToDelete);

            while (true) {
                if (win.askYesNo("\nAdd new choice")) {
                    Choice newChoice = new Choice();
                    newChoice.setName(win.askUser("Enter choice name"));
                    poll.getChoices().add(newChoice);
                }
                else
                    break;
            }

            if (source.getTestMode().compareTo("true") == 0) {
                int correctChoice = -1;
                System.out.println();
                while (true) {
                    correctChoice = Integer.parseInt(win
                            .askUser("Enter correct choice(1 - "
                                    + poll.getChoices().size()));

                    if (correctChoice >= 1
                            && correctChoice <= poll.getChoices().size())
                        break;

                    win.alert("Wrong answer. Enter again");
                }
                poll.setCorrectChoice(Integer.toString(correctChoice));
            }
        }

        // TODO: Ask if add new poll and add it :)

        return source;
    }

}
