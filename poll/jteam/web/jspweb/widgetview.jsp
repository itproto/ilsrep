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

String res="     <script type=\"text/javascript\" src=\"./editor/jquery.js\" ></script>    <script type=\"text/javascript\" src=\"./editor/jquery.page.js\" >";
res+="</script> <script type=\"text/javascript\" src=\"./Ajax.js\"></script> <link rel=\"stylesheet\" type=\"text/css\" href=\"class.css\" />";
res+="<div id=frame><table ><tr><td colspan=2 id=pollname>"+question+"</td></tr>";
res+="<tr><td  id=widget_poll>";
Pollsession sess=db.getPollsessionById(request.getParameter("widget"));
Poll currentPoll=sess.getPolls().get(0);


res+="\n<form id=polls >\n <table>";
boolean rowtype=true;
for( Choice currentChoice : currentPoll.getChoices()){
res+="<tr ";

res+="><td><input type='radio'  name='choice' value='"+currentChoice.getId()+"' onClick='this.checked=true;sendResult("+request.getParameter("widget")+",\"http://"+request.getServerName()+":"+Integer.toString(request.getServerPort())+"\")'>"+currentChoice.getName()+"</td></tr>";
rowtype=rowtype ? false :true;
}

res+="<tr><td align=center><Input type='hidden' name='widget' value='"+request.getParameter("widget")+"'></td></tr><tr><td id='dontvote'><a  onclick='dontVote();'>Show me results</a></td></tr></table>";
res+="</form>";



res+="</td>";

res+="<td style='display:none' id='widget_td'><table id=widget >";

for (int i=0;i<results.size();i++){
	res+="<tr ><td>"+results.get(i).getName()+"</td></tr><tr><td id='result"+Integer.toString(i)+"'><div  name='empty' class=empty ><div name='full' style='width:"+Integer.toString(Integer.parseInt(results.get(i).getPercent())*2)+"px;' class=full></div><div name='votes' class=votes><span id='votes2'>"+results.get(i).getVotes()+"</span> votes (<span>"+results.get(i).getPercent()+"</span>%)</div></div></td></tr>";
		}
		res+="</table></td></tr></table></div>";
out.println(res);
%>