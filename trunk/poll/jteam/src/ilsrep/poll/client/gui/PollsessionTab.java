package ilsrep.poll.client.gui;

import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.AnswerItem;
import ilsrep.poll.common.protocol.Answers;

import java.awt.Component;
import java.awt.Dimension;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;
import java.util.Enumeration;

import javax.swing.AbstractButton;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JTextField;

/**
 * This is JPanel with contents to process pollsession.
 * 
 * @author TKOST
 * 
 */
public class PollsessionTab extends JPanel implements ActionListener {

    /**
     * Serial version UID.
     */
    private static final long serialVersionUID = 1379968886853357259L;

    /**
     * Stands for "next frame" action(ask next Poll).
     */
    public static final String NEXT_ACTION = "next";

    /**
     * Stands for "finish" action(close tab).
     */
    public static final String FINISH_ACTION = "finish";

    /**
     * Name of custom choice radio button.
     */
    public static final String CUSTOM_CHOICE_RADIO_BUTTON_NAME = "custom";

    /**
     * Text of custom choice radio button.
     */
    public static final String CUSTOM_CHOICE_RADIO_BUTTON_TEXT = "custom choice";

    /**
     * Text of custom choice radio button.
     */
    public static final String CUSTOM_CHOICE_RADIO_BUTTON_ACTION = "customRadioButtonSelected";

    /**
     * Text of custom choice radio button.
     */
    public static final String ANOTHER_RADIO_BUTTON_ACTION = "anotherRadioButtonSelected";

    /**
     * Pollsession that is proceed by this tab.
     */
    private Pollsession session = null;

    /**
     * Window that contains this tab.
     */
    private MainWindow owningWindow = null;

    /**
     * "Header" to each "frame".
     */
    private JPanel header = null;

    /**
     * "Footer" to each "frame".
     */
    private JPanel footer = null;

    /**
     * Indicates if session was already started, for preventing second launch.
     */
    private boolean started = false;

    /**
     * Indicates what Poll is beeing asked now.
     */
    private int currentFrame = -1;

    /**
     * "Next" button of this tab.<br>
     * At end of passing pollsession is used as "Finish"
     */
    private JButton nextButton = null;

    /**
     * Results of pollsession.
     */
    private Answers pollResultList = null;

    /**
     * Choices of each poll.
     */
    private ButtonGroup choices = null;

    /**
     * Radio button to select custom choice.
     */
    private JRadioButton customChoiceRadioButton = null;

    /**
     * Field to enter custom choice.
     */
    private JTextField customChoiceField = null;

    /**
     * Creates new <code>PollsessionTab</code>, for passing given pollsession.
     * 
     * @param session
     *            Pollsession, to process by this tab.
     */
    public PollsessionTab(Pollsession session, MainWindow owningWindow) {
        this.session = session;
        this.owningWindow = owningWindow;

        pollResultList = new Answers();
        pollResultList.setAnswers(new ArrayList<AnswerItem>());
        pollResultList.setPollSesionId(session.getId());
    }

    /**
     * Starts passing pollsession.
     */
    public void start() {
        if (started)
            return;
        else
            started = true;

        // Creating frame header.
        header = new JPanel();

        header.add(new JLabel(session.getName() + " (ID: " + session.getId()
                + ")"));

        // Creating frame footer.
        footer = new JPanel();

        BoxLayout footerLayout = new BoxLayout(footer, BoxLayout.LINE_AXIS);

        footer.setLayout(footerLayout);

        footer.add(Box.createHorizontalGlue());

        nextButton = new JButton("Next");
        nextButton.setActionCommand(NEXT_ACTION);
        nextButton.addActionListener(this);

        footer.add(nextButton);

        actionPerformed(null);
    }

