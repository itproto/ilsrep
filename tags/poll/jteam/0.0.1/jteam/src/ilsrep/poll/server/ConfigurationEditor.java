package ilsrep.poll.server;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;

import javax.swing.JFileChooser;
import javax.xml.bind.JAXBException;

import net.sf.xpilotpanel.i18n.XPPI18N;
import net.sf.xpilotpanel.preferences.Preferences;
import net.sf.xpilotpanel.preferences.PreferencesEditor;
import net.sf.xpilotpanel.preferences.model.PreferencesModel;

/**
 * Visual configuration editor for poll server.
 * 
 * @author TKOST
 * 
 */
public class ConfigurationEditor {

    /**
     * Main method for configuration editor.
     * 
     * @param args
     *            Command line arguments.
     */
    public static void main(String[] args) {
        // Creating file chooser GUI.
        JFileChooser fileChooser = new JFileChooser(System.getProperties()
                .getProperty("user.dir"));
        fileChooser.setDialogTitle("Select poll server configuration file");

        // Starts GUI that allow user to choose file and then launches GUI that
        // edits it(using libraries from XPilotPanel).
        int userChooseOption = -1;
        userChooseOption = fileChooser.showOpenDialog(null);
        if (userChooseOption == JFileChooser.APPROVE_OPTION) {
            File configurationFile = fileChooser.getSelectedFile();

            if (configurationFile.exists() && configurationFile.isFile()
                    && configurationFile.canRead()
                    && configurationFile.canWrite())
                editConfiguration(configurationFile);
            else {
                System.out
                        .println("File can't be read or other error! Quitting!");
            }
        }
        else {
            System.out.println("You didn't specify file! Quitting");
        }
    }

    /**
     * Starts GUI that edits configuration file. <br>
     * 
     * It use libraries from XPilotPanel( http://xpilotpanel.sourceforge.net).
     * 
     * @param configurationFile
     */
    public static void editConfiguration(File configurationFile) {
        // Loading Preferences using model in jar and specified file.
        Preferences configuration = null;
        try {
            configuration = loadPreferences(configurationFile);
        }
        catch (FileNotFoundException e) {
            System.out
                    .println("ERROR: File was deleted by other program after selected. Quitting");
            System.exit(3);
        }
        catch (IOException e) { // Is thrown when configuration file can't be
            // loaded.
            System.out
                    .println("ERROR: while loading configuration model. Quitting!");
            System.exit(1);
        }
        catch (JAXBException e) { // Is thrown when configuration file is
            // corrupted(JAXB can't parse xml).
            System.out
                    .println("ERROR: configuration model is corrupted. Quitting!");
            System.exit(2);
        }

        // Building and starting PreferencesEditor.
        XPPI18N i18nSystem = new XPPI18N("data");
        PreferencesEditor configurationEditor = new PreferencesEditor(
                configuration, i18nSystem, "i18n");
        configurationEditor.setInterruptThread(Thread.currentThread());
        configurationEditor.setVisible(true);

        try {
            Thread.sleep(0); // Making current thread wait till
            // PreferencesEditor finishes editing and interrupt this thread to
            // execute code below.
        }
        catch (InterruptedException e) {
        }

        // Saving Preferences to same file from which they were loaded.
        OutputStream savingStream = null;
        try {
            savingStream = new FileOutputStream(configurationFile);
            configuration.store(savingStream);
        }
        catch (FileNotFoundException e) {
            System.out
                    .println("ERROR: File was deleted by other program after selected. Quitting");
            System.exit(3);
        }
        catch (IOException e) {
            System.out
                    .println("ERROR: Error writing file that was selected and"
                            + "approved as writeable(another program should have changed"
                            + "it). Quitting");
            System.exit(4);
        }
    }

    public static Preferences loadPreferences(File configurationFile)
            throws IOException, JAXBException {
        // Loading configuration model from jar.
        PreferencesModel configurationModel = null;
        configurationModel = PreferencesModel
                .loadModelFromURL(ConfigurationEditor.class.getClassLoader()
                        .getResource("data/configurationModel.xml"));

        Preferences configuration = null;

        // Loading preferences from specified file.
        configuration = Preferences.loadPreferences(configurationModel,
                new FileInputStream(configurationFile));

        return configuration;
    }
}
