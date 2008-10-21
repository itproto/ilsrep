package ilsrep.poll.common.model;

import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlValue;

/**
 * The "description" element.
 * 
 * @author TKOST
 * 
 */
@XmlRootElement(name = "description")
public class Description {

    /**
     * Value of element.
     */
    protected String value = null;

    /**
     * @see #value
     */
    @XmlValue
    public String getValue() {
        return value;
    }

    /**
     * @see #value
     */
    public void setValue(String value) {
        this.value = value;
    }

    /**
     * @see java.lang.Object#clone()
     */
    @Override
    public Object clone() throws CloneNotSupportedException {
        Description clonedDescription = new Description();
        clonedDescription.setValue(value);

        return clonedDescription;
    }

}
