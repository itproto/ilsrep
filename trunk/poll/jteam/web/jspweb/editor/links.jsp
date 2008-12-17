<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="ilsrep.poll.common.protocol.Pollsessionlist"%>
<%@page import="ilsrep.poll.common.protocol.Item"%>
<%@page import="webservice.endpoint.WebJPoll"%>
<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%! 
public String links(boolean side) throws Exception{
	if (side==(true)) {
String links="";
links="<ul ><li class='first'><a href='./index.jsp'> All Sessions </a></li>";
links+="<li class='create'><a href=\"./index.jsp?session=new\">Create New Session</a></li>";
Pollsessionlist sessions;
	WebJPoll db=service.getWebJPollPort();
try {
sessions=db.getPollsessionlist();
 for ( Item sess : sessions.getItems()) {
links+="<li><a href=\"./index.jsp?session="+sess.getId()+"&poll=0 \">"+sess.getName()+"</a></li>\n";
}
links+="</ul>";} catch (Exception e){links=e.getMessage();}
return links;
} else {
	String links="";
links="<ul >";

Pollsessionlist sessions;
	WebJPoll db=service.getWebJPollPort();
	try {
sessions=db.getPollsessionlist();
 for ( Item sess : sessions.getItems()) {
links+="<li><a href=\"./index.jsp?session="+sess.getId()+"&poll=0 \">"+sess.getName()+"</a></li>\n";
}
links+="</ul>";} catch (Exception e){links=e.getMessage();}
return links;
	
	
	
	
	}
}
%>
