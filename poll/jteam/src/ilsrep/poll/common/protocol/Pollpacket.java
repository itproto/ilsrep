package ilsrep.poll.common.protocol;


import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlRootElement;

import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.Answers;
import ilsrep.poll.common.protocol.User;
/**
 * The "pollpacket" element.
 * 
 * @author TKOST
 * 
 */
@XmlRootElement(name = "pollpacket")
public class Pollpacket {

    /**
     * Client's request to server.
     */
    protected Request request = null;

    /**
     * Server's response for client's request of pollsession list stored on
     * server.
     */
  

    protected Pollsessionlist pollsessionList = null;
     /**
     * A set of answer data
     */
    
      protected Answers answerlist = null;
      
      protected User user=null;

    /**
     * Used when:
     * <ul>
     * <li>server sends poll xml for client's request</li>
     * <li>editor sends xml created by him to server</li>
     * </ul>
     */
    protected Pollsession pollsession = null;

    /**
     * @see #request
     */
    public Request getRequest() {
        return request;
    }

    /**
     * @see #request
     */
    public void setRequest(Request request) {
        this.request = request;
    }

    /**
     * @see #pollsessionList
     */
    @XmlElement(name = "pollsessionlist")
    public Pollsessionlist getPollsessionList() {
        return pollsessionList;
    }

    /**
     * @see #pollsessionList
     */
    public void setPollsessionList(Pollsessionlist pollsessionList) {
        this.pollsessionList = pollsessionList;
    }

    /**
     * @see #pollsession
     */
    public Pollsession getPollsession() {
        return pollsession;
    }

    /**
     * @see #pollsession
     */

    public void setPollsession(Pollsession pollsession) {
        this.pollsession = pollsession;
    }
/**
     * @see #answerlist
     */
    @XmlElement(name = "resultslist")
    public void setResultsList(Answers answer) {
        this.answerlist = answer;
    }
/**
     * @see #answerlist
     */
    public Answers getResultsList() {
        return answerlist;
    }

/**
     * @see #user
     */
    @XmlElement(name = "user")
    public void setUser(User user) {
        this.user = user;
    }

    public User getUser() {
        return user;
    }
}
