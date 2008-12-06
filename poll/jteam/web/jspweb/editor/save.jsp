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
<%@ include file="./links.jsp" %>
<%!
public String saveToDB(String session, String xml) throws Exception{
	
	Pollsession sess;
WebJPoll_Service service=new WebJPoll_Service();
	WebJPoll db=service.getWebJPollPort();
JAXBContext pollContext = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller mr = pollContext.createUnmarshaller();
        StringReader reader = new StringReader(xml);
        sess=(Pollsession)mr.unmarshal(reader);
        if (session.equals("new")){
 db.updatePollsession("-1",sess);
	        return("<h3>Session Created</h3>"+links(false));	        
	        } else {
        db.updatePollsession(sess.getId(),sess);
    
       return("<h2>Session Updated</h2>"+links(false));
}   
	
	
	}

%>