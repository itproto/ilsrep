package ua.com.interlogic.ils.task7.model;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.List;
import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlElementWrapper;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "poll" element.
 * 
 * @author TKOST
 * @author DCR
 */
@XmlRootElement(name = "poll")
public class PollElement {

    /**
     * Poll's ID.
     */
    protected String id = null;

    /**
     * Poll's name.
     */
    protected String name = null;

    /**
     * Poll's description, aka question.
     */
    protected DescriptionElement description = null;

    /**
     * List of choices.
     */
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

    /**
     * Polls user in console for selection.
     * 
     * @return Choice, or null if user entered wrong choice.
     */
    public String queryUser() {
        System.out.println("Name: " + this.getName());
        System.out.println("Desription: " + this.getDescription().getValue());
        for (ChoiceElement el : this.getChoices()) {
            System.out.println("| " + el.getId() + "| " + el.getName());

        }

        String selection = null;
        int sid = -1;
        InputStreamReader converter = new InputStreamReader(System.in);
        BufferedReader in = new BufferedReader(converter);
        try {
            sid = Integer.parseInt(in.readLine());
        }
        catch (NumberFormatException e) {
        }
        catch (IOException e) {
        }

        for (ChoiceElement el : this.getChoices()) {
            if (sid == Integer.parseInt(el.getId()))
                selection = el.getName();
        }

        System.out.println();

        return selection;
    }

}
