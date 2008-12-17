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
<%@ include file="./links.jsp" %>
<%!
public String showForm(){
return("<form action='./index.jsp' method='post'><div class=\"ILbox\">"+
	"<table>"+
		"<tr>"+
			"<td><b><span style=\"FONT-SIZE: 11px\">Please login</span></b></td>"+
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
		"	 <td><input type=\"submit\"  value=\"Login\"  /></td>"+
		"</tr>"+
	"</table>"+
"</div></form>");

}
public String login(HttpSession hsession, String name, String password) throws Exception{

	String err = null;
if (hsession.getAttribute("username")!=null) {
return("<h2>Welcome "+(String)hsession.getAttribute("username")+"</h2>"+links());
} else if (name==null){
return(showForm());
} else {
//		DBManager db;
int numberOfPolls=0;
  	
	WebJPoll db=service.getWebJPollPort();
if (!db.checkUser(name)) {
	err="<h2>No such user</h2>";
	return(err+showForm());
} else {
	if(!db.authUser(name,password)) {
				err="<h2>Wrong password</h2>";
	    return(err+showForm());
				} else {
					hsession.setAttribute("username",name);
					return("<h2>Logged in</h2>"+links());
										}
				
	
	}
	
	
	}
}
%>