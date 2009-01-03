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
String res="<table>";
res+="<tr><td colspan=2>"+question+"</td></tr>";

for (int i=0;i<results.size();i++){
	res+="<tr><td>"+results.get(i).getName()+"</td><td>"+results.get(i).getPercent()+"</td></tr>";
		}
		res+="</table>";
out.println(res);
%>