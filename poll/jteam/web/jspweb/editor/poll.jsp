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
public String editPoll(String sessi, String user) throws Exception{
String output="";
		if (!(sessi.equals("new"))){
	Pollsession sess;
DBManager db;

int numberOfPolls=0;
db = new SQLiteDBManager(null,getServletContext().getRealPath("/")+"/pollserver.db");
sess=db.getPollsessionById(sessi);
JAXBContext pollContext = JAXBContext.newInstance(Pollsession.class);
        Marshaller mr = pollContext.createMarshaller();
        StringWriter wr = new StringWriter();
        mr.marshal(sess, wr);
        output=wr.toString();
        output=output.replaceAll("<","&lt;");
        output=output.replaceAll(">","&gt;");
} else {
	output="<pollsession name=\"new\" id=\"1\" testMode=\"false\"></pollsession>";
	output=output.replaceAll("<","&lt;");
        output=output.replaceAll(">","&gt;");
	}
        output="<form name=\"frmEdit\" id=\"frmEdit\" method=\"get\" action=\"noaction\">"+
	"<table>"+
	"<tr><td>Session Name:</td><td><input type=\"text\" name=\"SessName\" id=\"SessName\" disabled></td></tr>"+
	"<tr><td>Allow Test Mode</td><td><INPUT TYPE=\"checkbox\" id=\"TestMode\" NAME=\"TestMode\" disabled onclick=\"onTestMode()\"></td></tr>"+
	"<tr id='minscoretr'><td>Minimum Score</td><td><INPUT TYPE=\"text\" id=\"MinScore\" NAME=\"MinScore\" disabled ></td></tr>"+
	"</table>"+
	"<table name=\"polltbl\" id=\"polltbl\">"+
	"<tr id='plnm'><td>Poll Name:</td><td  colspan=2><input type=\"text\" name=\"PollName\" id=\"PollName\" disabled></td></tr>"+
	"<tr id='pldsc'><td>Poll Desc:</td><td  colspan=2><input type=\"text\" name=\"PollDesc\" id=\"PollDesc\" disabled></td></tr>"+
	"<tr id='cuc'><td>Allow custom choice</td><td colspan=2><INPUT TYPE=\"checkbox\" NAME=\"Custom\" id=\"Custom\" disabled ></td></tr>"+
	"<tr id='coc'><td>Correct choice</td><td colspan=2>Choice option</td></tr>"+
	"</table>"+
	"<div style=\"text-align: center\">"+
	"<input type=\"button\" name=\"cmdMoveFirst\" id=\"cmdMoveFirst\" class=\"navigation\" value=\"<<\" title=\"Move First\" disabled onclick=\"navigateUserList(\'first\')\">"+
    "<input type=\"button\" name=\"cmdMovePrevious\" id=\"cmdMovePrevious\" class=\"navigation\" value=\"<\" title=\"Move Previous\" disabled onclick=\"navigateUserList(\'previous\')\">"+
    "<input type=\"button\" name=\"cmdMoveNext\" id=\"cmdMoveNext\" class=\"navigation\" value=\">\" title=\"Move Next\" disabled onclick=\"navigateUserList(\'next\')\">"+
    "<input type=\"button\" name=\"cmdMoveLast\" id=\"cmdMoveLast\" class=\"navigation\" value=\">>\" title=\"Move Last\" disabled onclick=\"navigateUserList(\'last\')\">"+
    "</div>"+
    "<div style=\"text-align: center\">"+
    "<input type=\"button\" name=\"cmdAddNew\" id=\"cmdAddNew\" class=\"actionButton\" value=\"Add Poll\" title=\"Add New\" disabled onclick=\"cmdAddNewClicked()\">"+
    "<input type=\"button\" name=\"cmdEdit\" id=\"cmdEdit\" class=\"actionButton\" value=\"Edit\" title=\"Edit\" disabled onclick=\"cmdEditClicked()\">"+
    "<input type=\"button\" name=\"cmdDelete\" id=\"cmdDelete\" class=\"actionButton\" value=\"Delete\" title=\"Delete\" disabled onclick=\"cmdDeleteClicked()\">"+
    "<input type=\"button\" name=\"cmdAddChoice\" id=\"cmdAddChoice\" class=\"actionButton\" value=\"Add Choice\" title=\"Add Choice\" disabled onclick=\"addUserRowToTable()\">"+
    "<input type=\"button\" name=\"cmdCancel\" id=\"cmdCancel\" class=\"actionButton\" value=\"Cancel\" title=\"Cancel\" disabled onclick=\"cmdCancelClicked()\">"+
    "<input type=\"button\" name=\"cmdSave\" id=\"cmdSave\" class=\"actionButton\" value=\"Save\" title=\"Save\" disabled onclick=\"cmdSaveClicked()\">"+
    "</div>"+
    "<div style=\"text-align: center\"><input type=\"button\" name=\"cmdSend\" id=\"cmdSend\" class=\"actionButton\" value=\"Save Session\" title=\"Save Session\"  onclick=\"cmdSaveSessionClicked()\"></div>"+
    "<textarea id=\"txtDatabase\" style=\"display: none;\" rows=\"1\" cols=\"1 \">"+
	output+
 "</textarea>"+
 "</form>"+
"<form action=\"./index.jsp\" id=\"submitform\">"+
"<input type='hidden' name='result' id='result'><input type='hidden' id='sessiontype' name='sessiontype' value=\""+sessi+"\"></form>";
	if (user==null){
		
		output="<h2>Please login first</h2> <a href='./../index.jsp'>Login</a>";
		}
	

        return(output);
    
}
	%>