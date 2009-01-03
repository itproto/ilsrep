<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%@page import="webservice.endpoint.WebJPoll"%>
<%@page import="javax.xml.ws.WebServiceRef"%>
<%@page import="javax.xml.namespace.QName"%>
<%@page import="java.net.URL"%>
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
List<Results> results=db.getStatistics(request.getParameter("widget"));
String question=db.getPollsessionById(request.getParameter("widget")).getPolls().get(0).getName();

String res="<link rel=\"stylesheet\" type=\"text/css\" href=\"class.css\" />";
res+="<div id=frame><table ><tr><td colspan=2 id=pollname>"+question+"</td></tr>";
res+="<tr><td  id=widget_poll>INSERT POLL HERE</td>";

res+="<td><table id=widget>";

for (int i=0;i<results.size();i++){
	res+="<tr><td>"+results.get(i).getName()+"</td><td><div style='width:200px;' class=empty ><div style='width:"+Integer.toString(Integer.parseInt(results.get(i).getPercent())*2)+"px;' class=full></div></div></td></tr>";
		}
		res+="</table></td></tr></table></div>";
out.println(res);
%>