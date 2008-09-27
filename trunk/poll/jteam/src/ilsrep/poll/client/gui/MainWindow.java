package ilsrep.poll.client.gui;

import ilsrep.poll.client.TcpCommunicator;
import ilsrep.poll.common.Versioning;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.Item;
import ilsrep.poll.common.protocol.Pollsessionlist;

import java.awt.Component;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.io.IOException;
import java.net.UnknownHostException;
import java.util.Vector;

import javax.swing.Box;
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
import javax.swing.JPasswordField;
import javax.swing.JTable;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.ListSelectionModel;
import javax.swing.SwingUtilities;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;
import javax.swing.table.DefaultTableModel;
import javax.swing.table.TableModel;
import javax.xml.bind.JAXBException;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

/**
 * Main GUI for Poll program(combines client, editor and any other possible Poll
 * client programs).
 * 
 * @author TKOST
 * 
 */
public class MainWindow extends JFrame {

    /**
     * Namespace path to log4j configuration for client.
     */
    public static String CLIENT_LOGGER_CONFIGURATION_FILE = "data/log4j-conf-client-gui.properties";

    /**
     * Log4j Logger for this class.
     */
    private static Logger logger = Logger.getLogger(MainWindow.class);

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
     * User name to login on server.
     */
    protected String user = null;

    /**
     * Password to login on server.
     */
    protected String password = null;

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
     * Tabbed pane of this window.
     */
    protected CloseableTabbedPane tabbedPane = null;

    /**
     * Panel that contains pollsession list.
     */
    protected JPanel listPanel = null;

    /**
     * Content of "About" tab.
     */
    protected JPanel aboutTabPanel = null;

    /**
     * Dialog for server/port and user/password selection.
     */
    protected ServerSelectDialog serverAndUserSelectDialog = null;

