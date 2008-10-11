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
		DBManager db;
int numberOfPolls=0;
db = new SQLiteDBManager(null,getServletContext().getRealPath("/")+"/pollserver.db");
if (db.checkUser(name).equals("false")) {
	err="<h2>No such user</h2>";
	return(err+showForm());
} else {
	if(db.authUser(name,password).equals("false")) {
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