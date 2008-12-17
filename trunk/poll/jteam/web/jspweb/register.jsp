<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.common.protocol.Answers"%>
<%@page import="ilsrep.poll.common.protocol.AnswerItem"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="java.util.ArrayList"%>
<%@page import="java.util.List"%>
<%@page import="javax.servlet.ServletRequest"%>
<%@page import="webservice.endpoint.WebJPoll"%>
<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%@page import="javax.xml.ws.WebServiceRef"%>
<%@page import="javax.xml.namespace.QName"%>
<%@page import="java.net.URL"%>
<%@ include file="./login.jsp" %>
<%!
public String showRegform(){
return("<form action='./index.jsp' method='post'><div class=\"ILbox\">"+
	"<table>"+
		"<tr>"+
			"<td><b><span style=\"FONT-SIZE: 11px\">Please enter</span></b></td>"+
		"</tr>"+
		"<tr>"+
			"<td>Login:<br>"+
				"<input name='name' type=\"text\"  class=\"boxinput\" />"+
			"</td>"+
		"</tr>"+
		"<tr>"+
			"<td>Password:<br>"+
				"<input name=\"password\" type=\"password\"class=\"boxinput\" />"+
			"</td>"+
		"</tr>"+
		"<tr>"+
			"<td>Password again:<br>"+
				"<input name=\"password2\" type=\"password\"class=\"boxinput\" />"+
			"</td>"+
		"</tr>"+
		"<tr>"+
		"	 <td><input type=\"submit\"  value=\"Login\"  /><input type=\"hidden\"  name=\"register\" value='true'  /></td>"+
		"</tr>"+
	"</table>"+
"</div></form>");

}
public String register(String name, String password, String password2) throws Exception {
	
	WebJPoll db=service.getWebJPollPort();
int numberOfPolls=0;
	String err="";
	boolean restart=false;
if (name==null) {
	return (showRegform());
		}	else {
			if (db.checkUser(name)) {
				err="<h3>User Exists</h3>";
				restart=true;
				} 
			if(!(password.equals(password2))) {
				err+="<h3>Passwords don't match</h3>";
				restart=true;
				}
			if(restart) {
				return(err+showRegform());
				
				} else {
					db.createUser(name,password);
					return("<h3>User created</h3>"+showForm());
					
					
					}
			
			
			
			}
	
	}

%>