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
<%@page import="webservice.endpoint.WebJPoll"%>
<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%@page import="javax.xml.ws.WebServiceRef"%>
<% String PLEASE_ENTER_POLL="Please choose poll";
%>
<%!
 //@WebServiceRef(wsdlLocation="http://localhost:8080/WebJPoll/WebJPoll?wsdl")
//  static WebJPoll_Service service; 

public String editPoll(String sessi, String user) throws Exception{
	 WebJPoll_Service service=new WebJPoll_Service();
	WebJPoll port=service.getWebJPollPort();
String output="";
		if (!(sessi.equals("new"))){
	Pollsession sess;
int numberOfPolls=0;
sess=port.getPollsessionById(sessi);
//db.close();
JAXBContext pollContext = JAXBContext.newInstance(Pollsession.class);
        Marshaller mr = pollContext.createMarshaller();
        StringWriter wr = new StringWriter();
        mr.marshal(sess, wr);
        output=wr.toString();
        output=output.replaceAll("<","&lt;");
        output=output.replaceAll(">","&gt;");
} else {
	output="&lt;?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?&gt;&lt;pollsession testMode=\"false\" name=\"session name\" id=\"51\" date=\"2008-10-22 17:21:56\"&gt;&lt;poll name=\"new poll\" id=\"139\" customChoiceEnabled=\"false\" correctChoice=\"-1\"&gt;&lt;choices&gt;&lt;choice name=\"choice_1\" id=\"421\"/&gt;&lt;choice name=\"choice_2\" id=\"422\"/&gt;&lt;/choices&gt;&lt;description&gt;description&lt;/description&gt;&lt;/poll&gt;&lt;/pollsession&gt;";
	//output=output.replaceAll("<","&lt;");
      //  output=output.replaceAll(">","&gt;");
	}
        output="<form name=\"frmEdit\" id=\"frmEdit\" method=\"get\" action=\"noaction\" >"+
	"<table id=\"polltbl\">"+
	"<tr><td>Session Name:</td><td><input type=\"text\" name=\"SessName\" id=\"SessName\" ></td></tr>"+
	"<tr><td>Allow Test Mode</td><td><INPUT TYPE=\"checkbox\" id=\"TestMode\" NAME=\"TestMode\"  onclick=\"onTestMode()\"></td></tr>"+
	"<tr id='minscoretr'><td>Minimum Score</td><td><INPUT TYPE=\"text\" id=\"MinScore\" NAME=\"MinScore\"  ></td></tr>"+
	"<tr id='addpolltosession'><td  align='center' COLSPAN=2><img src=\"../images/but.png\" name=\"cmdAddNew\" id=\"cmdAddNew\" class=\"actionButton\" value=\"Add Poll\" title=\"Add New\" onMouseover='hoverButPlus(\"cmdAddNew\")' onMouseout='outButPlus(\"cmdAddNew\")' onMousedown='pressButPlus(\"cmdAddNew\")' onMouseup='releaseButPlus(\"cmdAddNew\")' onclick=\"cmdAddNewClicked()\"><img src=\"../images/but2.png\" name=\"cmdDelete\" id=\"cmdDelete\" class=\"actionButton\" value=\"Delete\" title=\"Delete\" onMouseover='hoverButMin(\"cmdDelete\")' onMouseout='outButMin(\"cmdDelete\")' onMousedown='pressButMin(\"cmdDelete\")' onMouseup='releaseButMin(\"cmdDelete\")' onclick=\"cmdDeleteClicked()\"><img src=\"../images/cmdEditPoll.png\" name=\"cmdEditPoll\" id=\"cmdEditPoll\" onMouseover='navOver(\"cmdEditPoll\")' onMouseout='navOut(\"cmdEditPoll\")' onMousedown='navDown(\"cmdEditPoll\")' onMouseup='navUp(\"cmdEditPoll\")'  class=\"actionButton\"   onclick=\"cmdTogglePoll()\"></td></tr>"+
	"<tr id='pllname'><td>Poll Name:</td><td  colspan=2><input type=\"text\" name=\"PollName\" id=\"PollName\" ></td></tr>"+
	"<tr id='polldescription'><td>Poll Desc:</td><td  colspan=2><input type=\"text\" name=\"PollDesc\" id=\"PollDesc\" ></td></tr>"+
	"<tr id='customch'><td>Allow custom choice</td><td colspan=2><INPUT TYPE=\"checkbox\" NAME=\"Custom\" id=\"Custom\"  > Not available in test mode</td></tr>"+
	"<tr id=\"navi\" style=\"text-align: center\"><td colspan=2>"+
	"<img ALIGN=ABSMIDDLE src=\"../images/cmdMoveFirst.png\" onMouseover='navOver(\"cmdMoveFirst\")' onMouseout='navOut(\"cmdMoveFirst\")' onMousedown='navDown(\"cmdMoveFirst\")' onMouseup='navUp(\"cmdMoveFirst\")' name=\"cmdMoveFirst\" id=\"cmdMoveFirst\" class=\"navigation\"  >"+
    "<img ALIGN=ABSMIDDLE  src=\"../images/cmdMovePrevious.png\"  onMouseover='navOver(\"cmdMovePrevious\")' onMouseout='navOut(\"cmdMovePrevious\")' onMousedown='navDown(\"cmdMovePrevious\")' onMouseup='navUp(\"cmdMovePrevious\")' name=\"cmdMovePrevious\" id=\"cmdMovePrevious\" class=\"navigation\"   >"+
    "<span id='position'></span>"+
    "<img ALIGN=ABSMIDDLE src=\"../images/cmdMoveNext.png\"  onMouseover='navOver(\"cmdMoveNext\")' onMouseout='navOut(\"cmdMoveNext\")' onMousedown='navDown(\"cmdMoveNext\")' onMouseup='navUp(\"cmdMoveNext\")' name=\"cmdMoveNext\" id=\"cmdMoveNext\" class=\"navigation\"    >"+
    "<img ALIGN=ABSMIDDLE src=\"../images/cmdMoveLast.png\"  onMouseover='navOver(\"cmdMoveLast\")' onMouseout='navOut(\"cmdMoveLast\")' onMousedown='navDown(\"cmdMoveLast\")' onMouseup='navUp(\"cmdMoveLast\")' name=\"cmdMoveLast\" id=\"cmdMoveLast\" class=\"navigation\"   >"+
    "</td></tr>"+
	"<tr id='correctc'><td>Correct choice</td><td colspan=2>Choice option</td></tr>"+
	"<tr id='adpolltosession'><td COLSPAN=2 align=center><img src=\"../images/but.png\" name=\"cmdAddChoice\" id=\"cmdAddChoice\" class=\"actionButton\" value=\"Add Choice\" title=\"Add Choice\"  onMouseover='hoverButPlus(\"cmdAddChoice\")' onMouseout='outButPlus(\"cmdAddChoice\")' onMousedown='pressButPlus(\"cmdAddChoice\")' onMouseup='releaseButPlus(\"cmdAddChoice\")' onclick=\"addUserRowToTable()\"><img src=\"../images/cmdEditChoice.png\" name=\"cmdEditChoice\" id=\"cmdEditChoice\" onMouseover='navOver(\"cmdEditChoice\")' onMouseout='navOut(\"cmdEditChoice\")' onMousedown='navDown(\"cmdEditChoice\")' onMouseup='navUp(\"cmdEditChoice\")'  class=\"actionButton\"   onclick=\"cmdToggleChoice()\"></td></tr>"+
	"</table>"+
	"<div style=\"text-align: center\">"+
    "<img src=\"../images/cmdSend.png\" name=\"cmdSend\" id=\"cmdSend\" onMouseover='navOver(\"cmdSend\")' onMouseout='navOut(\"cmdSend\")' onMousedown='navDown(\"cmdSend\")' onMouseup='navUp(\"cmdSend\")'  class=\"actionButton\" value=\"Save Session\" title=\"Save Session\"  onclick=\"cmdSaveSessionClicked()\">"+
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