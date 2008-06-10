package ua.com.interlogic.ils.task7;

import java.io.File;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;

import ua.com.interlogic.ils.task7.model.ChoiceElement;
import ua.com.interlogic.ils.task7.model.PollElement;
import ua.com.interlogic.ils.task7.model.PollsElement;

/**
 * @author Taras Kostiak
 * 
 */
public class Poll {

    public static void main(String[] args) throws JAXBException {
        // TODO: написати poll-програму :)

        JAXBContext cont = JAXBContext.newInstance(PollsElement.class);
        Unmarshaller um = cont.createUnmarshaller();
        File f = new File("Polls.xml");
        PollsElement pls = (PollsElement) um.unmarshal(f);

        for (PollElement e : pls.getPolls()) {
            System.out.println("Id: " + e.getId());
            System.out.println("Name: " + e.getName());
            System.out.println("Desription: " + e.getDescription().getValue());
            for (ChoiceElement el : e.getChoices()) {
                System.out.println("Choice(id = " + el.getId() + "): "
                        + el.getName());
            }
            System.out.println();
        }
    }

}
