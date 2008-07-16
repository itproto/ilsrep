package ilsrep.poll.common;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "request" element.
 * 
 * @author TKOST
 * 
 */
@XmlRootElement(name = "request")
public class Request {

    /**
     * Indicates of type of request: get list of xml's stored on server.
     */
    public static final String TYPE_LIST = "list";

    /**
     * Indicates of type of request: get pollxml by id.
     */
    public static final String TYPE_POLLXML = "pollxml";

    /**
     * Type of request:
     * <ul>
     * <li>list {@link Request#TYPE_LIST}</li>
     * <li>pollxml {@link Request#TYPE_POLLXML}<br>
     * This one also require id ({@link Request#id}) field be filled</li>
     * </ul>
     * 
     */
    protected String type = null;

    /**
     * Id of xml to get.<br>
     * Used with type {@link Request#TYPE_POLLXML}.
     */
    protected String id = null;

    /**
     * @see #type
     */
    @XmlAttribute(required = true)
    public String getType() {
        return type;
    }

    /**
     * @see #type
     */
    public void setType(String type) {
        this.type = type;
    }

    /**
     * @see #id
     */
    @XmlAttribute
    public String getId() {
        return id;
    }

    /**
     * @see #id
     */
    public void setId(String id) {
        this.id = id;
    }

}
