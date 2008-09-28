package ilsrep.poll.common.model;

import ilsrep.poll.client.PollClient;
import ilsrep.poll.client.gui.old.GUIUtil;

import java.io.IOException;
import java.util.List;

import javax.swing.ButtonGroup;
import javax.swing.JRadioButton;
import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlElementWrapper;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlTransient;

/**
 * The "poll" element.
 * 
 * @author TKOST
 * @author DRC
 */
@XmlRootElement(name = "poll")
public class Poll {

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
    protected Description description = null;

    /**
     * List of choices.
     */
    protected List<Choice> choices = null;

    /**
     * Shows if custom choice is enabled("true" if enabled).
     */
    protected String customEnabled = null;

    /**
     * Id of correct choice.
     */
    protected String correctChoice = null;

    /**
     * TODO: DRC: Fix comment.
     */
    @XmlTransient
    public String pass = "FAIL";

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
    @XmlElementWrapper(name = "choices")
    @XmlElementRef
    public List<Choice> getChoices() {
        return choices;
    }

    /**
     * @see #choices
     */
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

    @XmlTransient
    public int selectedId = 0;

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

        int pollNumber = 1;
        for (Choice cur : this.getChoices()) {
            System.out.println("( " + pollNumber + " ) " + cur.getName());
            pollNumber++;
        }
        if (checkCustomEnabled())
            System.out.println("( 0 ) for your choice");

        String selection = null;
        int selectionId = -1;
        // reading input data
        selectionId = Integer.parseInt(PollClient.readFromConsole(
                "Enter choice", PollClient.ANSWER_TYPE_INTEGER));

        while (selectionId < 0 || selectionId > getChoices().size())
            selectionId = Integer.parseInt(PollClient
                    .readFromConsole("Wrong answer, enter again",
                            PollClient.ANSWER_TYPE_INTEGER));

        // checking whether to output custom choice line
        if (checkCustomEnabled() && selectionId == 0) {
            selectedId = 0;
            selection = PollClient.readFromConsole("Please enter your choice");

        }
        else {
            selectionId = Integer.parseInt(getChoices().get(selectionId - 1)
                    .getId());
            selectedId = selectionId;
            for (Choice cur : this.getChoices()) {
                // converting selection number to what it represents

                if (selectionId == Integer.parseInt(cur.getId()))
                    selection = cur.getName();

            }
        }
        if (selectionId == Integer.parseInt(this.getCorrectChoice()))
            this.pass = "PASS";

        System.out.println();
        // return the selected element

        return selection;
    }

    public String queryUserGUI(GUIUtil win) throws IOException {
        String query = "<html>Name: " + this.getName() + "<br>Desription: "
                + this.getDescription().getValue() + "<html>";

        int pollNumber = 1;
        ButtonGroup group = new ButtonGroup();
        for (Choice cur : this.getChoices()) {
            JRadioButton jrb = new JRadioButton(cur.getName());
            // System.out.println(cur.getName() + "\n");
            jrb.setActionCommand(Integer.toString(pollNumber));
            group.add(jrb);
            pollNumber++;
        }
        if (checkCustomEnabled()) {
            JRadioButton jrb = new JRadioButton("your choice");
            jrb.setActionCommand("0");
            group.add(jrb);
        }
        String selection = null;
        int selectionId = -1;
        // reading input data
        selectionId = Integer.parseInt(win.getChoice(group, query));

        while (selectionId < 0 || selectionId > getChoices().size())
            selectionId = Integer.parseInt(win.getChoice(group, query));

        // checking whether to output custom choice line
        if (checkCustomEnabled() && selectionId == 0) {
            selectedId = 0;
            selection = win.askUser("eneter your choice");

        }
        else {
            selectionId = Integer.parseInt(getChoices().get(selectionId - 1)
                    .getId());
            selectedId = selectionId;
            for (Choice cur : this.getChoices()) {
                // converting selection number to what it represents

                if (selectionId == Integer.parseInt(cur.getId()))
                    selection = cur.getName();

            }
        }
        if (selectionId == Integer.parseInt(this.getCorrectChoice()))
            this.pass = "PASS";

        // System.out.println();
        // return the selected element

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

    /**
     * @see #correctChoice
     */
    @XmlAttribute(name = "correctChoice")
    public String getCorrectChoice() {
        return correctChoice != null ? correctChoice : "-1";
    }

    /**
     * @see #correctChoice
     */
    public void setCorrectChoice(String correctChoice) {
        this.correctChoice = correctChoice;
    }

    /**
     * Returns choice by id.
     * 
     * @param pollId
     *            Id of choice.
     * @return Choice matching given id or <code>null</code>, if such one not
     *         present.
     */
    public Choice getChoiceById(String choiceId) {
        for (Choice choice : choices) {
            if (choice.getId().equals(choiceId))
                return choice;
        }

        return null;
    }

}
