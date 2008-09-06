package ilsrep.poll.client.gui;

import java.awt.FlowLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JTextField;

public class GUIUtil extends JFrame implements ActionListener {

    /**
     * Serial version UID.
     */
    private static final long serialVersionUID = -2009987299459890563L;

    private String passwordtest = null;

    private JDialog dialog = new JDialog(this, "New Password", true);

    private JLabel jjb1 = new JLabel("Enter password");

    private JLabel jjb2 = new JLabel("Confirm password");

    private JTextField jtxt1 = new JTextField(10);

    private JTextField jtxt2 = new JTextField(10);

    private JButton bb1 = new JButton("Ok");

    public GUIUtil() {

        this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        // this.setVisible(true);

    }

    public String askUser(String query) {
        // String s = JOptionPane.showInputDialog(query);
        JOptionPane pane = new JOptionPane();
        pane.setMessageType(JOptionPane.QUESTION_MESSAGE);
        pane.setMessage(query);
        Object[] a = { "OK" };
        pane.setOptions(a);
        pane.setInitialValue("OK");
        pane.setOptionType(JOptionPane.DEFAULT_OPTION);
        pane.setWantsInput(true);
        JDialog dialog = pane.createDialog(this, query);
        dialog.setVisible(true);
        String selectedValue = (String) pane.getInputValue();

        return selectedValue;
    }

    public int askUserChoice(String query, Object[] options) {
        int s = JOptionPane.showOptionDialog(this, query, "Please choose",
                JOptionPane.YES_NO_CANCEL_OPTION, JOptionPane.QUESTION_MESSAGE,
                null, options, options[0]);
        return s;
    }

    /**
     * Shows warning dialog with given alertion.
     * 
     * @param alertion
     *            Alrertion, to show.
     */
    public void alert(String alertion) {
        JOptionPane.showMessageDialog(null, alertion, alertion,
                JOptionPane.ERROR_MESSAGE);
    }

    /**
     * Shows info dialog with given alertion.
     * 
     * @param alertion
     *            Alrertion, to show.
     */
    public void infoWindow(String alertion) {
        JOptionPane.showMessageDialog(null, alertion, alertion,
                JOptionPane.INFORMATION_MESSAGE);
    }

    public Boolean askYesNo(String query) {
        Object[] options = { "Yes", "No" };
        int s = JOptionPane.showOptionDialog(this, query, "Please choose",
                JOptionPane.YES_NO_CANCEL_OPTION, JOptionPane.QUESTION_MESSAGE,
                null, options, options[0]);
        return (s == 0) ? true : false;

    }

    public String createPass() {
        FlowLayout layout = new FlowLayout();
        dialog.getContentPane().setLayout(layout);
        dialog.getContentPane().add(jjb1);
        dialog.getContentPane().add(jtxt1);
        dialog.getContentPane().add(jjb2);
        dialog.getContentPane().add(jtxt2);
        bb1.setActionCommand("trypass");
        bb1.addActionListener(this);
        dialog.getContentPane().add(bb1);
        dialog.getContentPane().doLayout();
        dialog.pack();
        dialog.setVisible(true);
        while (passwordtest == null) {
            try {
                Thread.sleep(250);
            }
            catch (Exception arg) {
                System.out.println(arg.getMessage());
            }
        }
        return passwordtest;
    }

    public String getChoice(ButtonGroup group, String query) {
        this.setTitle("Please choose");
        RadioPanel newContentPane = new RadioPanel(group, query);
        newContentPane.setOpaque(true); // content panes must be opaque
        setContentPane(newContentPane);

        // Display the window.
        pack();
        setVisible(true);
        while (newContentPane.reply.equals("-1")) {
            try {
                Thread.sleep(250);
            }
            catch (Exception arg) {
                System.out.println(arg.getMessage());
            }
        }

        return newContentPane.reply;

    }

    public void actionPerformed(ActionEvent e) {
        if ("trypass".equals(e.getActionCommand())) {
            if (jtxt1.getText().equals(jtxt2.getText())) {
                passwordtest = jtxt1.getText();

                dialog.dispose();
            }
            else
                this.alert("Passwords dont match");

        }
    }
}
