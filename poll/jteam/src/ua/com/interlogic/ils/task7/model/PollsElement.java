package ua.com.interlogic.ils.task7.model;

import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "polls" element.
 * 
 * @author Taras Kostiak
 * 
 */
@XmlRootElement(name = "polls")
public class PollsElement {

    /**
     * "poll" elements.
     */
    protected List<PollElement> polls = null;

    /**
     * @see #polls
     */
    @XmlElementRef
    public List<PollElement> getPolls() {
        return polls;
    }

    /**
     * @see #polls
     */
    public void setPolls(List<PollElement> polls) {
        this.polls = polls;
    }

}
