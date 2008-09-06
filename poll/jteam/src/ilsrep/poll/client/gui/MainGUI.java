package ilsrep.poll.client.gui;

import ilsrep.poll.client.TcpCommunicator;
import ilsrep.poll.common.protocol.Item;
import ilsrep.poll.common.protocol.Pollsessionlist;

import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.IOException;
import java.net.UnknownHostException;
import java.util.Vector;

import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;
import javax.swing.JPanel;
import javax.swing.JTable;
import javax.swing.JTextField;
import javax.swing.ListSelectionModel;
import javax.swing.SwingUtilities;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;
import javax.swing.table.DefaultTableModel;
import javax.swing.table.TableModel;

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
     * @see GUIUtil
     */
    protected GUIUtil guiUtil = null;

    /**
     * Indicates if server is valid(after was connected to it once).
     */
    protected boolean serverOk = false;

    /**
     * <code>TcpCommunicator</code> - connection routine manager for current
     * server.
     */
    protected TcpCommunicator serverCommunicator = null;

    /**
     * Shows what pollsession from table is currently selected by user.
     */
    protected int selectedPollsession = -1;

    /**
     * Pollsession list from last update.
     */
    protected Pollsessionlist currentSessionList = null;

    /**
     * Creates main window.
     */
    public MainGUI() {
        super("Poll Application");

        guiUtil = new GUIUtil();

        setJMenuBar(createMenu());
        setSize(800, 600);
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
        // System.exit(0);
    }

    /**
     * Set server to operate in current session.
     * 
     * @param server
     *            Server IP(host).
     * @param port
     *            Server port.
     */
    public void selectServer(String server, String port) {
        try {
            this.port = Integer.parseInt(port);
        }
        catch (NumberFormatException e) {
            selectNothingAndAlert("Server port must be integer!");
            return;
        }

        if (this.port <= 0) {
            selectNothingAndAlert("Port must be larger then 0!");
            return;
        }

        this.server = server;

        if (!updateList()) {
            selectNothingAndAlert("Can't connect to " + server + ":" + port
                    + "!");
            return;
        }
        else
            serverOk = true;
    }

    /**
     * Selects no server and show alert.
     * 
     * @param alertion
     *            Alert to show.
     */
    private void selectNothingAndAlert(String alertion) {
        server = null;
        port = -1;
        serverOk = false;

        if (alertion != null)
            guiUtil.alert(alertion);
    }

    /**
     * Updates pollsession list from server.
     * 
     * @return True, if list was updated.
     */
    private boolean updateList() {
        if (server == null || port <= 0)
            return false;

        if (connect()) {
            guiUtil.infoWindow("Click \"Ok\" to start update from " + server
                    + ":" + port + " and wait.");

            Pollsessionlist sessionList = serverCommunicator.listXml();
            disconnect();

            if (sessionList != null && sessionList.getItems() != null) {
                currentSessionList = sessionList;

                if (sessionList.getItems().size() == 0) {
                    JPanel contentPanel = new JPanel();

                    JLabel listIsEmptyLabel = new JLabel("Server(" + server
                            + ":" + port + ") pollsession list is empty!");

                    contentPanel.add(listIsEmptyLabel);

                    setContentPane(contentPanel);
                }
                else {
                    Vector<Vector<String>> pollsessionTableData = new Vector<Vector<String>>();

                    for (Item pollsessionIdName : sessionList.getItems()) {
                        Vector<String> sessionRow = new Vector<String>();
                        sessionRow.add(pollsessionIdName.getId());
                        sessionRow.add(pollsessionIdName.getName());

                        pollsessionTableData.add(sessionRow);
                    }

                    JTable pollsessionTable = new JTable(
                            new NonEditableTableModel());

                    pollsessionTable
                            .setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
                    pollsessionTable.getSelectionModel()
                            .addListSelectionListener(
                                    new ListSelectionListener() {

                                        public void valueChanged(
                                                ListSelectionEvent e) {
                                            if (e.getValueIsAdjusting())
                                                return;

                                            ListSelectionModel lsm = (ListSelectionModel) e
                                                    .getSource();
                                            if (lsm.isSelectionEmpty()) {
                                                selectedPollsession = -1;
                                            }
                                            else {
                                                selectedPollsession = lsm
                                                        .getMinSelectionIndex();
                                            }
                                        }
                                    });

                    final Vector<String> collumnNames = new Vector<String>();
                    if (collumnNames.size() == 0) {
                        collumnNames.add("Id");
                        collumnNames.add("Name");
                    }

                    NonEditableTableModel pollsessionTableModel = (NonEditableTableModel) pollsessionTable
                            .getModel();
                    pollsessionTableModel.setDataVector(pollsessionTableData,
                            collumnNames);

                    JPanel contentPanel = new JPanel();
                    contentPanel.setLayout(new BoxLayout(contentPanel,
                            BoxLayout.Y_AXIS));

                    contentPanel.add(new JLabel("Pollsession list on " + server
                            + ":" + port));
                    contentPanel.add(pollsessionTable);

                    setContentPane(contentPanel);
                }
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    /**
     * Establishes connection to server and stores in
     * {@link MainGUI#serverCommunicator}.
     * 
     * @return True, if connection was successful.
     */
    private boolean connect() {
        try {
            serverCommunicator = new TcpCommunicator(server, port);
            return true;
        }
        catch (UnknownHostException e) {
            return false;
        }
        catch (IOException e) {
            return false;
        }
    }

    /**
     * Disconnects from server, if connected.
     */
    private void disconnect() {
        if (serverCommunicator != null)
            serverCommunicator.finalize();
        serverCommunicator = null;
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

    /**
     * Dialog for server selection.
     * 
     * @author Taras Kostiak
     * 
     */
    private class ServerSelectDialog extends JDialog {

        /**
         * Serial version UID.
         */
        private static final long serialVersionUID = -5461631921011218819L;

        /**
         * Text field to read server from.
         */
        protected JTextField serverField = null;

        /**
         * Text field to read port from.
         */
        protected JTextField portField = null;

        /**
         * Check box to read if use local server with default port.
         */
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
                        serverField.setText(TcpCommunicator.DEFAULT_SERVER);
                        portField.setText("" + TcpCommunicator.DEFAULT_PORT);

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
            button.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {
                    ServerSelectDialog.this.dispose();
                    selectServer(serverField.getText(), portField.getText());
                }
            });

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

    /**
     * Represents non-editable(by user) table model.
     * 
     * @author TKOST
     * 
     * @see TableModel
     */
    private class NonEditableTableModel extends DefaultTableModel {

        /**
         * Serial version UID.
         */
        private static final long serialVersionUID = 8014961739308652081L;

        /**
         * Return false(i.e. non editable) for any element.
         * 
         * @see javax.swing.table.DefaultTableModel#isCellEditable(int, int)
         */
        public boolean isCellEditable(int row, int column) {
            return false;
        }
    }

}
