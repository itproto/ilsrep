package ua.com.interlogic.ils.task7.model;
import java.io.* ;
import java.net.*;
public class TcpCommunicator {
protected 	String receivedXml;
protected String pollId;
protected String answers;
protected String serverIp="127.0.0.1";
int port=43;
Socket clientSocket;
public  TcpCommunicator(){
	//Connecting and creating socket
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
protected void finalize(){
	// making sure that the socket is closed before the object is gced
	try {clientSocket.close();}  catch(Exception e ){};
	try {super.finalize();}  catch(Throwable e ){};
	}
public Reader getXML() {
	InputStream inFromServer=null;
	String xmlItself="";
	Reader xmlBuffered=null;
	try{
		//Generating input and output streams
	BufferedReader consoleInputReader = new BufferedReader(new InputStreamReader(System.in));
	System.out.println("Give me the id, baby:");
	 String id = consoleInputReader.readLine();
	 	DataOutputStream outToServer = new DataOutputStream(clientSocket.getOutputStream());
	 	inFromServer = clientSocket.getInputStream();
	 		 	//sending request
	 	outToServer.writeUTF("<getPollSession><pollSessionId>"+id+"</pollSessionId></getPollSession>");  
	 	System.out.println("Getting it, baby");
	 	String buffer;
	 	BufferedReader inputReader = new BufferedReader(new InputStreamReader(inFromServer));
	 	//Getting and parsing request. Reading line because
	 	// for some reason m test server returned first line empty, and the output started from second line.
	 inputReader.readLine();
	 	do {
		 	buffer=inputReader.readLine();
	 	xmlItself=xmlItself+"\n"+buffer;
 	} while(!(buffer.indexOf("/pollsession")>0));
 		System.out.println("Got it");
 		System.out.println(xmlItself);
 		//Making Reader out of string (needed for marshaller)
 		 	xmlBuffered= new StringReader(xmlItself);
  }catch(Exception e ){};
  //returning reader
	 	return xmlBuffered; 	
	
	}
	
	
	
	}