    /**
     * Creates main window.
     */
    public MainWindow() {
        super("Poll Application");

        setJMenuBar(createMenu());
        logger.debug("Created and set menu.");
        setSize(640, 480);
        logger.debug("Set size to main window: " + getSize().getWidth() + "x"
                + getSize().getHeight());

        listPanel = new JPanel();

        tabbedPane = new CloseableTabbedPane();
        getContentPane().add(tabbedPane);

        setIconImage(GUIUtilities.loadIcon(
                GUIUtilities.POLL_APPLICATION_LOGO_ICON).getImage());

        initAboutTab();
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
                selectServerItem.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.SERVER_KEY_ICON));
                selectServerItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        showSelectServerAndUserDialog();
                    }
                });

                JMenuItem exitItem = new JMenuItem();
                exitItem.setText("Exit");
                exitItem.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.DOOR_OUT_ICON));
                exitItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        exitProgram();
                    }
                });

                fileMenu.add(selectServerItem);
                fileMenu.add(exitItem);
            }

            JMenu clientActions = new JMenu();
            clientActions.setText("Client actions");
            {
                JMenuItem updateItem = new JMenuItem();
                updateItem.setText("Update list");
                updateItem.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.ARROW_ROTATE_CLOCKWISE_ICON));
                updateItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        updateList();
                    }
                });

                JMenuItem startSession = new JMenuItem();
                startSession.setText("Start poll session");
                startSession.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.PAGE_WHITE_GO_ICON));
                startSession.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        startPollsession();
                    }
                });

                clientActions.add(updateItem);
                clientActions.add(startSession);
            }

            JMenu helpMenu = new JMenu();
            helpMenu.setText("Help");
            {
                JMenuItem aboutItem = new JMenuItem();
                aboutItem.setText("About");
                aboutItem.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.INFORMATION_ICON));
                aboutItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        initAboutTab();
                    }
                });

                helpMenu.add(aboutItem);
            }

            menu.add(fileMenu);
            menu.add(clientActions);
            menu.add(helpMenu);
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
     * Creates new dialog for server/port and user/password pairs selection or
     * activates one if it was already created(with old values).
     */
    private void showSelectServerAndUserDialog() {
        if (serverAndUserSelectDialog == null)
            serverAndUserSelectDialog = new ServerSelectDialog();

        serverAndUserSelectDialog.setVisible(true);
    }

    /**
     * Set server to operate in current session.
     * 
     * @param server
     *            Server IP(host).
     * @param port
     *            Server port.
     */
    public void selectServerAndUser(String server, String port, String user,
            String password) {
        if (server == null || server.isEmpty() || port == null
                || port.isEmpty() || user == null || user.isEmpty()
                || password == null || password.isEmpty()) {
            String warnMessage = "One of server, port, user or password is empty - selected no server!";
            logger.warn(warnMessage);
            GUIUtilities.showWarningDialog(warnMessage);
            return;
        }

        logger.debug("User selected server " + server + ":" + port);
        try {
            this.port = Integer.parseInt(port);
        }
        catch (NumberFormatException e) {
            final String portMustBeIntegerWarning = "Server port must be integer!";
            selectNothingAndAlert(portMustBeIntegerWarning);
            logger.warn(portMustBeIntegerWarning + " (user entered" + port
                    + ")");
            serverOk = false;
            return;
        }

        if (this.port <= 0) {
            final String portMustBeLargerThenZeroWarning = "Port must be larger then 0!";
            selectNothingAndAlert(portMustBeLargerThenZeroWarning);
            logger.warn(portMustBeLargerThenZeroWarning + " (user entered"
                    + port + ")");
            serverOk = false;
            return;
        }

        this.server = server;
        this.user = user;
        this.password = password;

        logger.info("Server(" + this.server + ":" + this.port
                + ") selected by user(" + user
                + ") is valid. Using it for work.");

        if (!updateList()) {
            String cannotConnectToServerString = "Can't connect to " + server
                    + ":" + port + "!";
            selectNothingAndAlert(cannotConnectToServerString);
            logger.warn(cannotConnectToServerString);
            serverOk = false;
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
            GUIUtilities.showWarningDialog(alertion);
    }

    /**
     * Shows if server was selected by user.
     * 
     * @return True, if yes.
     */
    private boolean serverSelected() {
        return server != null && port > 0;
    }

    /**
     * Updates pollsession list from server.
     * 
     * @return True, if list was updated.
     */
    private boolean updateList() {
        if (!serverSelected()) {
            GUIUtilities.showWarningDialog("Server not selected!");
            logger.warn("Server not selected, can't update.");
            return false;
        }

        if (connect()) {
            GUIUtilities.showInfoDialog("Click \"Ok\" to start update from "
                    + server + ":" + port + " and wait.");

            Pollsessionlist sessionList = serverCommunicator.listXml();

            logger.info("Received " + sessionList.getItems().size()
                    + " elements list from server.");

            disconnect();

            if (sessionList != null && sessionList.getItems() != null) {
                currentSessionList = sessionList;

                if (sessionList.getItems().size() == 0) {
                    listPanel.removeAll();

                    String emptyList = "Server(" + server + ":" + port
                            + ") pollsession list is empty!";
                    JLabel listIsEmptyLabel = new JLabel(emptyList);
                    logger.warn(emptyList);

                    listPanel.add(listIsEmptyLabel);
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

                    Vector<String> collumnNames = null;
                    if (collumnNames == null) {
                        collumnNames = new Vector<String>();
                        collumnNames.add("Id");
                        collumnNames.add("Name");
                    }

                    NonEditableTableModel pollsessionTableModel = (NonEditableTableModel) pollsessionTable
                            .getModel();
                    pollsessionTableModel.setDataVector(pollsessionTableData,
                            collumnNames);

                    pollsessionTable.getColumnModel().getColumn(0)
                            .setPreferredWidth(
                                    (int) (0.25 * pollsessionTable.getSize()
                                            .getWidth()));

                    listPanel.removeAll();
                    listPanel.setLayout(new BoxLayout(listPanel,
                            BoxLayout.Y_AXIS));

                    listPanel.add(new JLabel("Pollsession list on " + server
                            + ":" + port));
                    listPanel.add(pollsessionTable);
                }

                updateListTab();

                return true;
            }
            else {
                if (sessionList == null)
                    logger
                            .warn("Pollsessionlist is null. Corrupted packet from server or response packet wasn't received at all.");
                else
                    if (sessionList.getItems() == null) {
                        logger
                                .warn("Items array of Pollsessionlist is null. Corrupted packet from server.");
                    }

                return false;
            }
        }
        else
            return false;
    }

    /**
     * If tab with pollsession list isn't created - creates it and focuses on
     * this tab.
     */
    private void updateListTab() {
        final String listTabName = "Pollsession list";

        activateTab(listTabName, listPanel);
    }

    /**
     * Processes pollsession in new tab.
     */
    private void startPollsession() {
        if (selectedPollsession >= 0)
            if (connect()) {
                try {
                    Pollsession sessionToStart = serverCommunicator
                            .getPollsession(currentSessionList.getItems().get(
                                    selectedPollsession).getId());

                    disconnect();

                    logger.info("Retrieved pollsession from server(id: "
                            + sessionToStart.getId() + ", name: "
                            + sessionToStart.getName() + ")");

                    PollsessionTab pollsessionTab = new PollsessionTab(
                            sessionToStart, this);

                    activateTab("Pollsession: " + sessionToStart.getName(),
                            pollsessionTab);

                    pollsessionTab.start();
                }
                catch (IOException e) {
                    GUIUtilities
                            .showWarningDialog("Exception while processing pollsession: "
                                    + e.getMessage());
                }
                catch (JAXBException e) {
                    GUIUtilities
                            .showWarningDialog("Exception while processing pollsession: "
                                    + e.getMessage());
                }
            }
            else
                GUIUtilities.showWarningDialog("Can't connect to " + server
                        + ":" + port + "!");
        else
            GUIUtilities.showWarningDialog("Pollsession not selected!");
    }

    /**
     * Establishes connection to server and stores in
     * {@link MainWindow#serverCommunicator}.
     * 
     * @return True, if connection was successful.
     */
    private boolean connect() {
        boolean connected = false;
        String exceptionMessage = null;

        try {
            serverCommunicator = new TcpCommunicator(server, port);
            connected = true;
        }
        catch (UnknownHostException e) {
            connected = false;
            exceptionMessage = e.getMessage();
        }
        catch (IOException e) {
            connected = false;
            exceptionMessage = e.getMessage();
        }

        if (!connected)
            logger.warn("Couldn't connect to " + server + ":" + port
                    + "! (exception message: " + exceptionMessage + ")");
        else
            logger.info("Connected to " + server + ":" + port + ".");

        return connected;
    }

    /**
     * Disconnects from server, if connected.
     */
    private void disconnect() {
        if (serverCommunicator != null)
            serverCommunicator.finalize();

        serverCommunicator = null;

        if (serverSelected())
            logger.info("Disconnected from " + server + ":" + port + ".");
    }

    /**
     * Builds "About" tab and/or(if it was already built) activates it.
     */
    private void initAboutTab() {
        final String aboutTabName = "About";

        if (aboutTabPanel == null) {
            aboutTabPanel = new JPanel();

            JTextArea infoTextArea = new JTextArea();

            final char endl = '\n';
            infoTextArea.setText("Poll Application" + endl + "GUI client"
                    + endl + endl + "Version:"
                    + Versioning.getVersion(Versioning.COMPONENT_CLIENT_GUI));
            infoTextArea.setEditable(false);

            aboutTabPanel.add(infoTextArea);
        }

        activateTab(aboutTabName, aboutTabPanel);
    }

    /**
     * Activates tab:
     * <ul>
     * <li>if it was not added yet - adds it</li>
     * <li>if it was added already - make focus on it</li>
     * </ul>
     * 
     * @param name
     *            Tab name.
     * @param content
     *            Reference to tab content.
     */
    private void activateTab(String name, JPanel content) {
        int tabIndex = tabbedPane.indexOfTab(name);

        if (tabIndex == -1) {
            logger.info("Added new tab: " + name);
            tabbedPane.addCloseableTab(name, content);
            tabbedPane.setSelectedIndex(tabbedPane.getTabCount() - 1);
        }
        else {
            logger.info("Activated tab: " + name);
            tabbedPane.setSelectedIndex(tabIndex);
        }
    }

    /**
     * Removes tab by given reference to tab content.
     * 
     * @param tabToRemove
     *            Reference to tab content.
     */
    public void removeTabByInstance(Component tabToRemove) {
        int tabIndexToRemove = tabbedPane.indexOfComponent(tabToRemove);

        if (tabIndexToRemove != -1)
            tabbedPane.remove(tabIndexToRemove);
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
                MainWindow gui = new MainWindow();
                gui.setVisible(true);
                gui.showSelectServerAndUserDialog();
            }
        };

        PropertyConfigurator.configure(MainWindow.class.getClassLoader()
                .getResource(CLIENT_LOGGER_CONFIGURATION_FILE));

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
         * Text field to enter server.
         */
        protected JTextField serverField = null;

        /**
         * Text field to enter port.
         */
        protected JTextField portField = null;

        /**
         * Text field to enter user.
         */
        protected JTextField userField = null;

        /**
         * <code>JPasswordField</code> field to enter password.
         */
        protected JPasswordField passwordField = null;

        /**
         * Check box to read if use local server with default port.
         */
        protected JCheckBox localCheckBox = null;

        /**
         * Creates current dialog.
         */
        public ServerSelectDialog() {
            super(MainWindow.this, "Select server and login", true);

            setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
            addWindowListener(new WindowAdapter() {

                public void windowClosing(WindowEvent e) {
                    closeSelectDialog();
                }
            });

            GridLayout layout = new GridLayout(0, 2, 6, 6);

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
                    closeSelectDialog();
                }
            });

            JLabel userLabel = new JLabel("User");
            userField = new JTextField();

            JLabel passwordLabel = new JLabel("Password");
            passwordField = new JPasswordField();

            add(serverLabel);
            add(serverField);

            add(portLabel);
            add(portField);

            add(localCheckBox);
            add(Box.createGlue());

            add(userLabel);
            add(userField);

            add(passwordLabel);
            add(passwordField);

            add(Box.createGlue());
            add(button);

            pack();
            setResizable(false);

            // Selecting "Connect to local server with default port" check box
            // and launching its ActionListener.
            localCheckBox.setSelected(true);
            localCheckBox.getActionListeners()[0].actionPerformed(null);
        }

        private void closeSelectDialog() {
            ServerSelectDialog.this.setVisible(false);
            selectServerAndUser(serverField.getText(), portField.getText(),
                    userField.getText(),
                    new String(passwordField.getPassword()));
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