    /**
     * Action <code>next</code> activates next frame.
     * 
     * @see java.awt.event.ActionListener#actionPerformed(java.awt.event.ActionEvent)
     */
    @Override
    public void actionPerformed(ActionEvent e) {
        // Default size of text field.
        final int collumsCount = 20;

        // Space between some components.
        final int componentSpace = 5;

        JPanel framePanel = null;
        if (e == null || e.getActionCommand().equals(NEXT_ACTION)) {
            framePanel = new JPanel();
            if (currentFrame == -1) {
                JLabel pollsessionIdLabel = new JLabel("Id");

                JTextField pollsessionIdField = new JTextField(collumsCount);
                pollsessionIdField.setText(session.getId());
                pollsessionIdField.setEditable(false);

                JLabel pollsessionNameLabel = new JLabel("Name");

                JTextField pollsessionNameField = new JTextField(collumsCount);
                pollsessionNameField.setText(session.getName());
                pollsessionNameField.setEditable(false);

                JLabel pollsessionDateLabel = new JLabel("Date of creation");

                JTextField pollsessionDateField = new JTextField(collumsCount);
                pollsessionDateField.setText(session.getDate());
                pollsessionDateField.setEditable(false);

                JLabel pollsessionLabel = new JLabel("Pollsession");
                JLabel infoLabel = new JLabel("information");

                framePanel.setLayout(new GridBagLayout());

                GridBagConstraints c = new GridBagConstraints();

                c.anchor = GridBagConstraints.LAST_LINE_END;
                c.gridx = 0;
                c.gridy = 0;
                c.insets.bottom = componentSpace;
                framePanel.add(pollsessionLabel, c);

                c.anchor = GridBagConstraints.LAST_LINE_START;
                c.gridx = 1;
                c.gridy = 0;
                c.insets.left = componentSpace;
                framePanel.add(infoLabel, c);

                c.anchor = GridBagConstraints.FIRST_LINE_START;
                c.gridx = 0;
                c.gridy = 1;
                c.insets.left = 0;
                c.insets.right = 5;
                framePanel.add(pollsessionIdLabel, c);

                c.gridx = 1;
                c.gridy = 1;
                framePanel.add(pollsessionIdField, c);

                c.gridx = 0;
                c.gridy = 2;
                framePanel.add(pollsessionNameLabel, c);

                c.gridx = 1;
                c.gridy = 2;
                framePanel.add(pollsessionNameField, c);

                c.gridx = 0;
                c.gridy = 3;
                framePanel.add(pollsessionDateLabel, c);

                c.gridx = 1;
                c.gridy = 3;
                framePanel.add(pollsessionDateField, c);
            }
            else
                if (currentFrame >= 0
                        && currentFrame <= session.getPolls().size()) {
                    String radioBoxChoice = null;

                    if (choices != null) {
                        Enumeration<AbstractButton> radioBoxes = choices
                                .getElements();

                        while (radioBoxes.hasMoreElements()) {
                            AbstractButton currentBox = (AbstractButton) radioBoxes
                                    .nextElement();

                            if (currentBox.isSelected()) {
                                radioBoxChoice = currentBox.getName();
                                break;
                            }
                        }

                        if (radioBoxChoice == null
                                || (radioBoxChoice
                                        .equals(CUSTOM_CHOICE_RADIO_BUTTON_NAME)
                                        && customChoiceField != null && customChoiceField
                                        .getText().isEmpty())) {
                            GUIUtilities
                                    .showWarningDialog("You didn't select your choice!");
                            return;
                        }
                        else {
                            Poll poll = session.getPolls()
                                    .get(currentFrame - 1);

                            AnswerItem answerItem = new AnswerItem();
                            answerItem.setQuestionId(poll.getId());
                            if (radioBoxChoice
                                    .equals(CUSTOM_CHOICE_RADIO_BUTTON_NAME)) {
                                answerItem.setCustomChoice(customChoiceField
                                        .getText());
                            }
                            else
                                answerItem.setAnswerId(radioBoxChoice);

                            pollResultList.getAnswers().add(answerItem);
                        }
                    }

                    if (currentFrame == session.getPolls().size()) {
                        // Show results and "Click "Next" to send results".

                        BoxLayout pageLayout = new BoxLayout(framePanel,
                                BoxLayout.PAGE_AXIS);

                        framePanel.setLayout(pageLayout);

                        framePanel.add(new JLabel("Results should go here!"));
                        framePanel.add(new JLabel(
                                "Click \"Next\" to send results"));
                    }
                    else {
                        Poll poll = session.getPolls().get(currentFrame);

                        JLabel pollIdLabel = new JLabel("Id");
                        JTextField pollIdField = new JTextField(collumsCount);
                        pollIdField.setText(poll.getId());
                        pollIdField.setEditable(false);

                        JLabel pollNameLabel = new JLabel("Name");
                        JTextField pollNameField = new JTextField(collumsCount);
                        pollNameField.setText(poll.getName());
                        pollNameField.setEditable(false);

                        JLabel pollDescriptionLabel = new JLabel("Description");
                        JTextField pollDescriptionField = new JTextField(
                                collumsCount);
                        pollDescriptionField.setText(poll.getDescription()
                                .getValue());
                        pollDescriptionField.setEditable(false);

                        GridBagLayout layout = new GridBagLayout();
                        framePanel.setLayout(layout);

                        GridBagConstraints c = new GridBagConstraints();

                        JLabel pollLabel = new JLabel("Poll");
                        JLabel infoLabel = new JLabel("information");

                        c.anchor = GridBagConstraints.LAST_LINE_END;
                        c.gridx = 0;
                        c.gridy = 0;
                        c.insets.bottom = componentSpace;
                        framePanel.add(pollLabel, c);

                        c.anchor = GridBagConstraints.LAST_LINE_START;
                        c.gridx = 1;
                        c.gridy = 0;
                        c.insets.left = componentSpace;
                        framePanel.add(infoLabel, c);

                        c.gridx = 0;
                        c.gridy = 1;
                        c.insets.left = 0;
                        c.insets.right = componentSpace;
                        c.anchor = GridBagConstraints.FIRST_LINE_START;
                        framePanel.add(pollIdLabel, c);

                        c.gridx = 1;
                        c.gridy = 1;
                        framePanel.add(pollIdField, c);

                        c.gridx = 0;
                        c.gridy = 2;
                        framePanel.add(pollNameLabel, c);

                        c.gridx = 1;
                        c.gridy = 2;
                        framePanel.add(pollNameField, c);

                        c.gridx = 0;
                        c.gridy = 3;
                        framePanel.add(pollDescriptionLabel, c);

                        c.gridx = 1;
                        c.gridy = 3;
                        framePanel.add(pollDescriptionField, c);

                        // Creating and adding choices.
                        choices = new ButtonGroup();

                        int lastYGridPosition = 3;

                        c.gridx = 0;

                        for (Choice choice : poll.getChoices()) {
                            JRadioButton choiceRadioButton = new JRadioButton();
                            choiceRadioButton.setName(choice.getId());
                            choiceRadioButton.setText(choice.getName());
                            choiceRadioButton.addActionListener(this);
                            choiceRadioButton
                                    .setActionCommand(ANOTHER_RADIO_BUTTON_ACTION);

                            choices.add(choiceRadioButton);

                            c.gridy = ++lastYGridPosition;
                            framePanel.add(choiceRadioButton, c);
                        }

                        if (poll.checkCustomEnabled()) {
                            customChoiceRadioButton = new JRadioButton();
                            customChoiceRadioButton
                                    .setName(CUSTOM_CHOICE_RADIO_BUTTON_NAME);
                            customChoiceRadioButton
                                    .setText(CUSTOM_CHOICE_RADIO_BUTTON_TEXT);
                            customChoiceRadioButton.addActionListener(this);
                            customChoiceRadioButton
                                    .setActionCommand(CUSTOM_CHOICE_RADIO_BUTTON_ACTION);
                            choices.add(customChoiceRadioButton);

                            customChoiceField = new JTextField(collumsCount);
                            customChoiceField.setEnabled(false);

                            c.gridy = ++lastYGridPosition;
                            framePanel.add(customChoiceRadioButton, c);

                            c.gridx = 1;
                            framePanel.add(customChoiceField, c);
                        }
                    }
                }
                else {
                    nextButton.setText("Finish");
                    nextButton.setActionCommand(FINISH_ACTION);

                    BoxLayout pageLayout = new BoxLayout(framePanel,
                            BoxLayout.PAGE_AXIS);

                    framePanel.setLayout(pageLayout);

                    framePanel.add(new JLabel("Results sent!"));
                    framePanel
                            .add(new JLabel("Click \"Finish\" to close tab."));

                    owningWindow.sendResults(pollResultList);
                }

            currentFrame++;

            BoxLayout pollsessionTabLayout = new BoxLayout(this,
                    BoxLayout.PAGE_AXIS);

            setLayout(pollsessionTabLayout);

            removeAll();

            add(header);
            add(Box.createVerticalGlue());
            add(framePanel);
            add(Box.createVerticalGlue());
            add(footer);
        }
        else
            if (e.getActionCommand().equals(FINISH_ACTION)) {
                owningWindow.removeTabByInstance(this);
            }
            else
                if (e.getActionCommand().equals(
                        CUSTOM_CHOICE_RADIO_BUTTON_ACTION)) {
                    if (customChoiceField != null)
                        customChoiceField.setEnabled(true);
                }
                else
                    if (e.getActionCommand()
                            .equals(ANOTHER_RADIO_BUTTON_ACTION)) {
                        if (customChoiceField != null)
                            customChoiceField.setEnabled(false);
                    }
    }

    /**
     * Sets maximum, minimum and actual size of component as requested.
     * 
     * @param component
     *            Component, to set size.
     * @param size
     *            Size, to set.
     */
    public static void setSize(Component component, Dimension size) {
        component.setMaximumSize(size);
        component.setMinimumSize(size);
        component.setSize(size);
    }

}
