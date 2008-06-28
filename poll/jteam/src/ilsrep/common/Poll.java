package ilsrep.common;

import ilsrep.client.PollClient;

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
public class Poll {

    /**
     * Poll's ID.
     */
    protected String id = null;
    
public String pass="FAIL";
    /**
     * Poll's name.
     */
    protected String name = null;

    /**
     * Poll's description, aka question.
     */
    protected Description description = null;

    /**
     * List of choices.
     */
    protected List<Choice> choices = null;

    /**
     * Shows if custom choice is enabled("true" if enbled).
     */
    protected String customEnabled = null;

    /**
     * @see #description
     */
    public Description getDescription() {
        return description;
    }

    /**
     * @see #description
     */
    public void setDescription(Description description) {
        this.description = description;
    }

    /**
     * @see #choices
     */
    public List<Choice> getChoices() {
        return choices;
    }
/**
*@see #correct choice
*/
protected String correctChoice="-1";
    /**
     * @see #choices
     */
    @XmlElementWrapper(name = "choices")
    @XmlElementRef
    public void setChoices(List<Choice> choices) {
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
     * @throws IOException
     *             When I/O exception occurs.
     */
    public String queryUser() throws IOException {
        PollClient.consoleClearScreen();
        System.out.println("Name: " + this.getName());
        System.out.println("Desription: " + this.getDescription().getValue());
        for (Choice cur : this.getChoices()) {
            System.out.println("( " + cur.getId() + " ) " + cur.getName());
        }
        if (checkCustomEnabled())
            System.out.println("( 0 ) for your choice");

        String selection = null;
        int selectionId = -1;
        //reading input data
        InputStreamReader converter = new InputStreamReader(System.in);
        BufferedReader input = new BufferedReader(converter);
        try {
            selectionId = Integer.parseInt(input.readLine());
        }
        catch (NumberFormatException e) {
        }
         //checking whether to output custom choice line
                if (checkCustomEnabled() && selectionId == 0) {
	        
            System.out.print("Please enter your choice: ");
            selection = input.readLine();
        }
        else
            for (Choice cur : this.getChoices()) {
	      // converting selection number to what it represents      
	      
                if (selectionId == Integer.parseInt(cur.getId()))
                    selection = cur.getName();
                   
            }
             if (selectionId == Integer.parseInt(this.getCorrectChoice())) this.pass="PASS";

           
        System.out.println();
//return the selected element

        return selection;
    }

    /**
     * @see #customEnabled
     */
    @XmlAttribute(name = "customChoiceEnabled")
    public String getCustomEnabled() {
        return customEnabled;
    }
    /**
     * @see #customEnabled
     */
    public void setCustomEnabled(String customEnabled) {
        this.customEnabled = customEnabled;
    }

    /**
     * Shows if custom choice is enabled.
     * 
     * @return True, if custom choice is enabled.
     */
    public boolean checkCustomEnabled() {
        return (customEnabled != null)
                && (customEnabled.compareTo("true") == 0);
    }

    @XmlAttribute(name = "correctChoice")
    public String getCorrectChoice() {
        return correctChoice;
    }
    public void setCorrectChoice(String correctChoice) {
        this.correctChoice = correctChoice;
    }

   
}
