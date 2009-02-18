package ilsrep.sender.protocol;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * @author TKOST
 * 
 */
@XmlRootElement(name = "response")
public class Response {

    protected boolean isOk = false;

    protected String message = null;

    @XmlAttribute(name = "isOk")
    public boolean isOk() {
        return isOk;
    }

    public void setOk(boolean isOk) {
        this.isOk = isOk;
    }

    @XmlAttribute(name = "message")
    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

}
