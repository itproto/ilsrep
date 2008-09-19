package ilsrep.poll.client.gui;

import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;

import java.awt.Component;
import java.awt.Dimension;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Enumeration;

import javax.swing.AbstractButton;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JPanel;
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

    // /**
    // * Results of pollsession.
    // */
    // private Answers pollResultList = null;

    /**
     * Choices of each poll.
     */
    private ButtonGroup choices = null;

    /**
     * Creates new <code>PollsessionTab</code>, for passing given pollsession.
     * 
     * @param session
     *            Pollsession, to process by this tab.
     */
    public PollsessionTab(Pollsession session, MainWindow owningWindow) {
        this.session = session;
        this.owningWindow = owningWindow;
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
                        && currentFrame < session.getPolls().size()) {
                    String choice = null;

                    if (choices != null) {
                        Enumeration<AbstractButton> radioBoxes = choices
                                .getElements();

                        while (radioBoxes.hasMoreElements()) {
                            AbstractButton currentBox = (AbstractButton) radioBoxes
                                    .nextElement();

                            if (currentBox.isSelected()) {
                                choice = currentBox.getName();
                                break;
                            }
                        }

                        if (choice == null) {
                            GUIUtilities
                                    .showWarningDialog("You didn't select your choice!");
                            return;
                        }
                        else {
                            // TODO: Store result.
                        }
                    }

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

                    // choices = new ButtonGroup();
                }
                else
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
                        nextButton.setText("Finish");
                        nextButton.setActionCommand(FINISH_ACTION);

                        BoxLayout pageLayout = new BoxLayout(framePanel,
                                BoxLayout.PAGE_AXIS);

                        framePanel.setLayout(pageLayout);

                        framePanel.add(new JLabel(
                                "Results (should be :)) sent!"));
                        framePanel.add(new JLabel(
                                "Click \"Finish\" to close tab."));
                    }
        }
        else
            if (e.getActionCommand().equals(FINISH_ACTION)) {
                owningWindow.removeTabByInstance(this);
                return;
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
