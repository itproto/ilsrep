package ilsrep.poll.common;
import ilsrep.poll.common.Poll;
import ilsrep.poll.common.Pollsession;
import ilsrep.poll.common.AnswerItem;
import ilsrep.poll.common.Pollpacket;
import ilsrep.poll.common.Answers;
import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.UnknownHostException;
import java.util.List;
import java.util.ArrayList;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;
import java.util.jar.Manifest;
import java.util.jar.JarFile;
import java.util.jar.JarFile;
import java.util.jar.Attributes;
import java.io.StringWriter;
import java.io.StringReader;
import java.io.StringWriter;
public class tastme{
	   public static void main(String[] args) throws JAXBException, IOException {
		try{String a="<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><pollpacket><resultslist username=\"1\" pollsessionid=\"1\"></resultslist></pollpacket>";
		a="<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><pollpacket><resultslist username=\"1\" pollsessionid=\"1\"><pollresult questionid=\"1\" answerid=\"1\"/><pollresult questionid=\"2\" answerid=\"5\"/></resultslist></pollpacket>";
		      
		   JAXBContext cont = JAXBContext.newInstance(Pollpacket.class);
        Unmarshaller um = cont.createUnmarshaller();
		    StringReader reader = new StringReader(a.toString().trim());

    Pollpacket     receivedPacket = (Pollpacket) um.unmarshal(reader); 
    System.out.println("Helo");}catch(Exception e){System.out.println(e.getMessage());};
         
         
	   }
	
	
	
	
	
	}