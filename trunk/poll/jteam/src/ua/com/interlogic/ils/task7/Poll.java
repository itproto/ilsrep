package ua.com.interlogic.ils.task7;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
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
        // Serialising xml file into memory.
        JAXBContext cont = JAXBContext.newInstance(PollsElement.class);
        Unmarshaller um = cont.createUnmarshaller();
        File f = new File("Polls2.xml");
        PollsElement pls = (PollsElement) um.unmarshal(f);

        // Processing polls.
        LogSaver plog = new LogSaver();

        String choice = null;
        for (PollElement e : pls.getPolls()) {
            while (choice == null)
                choice = e.queryUser();
            plog.pushMe(e.getName(), choice);
            choice = null;
        }

        // Showing poll results.
        plog.popMe();

        BufferedReader r = new BufferedReader(new InputStreamReader(System.in));
        r.readLine();
    }

}
