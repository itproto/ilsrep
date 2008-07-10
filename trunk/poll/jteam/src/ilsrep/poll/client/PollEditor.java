package ilsrep.poll.client;

import java.io.BufferedReader;
import java.io.InputStreamReader;
/**
 * Handles XML creation
 * 
 * @author DRC
 * 
 */
public class PollEditor {
// each parameter of XML is being promted to user. After entering number of the correct choice the user is required to enter
//minumum the number of choices that equals it
    public static void main(String[] args) {

        try {
	          String yesNoChoice = "y";
            BufferedReader consoleInputReader = new BufferedReader(
                    new InputStreamReader(System.in));
            String genXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
          //  System.out.println("enter pollsession number");
            String name ="1"; //default number
            genXml += "<pollsession id=\"" + name + "\" name=\"";
            System.out.println("Enter pollsession name:");
            name = consoleInputReader.readLine();
            genXml += name + "\" testMode=\"";
            System.out.println("Will this poll run in testmode? y/n");
            yesNoChoice = consoleInputReader.readLine();
            name = (yesNoChoice.indexOf("y") != -1) ? "true" : "false";
            genXml += name + "\" ";
            String testMode = name;
            System.out.println(testMode);
            if (testMode.indexOf("true") != -1) {
                System.out.println("enter minimum Score:");
                name = consoleInputReader.readLine();
                genXml += "minScore=\"" + name + "\" ";
            }
            genXml += "> \n";

             yesNoChoice = "y";
            String yesNoChoice2 = "y";
            int i = 1;
            int correct = 0;
            //This cycle serves to enter polls
            while (yesNoChoice2.indexOf("y") != -1) {
                System.out.println("eenter Poll name:");
                name = consoleInputReader.readLine();
                genXml += " <poll id=\"" + Integer.toString(i) + "\" name=\""
                        + name + "\"";
                if (testMode.indexOf("true") != -1) {
                    System.out.println("Enter number of the correct choice in poll:");
                    name = consoleInputReader.readLine();
                    genXml += " correctChoice=\"" + name + "\" ";
                    correct = Integer.parseInt(name);
                }

                genXml += " >\n";
                System.out.println("Enter Poll description:");
                name = consoleInputReader.readLine();
                genXml += " <description>" + name
                        + "</description>\n<choices>\n";
                int n = 1;

                yesNoChoice = "y";
                //this cycle is for entering options
                while (((correct >= n ) && (testMode.indexOf("true") != -1))
                        || (yesNoChoice.indexOf("y") != -1)) {

                    System.out.println("Enter Choice option:");
                    name = consoleInputReader.readLine();
                    genXml += "<choice id=\"" + Integer.toString(n)
                            + "\"  name=\"" + name + "\" />\n";
                    if (correct <= n) {
                        System.out.print("Add new choice option?y/n");

                        yesNoChoice = consoleInputReader.readLine();
                    }

                    n++;
                }
                ;

                genXml += "</choices>\n</poll>\n";

                System.out.print("Add new Poll? y/n");

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
            System.out.println("Invalid input or server is down");
            try {BufferedReader consoleInputReader = new BufferedReader( new InputStreamReader(System.in));
                    consoleInputReader.readLine();} catch(Exception exception){};
            
        }
        ;
    }

}
