package ua.com.interlogic.ils.task7.model;

import java.util.LinkedList;

/**
 * Saves name and choice pairs.
 * 
 * @author DCR
 */
public class LogSaver {

    /**
     * This class represents name and choice selection pair.
     */
    protected class LogHelper {

        /**
         * Name
         */
        String name = null;

        /**
         * Choice selection.
         */
        String selection = null;
    };

    /**
     * This list stores name and choice selection pairs.
     */
    protected LinkedList<LogHelper> lhelp = null;

    /**
     * Default constructor.
     */
    public LogSaver() {
        lhelp = new LinkedList<LogHelper>();
    }

    /**
     * Adds name and choice selection pair to LogSaver.
     * 
     * @param desc
     *            Name.
     * @param sid
     *            Choice selection.
     */
    public void pushMe(String desc, String sid) {
        LogHelper lh = new LogHelper();
        lh.name = desc;
        lh.selection = sid;
        lhelp.add(lh);

    }

    /**
     * Prints all saved name and choice selection pairs.<br>
     * 
     * TODO: (BUG) This removes list from memory, which could be used later.
     */
    public void popMe() {
        while (!(lhelp.size() == 0)) {
            LogHelper i = lhelp.removeFirst();
            System.out.println(i.name + "=>" + i.selection + "\n");
        }
    }

}
