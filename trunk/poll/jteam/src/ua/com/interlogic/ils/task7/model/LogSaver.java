package ua.com.interlogic.ils.task7.model;
import java.util.LinkedList;
//drax
public class LogSaver{
		
	private class loghelper {
	String name;
	String selection;
		};
	public LinkedList<loghelper> lhelp = new LinkedList<loghelper>();
	public LogSaver() {
		
		}
		public void PushMe(String desc, String sid){
			loghelper lh=new loghelper();
			lh.name=desc;
			lh.selection=sid;
		lhelp.add(lh);	
			
			}
	 public void PopMe(){
		 while (!(lhelp.size()==0))
{
loghelper i=lhelp.removeFirst();
System.out.println(i.name+"=>"+i.selection+"\n");
}
		 
		 }
	
	}