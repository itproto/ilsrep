package ilsrep.poll.common.model;

import java.util.ArrayList;
import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlAttribute;

/**
 * The "pollsession" element.
 * 
 * @author TKOST
 * 
 */
@XmlRootElement(name = "pollsession")
public class Pollsession {

    /*
     * TODO: DRC: Separate: each variable - its line; all are initialised with
     * <code>null</code>(fix logic where it interfere); each variable - must
     * have comment. Separated, but logic may corrupt.
     */

    /**
     * Pollsession id.
     */
    protected String id = null;

    /**
     * Pollsession name.
     */
    protected String name = null;

    /**
     * Minimal score to pass in test mode.
     */
    protected String minScore = null;

    /**
     * Date of creation.
     */
    protected String date = "";

    /**
     * "True" indicates of test mode.
     */
    protected String testMode = null;

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

    @XmlAttribute(name = "date")
    public String getDate() {
        return date;
    }

    /**
     * @see #id
     */
    public void setDate(String date) {
        this.date = date;
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

    /**
     * @see java.lang.Object#clone()
     */
    @Override
    public Object clone() throws CloneNotSupportedException {
        Pollsession clonedPollsession = new Pollsession();

        clonedPollsession.setId(id);
        clonedPollsession.setName(name);
        clonedPollsession.setMinScore(minScore);
        clonedPollsession.setDate(date);
        clonedPollsession.setTestMode(testMode);

        ArrayList<Poll> clonedPolls = new ArrayList<Poll>();
        for (Poll poll : polls)
            clonedPolls.add((Poll) poll.clone());

        clonedPollsession.setPolls(clonedPolls);

        return clonedPollsession;
    }

}
