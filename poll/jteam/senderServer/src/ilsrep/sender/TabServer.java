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
import ilsrep.sender.protocol.Url;
import ilsrep.sender.db.SQLiteDBManager;
import java.util.List;
import java.util.ArrayList;
import java.io.ByteArrayInputStream;
public class TabServer  extends HttpServlet {
SQLiteDBManager   db =null;	
	public void connectDB(){
		String contextPath = getServletContext().getRealPath("/"); 
String dbFilePath = contextPath + "WEB-INF/tabsender.db";
db = new SQLiteDBManager(dbFilePath);
		}
		
		
public void store(HttpServletRequest request,HttpServletResponse response) throws ServletException, IOException {
      PrintWriter out = response.getWriter();
	 Response res=new Response();
	  String user = (String)request.getParameter("username");
	       String pass = (String)request.getParameter("password");
	       res.setOk(db.authUser(user,pass).equals("true") ? true : false);
	              String output="error";
	              if(db.authUser(user,pass).equals("true")){
		              TabSender tabsender= new TabSender();
		                try {
	               
              JAXBContext urlContext = JAXBContext.newInstance(TabSender.class);
              Unmarshaller mrs = urlContext.createUnmarshaller();
              StringWriter wrs = new StringWriter();
            tabsender=(TabSender)mrs.unmarshal(new ByteArrayInputStream(((String)request.getParameter("urls")).getBytes()));
            db.storeTabs(tabsender.getUrls(),user);
            }catch(Exception e){output=e.getCause().toString();out.write(output);};
		              }
		              
               try {
              JAXBContext responseContext = JAXBContext.newInstance(Response.class);
              Marshaller mr = responseContext.createMarshaller();
              StringWriter wr = new StringWriter();
             mr.marshal(res, wr);
             output=wr.toString();
         }catch(Exception e){output=e.getMessage();};
             
            out.write(output);    
	
	}
	public void load(HttpServletRequest request,HttpServletResponse response) throws ServletException, IOException {
      PrintWriter out = response.getWriter();
	 Response res=new Response();
	  String user = (String)request.getParameter("username");
	       String pass = (String)request.getParameter("password");
	       res.setOk(db.authUser(user,pass).equals("true") ? true : false);
	        TabSender tabsender= new TabSender();
		              tabsender.setResponse(res);
	              String output="error";
	              
		                try {
	               if(db.authUser(user,pass).equals("true")){
		             tabsender.setUrls(db.getTabs(user));
	             }
              JAXBContext urlContext = JAXBContext.newInstance(TabSender.class);
              Marshaller mrs = urlContext.createMarshaller();
              StringWriter wrs = new StringWriter();
            mrs.marshal(tabsender,wrs);
            output=wrs.toString();
            }catch(Exception e){output=e.getMessage();};
		             
		               
            out.write(output);    
	
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
                if(request.getParameter("action").equals("store")){
	                 store(request,response);
	                }
	                if(request.getParameter("action").equals("load")){
	                 load(request,response);
	                }
}
	
	
	}