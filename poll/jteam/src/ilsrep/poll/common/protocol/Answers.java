package ilsrep.poll.common.protocol;

import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlAttribute;
/**
 * Class for string answer data.
 * 
 * 
 * 
 * @author DRC
 * 
 */
@XmlRootElement(name = "resultslist")
public class Answers {
/**
     * name of the user.
     */
	
    protected String username = null;
/**
     * id of the polsession
     */
    protected String id = null;
/**
     * list of polls and selected choices
     */
    protected List<AnswerItem> answerlist = null;

    /**
     * @see #username
     */
    @XmlAttribute(name = "username")
    public String getUsername() {
        return username;
    }
/**
     * @see #username
     */
    public void setUsername(String name) {
        this.username = name;
    }

    
/**
     * @see #id
     */
    
    @XmlAttribute(name = "pollsessionid")
    public String getPollSesionId() {
        return id;
    }
/**
     * @see #id
     */
    public void setPollSesionId(String name) {
        this.id = name;
    }

    
/**
     * @see #answerlist
     */
    @XmlElementRef
    public List<AnswerItem> getAnswers() {
        return answerlist;
    }
/**
     * @see #answerlist
     */
    public void setAnswers(List<AnswerItem> polls) {
        this.answerlist = polls;
    }

}
