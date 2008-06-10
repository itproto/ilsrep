package ua.com.interlogic.ils.task7;

import java.io.File;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;

import ua.com.interlogic.ils.task7.model.ChoiceElement;
import ua.com.interlogic.ils.task7.model.PollElement;
import ua.com.interlogic.ils.task7.model.PollsElement;
import ua.com.interlogic.ils.task7.model.LogSaver;



/**
 * @author Taras Kostiak
 * 
 */
public class Poll {

    public static void main(String[] args) throws JAXBException {
       

        JAXBContext cont = JAXBContext.newInstance(PollsElement.class);
        Unmarshaller um = cont.createUnmarshaller();
        File f = new File("Polls.xml");
        PollsElement pls = (PollsElement) um.unmarshal(f);
LogSaver plog = new LogSaver();
       for (PollElement e : pls.getPolls()) {
	        
	        e.QueryUser();
	        plog.PushMe(e.getName(),e.Selection);
	         
    }                       
                        
            
           
           plog.PopMe();
       
    }

}
