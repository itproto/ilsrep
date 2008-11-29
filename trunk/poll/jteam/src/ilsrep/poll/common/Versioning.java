package ilsrep.poll.common;

import java.io.IOException;
import java.util.jar.Attributes;
import java.util.jar.JarFile;
import java.util.jar.Manifest;

/**
 * Common utilities for Poll application.
 * 
 * @author Taras Kostiak
 * 
 */
public class Versioning {

    public static final String COMPONENT_SERVER = "ilsrep/poll/server/PollServer.class";

    public static final String COMPONENT_CLIENT_CONSOLE = "ilsrep/poll/client/PollClient.class";

    public static final String COMPONENT_EDITOR_CONSOLE = "ilsrep/poll/client/PollEditor.class";

    public static final String COMPONENT_CLIENT_GUI = "ilsrep/poll/client/gui/MainWindow.class";

    public static final String COMPONENT_WEB_STATISTICS = "ilsrep/poll/web/StatisticsServlet.class";

    /**
     * Loads version of given Poll application component.<br>
     * This won't work for web components.
     * 
     * @param component
     *            Name of component.
     * @return
     */
    public static String getVersion(String component) {
        JarFile jar = null;
        String version = null;

        try {
            jar = new JarFile("./poll.jar");
            Manifest manifest = jar.getManifest();
            Attributes attribs = manifest.getAttributes(component);
            version = attribs.getValue("Specification-Version");
        }
        catch (IOException e) {
            // version = "[Can't detect version of \"" + component
            // + "\" Poll application component]";
            version = "[Can't detect version]";
        }

        return version;
    }

}
