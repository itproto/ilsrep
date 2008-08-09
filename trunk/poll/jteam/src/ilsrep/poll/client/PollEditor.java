package ilsrep.poll.client;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
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
     * TODO: BUG: When not in test mode - adds correctChoice="-1" attribute to
     * each "poll" element.
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

            String yesNoChoice = "y";
            // BufferedReader consoleInputReader = new BufferedReader(
            // new InputStreamReader(System.in));
            String genXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
            String name = "1"; // default number
            genXml += "<pollsession id=\"" + name + "\" name=\"";
            // System.out.print("Enter pollsession name: ");
            name = PollClient.readFromConsole("\nEnter pollsession name");
            genXml += name + "\" testMode=\"";
            // System.out.print("Will this poll run in testmode? [y/n]: ");
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
                name = PollClient.readFromConsole("Enter minimum score",
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
                genXml += " <poll id=\"" + Integer.toString(i) + "\" name=\""
                        + name + "\"";
                if (testMode.indexOf("true") != -1) {
                    // System.out
                    // .print("Enter number of the correct choice in poll: ");
                    name = PollClient.readFromConsole(
                            "Enter number of the correct choice in poll",
                            PollClient.ANSWER_TYPE_INTEGER);
                    genXml += " correctChoice=\"" + name + "\" ";
                    correct = Integer.parseInt(name);
                }
                String yesNoChoice3 = PollClient.readFromConsole(
                        "Allow custom choice for this poll?",
                        PollClient.Y_N_ANSWER_SET);
                if (yesNoChoice3.indexOf("y") != -1) {
                    genXml += " customEnabled=\"true\" ";
                }

                genXml += " >\n";
                // System.out.print("Enter poll description: ");
                name = PollClient.readFromConsole("Enter poll description");
                genXml += " <description>" + name
                        + "</description>\n<choices>\n";
                int n = 1;

                yesNoChoice = "y";
                // this cycle is for entering options
                while (((correct >= n) && (testMode.indexOf("true") != -1))
                        || (yesNoChoice.indexOf("y") != -1)) {

                    // System.out.print("Enter choice option: ");
                    name = PollClient.readFromConsole("Enter next choice");
                    genXml += "<choice id=\"" + Integer.toString(n)
                            + "\"  name=\"" + name + "\" />\n";
                    if (correct <= n) {
                        // System.out.print("Add new choice option? [y/n]: ");

                        yesNoChoice = PollClient.readFromConsole(
                                "Add new choice?", PollClient.Y_N_ANSWER_SET);
                    }

                    n++;
                }

                genXml += "</choices>\n</poll>\n";

                // System.out.print("Add new Poll? [y/n]: ");

                yesNoChoice2 = PollClient.readFromConsole("Add new Poll?",
                        PollClient.Y_N_ANSWER_SET);

                i++;
            }
            genXml += "</pollsession>\n\n";
            System.out.println("\n" + genXml);
            TcpCommunicator communicator = new TcpCommunicator();
            communicator.sendXml(genXml);

        }
        catch (Exception e) {
            System.out.println("Invalid input or server is down");
            try {
            }
            catch (Exception exception) {
            }
        }
        BufferedReader consoleInputReader = new BufferedReader(
                new InputStreamReader(System.in));
        consoleInputReader.readLine();
    }

}
