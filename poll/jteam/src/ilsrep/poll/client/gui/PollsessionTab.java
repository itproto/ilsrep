package ilsrep.poll.client.gui;

import ilsrep.poll.common.model.Pollsession;

import java.awt.Component;
import java.awt.Dimension;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.Box;
import javax.swing.BoxLayout;
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
     * Pollsession that is proceed by this tab.
     */
    private Pollsession session = null;

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

    // /**
    // * Results of pollsession.
    // */
    // private Answers pollResultList = null;

    /**
     * Creates new <code>PollsessionTab</code>, for passing given pollsession.
     * 
     * @param session
     *            Pollsession, to process by this tab.
     */
    public PollsessionTab(Pollsession session) {
        this.session = session;
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

        JButton nextButton = new JButton("Next");
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
        removeAll();

        BoxLayout pollsessionTabLayout = new BoxLayout(this,
                BoxLayout.PAGE_AXIS);

        setLayout(pollsessionTabLayout);

        JPanel framePanel = new JPanel();
        {
            if (currentFrame == -1) {
                framePanel.setLayout(new GridLayout(0, 2));

                JLabel pollsessionIdLabel = new JLabel("Id");
                // setSize(pollsessionIdLabel, pollsessionIdLabel
                // .getPreferredSize());

                JTextField pollsessionIdField = new JTextField(session.getId());
                pollsessionIdField.setEditable(false);
                // setSize(pollsessionIdField, pollsessionIdField
                // .getPreferredSize());

                framePanel.add(pollsessionIdLabel);
                setSize(pollsessionIdLabel, pollsessionIdLabel
                        .getPreferredSize());
                // framePanel.add(Box.createGlue());
                framePanel.add(pollsessionIdField);
                setSize(pollsessionIdField, pollsessionIdField
                        .getPreferredSize());
                // framePanel.add(Box.createGlue());
            }
            else {
                framePanel.add(new JLabel("Polls should be asked here!"));
            }
        }

        currentFrame++;

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
