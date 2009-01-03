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
<%@page import="javax.xml.bind.Unmarshaller"%>
<%@page import="java.io.StringReader"%>
<%@page import="webservice.endpoint.WebJPoll"%>
<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%!
public String saveWidgetToDB(String session, String xml, String path) throws Exception{
	Pollsession sess;
	WebJPoll db=service.getWebJPollPort();
JAXBContext pollContext = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller mr = pollContext.createUnmarshaller();
        StringReader reader = new StringReader(xml);
        sess=(Pollsession)mr.unmarshal(reader);
       int newId=db.storePollsession(sess);
	        String output="<h3>Widget Created</h3>";	        
	        output+="<h4>Use this code</h4>";
	        output+="<code>&lt;iframe src=\"http://"+path+"/jspweb/widgetview.jsp?widget="+newId+"\"/&gt;</code>";
	        return(output);
	        
	        
	 	
	        
	        
	        
	        
	
	
	}

%>