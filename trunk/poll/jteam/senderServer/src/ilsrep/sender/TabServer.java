package ilsrep.sender;
import java.io.IOException;
import javax.servlet.ServletException;
import java.io.PrintWriter;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServlet;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;
import java.io.StringWriter;
import ilsrep.sender.protocol.Response;
import ilsrep.sender.protocol.TabSender;
import ilsrep.sender.db.SQLiteDBManager;

public class TabServer  extends HttpServlet {
SQLiteDBManager   db =null;	
	public void connectDB(){
		String contextPath = getServletContext().getRealPath("/"); 
String dbFilePath = contextPath + "WEB-INF/tabsender.db";
db = new SQLiteDBManager(dbFilePath);
		}

	public void register(HttpServletRequest request,HttpServletResponse response) throws ServletException, IOException {
		 Response res=new Response();
         
	   
	        String user = (String)request.getParameter("username");
	       String pass = (String)request.getParameter("password");
	       String output="error";
	          PrintWriter out = response.getWriter();
	       
	          try {
	       res.setOk(db.checkUser(user).equals("true") ? false : true);
	        if(!db.checkUser(user).equals("true")) {
		                                      db.createUser(user,pass);
               }
               
            
              JAXBContext responseContext = JAXBContext.newInstance(Response.class);
              Marshaller mr = responseContext.createMarshaller();
              StringWriter wr = new StringWriter();
             mr.marshal(res, wr);
            output=wr.toString();
            
         }catch(Exception e){output=e.getMessage();};
         
           
             
            out.write(output);        
	                                     
               
		}
		
	public void login(HttpServletRequest request,HttpServletResponse response) throws ServletException, IOException {

		 Response res=new Response();
         
	     
	        String user = (String)request.getParameter("username");
	       String pass = (String)request.getParameter("password");
	       res.setOk(db.authUser(user,pass).equals("true") ? true : false);
	              String output="error";
               try {
              JAXBContext responseContext = JAXBContext.newInstance(Response.class);
              Marshaller mr = responseContext.createMarshaller();
              StringWriter wr = new StringWriter();
             mr.marshal(res, wr);
             output=wr.toString();
         }catch(Exception e){output=e.getMessage();};
              PrintWriter out = response.getWriter();
            out.write(output);        
	                                     
               
		}
	  public void doGet(HttpServletRequest request,HttpServletResponse response) throws ServletException, IOException  {
		  		connectDB();
             if(request.getParameter("action").equals("register")){
             register(request,response);
	             }
              if(request.getParameter("action").equals("login")){
             login(request,response);
	             }  
                
}
	
	
	}