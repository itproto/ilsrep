package ilsrep.sender.protocol;

import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlElementWrapper;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * Root class for TabSender protocol.
 * 
 * @author TKOST
 * 
 */
@XmlRootElement(name = "tabsender")
public class TabSender {

    protected Action action = null;

    protected User user = null;

    protected List<Url> urls = null;

    protected Response response = null;

    public Action getAction() {
        return action;
    }

    public void setAction(Action action) {
        this.action = action;
    }

    public User getUser() {
        return user;
    }

    public void setUser(User user) {
        this.user = user;
    }

    @XmlElementWrapper(name = "urls")
    @XmlElementRef
    public List<Url> getUrls() {
        return urls;
    }

    public void setUrls(List<Url> urls) {
        this.urls = urls;
    }

    public Response getResponse() {
        return response;
    }

    public void setResponse(Response response) {
        this.response = response;
    }

}
