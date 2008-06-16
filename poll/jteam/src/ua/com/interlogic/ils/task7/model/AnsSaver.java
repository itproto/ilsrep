package ua.com.interlogic.ils.task7.model;

import java.util.LinkedList;

/**
 * Saves name and choice pairs.
 * 
 * @author DCR
 */
public class AnsSaver {

    /**
     * This class represents name and choice selection pair.
     */
    protected class AnsHelper {

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
    protected LinkedList<AnsHelper> ansList = null;

    /**
     * Default constructor.
     */
    public AnsSaver() {
        ansList = new LinkedList<AnsHelper>();
    }

    /**
     * Adds name and choice selection pair to LogSaver.
     * 
     * @param desc
     *            Name.
     * @param sid
     *            Choice selection.
     */
    public void pushAns(String poll, String answer) {
        AnsHelper listElement = new AnsHelper();
        listElement.name = poll;
        listElement.selection = answer;
        ansList.add(listElement);

    }

    /**
     * Prints all saved name and choice selection pairs.<br>
     * 
     * TODO: (BUG) This removes list from memory, which could be used later.
     * DRC: you propose i should just read the list instead of clearing it?
     */
    public void popAns() {
        while (!(ansList.size() == 0)) {
            AnsHelper cur = ansList.removeFirst();
            System.out.println(cur.name + " => " + cur.selection + "\n");
        }
    }

}
