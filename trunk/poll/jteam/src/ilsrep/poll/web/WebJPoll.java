package ilsrep.poll.server;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.server.db.SQLiteDBManager;
import ilsrep.poll.server.db.DBManager;
import ilsrep.poll.common.protocol.AnswerItem;
import ilsrep.poll.common.protocol.Answers;
public class WebJPoll{
	private DBManager db;
	public void connect()throws Exception{
	db=new SQLiteDBManager(null,"./pollserver.db");
}
	 public Pollsession getPollsessionById(String id) throws Exception {
		 connect();
		 return(db.getPollsessionById(id));
	 
		 }
	public void createUser(String name, String pass)throws Exception {
		connect();
		 db.createUser(name, pass);
	 
		 }
		
    public boolean checkUser(String name) throws Exception { 
	    connect();
		return((db.checkUser(name).equals("true")? true : false ));	  
			  }
	public boolean authUser(String name, String pass) throws Exception {
		connect();
		return((db.authUser(name, pass).equals("true")? true : false ));
		}
	public Pollsessionlist getPollsessionlist() throws Exception {
		connect();
		return db.getPollsessionlist();
	}
	public void saveResults(Answers ans) throws Exception {
		connect();
		db.saveResults(ans);
		}
		public int storePollsession(Pollsession sess) throws Exception {
			connect();
			return db.storePollsession(sess);
			}
	 public void removePollsession(String id)throws Exception {
		 connect();
		 db.removePollsession(id);
		 }
		 public void updatePollsession(String id, Pollsession sess) throws Exception {
			 connect();
			 db.updatePollsession(id,sess);
			 }
	}