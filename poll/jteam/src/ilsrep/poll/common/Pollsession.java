package ilsrep.poll.common;

import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlAttribute;

/**
 * The "polls" element.
 * 
 * @author TKOST
 * 
 */
@XmlRootElement(name = "pollsession")
public class Pollsession {

    /**
     * TODO: DRC: Separate: each variable - its line; all are initialised with
     * <code>null</code>(fix logic where it interfere); each variable - must
     * have comment.
     */
    private String minScore = "-1", name, id, testMode = "false";

    /**
     * "poll" elements.
     */
    protected List<Poll> polls = null;

    /**
     * @see #minScore
     */
    @XmlAttribute(name = "minScore")
    public String getMinScore() {
        return minScore;
    }

    public void setMinScore(String minScore) {
        this.minScore = minScore;
    }

    /**
     * @see #name
     */
    @XmlAttribute(name = "name")
    public String getName() {
        return name;
    }

    /**
     * @see #name
     */
    public void setName(String name) {
        this.name = name;
    }

    /**
     * @see #id
     */
    @XmlAttribute(name = "id")
    public String getId() {
        return id;
    }

    /**
     * @see #id
     */
    public void setId(String id) {
        this.id = id;
    }

    /**
     * @see #testMode
     */
    @XmlAttribute(name = "testMode")
    public String getTestMode() {
        return testMode;
    }

    /**
     * @see #testMode
     */
    public void setTestMode(String testMode) {
        this.testMode = testMode;
    }

    /**
     * @see #polls
     */
    @XmlElementRef
    public List<Poll> getPolls() {
        return polls;
    }

    /**
     * @see #polls
     */
    public void setPolls(List<Poll> polls) {
        this.polls = polls;
    }

}
