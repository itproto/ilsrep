package ilsrep.sender.protocol;

import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * @author TKOST
 * 
 */
@XmlRootElement(name = "action")
public class Action {

    public static final String TASK_REGISTER = "register";

    public static final String TASK_LOGIN = "login";

    public static final String TASK_GETTABS = "getTabs";

    public static final String TASK_STORETABS = "storeTabs";

    protected String task = null;

    @XmlAttribute(name = "task")
    public String getTask() {
        return task;
    }

    public void setTask(String task) {
        this.task = task;
    }

}
