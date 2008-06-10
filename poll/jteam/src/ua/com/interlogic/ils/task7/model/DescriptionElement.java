package ua.com.interlogic.ils.task7.model;

import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlValue;

/**
 * The "description" element. 
 * 
 * @author Taras Kostiak
 *
 */
@XmlRootElement(name = "description")
public class DescriptionElement {

    /**
     * Value of element.
     */
    protected String value = null;
    
    /**
     * @see value
     */
    @XmlValue
    public String getValue() {
        return value;
    }

    /**
     * @see value
     */
    public void setValue(String value) {
        this.value = value;
    }
    
}
