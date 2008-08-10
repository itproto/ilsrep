package ilsrep.poll.common;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlAttribute;
/**
 * Saves answer session
 * 
 * @author DRC
 * 
 */
 @XmlRootElement(name = "pollresult")
public class AnswerItem {
	protected String poll;
	protected String choice="-1";
	protected String customChoice=null;
	
	public AnswerItem(int poll_id,int choice_id){
		poll=Integer.toString(poll_id);
		choice=Integer.toString(choice_id);
				}
				public AnswerItem(){}
       public AnswerItem(int poll_id,String choice_id){
		poll=Integer.toString(poll_id);
		customChoice=choice_id;
				}
	//
	@XmlAttribute(name = "questionid")
    public String getQuestionId() {
        return poll;
    }

    public void setQuestionId(String name) {
        this.poll = name;
    }	
	//
		//
	@XmlAttribute(name = "answerid")
    public String getAnswerId() {
        return choice;
    }

    public void setAnswerId(String name) {
        this.choice = name;
    }	
	//
	//
	@XmlAttribute(name = "customchoice")
    public String getCustomChoice() {
        return customChoice;
    }

    public void setCustomChoice(String name) {
        this.customChoice = name;
    }	
	//
	
	
	}