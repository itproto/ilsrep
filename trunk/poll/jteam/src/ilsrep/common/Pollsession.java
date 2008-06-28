package ilsrep.common;

import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlAttribute;

/**
 * The "polls" element.
 * 
 * @author Taras Kostiak
 * 
 */
@XmlRootElement(name = "pollsession")

public class Pollsession {
private String minScore="-1", name,id,testMode="false";
    /**
     * "poll" elements.
     */
    protected List<Poll> polls = null;

    /**
     * @see #polls
     */
      @XmlAttribute(name = "minScore")
    public String getMinScore() {
        return minScore;
    }
    public void setMinScore(String minScore) {
        this.minScore =minScore;
    }
    @XmlAttribute(name = "name")
   public String getName() {
        return name;
    }
    public void setName(String name) {
        this.name =name;
    }
    @XmlAttribute(name = "id")
   public String getId() {
        return id;
    }
    public void setId(String id) {
        this.id =id;
    }
    @XmlAttribute(name = "testMode")
   public String getTestMode() {
        return testMode;
    }
    public void setTestMode(String testMode) {
        this.testMode =testMode;
    }

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
