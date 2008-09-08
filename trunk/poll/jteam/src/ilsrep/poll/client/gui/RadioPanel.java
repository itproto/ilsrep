package ilsrep.poll.client.gui;

import java.awt.BorderLayout;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Enumeration;

import javax.swing.AbstractButton;
import javax.swing.BorderFactory;
import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;

public class RadioPanel extends JPanel implements ActionListener {

    /**
     * Serial version UID.
     */
    private static final long serialVersionUID = 167021421308059421L;

    public String reply = "-1";

    private Enumeration<AbstractButton> e;

    RadioPanel(ButtonGroup group, String query) {
        super(new BorderLayout());
        JPanel radioPanel = new JPanel(new GridLayout(0, 1));
        e = group.getElements();

        while (e.hasMoreElements()) {
            radioPanel.add((JRadioButton) e.nextElement());
        }
        e = group.getElements();
        add(radioPanel, BorderLayout.LINE_START);
        JButton b2 = new JButton("OK");
        JLabel jlb = new JLabel(query);
        jlb.setToolTipText(query);
        add(jlb, BorderLayout.NORTH);
        add(b2, BorderLayout.CENTER);
        b2.setActionCommand("Go");
        b2.addActionListener(this);
        setBorder(BorderFactory.createEmptyBorder(20, 20, 20, 20));

    }

    public void actionPerformed(ActionEvent act) {
        if (act.getActionCommand().equals("Go")) {
            while (e.hasMoreElements()) {
                JRadioButton jrb = (JRadioButton) e.nextElement();
                if (jrb.isSelected()) {
                    reply = jrb.getActionCommand();
                    // System.out.println(jrb.getActionCommand());
                }
            }

        }
    }

}
