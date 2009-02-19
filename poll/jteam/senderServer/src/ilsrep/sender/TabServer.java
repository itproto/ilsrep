package ilsrep.sender;
import java.io.*;
import javax.servlet.*;
import javax.servlet.http.*;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;
import java.io.StringWriter;
import ilsrep.sender.protocol.Response;
import ilsrep.sender.db.SQLiteDBManager;

public class TabServer  extends HttpServlet {
SQLiteDBManager   db = new SQLiteDBManager("./tabsender.db");
	public void register(HttpServletRequest request,HttpServletResponse response) throws ServletException, IOException {
		 Response res=new Response();
         
	   
	        String user = (String)request.getAttribute("username");
	       String pass = (String)request.getAttribute("password");
	       res.setOk(db.checkUser(user).equals("true") ? false : true);
	        if(db.checkUser(user).equals("false")) {
		                                      db.createUser(user,pass);
               }
               String output="error";
               try {
              JAXBContext responseContext = JAXBContext.newInstance(Response.class);
              Marshaller mr = responseContext.createMarshaller();
              StringWriter wr = new StringWriter();
             mr.marshal(request, wr);
             output=wr.toString();
         }catch(Exception e){};
              PrintWriter out = response.getWriter();
            out.write(output);        
	                                     
               
		}
		
	public void login(HttpServletRequest request,HttpServletResponse response) throws ServletException, IOException {
		 Response res=new Response();
         
	     
	        String user = (String)request.getAttribute("username");
	       String pass = (String)request.getAttribute("password");
	       res.setOk(db.authUser(user,pass).equals("true") ? true : false);
	              String output="error";
               try {
              JAXBContext responseContext = JAXBContext.newInstance(Response.class);
              Marshaller mr = responseContext.createMarshaller();
              StringWriter wr = new StringWriter();
             mr.marshal(request, wr);
             output=wr.toString();
         }catch(Exception e){};
              PrintWriter out = response.getWriter();
            out.write(output);        
	                                     
               
		}
	  public void doGet(HttpServletRequest request,HttpServletResponse response) throws ServletException, IOException  {
             if(request.getAttribute("action").equals("register")){
             register(request,response);
	             }
              if(request.getAttribute("action").equals("register")){
             login(request,response);
	             }  
                
}
	
	
	}