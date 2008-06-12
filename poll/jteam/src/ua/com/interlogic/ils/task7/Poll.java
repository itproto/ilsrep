package ua.com.interlogic.ils.task7;

import java.io.File;

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
     */
    public static void main(String[] args) throws JAXBException {
        // Serialising xml file into memory.
        JAXBContext cont = JAXBContext.newInstance(PollsElement.class);
        Unmarshaller um = cont.createUnmarshaller();
        File f = new File("Polls.xml");
        PollsElement pls = (PollsElement) um.unmarshal(f);

        // Processing polls.
        LogSaver plog = new LogSaver();

        for (PollElement e : pls.getPolls()) {
            e.queryUser();
            plog.pushMe(e.getName(), e.selection);
        }

        // Showing poll results.
        plog.popMe();
    }

}
