package ilsrep.poll.common.protocol;

import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlAttribute;

/**
 * The User management class
 * 
 * @author DRC
 * 
 */
@XmlRootElement(name = "user")
public class User{

/**
* User name.
*/
	protected String userName=null;
/**
* password.
*/
	protected String password=null;
/**
* if is authenticated.
*/
	protected Boolean isAuth=false;
/**
* if is a new user.
*/
	protected Boolean isNew=false;
/**
* if user exists.
*/
	protected Boolean isExist=false;

/**
* @see #userName
*/
    @XmlAttribute(name = "username")
    public String getUserName() {
        return userName;
    }

    public void setUserName(String tmp) {
        this.userName = tmp;
    }
/**
* @see #password
*/
    @XmlAttribute(name = "password")
    public String getPass() {
        return password;
    }

    public void setPass(String tmp) {
        this.password = tmp;
    }
/**
* @see #isAuth
*/
    @XmlAttribute(name = "auth")
    public String getAuth() {
       return this.isAuth ? "true" : "false" ;
        
	    
    }

    public void setAuth(String tmp) {
        this.isAuth = tmp.equals("true")? true : false ;
    }	
/**
* @see #isNew
*/
    @XmlAttribute(name = "new")
    public String getNew() {
       return this.isNew ? "true" : "false" ;
        
	    
    }

    public void setNew(String tmp) {
        this.isNew = tmp.equals("true")? true : false ;
    }		
	
/**
* @see #isExist
*/
    @XmlAttribute(name = "exist")
    public String getExist() {
       return this.isExist ? "true" : "false" ;
        
	    
    }

    public void setExist(String tmp) {
        this.isExist = tmp.equals("true")? true : false ;
    }		
	
	
}