package ilsrep.poll.server;

import ilsrep.poll.common.Pollsession;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FilenameFilter;
import java.io.IOException;
import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.List;
import java.util.Vector;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;

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
     * Main method for poll server. <br>
     * 
     * Exit codes: <br>
     * <ul>
     * <li>0 - normal</li>
     * <li>1 - configuration file isn't specified</li>
     * <li>2 - configuration file can't be loaded</li>
     * </ul>
     * 
     * @param args
     *            Command line arguments.
     */
    public static void main(String[] args) {
        // Check if configuration file specified.
        if (args.length == 0) {
            System.out
                    .println("ERROR: Configuration file not specified. Quitting!");
            System.exit(1);
        }

        // Creating poll server instance and loading poll xml's.
        PollServer serverInstance = new PollServer(args[0]);

        // Lauching server listening to clients on port.
        serverInstance.lauch();

        // Exiting program with exit code got from server.
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
    protected String alternativeIPAddress = null;

    /**
     * Creates <code>PollServer</code> and reads configuration from specified
     * file.
     * 
     * @param configurationFileName
     *            Configuration file name.
     */
    public PollServer(String configurationFileName) {
        File configurationFile = new File(configurationFileName);

        // Check if configuration file exists, is file and is readable.
        if (!(configurationFile.exists() && configurationFile.isFile() && configurationFile
                .canRead())) {
            System.out
                    .println("ERROR: Configuration file can't be read. Quitting!");
            serverShutdown(2);
        }

        // Loading configuration.
        configuration = null;
        try {
            configuration = ConfigurationEditor
                    .loadPreferences(configurationFile);
        }
        catch (IOException e) { // Is thrown when configuration file can't be
            // loaded.
            System.out
                    .println("ERROR: I/O error while reading configuration. Quitting!");
            serverShutdown(2);
        }
        catch (JAXBException e) { // Is thrown when configuration file is
            // corrupted(JAXB can't parse xml) or model corrupted.
            System.out
                    .println("ERROR: specified configuration file is corrupted. Quitting!");
            serverShutdown(2);
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
                PreferenceSelector pSelector = new PreferenceSelector();
                pSelector.setName("maxConnections");
                maxConnections = Integer.parseInt(configuration.getModel()
                        .getPreferenceBySelector(pSelector).getDefaultValue());
            }
        }

        if (configuration.get("useAlternativeIPAddress").compareTo("true") == 0) {
            useAlternativeIPAddress = true;
            alternativeIPAddress = configuration.get("alternativeIPAddress");
        }

        // Reading all poll xml's from specified directory into memory(objects).
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
                    JAXBContext cont = JAXBContext
                            .newInstance(Pollsession.class);
                    Unmarshaller um = cont.createUnmarshaller();

                    Pollsession session = (Pollsession) um
                            .unmarshal(new FileInputStream(file));
                    pollsessions.add(session);
                }
                catch (JAXBException e) {
                    // Doing nothing - ignoring file.
                }
                catch (FileNotFoundException e) {
                    // Doing nothing - ignoring file.
                }
            }
        }
    }

    /**
     * Normal server shutdown. <br>
     * Stops communication with all clients and exits program.
     */
    public void serverShutodown() {
        serverShutdown(0);
    }

    /**
     * Server shutdown with problem.
     * 
     * @param code
     *            Code of problem(0 - for normal shutdown).
     * @see #serverShutodown()
     */
    public void serverShutdown(int code) {
        exitCode = code;
    }

    /**
     * @see #exitCode
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
        ServerSocket serverSock = null;
        try {
            serverSock = new ServerSocket(port, maxConnections);
        }
        catch (IOException e) {
            System.out
                    .println("ERROR! Can't bind ServerSocket to port. Quitting!");
            serverShutdown(3);
            return;
        }

        while (true) {
            Socket client = null;
            try {
                client = serverSock.accept();
            }
            catch (IOException e) {
                continue;
            }

            Class<?> handlerClass = null;
            try {
                handlerClass = Class
                        .forName("ilsrep.poll.server.PollClientHandler");
                Constructor<?> constructor = handlerClass.getConstructor();

                ClientHandler handler = (ClientHandler) constructor
                        .newInstance();

                handler.handle(client, this);
            }
            catch (ClassNotFoundException e) {// Only will be invoked if
                // "PollClientHandler" is not in
                // classpath.
                System.out
                        .println("ERROR! Class PollClientHandler not found. Quitting!");
                serverShutdown(4);
                return;
            }
            catch (NoSuchMethodException e) {
                System.out
                        .println("ERROR! Class PollClientHandler don't have required constructor. Quitting!");
                serverShutdown(4);
                return;
            }
            catch (IllegalArgumentException e) {
                // Fix...
                serverShutdown(4);
                return;
            }
            catch (InstantiationException e) {
                // Fix...
                serverShutdown(4);
                return;
            }
            catch (IllegalAccessException e) {
                // Fix...
                serverShutdown(4);
                return;
            }
            catch (InvocationTargetException e) {
                // Fix...
                serverShutdown(4);
                return;
            }
        }
    }

}
