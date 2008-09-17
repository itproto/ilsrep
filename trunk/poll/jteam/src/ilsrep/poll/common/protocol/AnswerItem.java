package ilsrep.poll.common.protocol;

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

    /**
     * ID od poll
     */
    protected String poll = null;

    /**
     * ID of entered choice
     */
    protected String choice = "-1";

    /**
     * custom choice
     */
    protected String customChoice = null;

    /**
     * Sets poll id and choice id
     * 
     * @param poll_id
     *            Id of poll.
     * @param choice_id
     *            id of entered choice
     * 
     * 
     * 
     */
    public AnswerItem setItem(int poll_id, int choice_id) {
        poll = Integer.toString(poll_id);
        choice = Integer.toString(choice_id);
        return this;
    }

    /**
     * Sets poll id and custom choice
     * 
     * @param poll_id
     *            Id of poll.
     * @param choice_id
     *            a String of custom choice
     * 
     * 
     * 
     */
    public AnswerItem setItem(int poll_id, String choice_id) {
        poll = Integer.toString(poll_id);
        customChoice = choice_id;
        return this;
    }

    /**
     * @see #poll
     */
    @XmlAttribute(name = "questionid")
    public String getQuestionId() {
        return poll;
    }

    /**
     * @see #poll
     */
    public void setQuestionId(String name) {
        this.poll = name;
    }

    /**
     * @see #choice
     */
    @XmlAttribute(name = "answerid")
    public String getAnswerId() {
        return choice;
    }

    /**
     * @see #choice
     */
    public void setAnswerId(String name) {
        this.choice = name;
    }

    /**
     * @see #customChoice
     */
    @XmlAttribute(name = "customchoice")
    public String getCustomChoice() {
        return customChoice;
    }

    /**
     * @see #customChoice
     */
    public void setCustomChoice(String name) {
        this.customChoice = name;
    }

}
