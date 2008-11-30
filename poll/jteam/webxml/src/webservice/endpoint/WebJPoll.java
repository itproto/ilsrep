package webservice.endpoint;
import javax.jws.WebService;
import javax.jws.WebMethod;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.server.db.SQLiteDBManager;
import ilsrep.poll.server.db.DBManager;
import ilsrep.poll.common.protocol.AnswerItem;
import ilsrep.poll.common.protocol.Answers;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import java.io.StringWriter;
import javax.servlet.ServletContext;
import javax.xml.ws.handler.MessageContext;
import javax.servlet.http.HttpServlet;
import java.net.URL;

@WebService(name="WebJPoll", serviceName="WebJPoll")
public class WebJPoll{
	private DBManager db;
	public void connect()throws Exception{
		
String path=this.getClass().getResource("/pollserver.db").toString();


         
	db=new SQLiteDBManager(null,path);
}
@WebMethod
	 public Pollsession getPollsessionById(String id) throws Exception {
		 connect();
		 Pollsession sess=db.getPollsessionById(id);
	/*	 JAXBContext pollContext = JAXBContext.newInstance(Pollsession.class);
        Marshaller mr = pollContext.createMarshaller();
        StringWriter wr = new StringWriter();
        mr.marshal(sess, wr);
        String output=wr.toString();
	 return(output);*/
	 return(sess);
		 }
		 @WebMethod
	public void createUser(String name, String pass)throws Exception {
		connect();
		 db.createUser(name, pass);
	 
		 }
		@WebMethod
    public boolean checkUser(String name) throws Exception { 
	    connect();
		return((db.checkUser(name).equals("true")? true : false ));	  
			  }
			  @WebMethod
	public boolean authUser(String name, String pass) throws Exception {
		connect();
		return((db.authUser(name, pass).equals("true")? true : false ));
		}
		@WebMethod
	public Pollsessionlist getPollsessionlist() throws Exception {
		connect();
			 Pollsessionlist sess=db.getPollsessionlist();
	/*	 JAXBContext pollContext = JAXBContext.newInstance(Pollsessionlist.class);
        Marshaller mr = pollContext.createMarshaller();
        StringWriter wr = new StringWriter();
        mr.marshal(sess, wr);
        String output=wr.toString();
	 return(output);*/
	 return(sess);
	}
	@WebMethod
		public int storePollsession(Pollsession sess) throws Exception {
			connect();
			return db.storePollsession(sess);
			}
			@WebMethod
	 public void removePollsession(String id)throws Exception {
		 connect();
		 db.removePollsession(id);
		 }
		 @WebMethod
		 public void updatePollsession(String id, Pollsession sess) throws Exception {
			 connect();
			 db.updatePollsession(id,sess);
			 }
	}