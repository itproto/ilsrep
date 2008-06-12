package ua.com.interlogic.ils.task7;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;

import ua.com.interlogic.ils.task7.model.PollElement;
import ua.com.interlogic.ils.task7.model.PollsElement;
import ua.com.interlogic.ils.task7.model.LogSaver;

/**
 * Main class for task 7 - Poll.
 * 
 * @author TKOST
 * @author DCR
 * 
 */
public class Poll {

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
        BufferedReader r = new BufferedReader(new InputStreamReader(System.in));
        System.out.println("Welcome to polls client program!\n");
        System.out.print("Please enter your name: ");
        String name = r.readLine();
        System.out.print("Please enter filename to read poll xml "
                + "from\n[press enter for default \"Polls.xml\"]: ");
        String fileName = r.readLine();
        if (fileName.compareTo("") == 0)
            fileName = "Polls.xml";

        // Serialising xml file into object model.
        JAXBContext cont = JAXBContext.newInstance(PollsElement.class);
        Unmarshaller um = cont.createUnmarshaller();
        File f = new File(fileName);
        PollsElement pls = (PollsElement) um.unmarshal(f);

        // Showing xml, generated from already read object model.
        Marshaller mr = cont.createMarshaller();
        System.out.println();
        mr.setProperty("jaxb.formatted.output", true);
        mr.marshal(pls, System.out);

        // Processing polls.
        LogSaver plog = new LogSaver();

        System.out.print("\nOk, " + name + ", are you ready for poll? [y/n]");
        String yNChoice = r.readLine();
        if (!(yNChoice.compareTo("y") == 0))
            return;

        String choice = null;
        for (PollElement e : pls.getPolls()) {
            while (choice == null)
                choice = e.queryUser();
            plog.pushMe(e.getName(), choice);
            choice = null;
        }

        // Showing poll results.
        consoleClearScreen();
        plog.popMe();

        r.readLine();
    }

    /**
     * Clears console(prints 40 blank lines).
     */
    public static void consoleClearScreen() {
        for (int i = 0; i < 40; i++)
            System.out.println();
    }

}
