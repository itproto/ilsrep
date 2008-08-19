package ilsrep.poll.client;

import ilsrep.poll.common.Choice;
import ilsrep.poll.common.Poll;
import ilsrep.poll.common.Pollsession;
import ilsrep.poll.common.Description;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import java.util.jar.Manifest;
import java.util.jar.JarFile;
import java.util.jar.Attributes;

/**
 * Handles XML creation and uploading to server.
 * 
 * @author DRC
 * 
 */
public class PollEditor {

    // each parameter of XML is being promted to user. After entering number of
    // the correct choice the user is required to enter
    // minumum the number of choices that equals it
    /**
     * Main method for poll editor. <br>
     * 
     * @param args
     *            Command line arguments.
     * @throws IOException
     *             On console input errors.
     */
public static Choice createChoice(int id){
Choice choice=new Choice();
choice.setId(Integer.toString(id));
choice.setName(PollClient.readFromConsole(Integer.toString(id) +")>"));
return choice;
}
public static Poll createPoll(int id, Boolean testmode) {
Poll poll=new Poll();
poll.setId(Integer.toString(id));
poll.setName(PollClient.readFromConsole("Enter poll name"));
Description desc= new Description();
desc.setValue(PollClient.readFromConsole("Enter poll description"));
poll.setDescription(desc);
poll.setCustomEnabled(PollClient.readFromConsole("Allow custom choice?",PollClient.Y_N_ANSWER_SET).equals("y")?" true" : "false");
int choiceNum=Integer.parseInt(PollClient.readFromConsole("How many options will the poll have?"));
ArrayList<Choice> choices=new ArrayList<Choice>();
for (int i=1;i<=choiceNum;i++) choices.add(createChoice(i));
poll.setChoices(choices);
if(testmode) poll.setCorrectChoice(PollClient.readFromConsole("Enter correct choice number"));
return poll;           
}

    public static void main(String[] args) throws IOException {
        try {
            JarFile jar = new JarFile("./poll.jar");
            Manifest manifest = jar.getManifest();
            Attributes attribs = manifest
                    .getAttributes("ilsrep/poll/client/PollEditor.class");
            System.out.println("Poll Editor\nVersion: "
                    + attribs.getValue("Specification-Version"));

            String actionChoice = PollClient.readFromConsole("\nSelect action:"
                    + "\n(1) Create new poll session"
                    + "\n(2) Edit existing oll session at server"
                    + "\n(3) Delete existing poll session from server"
                    + "\n[press enter for default (1)]");
            if (actionChoice.compareTo("2") == 0) {
                TcpCommunicator communicator = new TcpCommunicator();

                communicator.listXml();

                String pollsessToEditChoice = PollClient.readFromConsole(
                        "\nChoose pollsession to edit",
                        PollClient.ANSWER_TYPE_INTEGER);

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
                    System.out.println("No such id on server.");
                }
            }
            else
                if (actionChoice.compareTo("3") == 0) {
                    TcpCommunicator communicator = new TcpCommunicator();

                    communicator.listXml();

                    String pollsessToDeleteChoice = PollClient.readFromConsole(
                            "\nChoose pollsession to delete",
                            PollClient.ANSWER_TYPE_INTEGER);

                    communicator.deleteXml(pollsessToDeleteChoice);

                    communicator.finalize();
                }
                else {
Pollsession sess=new Pollsession();
sess.setName(PollClient.readFromConsole("Enter pollsession name"));
 sess.setTestMode(((PollClient.readFromConsole(
                    "Will this poll run in testmode?",
                    PollClient.Y_N_ANSWER_SET).compareTo("y") == 0) ? "true"
                    : "false"));
 if (sess.getTestMode().equals("true")) {
                sess.setMinScore(PollClient.readFromConsole(
                        "Enter minimum score", PollClient.ANSWER_TYPE_DOUBLE));
            }
int pollNum=Integer.parseInt(PollClient.readFromConsole("How many polls will the session have?"));
ArrayList<Poll> polls=new ArrayList<Poll>();
for (int i=1;i<=pollNum;i++) polls.add(createPoll(i,sess.getTestMode().equals("true")? true : false ));
sess.setPolls(polls);
TcpCommunicator communicator = new TcpCommunicator();

                    communicator.sendPollsession(sess);

                    communicator.finalize();


                }
        }
        catch (Exception e) {
            System.out.println("Invalid input or server is down");
        }

