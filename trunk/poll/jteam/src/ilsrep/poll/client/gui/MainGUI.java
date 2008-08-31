package ilsrep.poll.client.gui;

import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;
import javax.swing.JTextField;
import javax.swing.SwingUtilities;

/**
 * Main GUI for Poll program(combines client, editor and any other possible Poll
 * client programs).
 * 
 * @author TKOST
 * 
 */
public class MainGUI extends JFrame {

    /**
     * Serial version UID.
     */
    private static final long serialVersionUID = -8569069240045788600L;

    /**
     * Menu of this window.
     */
    private JMenuBar menu = null;

    /**
     * Server to connect to.
     */
    protected String server = null;

    /**
     * Port to connect to.
     */
    protected int port = -1;

    /**
     * Indicates if connect to local server with default port.
     */
    protected boolean conectLocal = false;

    /**
     * Creates main window.
     */
    public MainGUI() {
        super("Poll Application");

        setJMenuBar(createMenu());
        setSize(256, 128);
    }

    /**
     * Creates menu for this window.
     * 
     * @return Menu.
     */
    private JMenuBar createMenu() {
        if (menu == null) {
            menu = new JMenuBar();

            JMenu fileMenu = new JMenu();
            fileMenu.setText("File");
            {
                JMenuItem selectServerItem = new JMenuItem();
                selectServerItem.setText("Select server");
                selectServerItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        ServerSelectDialog selectDialog = new ServerSelectDialog();
                        selectDialog.setVisible(true);
                    }
                });

                JMenuItem exitItem = new JMenuItem();
                exitItem.setText("Exit");
                exitItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        exitProgram();
                    }
                });

                fileMenu.add(selectServerItem);
                fileMenu.add(exitItem);
            }

            menu.add(fileMenu);
        }

        return menu;
    }

    /**
     * Exits program.
     */
    private void exitProgram() {
        dispose();
        System.exit(0);
    }

    /**
     * Main method for this class.<br>
     * Laucnhes GUI.
     * 
     * @param args
     *            Command-line parameters.
     */
    public static void main(String[] args) {
        Thread guiStartThread = new Thread() {

            /**
             * Starts GUI.
             */
            @Override
            public void run() {
                MainGUI gui = new MainGUI();
                gui.setVisible(true);
            }
        };

        SwingUtilities.invokeLater(guiStartThread);
    }

    private class ServerSelectDialog extends JDialog {

        /**
         * Serial version UID.
         */
        private static final long serialVersionUID = -5461631921011218819L;

        protected JTextField serverField = null;

        protected JTextField portField = null;

        protected JCheckBox localCheckBox = null;

        /**
         * Creates current dialog.
         */
        public ServerSelectDialog() {
            super(MainGUI.this, "Select server", true);

            GridLayout layout = new GridLayout(3, 2, 6, 6);

            setLayout(layout);

            JLabel serverLabel = new JLabel("Server");
            serverField = new JTextField();

            JLabel portLabel = new JLabel("Port");
            portField = new JTextField();

            localCheckBox = new JCheckBox(
                    "Connect to local server with default port");
            localCheckBox.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {
                    if (localCheckBox.isSelected()) {
                        serverField.setText("127.0.0.1");
                        portField.setText("3310");

                        serverField.setEnabled(false);
                        portField.setEnabled(false);
                    }
                    else {
                        serverField.setEnabled(true);
                        portField.setEnabled(true);
                    }
                }
            });

            JButton button = new JButton("Select");

            add(serverLabel);
            add(serverField);

            add(portLabel);
            add(portField);

            add(localCheckBox);
            add(button);

            pack();
            setResizable(false);

            // Selecting "Connect to local server with default port" check box
            // and launching its ActionListener.
            localCheckBox.setSelected(true);
            localCheckBox.getActionListeners()[0].actionPerformed(null);
        }

    }

}
