<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.common.protocol.Answers"%>
<%@page import="ilsrep.poll.common.protocol.AnswerItem"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="java.util.ArrayList"%>
<%@page import="java.util.List"%>
<%@page import="javax.servlet.ServletRequest"%>
<%@page import="java.text.DecimalFormat"%>
<%@page import="webservice.endpoint.WebJPoll"%>
<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%!
public Poll getPollbyid(Pollsession sess, String id){
	Poll poll=null;
	for (Poll cur : sess.getPolls()) {
		if (cur.getId().equals(id)) poll=cur;
		}
	return poll;
	}
public Choice getChoicebyid(Poll poll, String id){
	Choice choice=null;
	for (Choice cur : poll.getChoices()) {
		if (cur.getId().equals(id)) choice=cur;
		}
	return choice;
	}
public String getRes(Object ansobj) throws Exception{
	Answers ans= (Answers)ansobj;
		
String res="<table>";
 List<AnswerItem> answers = ans.getAnswers();;
Pollsession sess;
 WebJPoll_Service service=new WebJPoll_Service();
	WebJPoll db=service.getWebJPollPort();
sess=db.getPollsessionById(ans.getPollSesionId());
if(sess.getTestMode().equals("true")){ res+="<tr><th class=\"first\" colspan=2><strong>Your selection</strong></th><th rowspan=2><strong>Result</strong></th></tr>\n";
} else {
 res+="<tr><th colspan=2><strong>Your selection</strong></th></tr>\n";
}
res+="<tr><th class=\"first\"><strong>Poll</strong></th><th><strong>Answer</strong></th></tr>\n";
float numberTotal=0;
float numberPassed=0;
boolean rowtype=true;

 

for (AnswerItem ansItm: answers) {
	Poll currentPoll=getPollbyid(sess, ansItm.getQuestionId());
	Choice currentChoice=getChoicebyid(currentPoll,ansItm.getAnswerId());
numberTotal++;
if(ansItm.getCustomChoice()!=null){
if(sess.getTestMode().equals("true")){ 
res+="<tr ";
res+= rowtype ? "class=\"rowA\"" : "class=\"rowB\"" ;
res+="><td>"+currentPoll.getName()+"</td><td>"+ansItm.getCustomChoice()+"</td><td>FAIL</td></tr>\n";
}else {
res+="<tr ";
res+= rowtype ? "class=\"rowA\"" : "class=\"rowB\"" ;
res+="><td>"+currentPoll.getName()+"</td><td>"+ansItm.getCustomChoice()+"</td></tr>\n";
}
} else {
int selectionId=0;
String pass="";
pass=(currentPoll.getCorrectChoice().equals(ansItm.getAnswerId())) ? "PASS" : "FAIL";
if(currentPoll.getCorrectChoice().equals(ansItm.getAnswerId())) numberPassed++;
if(sess.getTestMode().equals("true")){ 
res+="<tr ";
res+= rowtype ? "class=\"rowA\"" : "class=\"rowB\"" ;
res+="><td>"+currentPoll.getName()+"</td><td>"+currentChoice.getName()+"</td><td>"+pass+"</td></tr>\n";
}else {
res+="<tr ";
res+= rowtype ? "class=\"rowA\"" : "class=\"rowB\"" ;
res+="><td>"+currentPoll.getName()+"</td><td>"+currentChoice.getName()+"</td></tr>\n";
}


        }
        
rowtype=rowtype ? false :true;
}
res+="</table>";
Float ress=numberPassed/numberTotal;
if(sess.getTestMode().equals("true")){
		
DecimalFormat df = new DecimalFormat("#.##");
res+="<br/><strong>Your score  "+df.format(ress)+"</strong>";  

res+=((ress)>Float.parseFloat(sess.getMinScore()))?"<h2>You Pass</h2>":"<h2>You Fail </h2>\n";
}
db.saveResults(ans);
return(res);
}
%>
