package ilsrep.poll.client;

import ilsrep.poll.common.Choice;
import ilsrep.poll.common.Poll;
import ilsrep.poll.common.Pollsession;

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
                    String yesNoChoice = "y";
                    // BufferedReader consoleInputReader = new BufferedReader(
                    // new InputStreamReader(System.in));
                    String genXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
                    String name = "1"; // default number
                    genXml += "<pollsession id=\"" + name + "\" name=\"";
                    // System.out.print("Enter pollsession name: ");
                    name = PollClient
                            .readFromConsole("\nEnter pollsession name");
                    genXml += name + "\" testMode=\"";
                    // System.out.print("Will this poll run in testmode? [y/n]:
                    // ");
                    System.out.println();
                    yesNoChoice = PollClient.readFromConsole(
                            "Will this poll run in testmode?",
                            PollClient.Y_N_ANSWER_SET);
                    name = (yesNoChoice.indexOf("y") != -1) ? "true" : "false";
                    genXml += name + "\" ";
                    String testMode = name;
                    if (testMode.indexOf("true") != -1) {
                        // System.out.print("Enter minimum score: ");
                        System.out.println();
                        name = PollClient.readFromConsole(
                                "Enter minimum score",
                                PollClient.ANSWER_TYPE_DOUBLE);
                        genXml += "minScore=\"" + name + "\" ";
                    }
                    genXml += "> \n";

                    yesNoChoice = "y";
                    String yesNoChoice2 = "y";
                    int i = 1;
                    int correct = 0;
                    // This cycle serves to enter polls
                    while (yesNoChoice2.indexOf("y") != -1) {
                        // System.out.print("Enter poll name: ");
                        System.out.println();
                        name = PollClient.readFromConsole("Enter poll name");
                        genXml += " <poll id=\"" + Integer.toString(i)
                                + "\" name=\"" + name + "\"";
                        if (testMode.indexOf("true") != -1) {
                            // System.out
                            // .print("Enter number of the correct choice in
                            // poll:
                            // ");
                            name = PollClient
                                    .readFromConsole(
                                            "Enter number of the correct choice in poll",
                                            PollClient.ANSWER_TYPE_INTEGER);
                            genXml += " correctChoice=\"" + name + "\" ";
                            correct = Integer.parseInt(name);
                        }
                        String yesNoChoice3 = PollClient.readFromConsole(
                                "Allow custom choice for this poll?",
                                PollClient.Y_N_ANSWER_SET);
                        if (yesNoChoice3.indexOf("y") != -1) {
                            genXml += " customChoiceEnabled=\"true\" ";
                        }

                        genXml += " >\n";
                        // System.out.print("Enter poll description: ");
                        name = PollClient
                                .readFromConsole("Enter poll description");
                        genXml += " <description>" + name
                                + "</description>\n<choices>\n";
                        int n = 1;

                        yesNoChoice = "y";
                        // this cycle is for entering options
                        while (((correct >= n) && (testMode.indexOf("true") != -1))
                                || (yesNoChoice.indexOf("y") != -1)) {

                            // System.out.print("Enter choice option: ");
                            name = PollClient
                                    .readFromConsole("Enter next choice");
                            genXml += "<choice id=\"" + Integer.toString(n)
                                    + "\"  name=\"" + name + "\" />\n";
                            if (correct <= n) {
                                // System.out.print("Add new choice option?
                                // [y/n]:
                                // ");

                                yesNoChoice = PollClient.readFromConsole(
                                        "Add new choice?",
                                        PollClient.Y_N_ANSWER_SET);
                            }

                            n++;
                        }

                        genXml += "</choices>\n</poll>\n";

                        // System.out.print("Add new Poll? [y/n]: ");

                        yesNoChoice2 = PollClient.readFromConsole(
                                "Add new Poll?", PollClient.Y_N_ANSWER_SET);

                        i++;
                    }
                    genXml += "</pollsession>\n\n";
                    System.out.println("\n" + genXml);
                    TcpCommunicator communicator = new TcpCommunicator();
                    communicator.sendXml(genXml);
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
