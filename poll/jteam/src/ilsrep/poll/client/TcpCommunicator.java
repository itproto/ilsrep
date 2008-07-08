package ilsrep.poll.client;
import java.io.* ;
import java.net.*;
/**
 *Handles TCP socket connection with the server
 *
 * @author DRC
 * 
 */
public class TcpCommunicator {
/**
 *Stores received XML data
 *
 * @see TcpCommunicator
 * 
 */	
protected 	String receivedXml;
/**
 *Stores the ID of selected poll
 *
 * @see TcpCommunicator
 * 
 */	
protected String pollId;
/**
 *Stores data that may be sent to server.
 *For future versions.
 *
 * @see TcpCommunicator
 * 
 */	
protected String answers;
/**
 *Stores POLL server IP
 *
 * @see TcpCommunicator
 * 
 */	
protected String serverIp="127.0.0.1";
/**
 *Stores POLL server port
 *
 * @see TcpCommunicator
 * 
 */	
int port=3320;
/**
 *Data connection socket.
 *
 * @see TcpCommunicator
 * 
 */	
Socket clientSocket;
/**
 *Constructor method, connects to a socket.
 *
 * @see TcpCommunicator
 * 
 */	
public  TcpCommunicator(){
	
try{	System.out.println("Connecting to "+serverIp+" on "+Integer.toString(port));
	clientSocket=new Socket(serverIp,port);
	System.out.println("Take a deep breath");} 
	 	catch (UnknownHostException e)
 
 	{
 
 		System.out.println("Sock:"+e.getMessage());
 
     }
 
 	catch (EOFException e)
 
 	{
 
 		System.out.println("EOF:"+e.getMessage());
 
 	}
 
 	catch (IOException e)
 
 	{
 
 		System.out.println("IO:"+e.getMessage());
 
 	}
		}
/**
 *Making sure that the socket is closed before the object is gced. Virtually a destructor.
 *
 * @see TcpCommunicator
 * 
 */	
protected void finalize(){
	try {clientSocket.close();}  catch(Exception e ){};
	try {super.finalize();}  catch(Throwable e ){};
	}
/**
 *Retreives XML from server. Asks for the poll id, 
 *sends XML request, receives byte data from server
 *and converts it to a Reader object.
 * 
 * @return xmlBuffered received XML from server
 *
 * @see TcpCommunicator
 */		
public Reader getXML() {
	InputStream inFromServer=null;
	String xmlItself="";
	Reader xmlBuffered=null;
		try{
		//Generating input and output streams
		
		
		BufferedReader consoleInputReader = new BufferedReader(new InputStreamReader(System.in));
		DataOutputStream outToServer = new DataOutputStream(clientSocket.getOutputStream());
		inFromServer = clientSocket.getInputStream();
		
			xmlItself="";
			System.out.println("Give me the id, baby:");
		String id = consoleInputReader.readLine();
		//sending request
		outToServer.writeUTF("<getPollSession><pollSessionId>"+id+"</pollSessionId></getPollSession> \n");  
		System.out.println("Getting it, baby");
		
	 	String buffer;
	 	
	 	BufferedReader inputReader = new BufferedReader(new InputStreamReader(inFromServer));
	 	//Getting and parsing request. Reading line because
	 	// for some reason m test server returned first line empty, and the output started from second line.
	 	inputReader.readLine();
	 	boolean eternal=true;
	 		try { while(eternal) {
		 	buffer=inputReader.readLine();
		 	if  (buffer.indexOf("-1")!=-1) break;
		 
	 		xmlItself=xmlItself+"\n"+buffer;
	 			 		System.out.println(buffer); 
	 			 			if  (buffer.indexOf("/pollses")!=-1) break;
 			}} catch (Exception m){
 			
	 	System.out.println("Got it"); 
 	}
	 	

;	 
		 	
 		System.out.println(xmlItself);
 		//Making Reader out of string (needed for marshaller)
 		xmlBuffered= new StringReader(xmlItself);
  }catch(Exception e ){System.out.println("ExCePtIoN"); e.printStackTrace();;};
  //returning reader
  	return xmlBuffered; 	
	
	}
	public void sendXml(String genXml){
		
		InputStream inFromServer=null;
	String xmlItself="";
	Reader xmlBuffered=null;
		try{
		//Generating input and output streams
		
		
		BufferedReader consoleInputReader = new BufferedReader(new InputStreamReader(System.in));
		DataOutputStream outToServer = new DataOutputStream(clientSocket.getOutputStream());
		inFromServer = clientSocket.getInputStream();
		
		outToServer.writeUTF(genXml);  
		System.out.println("Sent it, baby");
		}catch (Exception e){System.out.println("ExCePtIoN"); e.printStackTrace();}
		
		
		}
	
	
	
	}