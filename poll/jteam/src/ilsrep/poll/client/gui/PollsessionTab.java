package ilsrep.poll.client.gui;

import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.AnswerItem;
import ilsrep.poll.common.protocol.Answers;

import java.awt.Component;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;
import java.util.Enumeration;

import javax.swing.AbstractButton;
import javax.swing.BorderFactory;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JTextArea;
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
     * Font for "big" labels(labels with big font size).
     */
    private Font bigLabelsFont = null;

    /**
     * Indicates if show decor(for ChangeListener in tabbed pane of main
     * window).
     */
    private boolean showDecor = false;

    /**
     * If render header in each frame.
     */
    private boolean showHeader = false;

    /**
     * If render footer in each frame.
     */
    private boolean showFooter = false;

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

        // Label for getting default font name for label.
        JLabel testLabel = new JLabel();
        bigLabelsFont = new Font(testLabel.getFont().getName(), Font.BOLD, 16);
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

        BoxLayout headerLayout = new BoxLayout(header, BoxLayout.LINE_AXIS);
        header.setLayout(headerLayout);

        header.setBorder(BorderFactory.createEmptyBorder(6, 6, 0, 0));

        header.add(createColoredLabel("Poll session", session.getName()));
        header.add(Box.createHorizontalGlue());

        // Creating frame footer.
        footer = new JPanel();

        BoxLayout footerLayout = new BoxLayout(footer, BoxLayout.LINE_AXIS);

        footer.setLayout(footerLayout);

        footer.add(Box.createHorizontalGlue());

        nextButton = new JButton("Start");
        nextButton.setActionCommand(NEXT_ACTION);
        nextButton.addActionListener(this);

        JPanel nextButtonPanel = new JPanel();
        nextButtonPanel.setLayout(new BoxLayout(nextButtonPanel,
                BoxLayout.X_AXIS));
        nextButtonPanel.add(nextButton);

        final int borderThickness = 6;

        nextButtonPanel.setBorder(BorderFactory.createEmptyBorder(
                borderThickness, borderThickness, borderThickness,
                borderThickness));

        footer.add(nextButtonPanel);

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
                // JLabel pollsessionIdLabel = new JLabel("Id");
                //
                // JTextField pollsessionIdField = new JTextField(collumsCount);
                // pollsessionIdField.setText(session.getId());
                // pollsessionIdField.setEditable(false);

                // JLabel pollsessionNameLabel = new
                // JLabel("Poll session name");

                // JTextField pollsessionNameField = new
                // JTextField(collumsCount);
                // pollsessionNameField.setText(session.getName());
                // pollsessionNameField.setEditable(false);

                // JLabel pollsessionDateLabel = new JLabel("Date of creation");
                //
                // JTextField pollsessionDateField = new
                // JTextField(collumsCount);
                // pollsessionDateField.setText(session.getDate());
                // pollsessionDateField.setEditable(false);

                // JLabel pollsessionLabel = new JLabel("Get ready to pass");
                // JLabel infoLabel = new JLabel("poll session");

                framePanel.setLayout(new GridBagLayout());

                GridBagConstraints c = new GridBagConstraints();

                // c.anchor = GridBagConstraints.LAST_LINE_END;
                // c.gridx = 0;
                // c.gridy = 0;
                // c.insets.bottom = componentSpace;
                // framePanel.add(pollsessionLabel, c);
                //
                // c.anchor = GridBagConstraints.LAST_LINE_START;
                // c.gridx = 1;
                // c.gridy = 0;
                // c.insets.left = componentSpace;
                // framePanel.add(infoLabel, c);

                // c.anchor = GridBagConstraints.FIRST_LINE_START;
                // c.gridx = 0;
                // c.gridy = 1;
                // c.insets.left = 0;
                // c.insets.right = 5;
                // framePanel.add(pollsessionIdLabel, c);
                //
                // c.gridx = 1;
                // c.gridy = 1;
                // framePanel.add(pollsessionIdField, c);

                c.gridx = 0;
                c.gridy = 0;
                framePanel.add(header, c);

                // c.gridx = 1;
                // c.gridy = 0;
                // framePanel.add(pollsessionNameField, c);

                c.gridx = 0;
                c.gridy = 1;
                c.anchor = GridBagConstraints.CENTER;
                c.insets = new Insets(6, 6, 0, 0);
                framePanel.add(nextButton, c);

                owningWindow.showDecor(true);
                showDecor = true;

                // c.gridx = 0;
                // c.gridy = 3;
                // framePanel.add(pollsessionDateLabel, c);
                //
                // c.gridx = 1;
                // c.gridy = 3;
                // framePanel.add(pollsessionDateField, c);
            }
            else
                if (currentFrame >= 0
                        && currentFrame <= session.getPolls().size()) {
                    showHeader = true;

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
                        showFooter = true;
                        footer.add(nextButton);

                        BoxLayout pageLayout = new BoxLayout(framePanel,
                                BoxLayout.PAGE_AXIS);

                        JTextArea resultsArea = new JTextArea(generateResults(
                                pollResultList, session));
                        resultsArea.setFont(new Font(resultsArea.getFont()
                                .getName(), Font.PLAIN, 14));
                        //resultsArea.setBorder(BorderFactory.createEmptyBorder(
                        // 0, 0, 6, 0));

                        // JLabel clickNextLabel = new JLabel(
                        // "Click \"Next\" to send results");
                        // clickNextLabel.setFont(bigLabelsFont);

                        framePanel.setLayout(pageLayout);

                        framePanel.add(resultsArea);

                        owningWindow.showDecor(false);
                        showDecor = false;

                        // framePanel.add(clickNextLabel);

                        nextButton.setText("Send results");
                        nextButton.setIcon(GUIUtilities
                                .loadIcon(GUIUtilities.TICK_ICON));
                    }
                    else {
                        Poll poll = session.getPolls().get(currentFrame);

                        JLabel pollNameLabel = new JLabel("Poll:");

                        JLabel pollNameColorLabel = createColoredLabel(null,
                                poll.getName());

                        JLabel pollDescriptionLabel = new JLabel("Description:");

                        JLabel pollDescriptionColorLabel = createColoredLabel(
                                null, poll.getDescription().getValue());

                        GridBagLayout layout = new GridBagLayout();
                        framePanel.setLayout(layout);

                        GridBagConstraints c = new GridBagConstraints();

                        c.anchor = GridBagConstraints.FIRST_LINE_START;
                        c.insets.bottom = componentSpace;
                        c.insets.left = componentSpace;
                        c.gridx = 0;
                        c.gridy = 0;
                        framePanel.add(pollNameLabel, c);

                        c.gridx = 1;
                        c.gridy = 0;
                        framePanel.add(pollNameColorLabel, c);

                        c.gridx = 0;
                        c.gridy = 1;
                        framePanel.add(pollDescriptionLabel, c);

                        c.gridx = 1;
                        c.gridy = 1;
                        framePanel.add(pollDescriptionColorLabel, c);

                        // Creating and adding choices.
                        choices = new ButtonGroup();

                        JPanel choicesPanel = new JPanel();
                        choicesPanel.setLayout(new GridBagLayout());

                        GridBagConstraints choicesConstr = new GridBagConstraints();
                        choicesConstr.gridx = 0;
                        choicesConstr.gridy = -1;
                        choicesConstr.anchor = GridBagConstraints.LINE_START;

                        for (Choice choice : poll.getChoices()) {
                            JRadioButton choiceRadioButton = new JRadioButton();
                            choiceRadioButton.setName(choice.getId());
                            choiceRadioButton.setText(choice.getName());
                            choiceRadioButton.addActionListener(this);
                            choiceRadioButton
                                    .setActionCommand(ANOTHER_RADIO_BUTTON_ACTION);

                            choices.add(choiceRadioButton);

                            choicesConstr.gridy++;
                            choicesPanel.add(choiceRadioButton, choicesConstr);
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

                            choicesConstr.gridy++;
                            choicesPanel.add(customChoiceRadioButton,
                                    choicesConstr);

                            choicesConstr.gridx = 1;
                            choicesPanel.add(customChoiceField, choicesConstr);
                        }

                        choicesConstr.gridx = 0;
                        choicesConstr.gridy++;
                        choicesConstr.gridwidth = 2;
                        choicesConstr.fill = GridBagConstraints.HORIZONTAL;
                        choicesPanel.add(Box.createRigidArea(new Dimension(390,
                                0)), choicesConstr);

                        JPanel choicesPanelWrapper = new JPanel();
                        choicesPanelWrapper.setLayout(new BoxLayout(
                                choicesPanelWrapper, BoxLayout.PAGE_AXIS));
                        choicesPanelWrapper.add(choicesPanel);
                        choicesPanelWrapper.setBorder(BorderFactory
                                .createTitledBorder("Please select choice"));

                        c.gridx = 0;
                        c.gridy = 2;
                        c.gridwidth = 4;
                        c.fill = GridBagConstraints.HORIZONTAL;
                        framePanel.add(choicesPanelWrapper, c);

                        c.gridx = 3;
                        c.gridy = 3;
                        c.anchor = GridBagConstraints.LINE_END;
                        c.gridwidth = 1;
                        c.fill = GridBagConstraints.NONE;
                        framePanel.add(nextButton, c);

                        owningWindow.showDecor(true);
                        showDecor = true;

                        nextButton.setText("Select");
                    }
                }
                else {
                    nextButton.setText("Finish");
                    nextButton.setActionCommand(FINISH_ACTION);

                    BoxLayout pageLayout = new BoxLayout(framePanel,
                            BoxLayout.PAGE_AXIS);

                    framePanel.setLayout(pageLayout);

                    JLabel resultsSentLabel = new JLabel("Results sent!");

                    resultsSentLabel.setFont(bigLabelsFont);

                    framePanel.add(Box.createVerticalGlue());
                    framePanel.add(resultsSentLabel);
                    framePanel.add(Box.createRigidArea(new Dimension(0, 16)));
                    framePanel.add(nextButton);

                    owningWindow.sendResults(pollResultList);

                    owningWindow.showDecor(true);
                    showDecor = true;

                    showFooter = false;
                }

            currentFrame++;

            BoxLayout pollsessionTabLayout = new BoxLayout(this,
                    BoxLayout.PAGE_AXIS);

            setLayout(pollsessionTabLayout);

            removeAll();

            if (showHeader)
                add(header);
            else
                add(Box.createVerticalGlue());

            add(framePanel);
            add(Box.createVerticalGlue());

            if (showFooter)
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
     * Generates results output as string.
     * 
     * @param results
     *            Source of pollsession resuls.
     * @param session
     *            Pollsession matching current results.
     * @return Generated string.
     */
    public static String generateResults(Answers results, Pollsession session) {
        String resultingOutput = "";
        int i = 0;
        int n = 0;

        for (Poll cur : session.getPolls()) {
            String answeredChoice = results.getAnswerItemByPollId(cur.getId())
                    .getAnswerId();
            String choiceText = (!answeredChoice.equals("-1")) ? cur
                    .getChoiceById(answeredChoice).getName() : results
                    .getAnswerItemByPollId(cur.getId()).getCustomChoice();

            if (session.getTestMode().compareTo("true") == 0) {
                n++;
                boolean answeredCorrect = !answeredChoice.equals("-1")
                        && answeredChoice.equals(cur.getCorrectChoice());

                if (answeredCorrect)
                    i++;

                resultingOutput += cur.getName() + " => " + choiceText + " ("
                        + (answeredCorrect ? "PASS" : "FAIL") + ")" + "\n";
            }
            else {
                resultingOutput += cur.getName() + " => " + choiceText + "\n";
            }
        }
        if (session.getTestMode().compareTo("true") == 0) {
            // BUG: May happen too long number after comma.
            resultingOutput += "\nYour score " + Float.toString((float) i / n)
                    + "\n";

            if (((float) i / n) >= Float.parseFloat(session.getMinScore())) {
                resultingOutput += "You pass!";
            }
            else {
                resultingOutput += "You fail.";
            }
        }

        return resultingOutput;
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

    /**
     * @see #showDecor
     */
    public boolean isShowDecor() {
        return showDecor;
    }

    /**
     * Creates label with not colored and colored parts, separated with
     * <code>: </code>.
     * 
     * @param notColoredPart
     *            Not colored part. <code>null</code> for no colored part and no
     *            separator.
     * @param coloredPart
     *            Colored part.
     * @return Label, as described.
     */
    public static JLabel createColoredLabel(String notColoredPart,
            String coloredPart) {
        return new JLabel("<html>"
                + (notColoredPart != null ? (notColoredPart + ": ") : "")
                + "<font color=#b42700><b>" + coloredPart
                + "</b></font></html>");
    }

}
