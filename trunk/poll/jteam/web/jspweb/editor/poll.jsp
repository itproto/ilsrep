<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="ilsrep.poll.common.protocol.AnswerItem"%>
<%@page import="ilsrep.poll.common.protocol.Answers"%>
<%@page import="javax.xml.bind.JAXBContext"%>
<%@page import="javax.xml.bind.JAXBContext"%>
<%@page import="javax.xml.bind.JAXBException"%>
<%@page import="javax.xml.bind.Marshaller"%>
<%@page import="java.io.StringWriter"%>
<% String PLEASE_ENTER_POLL="Please choose poll";
%>
<%!
public String editPoll(String sessi) throws Exception{
	Pollsession sess;
DBManager db;
int numberOfPolls=0;
db = new SQLiteDBManager(null,getServletContext().getRealPath("/")+"/pollserver.db");
sess=db.getPollsessionById(sessi);
JAXBContext pollContext = JAXBContext.newInstance(Pollsession.class);
        Marshaller mr = pollContext.createMarshaller();
        StringWriter wr = new StringWriter();
        mr.marshal(sess, wr);
        String output=wr.toString();
        output=output.replaceAll("<","&lt;");
        output=output.replaceAll(">","&gt;");
	
        output="<form name=\"frmEdit\" id=\"frmEdit\" method=\"get\" action=\"noaction\">"+
	"<table>"+
	"<tr><td>Session Name:</td><td><input type=\"text\" name=\"SessName\" id=\"SessName\" disabled></td></tr>"+
	"<tr><td>Allow Test Mode</td><td><INPUT TYPE=\"checkbox\" NAME=\"TestMode\" disabled></td></tr>"+
	"</table>"+
	"<table name=\"polltbl\" id=\"polltbl\">"+
	"<tr><td>Poll Name:</td><td><input type=\"text\" name=\"PollName\" id=\"PollName\" disabled></td></tr>"+
	"<tr><td>Poll Desc:</td><td><input type=\"text\" name=\"PollDesc\" id=\"PollDesc\" disabled></td></tr>"+
	"<tr><td>Allow custom choice</td><td><INPUT TYPE=\"checkbox\" NAME=\"Custom\" disabled></td></tr>"+
	"<tr><td>Correct choice</td><td>Choice option</td></tr>"+
	"</table>"+
	"<div style=\"text-align: center\">"+
	"<input type=\"button\" name=\"cmdMoveFirst\" id=\"cmdMoveFirst\" class=\"navigation\" value=\"<<\" title=\"Move First\" disabled onclick=\"navigateUserList(\'first\')\">"+
    "<input type=\"button\" name=\"cmdMovePrevious\" id=\"cmdMovePrevious\" class=\"navigation\" value=\"<\" title=\"Move Previous\" disabled onclick=\"navigateUserList(\'previous\')\">"+
    "<input type=\"button\" name=\"cmdMoveNext\" id=\"cmdMoveNext\" class=\"navigation\" value=\">\" title=\"Move Next\" disabled onclick=\"navigateUserList(\'next\')\">"+
    "<input type=\"button\" name=\"cmdMoveLast\" id=\"cmdMoveLast\" class=\"navigation\" value=\">>\" title=\"Move Last\" disabled onclick=\"navigateUserList(\'last\')\">"+
    "</div>"+
    "<div style=\"text-align: center\">"+
    "<input type=\"button\" name=\"cmdAddNew\" id=\"cmdAddNew\" class=\"actionButton\" value=\"Add New\" title=\"Add New\" disabled onclick=\"cmdAddNewClicked()\">"+
    "<input type=\"button\" name=\"cmdEdit\" id=\"cmdEdit\" class=\"actionButton\" value=\"Edit\" title=\"Edit\" disabled onclick=\"cmdEditClicked()\">"+
    "<input type=\"button\" name=\"cmdDelete\" id=\"cmdDelete\" class=\"actionButton\" value=\"Delete\" title=\"Delete\" disabled onclick=\"cmdDeleteClicked()\">"+
    "<input type=\"button\" name=\"cmdCancel\" id=\"cmdCancel\" class=\"actionButton\" value=\"Cancel\" title=\"Cancel\" disabled onclick=\"cmdCancelClicked()\">"+
    "<input type=\"button\" name=\"cmdSave\" id=\"cmdSave\" class=\"actionButton\" value=\"Save\" title=\"Save\" disabled onclick=\"cmdSaveClicked()\">"+
    "</div>"+
	"<textarea id=\"txtDatabase\" style=\"display: none;\" rows=\"1\" cols=\"1 \">"+
	output+
 "</textarea>"+
"</form>";
	
	
        return(output);
}
	%>