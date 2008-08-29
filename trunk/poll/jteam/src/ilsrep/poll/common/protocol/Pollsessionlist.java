package ilsrep.poll.common.protocol;

import ilsrep.poll.common.model.Item;

import java.util.List;

import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "pollsessionlist" element.
 * 
 * @author TKOST
 * 
 */
@XmlRootElement(name = "pollsessionlist")
public class Pollsessionlist {

    /**
     * Items of this pollsession list.<br>
     * {@link Item}
     * 
     */
    protected List<Item> items = null;

    /**
     * @see #items
     */
    @XmlElementRef
    public List<Item> getItems() {
        return items;
    }

    /**
     * @see #items
     */
    public void setItems(List<Item> items) {
        this.items = items;
    }

}
