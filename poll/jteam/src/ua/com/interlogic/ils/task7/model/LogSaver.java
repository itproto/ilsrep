package ua.com.interlogic.ils.task7.model;

import java.util.LinkedList;

/**
 * 
 * @author DCR
 */
public class LogSaver {

    private class LogHelper {

        String name;

        String selection;
    };

    public LinkedList<LogHelper> lhelp = new LinkedList<LogHelper>();

    public LogSaver() {
    }

    public void pushMe(String desc, String sid) {
        LogHelper lh = new LogHelper();
        lh.name = desc;
        lh.selection = sid;
        lhelp.add(lh);

    }

    public void popMe() {
        while (!(lhelp.size() == 0)) {
            LogHelper i = lhelp.removeFirst();
            System.out.println(i.name + "=>" + i.selection + "\n");
        }

    }

}