        System.out.println("\nPress enter to exit.");

        BufferedReader consoleInputReader = new BufferedReader(
                new InputStreamReader(System.in));
        consoleInputReader.readLine();
    }

    /**
     * Edits pollsession asking questions in console.
     * 
     * @param source
     *            Pollsession to edit.
     * @return Edited pollsession(reference to same object as source).
     */
    private static Pollsession editPollsession(Pollsession source) {
        System.out.println("\nPollsession name: " + source.getName());
        if (askYesNo("Change pollsession name")) {
            source.setName(PollClient
                    .readFromConsole("Enter new pollsession name"));
        }

        System.out.println("\nTest mode: " + source.getTestMode());
        if (askYesNo("Change test mode value")) {
            source.setTestMode(((PollClient.readFromConsole(
                    "Will this poll run in testmode?",
                    PollClient.Y_N_ANSWER_SET).compareTo("y") == 0) ? "true"
                    : "false"));

        }

        if (source.getTestMode().compareTo("true") == 0) {
            System.out.println();

            if (source.getTestMode() == null) {
                source.setMinScore(PollClient.readFromConsole(
                        "Enter minimum score", PollClient.ANSWER_TYPE_DOUBLE));
            }
            else {
                System.out.println("Current minimum score: "
                        + source.getMinScore());
                if (askYesNo("Change minimum score")) {
                    source.setMinScore(PollClient.readFromConsole(
                            "Enter minimum score",
                            PollClient.ANSWER_TYPE_DOUBLE));
                }
            }
        }
        else
            source.setMinScore(null);

        for (Poll poll : source.getPolls()) {
            System.out.println("\nPoll name: " + poll.getName());
            if (askYesNo("Change poll name")) {
                poll.setName(PollClient.readFromConsole("Enter new poll name"));
            }

            System.out.println("\nPoll description: "
                    + poll.getDescription().getValue());
            if (askYesNo("Change poll description")) {
                poll.getDescription().setValue(
                        (PollClient.readFromConsole("Enter new poll name")));
            }

            System.out.println("\nCurrent custom choice enabled value: "
                    + poll.getCustomEnabled());
            if (askYesNo("Change")) {
                poll
                        .setCustomEnabled((((PollClient.readFromConsole(
                                "Allow custom choice for this poll?",
                                PollClient.Y_N_ANSWER_SET).compareTo("y") == 0) ? "true"
                                : "false")));
            }

            List<Choice> choicesToDelete = new ArrayList<Choice>();

            for (Choice choice : poll.getChoices()) {
                System.out.println("\nChoice name: " + choice.getName());
                if (askYesNo("Remove this choice")) {
                    choicesToDelete.add(choice);
                }
                else
                    if (askYesNo("Change choice name")) {
                        choice.setName(PollClient
                                .readFromConsole("Enter new choice name"));
                    }
            }

            poll.getChoices().removeAll(choicesToDelete);

            while (true) {
                if (askYesNo("\nAdd new choice")) {
                    Choice newChoice = new Choice();
                    newChoice.setName(PollClient
                            .readFromConsole("Enter choice name"));
                    poll.getChoices().add(newChoice);
                }
                else
                    break;
            }

            if (source.getTestMode().compareTo("true") == 0) {
                int correctChoice = -1;
                System.out.println();
                while (true) {
                    correctChoice = Integer.parseInt(PollClient
                            .readFromConsole("Enter correct choice(1 - "
                                    + poll.getChoices().size() + ")",
                                    PollClient.ANSWER_TYPE_INTEGER));

                    if (correctChoice >= 1
                            && correctChoice <= poll.getChoices().size())
                        break;

                    System.out.println("Wrong answer. Enter again");
                }
                poll.setCorrectChoice(Integer.toString(correctChoice));
            }
        }

        // TODO: Ask if add new poll and add it :)

        return source;
    }

    /**
     * Promts user in console question given and accepts answer <code>y</code>
     * or <code>n</code>.<br>
     * Returns <code>true</code> only if users answers <code>yes</code>.
     * 
     * @param question
     * @return
     */
    public static boolean askYesNo(String question) {
        return PollClient.readFromConsole(question + "? [y/N]").compareTo("y") == 0;
    }

}
