package ilsrep.poll.statistics;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "item" element.
 * 
 * @author Drac
 * 
 */
@XmlRootElement(name = "result")
public class Results {

    /**
     * Percent of xml, logically referenced by this list.
     */
    protected String percent = null;

    /**
     * Name of xml, logically referenced by this list.
     */
    protected String name = null;

    /**
     * @see #percent
     */
    @XmlAttribute(required = true)
    public String getPercent() {
        return percent;
    }

    /**
     * @see #percent
     */
    public void setPercent(String percent) {
        this.percent = percent;
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
