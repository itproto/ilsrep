package ilsrep.poll.server;

import java.io.File;
import java.io.IOException;

import javax.xml.bind.JAXBException;

import net.sf.xpilotpanel.preferences.Preferences;

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
        if (args.length == 0) {
            System.out
                    .println("ERROR: Configuration file not specified. Quitting!");
            System.exit(1);
        }

        PollServer serverInstance = new PollServer(args[0]);
        serverInstance.lauch();

        System.exit(serverInstance.getExitCode());
    }

    /**
     * Configuration of this server.
     */
    Preferences configuration = null;

    /**
     * Stores code with which server's program should exit.
     */
    protected int exitCode = -1;

    /**
     * Creates <code>PollServer</code> and reads configuration from specified
     * file.
     * 
     * @param configurationFileName
     *            Configuration file name.
     */
    public PollServer(String configurationFileName) {
        File configurationFile = new File(configurationFileName);

        if (!(configurationFile.exists() && configurationFile.isFile() && configurationFile
                .canRead())) {
            System.out
                    .println("ERROR: Configuration file can't be read. Quitting!");
            serverShutdown(2);
        }

        configuration = null;
        try {
            configuration = ConfigurationEditor
                    .loadPreferences(configurationFile);
        }
        catch (IOException e) { // Is thrown when configuration file can't be
            // loaded.
            System.out
                    .println("ERROR: I/O error while loading configuration model. Quitting!");
            serverShutdown(2);
        }
        catch (JAXBException e) { // Is thrown when configuration file is
            // corrupted(JAXB can't parse xml) or model corrupted.
            System.out
                    .println("ERROR: specified configuration file is corrupted. Quitting!");
            serverShutdown(2);
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
        return exitCode;
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
        // TODO: Realsise this method.
    }

}
