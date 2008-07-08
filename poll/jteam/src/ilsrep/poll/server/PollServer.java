package ilsrep.poll.server;

import ilsrep.poll.common.Pollsession;
import java.io.* ;
import java.net.*;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FilenameFilter;
import java.io.IOException;
import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.List;
import java.util.*;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

import net.sf.xpilotpanel.preferences.Preferences;
import net.sf.xpilotpanel.preferences.model.PreferenceSelector;

/**
 * Main class for poll server.
 * 
 * @author TKOST
 * 
 */
public class PollServer {

    /**
     * Log4j Logger for this class.
     */
    private static Logger logger = Logger.getLogger(PollServer.class);

    /**
     * Namespace path to log4j configuration.
     */
    public static String LOGGER_CONFIGURATION_FILE = "data/log4j-conf.properties";

    /**
     * Main method for poll server. <br>
     * 
     * Exit codes: <br>
     * <ul>
     * <li>0 - normal</li>
     * <li>1 - configuration file isn't specified</li>
     * <li>2 - configuration file can't be loaded</li>
     * <li>3 - can't bind ServerSocket to port</li>
     * <li>4 - problems with reflection</li>
     * </ul>
     * 
     * @param args
     *            Command line arguments.
     */
    public static void main(String[] args) {
        // Loading log4j configuration.
        PropertyConfigurator.configure(PollServer.class.getClassLoader()
                .getResource(LOGGER_CONFIGURATION_FILE));

        // Check if configuration file specified.
        if (args.length == 0) {
            logger.fatal("Configuration file not specified. Quitting!");
            System.exit(1);
        }

        // Creating poll server instance and loading poll xml's.
        PollServer serverInstance = new PollServer(args[0]);

        // Lauching server listening to clients on port.
        serverInstance.lauch();

        // Exiting program with exit code got from server.
        logger.info("Exiting program with code: "
                + serverInstance.getExitCode());
        System.exit(serverInstance.getExitCode());
    }

    /**
     * Configuration of this server.
     */
    Preferences configuration = null;

    /**
     * Stores code with which server's program should exit. <br>
     * Also keeps server state.
     */
    protected int exitCode = -1;

    /**
     * Pollsessions that have been loaded to server.
     */
    protected List<Pollsession> pollsessions = null;

    /**
     * Port to start server on.
     */
    protected int port = -1;
Hashtable<String, File> pollFiles=new Hashtable<String, File>(); 
    /**
     * Maximum connections to server.
     */
    protected int maxConnections = -1;

    /**
     * Shows if start server on alternative IP address.
     */
    protected boolean useAlternativeIPAddress = false;

    /**
     * IP address to start server on.
     */
    protected InetAddress alternativeIPAddress = null;

    /**
     * Holds all active connections.
     */
    protected List<Socket> connections = null;

    /**
     * Creates <code>PollServer</code> and reads configuration from specified
     * file.
     * 
     * @param configurationFileName
     *            Configuration file name.
     */
    public PollServer(String configurationFileName) {
        logger.debug("Creating server instance.");

        File configurationFile = new File(configurationFileName);

        // Check if configuration file exists, is file and is readable.
        if (!(configurationFile.exists() && configurationFile.isFile() && configurationFile
                .canRead())) {
            logger.fatal("Configuration file can't be read. Quitting!");
            serverShutdown(2);
            return;
        }

        // Loading configuration.
        configuration = null;
        try {
            logger.info("Loading configuration from file: "
                    + configurationFile.getAbsolutePath());
            configuration = ConfigurationEditor
                    .loadPreferences(configurationFile);
        }
        catch (IOException e) { // Is thrown when configuration file can't be
            // loaded.
            logger.fatal("I/O error while reading configuration. Quitting!");
            serverShutdown(2);
            return;
        }
        catch (JAXBException e) { // Is thrown when configuration file is
            // corrupted(JAXB can't parse xml) or model corrupted.
            logger
                    .fatal("Specified configuration file is corrupted. Quitting!");
            serverShutdown(2);
            return;
        }

        // Processing configuration.
        try {
            port = Integer.parseInt(configuration.get("port"));
        }
        catch (NumberFormatException e) {
            port = -1;
        }
        finally {
            if (port <= 0) {
                logger
                        .warn("Specified port was wrong(not integer or less than zero). Using default.");
                PreferenceSelector pSelector = new PreferenceSelector();
                pSelector.setName("port");
                port = Integer.parseInt(configuration.getModel()
                        .getPreferenceBySelector(pSelector).getDefaultValue());
            }
        }

        try {
            maxConnections = Integer.parseInt(configuration
                    .get("maxConnections"));
        }
        catch (NumberFormatException e) {
            maxConnections = -1;
        }
        finally {
            if (maxConnections <= 0) {
                logger
                        .warn("Specified maxConnections was wrong(not integer or less than zero). Using default.");
                PreferenceSelector pSelector = new PreferenceSelector();
                pSelector.setName("maxConnections");
                maxConnections = Integer.parseInt(configuration.getModel()
                        .getPreferenceBySelector(pSelector).getDefaultValue());
            }
        }

        if (configuration.get("useAlternativeIPAddress").compareTo("true") == 0) {
            useAlternativeIPAddress = true;
            try {
                alternativeIPAddress = InetAddress.getByName(configuration
                        .get("alternativeIPAddress"));
            }
            catch (UnknownHostException e) {
                logger.warn("Wrong alternative IP address was specified("
                        + configuration.get("alternativeIPAddress")
                        + "). Using default.");
                alternativeIPAddress = null;
                useAlternativeIPAddress = false;
            }
        }

        // Reading all poll xml's from specified directory into memory(objects).
        //DRC to TKOST: For what frigging reason do we need to do that?
        pollsessions = new Vector<Pollsession>();

        File xmlDir = new File(configuration.get("pollXmlPath"));

        if (xmlDir.exists() && xmlDir.isDirectory()) {
            File[] filesInDir = xmlDir.listFiles(new FilenameFilter() {

                @Override
                public boolean accept(File dir, String name) {
                    int pointPosition = name.lastIndexOf((int) '.');
                    return name.substring(pointPosition + 1).compareTo("xml") == 0;
                }
            });

            for (File file : filesInDir) {
                try {
	                                    logger.info("Loading file as poll xml: "
                            + file.getAbsolutePath());
                    JAXBContext cont = JAXBContext
                            .newInstance(Pollsession.class);
                    Unmarshaller um = cont.createUnmarshaller();

                    Pollsession session = (Pollsession) um
                            .unmarshal(new FileInputStream(file));
                    pollsessions.add(session);
                    pollFiles.put(session.getId(),file);
                }
                catch (JAXBException e) {
                    logger.warn("Poll xml file is corrupted: "
                            + file.getAbsolutePath());
                }
                catch (FileNotFoundException e) {
                    logger.warn("Poll xml file is not found: "
                            + file.getAbsolutePath());
                }
            }
        }
    }

