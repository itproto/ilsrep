package org.apache.tapestry5.JPoll.pages;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.server.db.SQLiteDBManager;
import ilsrep.poll.server.db.DBManager;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.common.protocol.Item;
import ilsrep.poll.common.protocol.AnswerItem;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import org.apache.tapestry5.annotations.InjectPage;
import org.apache.tapestry5.annotations.Persist;
import org.apache.tapestry5.annotations.Property;
/**
 * Start page of application JPoll.
 */
public class StartPoll{
	@Persist
	@Property
	private List<AnswerItem> answers;
	@Persist
	@Property
	private String choicesList;
	@Persist
	@Property
	private String customChoice;
	@Persist 
	private int iterator=0;
	@Persist
	private  DBManager db;
	@Persist
	private Pollsessionlist sessions;
	@Persist
	private Pollsession sess;
	@Persist 
	private List<Poll> pollList;
	@Persist
	private Poll currentPoll;
	@Property
	private String choice;
	  @InjectPage
	  private Results results;
	@Persist
@Property
private String session;
	
public List<String> getChoices(){
	List<String> choices=new ArrayList<String>();
	for (Choice itm: currentPoll.getChoices()){
		choices.add(itm.getName());
	}
	return choices;
}

@Persist
@Property
private String session_id;
	  public Object initialize(String session) throws Exception
	  {
		  iterator=0;
		  answers=new ArrayList<AnswerItem>();
		  db = new SQLiteDBManager(null,"/pollserver.db");
this.sess=db.getPollsessionById(session);		  
				 this.pollList=sess.getPolls(); 
				 currentPoll=pollList.get(iterator);
				 iterator++;
	    return this;
	  }
	  public Object initialize() throws Exception
	  {
		  currentPoll=pollList.get(iterator);
		  iterator++;
      return this;
	  }
	  public String getSessionName() 
	  {
		return sess.getName();
	    
	  }
	  public String getPollName() 
	  {
	return currentPoll.getName();
	    
	  }
	  public String getPollDesc() 
	  {
		return currentPoll.getDescription().getValue();
	    
	  }
	  
	  public String getIterator() 
	  {
		return Integer.toString(this.iterator);
	    
	  }
	  public boolean getCustom(){
		  
		  return currentPoll.getCustomEnabled().equals("true") ? true : false;
	  }
	  
	  Object onSuccess() throws Exception
	  {
		   AnswerItem ansItem=new AnswerItem();
		  for (Choice itm: currentPoll.getChoices()){
			
		if (choicesList.equals(itm.getName())){	ansItem.setItem(Integer.parseInt(currentPoll.getId()), Integer.parseInt(itm.getId()));
		
		}
			}
		  if(choicesList.equals("Custom")){
			 ansItem.setItem(Integer.parseInt(currentPoll.getId()),customChoice);
		  }
		  answers.add(ansItem);
		 	  if(sess.getPolls().size()>iterator) return this.initialize();
	  else { return results.initalize(sess, answers);
	  }
	  	   
	  }
	  
	
	}
	
	
	
