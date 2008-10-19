package ilsrep.poll.client.gui;

import ilsrep.poll.client.TcpCommunicator;
import ilsrep.poll.common.Versioning;
import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.Answers;
import ilsrep.poll.common.protocol.Item;
import ilsrep.poll.common.protocol.Pollsessionlist;

import java.awt.Component;
import java.awt.Font;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyAdapter;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
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
import javax.swing.JTextField;
import javax.swing.ListSelectionModel;
import javax.swing.SwingUtilities;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;
import javax.swing.table.DefaultTableModel;
import javax.swing.table.TableModel;
import javax.xml.bind.JAXBException;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;
import org.jvnet.substance.skin.SubstanceOfficeSilver2007LookAndFeel;

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
     * Timeout for "double-click".
     */
    public static final int MOUSE_DOUBLE_CLICK_TIMEOUT = 200;

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
    protected int selectedPollsession = POLLSESSION_NOT_SELECTED;

    /**
     * Indicates that pollsession wasn't selected in current pollsession list.
     */
    protected static int POLLSESSION_NOT_SELECTED = -1;

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
     * Time of last mouse click.
     */
    protected long lastMouseClickedTime = -1;

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

        setLocationRelativeTo(null);
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
                        if (!checkServerSelected())
                            return;

                        if (connect())
                            updateList();

                        disconnect();
                    }
                });

                JMenuItem startSession = new JMenuItem();
                startSession.setText("Start poll session");
                startSession.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.PAGE_WHITE_GO_ICON));
                startSession.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        if (!checkServerSelected())
                            return;

                        startPollsession();
                    }
                });

                clientActions.add(updateItem);
                clientActions.add(startSession);
            }

            JMenu editorActions = new JMenu();
            editorActions.setText("Editor actions");
            {
                JMenuItem createNewItem = new JMenuItem();
                createNewItem.setText("Create new poll session");
                createNewItem.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.PAGE_WHITE_ADD_ICON));
                createNewItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        if (!checkServerSelected())
                            return;

                        createPollsession();
                    }
                });

                JMenuItem editExistingItem = new JMenuItem();
                editExistingItem.setText("Edit existing poll session");
                editExistingItem.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.PAGE_WHITE_EDIT_ICON));
                editExistingItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        if (!checkServerSelected())
                            return;

                        editPollsession();
                    }
                });

                JMenuItem deleteItem = new JMenuItem();
                deleteItem.setText("Delete poll session");
                deleteItem.setIcon(GUIUtilities
                        .loadIcon(GUIUtilities.PAGE_WHITE_DELETE_ICON));
                deleteItem.addActionListener(new ActionListener() {

                    @Override
                    public void actionPerformed(ActionEvent e) {
                        if (!checkServerSelected())
                            return;

                        deletePollsession();
                    }
                });

                editorActions.add(createNewItem);
                editorActions.add(editExistingItem);
                editorActions.add(deleteItem);
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
            menu.add(editorActions);
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
            String warnMessage = "One of user, password, server or port is empty - selected no server!";
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

        if (!connect()) {
            String cannotConnectToServerString = "Can't connect to " + server
                    + ":" + port + "!";
            selectNothingAndAlert(cannotConnectToServerString);
            logger.warn(cannotConnectToServerString);
            serverOk = false;
            return;
        }

        if (!login()) {
            String cannotConnectToServerString = "Login failed!";
            selectNothingAndAlert(cannotConnectToServerString);
            logger.warn(cannotConnectToServerString);
            serverOk = false;
            return;
        }

        if (!updateList()) {
            String cannotConnectToServerString = "Update list failed from "
                    + server + ":" + port + "!";
            selectNothingAndAlert(cannotConnectToServerString);
            logger.warn(cannotConnectToServerString);
            serverOk = false;
            return;
        }
        else
            serverOk = true;

        disconnect();
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
     * Updates pollsession list from server.<br>
     * You must be connected.
     * 
     * @return True, if list was updated.
     */
    public boolean updateList() {
        // GUIUtilities.showInfoDialog("Click \"Ok\" to start update from "
        // + server + ":" + port + " and wait.");

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

                Vector<Vector<Object>> pollsessionTableData = new Vector<Vector<Object>>();

                for (Item pollsessionIdName : sessionList.getItems()) {
                    Vector<Object> sessionRow = new Vector<Object>();
                    // sessionRow.add(pollsessionIdName.getId());

                    sessionRow.add(pollsessionIdName.getName());

                    pollsessionTableData.add(sessionRow);
                }

                JTable pollsessionTable = new JTable(
                        new NonEditableTableModel());

                pollsessionTable
                        .setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
                pollsessionTable.getSelectionModel().addListSelectionListener(
                        new ListSelectionListener() {

                            public void valueChanged(ListSelectionEvent e) {
                                if (e.getValueIsAdjusting())
                                    return;

                                ListSelectionModel lsm = (ListSelectionModel) e
                                        .getSource();
                                if (lsm.isSelectionEmpty()) {
                                    selectedPollsession = POLLSESSION_NOT_SELECTED;
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
                    // collumnNames.add("Id");
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

                pollsessionTable.addMouseListener(new MouseAdapter() {

                    /**
                     * @see java.awt.event.MouseAdapter#mouseClicked(java.awt.event.MouseEvent)
                     */
                    @Override
                    public void mouseClicked(MouseEvent e) {
                        if ((System.currentTimeMillis() - lastMouseClickedTime) < MOUSE_DOUBLE_CLICK_TIMEOUT) {
                            if (!checkServerSelected())
                                return;

                            startPollsession();
                        }

                        lastMouseClickedTime = System.currentTimeMillis();
                    }

                });

                listPanel.removeAll();
                listPanel.setLayout(new BoxLayout(listPanel, BoxLayout.Y_AXIS));

                listPanel.add(new JLabel("Pollsession list on " + server + ":"
                        + port));
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

    /**
     * If tab with pollsession list isn't created - creates it and focuses on
     * this tab.
     */
    private void updateListTab() {
        final String listTabName = "Pollsession list";

        if (tabbedPane.indexOfComponent(listPanel) != -1)
            listPanel.update(listPanel.getGraphics());

        activateTab(listTabName, listPanel);

        selectedPollsession = POLLSESSION_NOT_SELECTED;
    }

    /**
     * Processes pollsession in new tab.
     */
    private void startPollsession() {
        // When login was not validated before.
        if (!checkServerSelected())
            return;

        Pollsession sessionToStart = null;

        if (checkPollsessionSeleted())
            if ((sessionToStart = retrievePollsession()) != null) {
                PollsessionTab pollsessionTab = new PollsessionTab(
                        sessionToStart, this);

                activateTab("Pollsession: " + sessionToStart.getName(),
                        pollsessionTab);

                pollsessionTab.start();
            }
            else
                GUIUtilities.showWarningDialog("Can't connect to " + server
                        + ":" + port + "!");
    }

    /**
     * Retrieves current selected pollsession from server or shows warning on
     * connection/io problems.
     * 
     * @return Current pollsession or <code>null</code> if nothing selected.
     */
    private Pollsession retrievePollsession() {
        Pollsession retrievedPollsession = null;

        if (connect())
            try {
                retrievedPollsession = serverCommunicator
                        .getPollsession(currentSessionList.getItems().get(
                                selectedPollsession).getId());

                logger.info("Retrieved pollsession from server(id: "
                        + retrievedPollsession.getId() + ", name: "
                        + retrievedPollsession.getName() + ")");
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

        disconnect();

        return retrievedPollsession;
    }

    /**
     * Starts editing new pollsession.
     */
    private void createPollsession() {
        if (checkServerSelected()) {
            EditorTab editorTab = EditorTab.createNewPollsession(this);

            activateTab("Create new poll session", editorTab);

            editorTab.editPollsession();
        }
    }

    /**
     * Starts editing existing pollsession.
     */
    private void editPollsession() {
        if (checkPollsessionSeleted()) {
            Pollsession sessionToEdit = retrievePollsession();

            if (sessionToEdit != null) {
                EditorTab editorTab = EditorTab.editPollsession(this,
                        sessionToEdit);
                activateTab("Edit poll session: " + sessionToEdit.getName(),
                        editorTab);

                editorTab.editPollsession();
            }
        }
    }

    /**
     * Deletes pollsession from server.
     */
    private void deletePollsession() {
        if (checkPollsessionSeleted()) {
            if (GUIUtilities
                    .askYesNo("Do you really want to delete poll session: "
                            + currentSessionList.getItems().get(
                                    selectedPollsession).getName() + "?")) {
                if (connect()) {
                    serverCommunicator.deleteXml(currentSessionList.getItems()
                            .get(selectedPollsession).getId());

                    updateList();
                }
                disconnect();
            }
        }
    }

    /**
     * Checks if pollsession selected and shows warning if not.
     * 
     * @return <code>true</code>, if pollsession selected.
     */
    private boolean checkPollsessionSeleted() {
        if (selectedPollsession >= 0)
            return true;
        else {
            GUIUtilities.showWarningDialog("Pollsession not selected!");
            return false;
        }
    }

    /**
     * Checks if server is selected. If not - shows warning dialog.
     * 
     * @return <code>true</code>, if selected.
     */
    private boolean checkServerSelected() {
        if (!serverOk) {
            GUIUtilities.showWarningDialog("Server not selected!");
            logger
                    .warn("Server not selected, can't start pollsession or do other actions.");
            return false;
        }
        else
            return true;
    }

    /**
     * Establishes connection to server and stores in
     * {@link MainWindow#serverCommunicator}.
     * 
     * @return True, if connection was successful.
     */
    public boolean connect() {
        if (!serverSelected()) {
            GUIUtilities.showWarningDialog("Server not selected!");
            logger.warn("Server not selected, can't update.");
            return false;
        }

        boolean connected = false;
        String exceptionMessage = null;

        try {
            serverCommunicator = new TcpCommunicator(server, port);
            connected = true;
        }
        catch (Exception e) {
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
    public void disconnect() {
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

            BoxLayout aboutTabPanelLayout = new BoxLayout(aboutTabPanel,
                    BoxLayout.PAGE_AXIS);

            aboutTabPanel.setLayout(aboutTabPanelLayout);

            JLabel logoAndNameLabel = new JLabel("oll Application",
                    GUIUtilities
                            .loadIcon(GUIUtilities.POLL_APPLICATION_LOGO_ICON),
                    JLabel.LEFT);

            Font labelsFont = logoAndNameLabel.getFont().deriveFont(Font.BOLD);

            logoAndNameLabel.setFont(labelsFont);
            aboutTabPanel.add(logoAndNameLabel);

            JLabel guiClientLabel = new JLabel("GUI client");
            guiClientLabel.setFont(labelsFont);
            aboutTabPanel.add(guiClientLabel);

            aboutTabPanel.add(Box.createVerticalStrut(20));

            JLabel versionLabel = new JLabel("Version: "
                    + Versioning.getVersion(Versioning.COMPONENT_CLIENT_GUI));
            versionLabel.setFont(labelsFont);
            aboutTabPanel.add(versionLabel);
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
                setPollApplicationLookAndFeel();

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
     * Logins to server.<br>
     * You must be connected with {@link #connect()}.
     * 
     * @return If login was successful.
     */
    private boolean login() {
        return serverCommunicator.login(user, password);
    }

    /**
     * Sends results of pollsession to server.
     * 
     * @param results
     *            Results to send.
     */
    public void sendResults(Answers results) {
        connect();

        results.setUsername(user);

        serverCommunicator.sendResult(results);

        disconnect();
    }

    /**
     * Sends created/edited pollsession to server.
     * 
     * @param session
     * 
     * @return <code>true</code> if pollsession was ok and sent to server or
     *         <code>false</code> if pollsession was badly formed or sending
     *         failed.
     */
    public boolean sendPollsession(Pollsession session) {
        if (session.getName() == null || session.getName().isEmpty())
            return false;

        if (session.getTestMode() == null
                || !session.getTestMode().equals("true"))
            session.setTestMode("false");

        if (session.getTestMode().equals("true"))
            if (session.getMinScore() == null)
                return false;

        Collection<Poll> pollsToRemove = new ArrayList<Poll>();
        for (Poll poll : session.getPolls()) {
            if (poll.getName() == null || poll.getName().isEmpty()
                    || poll.getDescription() == null
                    || poll.getDescription().getValue() == null
                    || poll.getDescription().getValue().isEmpty()) {
                pollsToRemove.add(poll);
                continue;
            }

            Collection<Choice> choicesToRemove = new ArrayList<Choice>();

            for (Choice choice : poll.getChoices()) {
                if (choice.getName() == null || choice.getName().isEmpty())
                    choicesToRemove.add(choice);
            }

            poll.getChoices().removeAll(choicesToRemove);

            if (poll.getChoices().size() == 0) {
                pollsToRemove.add(poll);
                continue;
            }

            for (int i = 1; i <= poll.getChoices().size(); i++)
                poll.getChoices().get(i - 1).setId(Integer.toString(i));
        }

        session.getPolls().removeAll(pollsToRemove);

        for (int i = 1; i <= session.getPolls().size(); i++)
            session.getPolls().get(i - 1).setId(Integer.toString(i));

        if (session.getPolls().size() == 0)
            return false;

        for (Poll poll : session.getPolls())
            for (Choice choice : poll.getChoices())
                if (choice.getName().equals(poll.getCorrectChoice())) {
                    poll.setCorrectChoice(choice.getId());
                    break;
                }

        if (connect())
            try {
                if (session.getId() == null)
                    serverCommunicator.sendPollsession(session);
                else
                    serverCommunicator
                            .editPollsession(session.getId(), session);

                disconnect();
            }
            catch (JAXBException e) {
                return false;
            }
            catch (IOException e) {
                return false;
            }

        return true;
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
         * KeyListener to add to all components.
         */
        protected KeyListener closeDialogKeyListener = new KeyAdapter() {

            /**
             * @see java.awt.event.KeyAdapter#keyReleased(java.awt.event.KeyEvent)
             */
            @Override
            public void keyReleased(KeyEvent e) {
                if (e.getKeyCode() == KeyEvent.VK_ENTER)
                    closeSelectDialog();
            }
        };

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

            addKeyListener(closeDialogKeyListener);

            for (Component componentToSetKeyListener : getContentPane()
                    .getComponents())
                componentToSetKeyListener
                        .addKeyListener(closeDialogKeyListener);

            setLocationRelativeTo(null);
        }

        /**
         * Closes this dialog and selects server, port, user and password
         * entered by user into main window.
         */
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

    /**
     * Set L&F for Poll Application.
     */
    public static void setPollApplicationLookAndFeel() {
        try {
            // UIManager.setLookAndFeel(UIManager
            // .getCrossPlatformLookAndFeelClassName());

            // UIManager.setLookAndFeel(UIManager.getInstalledLookAndFeels()[3]
            // .getClassName());

            UIManager.setLookAndFeel(SubstanceOfficeSilver2007LookAndFeel.class
                    .getName());
        }
        catch (ClassNotFoundException e) {
        }
        catch (InstantiationException e) {
        }
        catch (IllegalAccessException e) {
        }
        catch (UnsupportedLookAndFeelException e) {
        }
    }

}