    /**
     * Normal server shutdown. <br>
     * Stops communication with all clients and exits program.
     */
    public void serverShutdown() {
        serverShutdown(0);
    }

    /**
     * Server shutdown with problem.
     * 
     * @param code
     *            Code of problem(0 - for normal shutdown).
     * @see #serverShutdown()
     */
    public void serverShutdown(int code) {
        exitCode = code;
    }

    /**
     * Returns code with what exit program.
     * 
     * @see #exitCode
     * @see #main(String[])
     */
    public int getExitCode() {
        return (exitCode >= 0) ? exitCode : 0;
    }

    /**
     * Shows if server is working.
     * 
     * @return True, if is.
     */
    public boolean isAlive() {
        return (exitCode == -1);
    }

    /**
     * Starts server listening on port.
     */
    public void lauch() {
        if (!isAlive())
            return;

        // Binding server to port.
        ServerSocket serverSock = null;
        try {
            if (!useAlternativeIPAddress)
                serverSock = new ServerSocket(port, maxConnections);
            else
                serverSock = new ServerSocket(port, maxConnections,
                        alternativeIPAddress);
            logger.info("Bound server to port: " + port);
            connections = new Vector<Socket>();
        }
        catch (IOException e) {
            logger.fatal("Can't bind ServerSocket to port. Quitting!");
            serverShutdown(3);
            return;
        }

        // Working body of server - connections are accepted and processed in
        // concurrent threads.
        while (true) {
            Socket client = null;
            try {
                logger.debug("Accepting client connection.");
                client = serverSock.accept();

                if (connections.size() > maxConnections) {
                    logger
                            .warn("Maximum number of connections used. Ignoring!");
                    client.close();
                    continue;
                }
            }
            catch (IOException e) {
                logger
                        .error("I/O exception while accepting client connection. Ignoring!");
                continue;
            }

            // Creating and lauching PollClientHandler to recieved connection
            // via reflection.
            String reflectionProblemMessage = "Problems with reflection. Quitting!";
            boolean reflectionProblemHappened = false;

            Class<?> handlerClass = null;
            try {
                handlerClass = Class
                        .forName("ilsrep.poll.server.PollClientHandler");
                Constructor<?> constructor = handlerClass.getConstructor();

                ClientHandler handler = (ClientHandler) constructor
                        .newInstance();

                registerConnection(client);
                handler.handle(client, this);
            }
            catch (ClassNotFoundException e) {// Only will be invoked if
                // "PollClientHandler" is not in
                // classpath.
                reflectionProblemHappened = true;
            }
            catch (NoSuchMethodException e) {
                reflectionProblemHappened = true;
            }
            catch (IllegalArgumentException e) {
                reflectionProblemHappened = true;
            }
            catch (InstantiationException e) {
                reflectionProblemHappened = true;
            }
            catch (IllegalAccessException e) {
                reflectionProblemHappened = true;
            }
            catch (InvocationTargetException e) {
                reflectionProblemHappened = true;
            }
            finally {
                if (reflectionProblemHappened) {
                    logger.fatal(reflectionProblemMessage);
                    serverShutdown(4);
                    return;
                }
            }
        }
    }

    /**
     * Searches for pollsession with specified id.
     * 
     * @param id
     *            Id to search for.
     * @return Pollsession with specified id.
     */
    public synchronized Pollsession getPollsessionById(String id) {
        Pollsession searchResult = null;

        for (Pollsession pls : pollsessions) {
            if (pls.getId().compareTo(id) == 0) {
                searchResult = pls;
                break;
            }
        }

        return searchResult;
    }

    /**
     * Adds connection to list of active connections.
     * 
     * @param socketToAdd
     *            Connection's socket to add.
     */
    public synchronized void registerConnection(Socket socketToAdd) {
        connections.add(socketToAdd);
        logger.debug("New connection accepted. Active connection count: "
                + connections.size());
    }

    /**
     * Removes connection from list of active connections.
     * 
     * @param socketToRemove
     *            Connection's socket to remove.
     */
    public synchronized void removeConnection(Socket socketToRemove) {
        for (int connIterator = 0; connIterator < connections.size(); connIterator++) {
            if (connections.get(connIterator) == socketToRemove) {
                connections.remove(connIterator);
                break;
            }
        }
        logger.debug("Connection closed. Active connection count: "
                + connections.size());
    }

}