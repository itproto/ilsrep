package ilsrep.poll.common.protocol;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlTransient;

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
    @XmlTransient
    public static final String TYPE_LIST = "getList";

    /**
     * Indicates of type of request: get pollxml by id.
     */
    @XmlTransient
    public static final String TYPE_POLLXML = "getPollsession";

    /**
     * Indicates of type of request: add new pollsession.
     */
    @XmlTransient
    public static final String TYPE_CREATE_POLLSESSION = "createPollsession";

    /**
     * Indicates of type of request: add remove existing pollsession at server.
     */
    @XmlTransient
    public static final String TYPE_REMOVE_POLLSESSION = "removePollsession";

    /**
     * Indicates of type of request: save pollsession seance results.
     */
    @XmlTransient
    public static final String TYPE_SAVE_RESULT = "saveResult";

    /**
     * Indicates of type of request: user management
     */
    @XmlTransient
    public static final String TYPE_USER = "user";

    /**
     * Indicates of type of request: update existing pollsession.
     */
    @XmlTransient
    public static final String TYPE_UPDATE_POLLSESSION = "updatePollsession";

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
