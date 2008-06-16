package ilsrep.poll.model;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "choice" element.
 * 
 * @author Taras Kostiak
 * 
 */
@XmlRootElement(name = "choice")
public class Choice {

    /**
     * Choice identifier.
     */
    protected String id = null;

    /**
     * Choice text.
     */
    protected String name = null;

    /**
     * @see #id
     */
    @XmlAttribute(required = true)
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
     * @see #name
     */
    @XmlAttribute(required = true)
    public String getName() {
        return name;
    }

    
    /**
     * @see #name
     */
    public void setName(String name) {
        this.name = name;
    }

}
