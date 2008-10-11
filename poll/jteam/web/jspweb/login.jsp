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
<%!
public String showForm(){
return("<form action='./index.jsp' method='post'><div class=\"ILbox\">"+
	"<table>"+
		"<tr>"+
			"<td><b><span style=\"FONT-SIZE: 11px\">On-line projektstatus</span></b></td>"+
		"</tr>"+
		"<tr>"+
			"<td>E-mail:<br>"+
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
public String login(HttpSession hsession, String name, String password){
if (hsession.getAttribute("username")!=null) {
return("Welcome"+(String)hsession.getAttribute("username"));
} else {
return(showForm());
}
}
%>