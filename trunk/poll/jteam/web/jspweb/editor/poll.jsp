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
        output="<form name=\"frmEdit\" id=\"frmEdit\" method=\"get\" action=\"noaction\" >"+
	"<table id=\"polltbl\">"+
	"<tr><td>Session Name:</td><td><input type=\"text\" name=\"SessName\" id=\"SessName\" ></td></tr>"+
	"<tr><td>Allow Test Mode</td><td><INPUT TYPE=\"checkbox\" id=\"TestMode\" NAME=\"TestMode\"  onclick=\"onTestMode()\"></td></tr>"+
	"<tr id='minscoretr'><td>Minimum Score</td><td><INPUT TYPE=\"text\" id=\"MinScore\" NAME=\"MinScore\"  ></td></tr>"+
	"<tr id='addpolltosession'><td  align='center' COLSPAN=2><img src=\"../images/cmdSend.png\" name=\"cmdSend\" id=\"cmdSend\" onMouseover='navOver(\"cmdSend\")' onMouseout='navOut(\"cmdSend\")' onMousedown='navDown(\"cmdSend\")' onMouseup='navUp(\"cmdSend\")'  class=\"actionButton\" value=\"Save Session\" title=\"Save Session\"  onclick=\"cmdSaveSessionClicked()\"><img src=\"../images/cmdEditPoll.png\" name=\"cmdEditPoll\" id=\"cmdEditPoll\" onMouseover='navOver(\"cmdEditPoll\")' onMouseout='navOut(\"cmdEditPoll\")' onMousedown='navDown(\"cmdEditPoll\")' onMouseup='navUp(\"cmdEditPoll\")'  class=\"actionButton\"   onclick=\"cmdTogglePoll()\"><img src=\"../images/but.png\" name=\"cmdAddNew\" id=\"cmdAddNew\" class=\"actionButton\" value=\"Add Poll\" title=\"Add New\" onMouseover='hoverButPlus(\"cmdAddNew\")' onMouseout='outButPlus(\"cmdAddNew\")' onMousedown='pressButPlus(\"cmdAddNew\")' onMouseup='releaseButPlus(\"cmdAddNew\")' onclick=\"cmdAddNewClicked()\"><img src=\"../images/but2.png\" name=\"cmdDelete\" id=\"cmdDelete\" class=\"actionButton\" value=\"Delete\" title=\"Delete\" onMouseover='hoverButMin(\"cmdDelete\")' onMouseout='outButMin(\"cmdDelete\")' onMousedown='pressButMin(\"cmdDelete\")' onMouseup='releaseButMin(\"cmdDelete\")' onclick=\"cmdDeleteClicked()\"></td></tr>"+
	"<tr id='pllname'><td>Poll Name:</td><td  colspan=2><input type=\"text\" name=\"PollName\" id=\"PollName\" ></td></tr>"+
	"<tr id='polldescription'><td>Poll Desc:</td><td  colspan=2><input type=\"text\" name=\"PollDesc\" id=\"PollDesc\" ></td></tr>"+
	"<tr id='customch'><td>Allow custom choice</td><td colspan=2><INPUT TYPE=\"checkbox\" NAME=\"Custom\" id=\"Custom\"  ></td></tr>"+
	"<tr id='correctc'><td>Correct choice</td><td colspan=2>Choice option</td></tr>"+
	"<tr id='adpolltosession'><td COLSPAN=2><img src=\"../images/but.png\" name=\"cmdAddChoice\" id=\"cmdAddChoice\" class=\"actionButton\" value=\"Add Choice\" title=\"Add Choice\"  onMouseover='hoverButPlus(\"cmdAddChoice\")' onMouseout='outButPlus(\"cmdAddChoice\")' onMousedown='pressButPlus(\"cmdAddChoice\")' onMouseup='releaseButPlus(\"cmdAddChoice\")' onclick=\"addUserRowToTable()\"><img src=\"../images/cmdEditChoice.png\" name=\"cmdEditChoice\" id=\"cmdEditChoice\" onMouseover='navOver(\"cmdEditChoice\")' onMouseout='navOut(\"cmdEditChoice\")' onMousedown='navDown(\"cmdEditChoice\")' onMouseup='navUp(\"cmdEditChoice\")'  class=\"actionButton\"   onclick=\"cmdToggleChoice()\"></td></tr>"+
	"</table>"+
	"<div id=\"navi\" style=\"text-align: center\">"+
	"<img src=\"../images/cmdMoveFirst.png\" onMouseover='navOver(\"cmdMoveFirst\")' onMouseout='navOut(\"cmdMoveFirst\")' onMousedown='navDown(\"cmdMoveFirst\")' onMouseup='navUp(\"cmdMoveFirst\")' name=\"cmdMoveFirst\" id=\"cmdMoveFirst\" class=\"navigation\"   onclick=\"navigateUserList(\'first\')\">"+
    "<img src=\"../images/cmdMovePrevious.png\"  onMouseover='navOver(\"cmdMovePrevious\")' onMouseout='navOut(\"cmdMovePrevious\")' onMousedown='navDown(\"cmdMovePrevious\")' onMouseup='navUp(\"cmdMovePrevious\")' name=\"cmdMovePrevious\" id=\"cmdMovePrevious\" class=\"navigation\"  onclick=\"navigateUserList(\'previous\')\">"+
    "<img src=\"../images/cmdMoveNext.png\"  onMouseover='navOver(\"cmdMoveNext\")' onMouseout='navOut(\"cmdMoveNext\")' onMousedown='navDown(\"cmdMoveNext\")' onMouseup='navUp(\"cmdMoveNext\")' name=\"cmdMoveNext\" id=\"cmdMoveNext\" class=\"navigation\"   onclick=\"navigateUserList(\'next\')\">"+
    "<img src=\"../images/cmdMoveLast.png\"  onMouseover='navOver(\"cmdMoveLast\")' onMouseout='navOut(\"cmdMoveLast\")' onMousedown='navDown(\"cmdMoveLast\")' onMouseup='navUp(\"cmdMoveLast\")' name=\"cmdMoveLast\" id=\"cmdMoveLast\" class=\"navigation\"   onclick=\"navigateUserList(\'last\')\">"+
    "</div>"+
    "<div style=\"text-align: center\">"+
    ""+
    "</div>"+
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