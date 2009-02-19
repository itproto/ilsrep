package ilsrep.sender.protocol;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * @author TKOST
 * 
 */
@XmlRootElement(name = "url")
public class Url {

    protected String link = null;

    @XmlAttribute(name = "link")
    public String getLink() {
        return link;
    }

    public void setLink(String link) {
        this.link = link;
    }

}
