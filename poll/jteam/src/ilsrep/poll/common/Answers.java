package ilsrep.poll.common;
import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlAttribute;
@XmlRootElement(name = "resultslist")
public class Answers{
	protected String username;
 protected String id;
	protected List<AnswerItem> answerlist;
	
	
	
	//
	@XmlAttribute(name = "username")
    public String getusername() {
        return username;
    }

    public void setusername(String name) {
        this.username = name;
    }	
	//
    
		//
	@XmlAttribute(name = "pollsessionid")
    public String getPollSesionId() {
        return id;
    }

    public void setPollSesionId(String name) {
        this.id = name;
    }	
	//
	
	
	@XmlElementRef
    public List<AnswerItem> getAnswers() {
        return answerlist;
    }

    /**
     * @see #polls
     */
    public void setPolls(List<AnswerItem> polls) {
        this.answerlist = polls;
    }
    
    
    
    }