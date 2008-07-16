package ilsrep.poll.common;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "item" element.
 * 
 * @author TKOST
 * 
 */
@XmlRootElement(name = "item")
public class Item {

    /**
     * Id of xml, logically referenced by this list.
     */
    protected String id = null;

    /**
     * Name of xml, logically referenced by this list.
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
