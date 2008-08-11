package ilsrep.poll.common;

import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlAttribute;

@XmlRootElement(name = "resultslist")
public class Answers {

    protected String username = null;

    protected String id = null;

    protected List<AnswerItem> answerlist = null;

    //
    @XmlAttribute(name = "username")
    public String getUsername() {
        return username;
    }

    public void setUsername(String name) {
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

    public void setAnswers(List<AnswerItem> polls) {
        this.answerlist = polls;
    }

}
