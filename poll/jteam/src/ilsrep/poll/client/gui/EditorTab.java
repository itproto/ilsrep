package ilsrep.poll.client.gui;

import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Description;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;

import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyAdapter;
import java.awt.event.KeyEvent;
import java.util.ArrayList;
import java.util.List;

import javax.swing.BoxLayout;
import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JComponent;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JTextField;

/**
 * This is JPanel with contents to create/edit pollsession.
 * 
 * @author TKOST
 * 
 */
public class EditorTab extends JPanel {

    /**
     * Serial version UID.
     */
    private static final long serialVersionUID = -2786062637046184092L;

    /**
     * Default size of text field.
     */
    public static final int COLLUMNS_COUNT = 20;

    /**
     * Space between some components.
     */
    public static final int COMPONENT_SPACE = 5;

    /**
     * Window that owns this tab. Is used to send created/edited pollsession to
     * server.
     */
    protected MainWindow owningWindow = null;

    /**
     * Current session that is edited.
     */
    protected Pollsession currentSession = null;

    /**
     * If creating new pollsession then <code>true</code>, if editing existing -
     * <code>false</code>.
     */
    protected boolean creating = true;

    /**
     * Holds references to JTextField's and JComboBox'es where user inputed
     * pollsession data.
     */
    protected List<JComponent> userAnswersList = null;

    /**
     * Holds references to {@link ChoicePanel} where user inputed choices and
     * correct choice.
     */
    protected List<ChoicePanel> testModeCorrectChoices = null;

    /**
     * Holds what poll number is edited now. -1 means editing pollsession info,
     * larger number then in {@link #currentSession}'s poll list - creating new.
     */
    protected int currentPollEditing = -1;

    /**
     * Creates editor tab for creating new pollsession.
     * 
     * @param owningWindow
     *            See {@link #owningWindow}.
     * 
     * @return Editor tab.
     */
    public static EditorTab createNewPollsession(MainWindow owningWindow) {
        return new EditorTab(owningWindow, null);
    }

    /**
     * Creates editor tab for editing existing pollsession.
     * 
     * @param owningWindow
     *            See {@link #owningWindow}.
     * 
     * @param session
     *            Pollsession to edit.
     * @return Editor tab.
     */
    public static EditorTab editPollsession(MainWindow owningWindow,
            Pollsession session) {
        return new EditorTab(owningWindow, session);
    }

    /**
     * Is private. To create use factory methods:
     * {@link #createNewPollsession()} and {@link #editPollsession(Pollsession)}
     * 
     * @param owningWindow
     *            See {@link #owningWindow}.
     * 
     * @param session
     *            Pollsession to edit, or <code>null</code> if to create new.
     */
    private EditorTab(MainWindow owningWindow, Pollsession session) {
        this.owningWindow = owningWindow;
        currentSession = session;

        userAnswersList = new ArrayList<JComponent>();
        testModeCorrectChoices = new ArrayList<ChoicePanel>();

        creating = (session == null);

        if (currentSession == null)
            currentSession = new Pollsession();

        setTestMode(currentSession.getTestMode() != null
                && currentSession.getTestMode().equals("true"));
    }

