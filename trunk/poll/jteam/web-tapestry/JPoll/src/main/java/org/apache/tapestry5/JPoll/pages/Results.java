package org.apache.tapestry5.JPoll.pages;
import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Choice;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.server.db.SQLiteDBManager;
import ilsrep.poll.server.db.DBManager;
import ilsrep.poll.common.protocol.AnswerItem;
import ilsrep.poll.common.protocol.Pollsessionlist;
import ilsrep.poll.common.protocol.Item;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import org.apache.tapestry5.annotations.InjectPage;
import org.apache.tapestry5.annotations.Persist;
import org.apache.tapestry5.annotations.Property;

public class Results{
	
	public class Res{
		
		private  String poll;
		
		private String answer;
		public String getPoll() {return this.poll;}
		public String getAnswer() {return this.answer;}
		public void setPoll(String pol) {poll=pol;}
		public void setAnswer(String pol) {answer=pol;}
		
	}
	
	
	@Persist
	@Property
	private List<AnswerItem> answers;
	 
	@Persist
	@Property
	private Pollsession session;
	
	
	public Object initalize(Pollsession sess, List<AnswerItem> ans){
		answers=ans;
		session=sess;
		return this;
			}
	public List<Res> getResulting(){
		List<Res> resList=new ArrayList<Res>();
		Poll  pollchosen=null;
		for (AnswerItem ansIt: answers){
			Res resIt=new Res();
		for (Poll pol: session.getPolls()){
			if(pol.getId().equals(ansIt.getQuestionId())){ resIt.setPoll(pol.getName());
			pollchosen=pol;
			}
		}
			resIt.setAnswer(ansIt.getCustomChoice());
			for (Choice ch: pollchosen.getChoices()){
				if(ch.getId().equals(ansIt.getAnswerId())) resIt.setAnswer(ch.getName());		
		}
		
		resList.add(resIt);
	}
	return resList;
}

}