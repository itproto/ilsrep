package ilsrep.poll.client;

import ilsrep.poll.common.Poll;
import ilsrep.poll.common.Pollsession;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;

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
        BufferedReader consoleInputReader = new BufferedReader(new InputStreamReader(System.in));
        System.out.println("Welcome to polls client program!\n");
        System.out.print("Please enter your name: ");
        String name = consoleInputReader.readLine();
                JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller um = cont.createUnmarshaller();
        Pollsession polls;
        System.out.print("Use server?(1) or local file?(2)");
      
          String yesNoChoice = consoleInputReader.readLine();
        if (!(yesNoChoice.compareTo("1") == 0)){
        System.out.print("Please enter filename to read poll xml "
                + "from\n[press enter for default \"Polls.xml\"]: ");
        String fileName = consoleInputReader.readLine();
        if (fileName.compareTo("") == 0)
            fileName = "Polls.xml";

        // Serialising xml file into object model.
       
        File pollFile = new File(fileName);
           polls= (Pollsession) um.unmarshal(pollFile);
        } else {
	        TcpCommunicator communicator = new TcpCommunicator();
	   Reader pollFile=communicator.getXML();
	           polls= (Pollsession) um.unmarshal(pollFile);
	        
	        }
   
      

        // Showing xml, generated from already read object model.
        Marshaller mr = cont.createMarshaller();
        System.out.println();
        mr.setProperty("jaxb.formatted.output", true);
        mr.marshal(polls, System.out);

        // Processing polls.
        AnswerSaver saveElement = new AnswerSaver();
saveElement.minScore=Float.parseFloat(polls.getMinScore());
saveElement.testMode=polls.getTestMode();
        System.out.print("\nOk, " + name + ", are you ready for poll? [y/n]");
        yesNoChoice = consoleInputReader.readLine();
        if (!(yesNoChoice.compareTo("y") == 0))
            return;

        String choice = null;
        for (Poll cur : polls.getPolls()) {
            while (choice == null)
                choice = cur.queryUser();
            saveElement.pushAnswer(cur.getName(), choice, cur.pass);
            choice = null;
        }

        // Showing poll results.
        consoleClearScreen();
        saveElement.popAnswer();

        consoleInputReader.readLine();
    }

    /**
     * Clears console(prints 40 blank lines).
     */
    public static void consoleClearScreen() {
        for (int i = 0; i < 40; i++)
            System.out.println();
    }

}
