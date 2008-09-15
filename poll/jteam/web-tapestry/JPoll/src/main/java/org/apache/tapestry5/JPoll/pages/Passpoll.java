package org.apache.tapestry5.JPoll.pages;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.server.db.SQLiteDBManager;
import ilsrep.poll.server.db.DBManager;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.common.protocol.Item;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.apache.tapestry5.annotations.InjectPage;
import org.apache.tapestry5.annotations.Persist;
import org.apache.tapestry5.annotations.Property;
/**
 * Start page of application JPoll.
 */
public class Passpoll
{
@Persist
private  DBManager db;
@Persist
private Pollsessionlist sessions;
@Persist
private Pollsession sess;
  @Persist
  @Property
    private String session;

  public Object initialize(String session) throws Exception
  {
    this.session = session;
    db = new SQLiteDBManager(null,"/pollserver.db");
	  sessions=db.getPollsessionlist();
	  for ( Item choose : sessions.getItems()) {
			if(choose.getName().equals(session)) this.sess=db.getPollsessionById(choose.getId());		  
			  }
    return this;
  }
  
  public String getSession_name() throws Exception{
	 
	  return sess.getName();
  }
  public String getSession_id() throws Exception{
		 
	  return sess.getId();
  }
  @InjectPage
  private StartPoll startpoll;
  Object onActionFromStart(String choice) throws Exception
  {
  
   return startpoll.initialize(choice);
   
  }
}