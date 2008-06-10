package ua.com.interlogic.ils.task7.model;

import java.util.List;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlElementWrapper;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "poll" element.
 * 
 * @author Taras Kostiak
 * 
 */
@XmlRootElement(name = "poll")
public class PollElement {

    protected String id = null;

    protected String name = null;

    protected DescriptionElement description = null;

    protected List<ChoiceElement> choices = null;

    /**
     * @see #description
     */
    public DescriptionElement getDescription() {
        return description;
    }

    /**
     * @see #description
     */
    public void setDescription(DescriptionElement description) {
        this.description = description;
    }

    /**
     * @see #choices
     */
    public List<ChoiceElement> getChoices() {
        return choices;
    }

    /**
     * @see #choices
     */
    @XmlElementWrapper(name = "choices")
    @XmlElementRef
    public void setChoices(List<ChoiceElement> choices) {
        this.choices = choices;
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

    /**
     * @see #name
     */
    @XmlAttribute
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
