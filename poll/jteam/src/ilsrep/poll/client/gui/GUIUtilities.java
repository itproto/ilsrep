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
     * Filename of <code>arrow_left.png</code>.
     */
    public static final String ARROW_LEFT = "arrow_left.png";

    /**
     * Filename of <code>door_out.png</code>.
     */
    public static final String DOOR_OUT_ICON = "door_out.png";

    /**
     * Filename of <code>page_white_go.png</code>.
     */
    public static final String PAGE_WHITE_GO_ICON = "page_white_go.png";

    /**
     * Filename of <code>page_white_add.png</code>.
     */
    public static final String PAGE_WHITE_ADD_ICON = "page_white_add.png";

    /**
     * Filename of <code>page_white_delete.png</code>.
     */
    public static final String PAGE_WHITE_DELETE_ICON = "page_white_delete.png";

    /**
     * Filename of <code>page_white_edit.png</code>.
     */
    public static final String PAGE_WHITE_EDIT_ICON = "page_white_edit.png";

    /**
     * Filename of <code>add.png</code>.
     */
    public static final String ADD_ICON = "add.png";

    /**
     * Filename of <code>cancel.png</code>.
     */
    public static final String CANCEL_ICON = "cancel.png";

    /**
     * Filename of <code>pencil.png</code>.
     */
    public static final String PENCIL_ICON = "pencil.png";

    /**
     * Filename of <code>tick.png</code>.
     */
    public static final String TICK_ICON = "tick.png";

    /**
     * Contains <code>Poll Application</code>, for window titles.
     */
    public static final String DIALOG_TITLE_BASE = "Poll Application";

    /**
     * Poll Application logo.
     */
    public static final String POLL_APPLICATION_LOGO_ICON = "logo.png";

    /**
     * Decor for pollsession tab.
     */
    public static final String DECOR_ICON = "decor.png";

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
     * Asks "Yes-No" to user with dialog.
     * 
     * @param question
     *            Question to ask.
     * @return True, if user selected "Yes".
     */
    public static boolean askYesNo(String question) {
        // JOptionPane infoPane = new JOptionPane(question,
        // JOptionPane.INFORMATION_MESSAGE, JOptionPane.YES_NO_OPTION);
        //
        // JDialog infoDialog = infoPane.createDialog(DIALOG_TITLE_BASE
        // + " - question");
        //
        // infoDialog.setIconImage(loadIcon(INFORMATION_ICON).getImage());
        //
        // infoDialog.setVisible(true);
        //
        // return false;
        Object[] options = { "Yes", "No" };
        int selectedValue = JOptionPane.showOptionDialog(null, question,
                DIALOG_TITLE_BASE, JOptionPane.YES_NO_OPTION,
                JOptionPane.QUESTION_MESSAGE, null, options, options[1]);
        return (selectedValue == 0) ? true : false;
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
