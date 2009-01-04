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
<%@page import="java.util.ArrayList"%>
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
if(!db.checkUser("web")) db.createUser("web","web");
Answers ans=new Answers();
	ans= new Answers();
	ans.setPollSesionId(request.getParameter("widget"));
	ans.setAnswers(new ArrayList<AnswerItem>());
	ans.setUsername("web");
	AnswerItem ansitem=new AnswerItem();
	ansitem.setItem(Integer.parseInt(db.getPollsessionById(request.getParameter("widget")).getPolls().get(0).getId()),Integer.parseInt(request.getParameter("choice")));
	ans.getAnswers().add(ansitem);
	db.saveResults(ans);
response.sendRedirect ("widgetview.jsp?widget="+request.getParameter("widget"));

%>