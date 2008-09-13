package ilsrep.poll.client.gui;

import java.net.URL;

import javax.swing.ImageIcon;
import javax.swing.JDialog;
import javax.swing.JOptionPane;

import ilsrep.poll.client.gui.old.GUIUtil;

/**
 * GUI utilities for Poll application.<br>
 * Replaces old buggy {@link GUIUtil}.
 * 
 * @author TKOST
 * 
 */
public class GUIUtilities {

    /**
     * Path to icons.
     */
    public static final String ICONS_LOCATION = "data/icons";

    /**
     * Filename of <code>exclamation.png</code>.
     */
    public static final String ERROR_ICON = "error.png";

    /**
     * Contains <code>Poll Application</code>, for window titles.
     */
    public static final String DIALOG_TITLE_BASE = "Poll Application";

    /**
     * Shows warning dialog.
     * 
     * @param message
     *            Message to show.
     */
    public static void showWarningDialog(String message) {
        JOptionPane warningPane = new JOptionPane(message,
                JOptionPane.WARNING_MESSAGE);

        JDialog warningDialog = warningPane.createDialog("Warning - "
                + DIALOG_TITLE_BASE);

        warningDialog.setIconImage(loadIcon(ERROR_ICON).getImage());

        warningDialog.setVisible(true);
    }

    /**
     * Loads icon from Poll application icons loaction.
     * 
     * @param name
     *            Icon filename.
     * @return Loaded icon or <code>null</code> if loading failed.
     */
    public static ImageIcon loadIcon(String name) {
        URL iconURL = GUIUtilities.class.getClassLoader().getResource(
                ICONS_LOCATION + '/' + name);

        ImageIcon loadedIcon = null;

        if (iconURL != null)
            loadedIcon = new ImageIcon(iconURL);

        return loadedIcon;
    }

}
