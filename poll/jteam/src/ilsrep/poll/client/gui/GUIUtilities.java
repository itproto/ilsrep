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
     * Filename of <code>information.png</code>.
     */
    public static final String INFORMATION_ICON = "information.png";

    /**
     * Filename of <code>server_key.png</code>.
     */
    public static final String SERVER_KEY_ICON = "server_key.png";

    /**
     * Filename of <code>arrow_rotate_clockwise.png</code>.
     */
    public static final String ARROW_ROTATE_CLOCKWISE_ICON = "arrow_rotate_clockwise.png";

    /**
     * Filename of <code>door_out.png</code>.
     */
    public static final String DOOR_OUT_ICON = "door_out.png";

    /**
     * Filename of <code>page_white_go.png</code>.
     */
    public static final String PAGE_WHITE_GO_ICON = "page_white_go.png";

    /**
     * Contains <code>Poll Application</code>, for window titles.
     */
    public static final String DIALOG_TITLE_BASE = "Poll Application";

    /**
     * Poll Application logo.
     */
    public static final String POLL_APPLICATION_LOGO_ICON = "logo.png";

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
     * Shows info dialog.
     * 
     * @param message
     *            Message to show.
     */
    public static void showInfoDialog(String message) {
        JOptionPane infoPane = new JOptionPane(message,
                JOptionPane.INFORMATION_MESSAGE);

        JDialog infoDialog = infoPane.createDialog("Information - "
                + DIALOG_TITLE_BASE);

        infoDialog.setIconImage(loadIcon(INFORMATION_ICON).getImage());

        infoDialog.setVisible(true);
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