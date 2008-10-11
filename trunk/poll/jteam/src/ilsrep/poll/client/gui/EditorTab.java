package ilsrep.poll.client.gui;

import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Description;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;

import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;
import java.util.List;

import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JComponent;
import javax.swing.JLabel;
import javax.swing.JPanel;
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
     * Holds references to JTextField's and JComboBox'es where user inputed
     * pollsession data.
     */
    protected List<JComponent> userAnswersList = null;

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

        JLabel pollsessionLabel = new JLabel("Editing poll session");
        c.anchor = GridBagConstraints.FIRST_LINE_START;
        c.gridx = 0;
        c.gridy = 0;
        c.insets.bottom = 32;
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
                editPollButton.setText("Edit");
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
                removePollButton.setText("Remove");
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
        addPollButton.setText("Add new poll");
        c.gridx = 0;
        c.gridy++;
        add(addPollButton, c);

        refreshTab();
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

        Poll poll = currentSession.getPolls().get(id);

        GridBagConstraints c = new GridBagConstraints();

        JLabel pollsessionLabel = new JLabel("Editing poll");
        c.anchor = GridBagConstraints.FIRST_LINE_START;
        c.gridx = 0;
        c.gridy = 0;
        c.insets.bottom = 32;
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
            for (int i = 0; i < poll.getChoices().size(); i++) {
                Choice choice = poll.getChoices().get(i);
                JTextField choiceNameField = new JTextField(COLLUMNS_COUNT);
                if (choice.getName() != null)
                    choiceNameField.setText(choice.getName());
                c.gridx = 0;
                c.gridy++;
                add(choiceNameField, c);
                userAnswersList.add(choiceNameField);

                JButton removeChoiceButton = new JButton();
                removeChoiceButton.setName("r" + i);
                removeChoiceButton.setText("Remove");
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
        addChoiceButton.setText("Add new choice");
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
        backButton.addActionListener(new ActionListener() {

            @Override
            public void actionPerformed(ActionEvent e) {
                saveCurrentPollEditingChanges();
                editPollsession();
            }
        });
        c.gridx = 3;
        c.gridy++;
        add(backButton, c);

        refreshTab();
    }

    /**
     * Saves results stored in {@link #userAnswersList} for pollsession root.
     */
    protected void savePollsessionRootChanges() {
        String newName = ((JTextField) userAnswersList.get(0)).getText();

        if (!newName.isEmpty())
            currentSession.setName(newName);

        String newMinScore = ((JTextField) userAnswersList.get(1)).getText();

        if (!newMinScore.isEmpty())
            try {
                // Checking if entered number is float.
                Float.parseFloat(newMinScore);

                currentSession.setMinScore(newMinScore);
            }
            catch (NumberFormatException e) {
                GUIUtilities
                        .showWarningDialog("Entered value for minimal score is not valid!");
            }

        currentSession.setTestMode(((JCheckBox) userAnswersList.get(2))
                .isSelected() ? "true" : "false");

        userAnswersList.clear();
    }

    /**
     * Saves results of current poll editing ({@link #currentPollEditing}).
     */
    protected void saveCurrentPollEditingChanges() {
        Poll currentPoll = currentSession.getPolls().get(currentPollEditing);

        String newName = ((JTextField) userAnswersList.get(0)).getText();

        if (!newName.isEmpty())
            currentPoll.setName(newName);

        String newDescription = ((JTextField) userAnswersList.get(1)).getText();

        if (!newDescription.isEmpty()) {
            if (currentPoll.getDescription() == null)
                currentPoll.setDescription(new Description());

            currentPoll.getDescription().setValue(newDescription);
        }

        currentPoll.setCustomEnabled(((JCheckBox) userAnswersList.get(2))
                .isSelected() ? "true" : "false");

        for (int i = 3; i < userAnswersList.size(); i++) {
            String newChoiceName = ((JTextField) userAnswersList.get(i))
                    .getText();

            if (!newChoiceName.isEmpty())
                currentPoll.getChoices().get(i - 3).setName(newChoiceName);
        }

        userAnswersList.clear();
    }

    /**
     * Refreshes tab look.
     */
    public void refreshTab() {
        update(getGraphics());
    }

}
