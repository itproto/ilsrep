package org.apache.tapestry5.JPoll.components;
import ilsrep.poll.common.protocol.Item;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.server.db.DBManager;
import ilsrep.poll.server.db.SQLiteDBManager;

import java.util.ArrayList;
import java.util.List;

import org.apache.tapestry5.JPoll.pages.Passpoll;
import org.apache.tapestry5.annotations.InjectPage;
import org.apache.tapestry5.annotations.Property;

public class Layout
{
private  DBManager db;
	 
	  private Pollsessionlist sessions;
  public List<String> getPolsessions() throws Exception
  {
	  db = new SQLiteDBManager(null,"/pollserver.db");
	  sessions=db.getPollsessionlist();
	 
	  List<String>names=new ArrayList<String>();
	  for ( Item sess : sessions.getItems()) {
	names.add(sess.getName());		  
	  }
    return names;
  }
@Property
  private String session;
  @InjectPage
  private Passpoll passpoll;

  Object onActionFromPoll(String choice) throws Exception
  {
  
   return passpoll.initialize(choice);
   
  }
  
}