    /**
     * Executes action on this ActionListener.
     * 
     * @param action
     *            Action command for action to execute.
     */
    public void editPollsession() {
        if (currentSession == null)
            currentSession = new Pollsession();

        currentPollEditing = -1;

        removeAll();

        userAnswersList.clear();

        GridBagLayout editorTabLayout = new GridBagLayout();
        setLayout(editorTabLayout);

        // GridBag layout constraints.
        GridBagConstraints c = new GridBagConstraints();

        JLabel pollsessionLabel = PollsessionTab.createColoredLabel(null,
                "Editing poll session");
        c.anchor = GridBagConstraints.FIRST_LINE_START;
        c.gridx = 0;
        c.gridy = 0;
        c.insets.bottom = 32;
        c.insets.left = COMPONENT_SPACE;
        add(pollsessionLabel, c);

        JLabel nameLabel = new JLabel("Name");
        c.gridx = 0;
        c.gridy = 1;
        c.insets.bottom = COMPONENT_SPACE;
        c.insets.left = COMPONENT_SPACE;
        add(nameLabel, c);

        JTextField textField = new JTextField(COLLUMNS_COUNT);
        if (currentSession.getName() != null
                && !currentSession.getName().isEmpty())
            textField.setText(currentSession.getName());
        c.gridx = 1;
        c.gridy = 1;
        c.gridwidth = 2;
        add(textField, c);
        userAnswersList.add(textField);

        JLabel minScoreLabel = new JLabel("Minimal score");
        c.gridx = 0;
        c.gridy = 2;
        c.gridwidth = 1;
        add(minScoreLabel, c);

        JTextField minScoreField = new JTextField(COLLUMNS_COUNT);
        if (currentSession.getMinScore() != null
                && !currentSession.getMinScore().isEmpty())
            minScoreField.setText(currentSession.getMinScore());
        c.gridx = 1;
        c.gridy = 2;
        c.gridwidth = 2;
        add(minScoreField, c);
        userAnswersList.add(minScoreField);

        JLabel testModeLabel = new JLabel("Test mode");
        c.gridx = 0;
        c.gridy = 3;
        c.gridwidth = 1;
        add(testModeLabel, c);

        JCheckBox testModeCheckBox = new JCheckBox();
        if (currentSession.getTestMode() != null
                && currentSession.getTestMode().equals("true"))
            testModeCheckBox.setSelected(true);
        c.gridx = 1;
        c.gridy = 3;
        add(testModeCheckBox, c);
        userAnswersList.add(testModeCheckBox);

        if (currentSession.getPolls() == null)
            currentSession.setPolls(new ArrayList<Poll>());

        if (currentSession.getPolls().size() == 0) {
            JTextField noPollsFiels = new JTextField(COLLUMNS_COUNT);
            noPollsFiels.setText("No polls yet");
            noPollsFiels.setEditable(false);
            c.gridx = 0;
            c.gridy = 4;
            add(noPollsFiels, c);
        }
        else {
            for (int i = 0; i < currentSession.getPolls().size(); i++) {
                Poll poll = currentSession.getPolls().get(i);
                JTextField pollNameField = new JTextField(COLLUMNS_COUNT);
                pollNameField.setText(poll.getName());
                pollNameField.setEditable(false);
                c.gridx = 0;
                c.gridy++;
                add(pollNameField, c);

                JButton editPollButton = new JButton();
                editPollButton.setName("e" + i);
                // editPollButton.setText("Edit");
                editPollButton.setToolTipText("Edit poll");
                editPollButton.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.PENCIL_ICON));
                editPollButton.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        Object o = e.getSource();

                        if (o instanceof JButton) {
                            savePollsessionRootChanges();
                            JButton removePollButton = (JButton) o;
                            editPoll(Integer.parseInt(removePollButton
                                    .getName().substring(1)));
                        }
                    }
                });

                c.gridx = 1;
                add(editPollButton, c);

                JButton removePollButton = new JButton();
                removePollButton.setName("r" + i);
                // removePollButton.setText("Remove");
                removePollButton.setToolTipText("Remove poll");
                removePollButton.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.CANCEL_ICON));
                removePollButton.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        Object o = e.getSource();

                        if (o instanceof JButton) {
                            JButton removePollButton = (JButton) o;
                            removePoll(Integer.parseInt(removePollButton
                                    .getName().substring(1)));
                            editPollsession();
                        }
                    }
                });
                c.gridx = 2;
                add(removePollButton, c);
            }
        }

        JButton addPollButton = new JButton();
        // addPollButton.setText("Add new poll");
        addPollButton.setToolTipText("Add new poll");
        addPollButton.setIcon(GUIUtilities.loadIcon(GUIUtilities.ADD_ICON));
        addPollButton.addActionListener(new ActionListener() {

            @Override
            public void actionPerformed(ActionEvent e) {
                savePollsessionRootChanges();

                if (currentSession.getPolls() == null)
                    currentSession.setPolls(new ArrayList<Poll>());

                currentSession.getPolls().add(new Poll());

                editPoll(currentSession.getPolls().size() - 1);
            }
        });
        c.gridx = 0;
        c.gridy++;
        add(addPollButton, c);

        JButton saveButton = new JButton();
        saveButton.setText(creating ? "Create" : "Save");
        saveButton.setIcon(GUIUtilities.loadIcon(GUIUtilities.TICK_ICON));
        saveButton.addActionListener(new ActionListener() {

            @Override
            public void actionPerformed(ActionEvent e) {
                savePollsessionRootChanges();

                String prepareErrorMessage = preparePollsession(currentSession);

                if (prepareErrorMessage != null) {
                    GUIUtilities
                            .showWarningDialog("Poll session is invalid, can't send! ("
                                    + prepareErrorMessage + ")");
                    editPollsession();
                    return;
                }

                if (!owningWindow.sendPollsession(currentSession)) {
                    GUIUtilities
                            .showWarningDialog("Failed to send poll session!");
                    editPollsession();
                }
                else {
                    owningWindow.removeTabByInstance(EditorTab.this);

                    if (owningWindow.connect()) {
                        owningWindow.updateList();
                        owningWindow.disconnect();
                    }
                }
            }
        });
        c.gridx = 2;
        c.gridy++;
        c.anchor = GridBagConstraints.LINE_END;
        add(saveButton, c);

        refreshTab();
    }

    /**
     * Prepares pollsession for sending(if pollsession is ok for sending fields
     * of given pollsession are rewritten with prepared ones).
     * 
     * @param session
     *            Pollsession to prepare.
     * @return Error message if given pollsession is broken(not ready for
     *         sending yet)
     */
    protected String preparePollsession(Pollsession originalSession) {
        Pollsession clonedSession = null;
        try {
            clonedSession = (Pollsession) originalSession.clone();
        }
        catch (CloneNotSupportedException e) {
            return "can't clone poll session! (this shoudn't happen)";
        }

        if (clonedSession.getName() == null
                || clonedSession.getName().isEmpty())
            return "poll session name not entered";

        if (clonedSession.getTestMode() == null
                || !clonedSession.getTestMode().equals("true"))
            clonedSession.setTestMode("false");

        if (clonedSession.getTestMode().equals("true")) {
            if (clonedSession.getMinScore() == null)
                return "poll session is in test mode, but minimal score not entered";
            else {
                try {
                    float minScore = Float.parseFloat(clonedSession
                            .getMinScore());

                    if (minScore <= 0 || minScore > 1)
                        throw new NumberFormatException();
                }
                catch (NumberFormatException e) {
                    return "entered value for minimal score"
                            + " should be in range (0.0, 1.0]";
                }
            }
        }

        if (clonedSession.getPolls() == null
                || clonedSession.getPolls().size() == 0)
            return "poll session have no polls";

        for (Poll poll : clonedSession.getPolls()) {
            if (poll.getName() == null || poll.getName().isEmpty()
                    || poll.getDescription() == null
                    || poll.getDescription().getValue() == null
                    || poll.getDescription().getValue().isEmpty())
                return "name or description of poll(" + poll.getName()
                        + ") not entered";

            if (poll.getChoices() == null || poll.getChoices().size() == 0)
                return "poll(" + poll.getName() + ") have no choices";

            for (Choice choice : poll.getChoices())
                if (choice.getName() == null || choice.getName().isEmpty())
                    return "one of poll's(" + poll.getName()
                            + ") choices name not entered";

            for (int i = 1; i <= poll.getChoices().size(); i++)
                poll.getChoices().get(i - 1).setId(Integer.toString(i));
        }

        for (int i = 1; i <= clonedSession.getPolls().size(); i++)
            clonedSession.getPolls().get(i - 1).setId(Integer.toString(i));

        for (Poll poll : clonedSession.getPolls())
            for (Choice choice : poll.getChoices())
                if (choice.getName().equals(poll.getCorrectChoice())) {
                    poll.setCorrectChoice(choice.getId());
                    break;
                }

        if (clonedSession.getTestMode().equals("true"))
            for (Poll poll : clonedSession.getPolls())
                if (poll.getCorrectChoice().equals("-1"))
                    return "poll(" + poll.getName()
                            + ") have no selected correct choice";

        originalSession.setId(clonedSession.getId());
        originalSession.setName(clonedSession.getName());
        originalSession.setDate(clonedSession.getDate());
        originalSession.setMinScore(clonedSession.getMinScore());
        originalSession.setTestMode(clonedSession.getTestMode());
        originalSession.setPolls(clonedSession.getPolls());

        return null;
    }

    /**
     * Removes specified poll.
     * 
     * @param id
     *            Relative poll id(from 0).
     */
    protected void removePoll(int id) {
        currentSession.getPolls().remove(id);
    }

    /**
     * Removes specified choice from {@link #currentPollEditing}.
     * 
     * @param choiceId
     *            Relative poll id(from 0).
     */
    protected void removeChoice(int choiceId) {
        currentSession.getPolls().get(currentPollEditing).getChoices().remove(
                choiceId);
    }

    /**
     * Starts editing specified poll.
     * 
     * @param id
     *            Relative poll id(from 0).
     */
    protected void editPoll(int id) {
        currentPollEditing = id;

        removeAll();

        userAnswersList.clear();
        testModeCorrectChoices.clear();

        Poll poll = currentSession.getPolls().get(id);

        GridBagConstraints c = new GridBagConstraints();

        JLabel pollsessionLabel = PollsessionTab.createColoredLabel(null,
                "Editing poll");
        c.anchor = GridBagConstraints.FIRST_LINE_START;
        c.gridx = 0;
        c.gridy = 0;
        c.insets.bottom = 32;
        c.insets.left = COMPONENT_SPACE;
        add(pollsessionLabel, c);

        JLabel nameLabel = new JLabel("Name");
        c.gridx = 0;
        c.gridy = 1;
        c.insets.bottom = COMPONENT_SPACE;
        c.insets.left = COMPONENT_SPACE;
        add(nameLabel, c);

        JTextField pollNameField = new JTextField(COLLUMNS_COUNT);
        if (poll.getName() != null && !poll.getName().isEmpty())
            pollNameField.setText(poll.getName());
        c.gridx = 1;
        c.gridy = 1;
        c.gridwidth = 2;
        add(pollNameField, c);
        userAnswersList.add(pollNameField);

        JLabel descriptionLabel = new JLabel("Description");
        c.gridx = 0;
        c.gridy = 2;
        c.gridwidth = 1;
        add(descriptionLabel, c);

        JTextField descriptionField = new JTextField(COLLUMNS_COUNT);
        if (poll.getDescription() != null
                && poll.getDescription().getValue() != null
                && !poll.getDescription().getValue().isEmpty())
            descriptionField.setText(poll.getDescription().getValue());
        c.gridx = 1;
        c.gridy = 2;
        c.gridwidth = 2;
        add(descriptionField, c);
        userAnswersList.add(descriptionField);

        JLabel customChoiceEnabledLabel = new JLabel("Custom choice enabled");
        c.gridx = 0;
        c.gridy = 3;
        c.gridwidth = 1;
        add(customChoiceEnabledLabel, c);

        JCheckBox customChoiceEnabledCheckBox = new JCheckBox();
        if (poll.getCustomEnabled() != null
                && poll.getCustomEnabled().equals("true"))
            customChoiceEnabledCheckBox.setSelected(true);
        c.gridx = 1;
        c.gridy = 3;
        add(customChoiceEnabledCheckBox, c);
        userAnswersList.add(customChoiceEnabledCheckBox);

        if (poll.getChoices() == null)
            poll.setChoices(new ArrayList<Choice>());

        if (poll.getChoices().size() == 0) {
            JTextField noPollsFiels = new JTextField(COLLUMNS_COUNT);
            noPollsFiels.setText("No choices yet");
            noPollsFiels.setEditable(false);
            c.gridx = 0;
            c.gridy = 4;
            add(noPollsFiels, c);
        }
        else {
            ButtonGroup correctChoicesRadioButtonGroup = new ButtonGroup();

            for (int i = 0; i < poll.getChoices().size(); i++) {
                Choice choice = poll.getChoices().get(i);

                if (pollsessionInTestMode()) {
                    ChoicePanel choicePanel = new ChoicePanel(
                            correctChoicesRadioButtonGroup, choice.getName(),
                            poll.getCorrectChoice() != null
                                    && choice.getName() != null
                                    && choice.getName().equals(
                                            poll.getCorrectChoice()));
                    c.gridx = 0;
                    c.gridy++;
                    add(choicePanel, c);
                    testModeCorrectChoices.add(choicePanel);
                }
                else {
                    JTextField choiceNameField = new JTextField(COLLUMNS_COUNT);
                    if (choice.getName() != null)
                        choiceNameField.setText(choice.getName());
                    c.gridx = 0;
                    c.gridy++;
                    add(choiceNameField, c);
                    userAnswersList.add(choiceNameField);
                }

                JButton removeChoiceButton = new JButton();
                removeChoiceButton.setName("r" + i);
                removeChoiceButton.setToolTipText("Remove choice");
                removeChoiceButton.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.CANCEL_ICON));
                removeChoiceButton.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        Object o = e.getSource();

                        if (o instanceof JButton) {
                            saveCurrentPollEditingChanges();

                            JButton removePollButton = (JButton) o;
                            removeChoice(Integer.parseInt(removePollButton
                                    .getName().substring(1)));

                            editPoll(currentPollEditing);
                        }
                    }
                });
                c.gridx = 1;
                add(removeChoiceButton, c);
            }
        }

        JButton addChoiceButton = new JButton();
        addChoiceButton.setToolTipText("Add new choice");
        addChoiceButton.setIcon(GUIUtilities.loadIcon(GUIUtilities.ADD_ICON));
        addChoiceButton.addActionListener(new ActionListener() {

            @Override
            public void actionPerformed(ActionEvent e) {
                saveCurrentPollEditingChanges();

                Poll pollToAddChoice = currentSession.getPolls().get(
                        currentPollEditing);

                if (pollToAddChoice.getChoices() == null)
                    pollToAddChoice.setChoices(new ArrayList<Choice>());

                pollToAddChoice.getChoices().add(new Choice());

                editPoll(currentPollEditing);
            }
        });
        c.gridx = 0;
        c.gridy++;
        add(addChoiceButton, c);

        JButton backButton = new JButton();
        backButton.setText("Back");
        backButton.setIcon(GUIUtilities.loadIcon(GUIUtilities.ARROW_LEFT));
        backButton.addActionListener(new ActionListener() {

            @Override
            public void actionPerformed(ActionEvent e) {
                saveCurrentPollEditingChanges();
                editPollsession();
            }
        });
        c.gridx = 2;
        c.gridy++;
        c.anchor = GridBagConstraints.LINE_END;
        add(backButton, c);

        refreshTab();
    }

    /**
     * Saves results stored in {@link #userAnswersList} for pollsession root.
     * 
     * @return <code>true</code>, if all values entered were correct.
     */
    protected void savePollsessionRootChanges() {
        currentSession.setName(((JTextField) userAnswersList.get(0)).getText());

        currentSession.setMinScore(((JTextField) userAnswersList.get(1))
                .getText());

        setTestMode(((JCheckBox) userAnswersList.get(2)).isSelected());

        userAnswersList.clear();
    }

    /**
     * Saves results of current poll editing ({@link #currentPollEditing}).
     */
    protected void saveCurrentPollEditingChanges() {
        Poll currentPoll = currentSession.getPolls().get(currentPollEditing);

        // if (!newName.isEmpty())
        currentPoll.setName(((JTextField) userAnswersList.get(0)).getText());

        if (currentPoll.getDescription() == null)
            currentPoll.setDescription(new Description());

        currentPoll.getDescription().setValue(
                ((JTextField) userAnswersList.get(1)).getText());

        currentPoll.setCustomEnabled(((JCheckBox) userAnswersList.get(2))
                .isSelected() ? "true" : "false");

        if (testModeCorrectChoices.size() == 0)
            for (int i = 3; i < userAnswersList.size(); i++) {
                currentPoll.getChoices().get(i - 3).setName(
                        ((JTextField) userAnswersList.get(i)).getText());
            }
        else
            for (int i = 0; i < testModeCorrectChoices.size(); i++) {
                String newChoiceName = testModeCorrectChoices.get(i)
                        .getChoiceNameField().getText();

                currentPoll.getChoices().get(i).setName(newChoiceName);

                if (testModeCorrectChoices.get(i)
                        .isCorrectChoiceButtonSelected())
                    currentPoll.setCorrectChoice(newChoiceName);
            }

        userAnswersList.clear();
    }

    /**
     * Refreshes tab look.
     */
    public void refreshTab() {
        update(getGraphics());
    }

    /**
     * Sets if test mode is enabled in pollsesson and do other routines needed
     * for saving correct choice.
     * 
     * @param testMode
     */
    protected void setTestMode(boolean testMode) {
        currentSession.setTestMode(testMode ? "true" : "false");

        if (currentSession.getPolls() == null)
            currentSession.setPolls(new ArrayList<Poll>());

        for (Poll poll : currentSession.getPolls()) {
            if (testMode) {
                Choice correctChoice = null;
                if (poll.getCorrectChoice() != null
                        && (correctChoice = poll.getChoiceById(poll
                                .getCorrectChoice())) != null)
                    poll.setCorrectChoice(correctChoice.getName());
            }
            else
                poll.setCorrectChoice(null);
        }
    }

    /**
     * Checks if current pollsession is in test mode.
     * 
     * @return <code>true</code>, if current pollsession is in test mode.
     */
    protected boolean pollsessionInTestMode() {
        return currentSession.getTestMode() != null
                && currentSession.getTestMode().equals("true");
    }

    /**
     * Represents radio button and text field for entering choice.<br>
     * If field's text is empty you can't select radio button - cos when sending
     * to server choice with empty name is removed.<br>
     * Is only used if poll is in test mode.
     * 
     * @author TKOST
     * 
     */
    private class ChoicePanel extends JPanel {

        /**
         * Button group for check box.
         */
        protected ButtonGroup buttonGroup = null;

        /**
         * Radio button to select if this choice is correct.
         */
        protected JRadioButton correctChoiceRadioButton = null;

        /**
         * Field to enter choice name.
         */
        protected JTextField choiceNameField = null;

        /**
         * Serial version UID.
         */
        private static final long serialVersionUID = -2331150865476639578L;

        /**
         * Creates new {@link ChoicePanel}.
         * 
         * @param group
         *            Button group for this panel's radio button.
         * @param initialName
         *            Initial name of choice.
         * @param selected
         *            If radio button for correct choice is selected.
         */
        public ChoicePanel(ButtonGroup group, String initialName,
                boolean selected) {
            buttonGroup = group;

            correctChoiceRadioButton = new JRadioButton();

            if (selected)
                correctChoiceRadioButton.setSelected(true);

            buttonGroup.add(correctChoiceRadioButton);

            choiceNameField = new JTextField(COLLUMNS_COUNT);

            if (initialName != null && !initialName.isEmpty())
                choiceNameField.setText(initialName);
            else
                correctChoiceRadioButton.setEnabled(false);

            choiceNameField.addKeyListener(new KeyAdapter() {

                /**
                 * @see java.awt.event.KeyAdapter#keyTyped(java.awt.event.KeyEvent)
                 */
                @Override
                public void keyTyped(KeyEvent e) {
                    if (choiceNameField.getText().isEmpty()) {
                        if (buttonGroup.getSelection() == correctChoiceRadioButton
                                .getModel())
                            buttonGroup.clearSelection();

                        correctChoiceRadioButton.setEnabled(false);
                    }
                    else
                        correctChoiceRadioButton.setEnabled(true);
                }
            });

            BoxLayout choicePanelLayout = new BoxLayout(this,
                    BoxLayout.LINE_AXIS);

            setLayout(choicePanelLayout);

            add(correctChoiceRadioButton);

            add(choiceNameField);
        }

        /**
         * @see #correctChoiceRadioButton
         */
        public JRadioButton getCorrectChoiceRadioButton() {
            return correctChoiceRadioButton;
        }

        /**
         * @see #choiceNameField
         */
        public JTextField getChoiceNameField() {
            return choiceNameField;
        }

        /**
         * Returns state of radio button that shows if this choice is correct.
         * 
         * @return <code>true</code> if radio button(that shows if this choice
         *         is correct) is selected.
         */
        public boolean isCorrectChoiceButtonSelected() {
            return correctChoiceRadioButton.isSelected();
        }

    }

}
