package ilsrep.poll.client;

import java.io.BufferedReader;
import java.io.InputStreamReader;

public class PollEditor {

    public static void main(String[] args) {

        try {
            BufferedReader consoleInputReader = new BufferedReader(
                    new InputStreamReader(System.in));
            String genXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
            System.out.println("enter pollsession number");
            String name = consoleInputReader.readLine();
            genXml += "<pollsession id=\"" + name + "\" name=\"";
            System.out.println("enter pollsession name");
            name = consoleInputReader.readLine();
            genXml += name + "\" testMode=\"";
            System.out.println("testmode?");
            name = consoleInputReader.readLine();
            genXml += name + "\" ";
            String testMode = name;
            System.out.println(testMode);
            if (testMode.indexOf("true") != -1) {
                System.out.println("minScore");
                name = consoleInputReader.readLine();
                genXml += "minScore=\"" + name + "\" ";
            }
            genXml += "> \n";

            String yesNoChoice = "y";
            String yesNoChoice2 = "y";
            int i = 1;
            int correct = 0;
            while (yesNoChoice2.indexOf("y") != -1) {
                System.out.println("enter name");
                name = consoleInputReader.readLine();
                genXml += " <poll id=\"" + Integer.toString(i) + "\" name=\""
                        + name + "\"";
                if (testMode.indexOf("true") != -1) {
                    System.out.println("correct Choice");
                    name = consoleInputReader.readLine();
                    genXml += " correctChoice=\"" + name + "\" ";
                    correct = Integer.parseInt(name);
                }

                genXml += " >\n";
                System.out.println("enter description");
                name = consoleInputReader.readLine();
                genXml += " <description>" + name
                        + "</description>\n<choices>\n";
                int n = 1;

                yesNoChoice = "y";
                while (((correct >= n - 1) && (testMode.indexOf("true") != -1))
                        || (yesNoChoice.indexOf("y") != -1)) {

                    System.out.println("enter choice");
                    name = consoleInputReader.readLine();
                    genXml += "<choice id=\"" + Integer.toString(n)
                            + "\"  name=\"" + name + "\" />\n";
                    if (correct <= n) {
                        System.out.print("new choice?");

                        yesNoChoice = consoleInputReader.readLine();
                    }

                    n++;
                }
                ;

                genXml += "</choices>\n</poll>\n";

                System.out.print("new poll?");

                yesNoChoice2 = consoleInputReader.readLine();

                i++;

            }
            ;
            genXml += "</pollsession>\n\n";
            System.out.println(genXml);
            TcpCommunicator communicator = new TcpCommunicator();
            communicator.sendXml(genXml);

        }
        catch (Exception e) {
            System.out.println("EXCEPTION");
        }
        ;
    }

}
