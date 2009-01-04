<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%@page import="webservice.endpoint.WebJPoll"%>
<%@page import="javax.xml.ws.WebServiceRef"%>
<%@page import="javax.xml.namespace.QName"%>
<%@page import="java.net.URL"%>
<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="ilsrep.poll.common.protocol.AnswerItem"%>
<%@page import="ilsrep.poll.common.protocol.Answers"%>
<%@page import="java.net.URL"%>
<%@page import="java.util.List"%>
<%@page import="ilsrep.poll.statistics.Results"%>
<%!
WebJPoll_Service service;
public void setService(String name,String port) { try{
		QName serviceName = new QName("http://endpoint.webservice/","WebJPoll");
URL url = new URL("http://"+name+":"+port+"/WebJPoll/WebJPoll?wsdl"); 
service=new WebJPoll_Service(url,serviceName);
} catch(Exception e){};
}
%>

<%
setService(request.getServerName(),Integer.toString(request.getServerPort()));
WebJPoll db=service.getWebJPollPort();
%>
<%
List<Results> results=db.getStatisticsWidget(request.getParameter("widget"));
String question=db.getPollsessionById(request.getParameter("widget")).getPolls().get(0).getName();

String res="<link rel=\"stylesheet\" type=\"text/css\" href=\"class.css\" />";
res+="<div id=frame><table ><tr><td colspan=2 id=pollname>"+question+"</td></tr>";
res+="<tr><td  id=widget_poll>";
Pollsession sess=db.getPollsessionById(request.getParameter("widget"));
Poll currentPoll=sess.getPolls().get(0);


res+="\n<form id=polls action=widgetresult.jsp>\n <table>";
boolean rowtype=true;
for( Choice currentChoice : currentPoll.getChoices()){
res+="<tr ";

res+="><td><input type='radio'  name='choice' value='"+currentChoice.getId()+"' CHECKED>"+currentChoice.getName()+"</td></tr>";
rowtype=rowtype ? false :true;
}

res+="<tr><td align=center><Input type='hidden' name='widget' value='"+request.getParameter("widget")+"'><button onMouseover='navOver(\"cmdMoveNext\")' onMouseout='navOut(\"cmdMoveNext\")' onMousedown='navDown(\"cmdMoveNext\")' onMouseup='navUp(\"cmdMoveNext\")' onClick='document.getElementById(\"polls\").submit();' ><img src='./images/cmdMoveNext.png' name=\"cmdMoveNext\" id=\"cmdMoveNext\" >Next</button></td></tr></table>";
res+="</div></form>";



res+="</td>";

res+="<td><table id=widget>";

for (int i=0;i<results.size();i++){
	res+="<tr><td>"+results.get(i).getName()+"</td><td><div style='width:200px;' class=empty ><div style='width:"+Integer.toString(Integer.parseInt(results.get(i).getPercent())*2)+"px;' class=full></div></div></td></tr>";
		}
		res+="</table></td></tr></table></div>";
out.println(res);
%>