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
<%!
public String getRes(ServletRequest request) throws Exception{
String res="<table border=\"1\">";
 List<AnswerItem> answers = new ArrayList<AnswerItem>();
Pollsession sess;
DBManager db;
db = new SQLiteDBManager(null,getServletContext().getRealPath("/")+"/pollserver.db");
sess=db.getPollsessionById(request.getParameter("session"));
if(sess.getTestMode().equals("true")){ res+="<tr><td colspan=2>Your selection</td><td>Result</td></tr>\n";
} else {
 res+="<tr><td colspan=2>Your selection</td></tr>\n";
}

float numberTotal=0;
float numberPassed=0;
for (Poll currentPoll : sess.getPolls()) {
numberTotal++;
AnswerItem answerItem = new AnswerItem();
if(request.getParameter(currentPoll.getName()).equals("custom_choice")){
answers.add(answerItem.setItem(Integer.parseInt(currentPoll.getId()), request.getParameter("custom"+currentPoll.getName())));
if(sess.getTestMode().equals("true")){ 
res+="<tr><td>"+currentPoll.getName()+"</td><td>"+request.getParameter("custom"+currentPoll.getName())+"</td><td>FAIL</td></tr>\n";
}else {
res+="<tr><td>"+currentPoll.getName()+"</td><td>"+request.getParameter("custom"+currentPoll.getName())+"</td></tr>\n";
}
} else {
int selectionId=0;
String pass="";
              for (Choice currentChoice : currentPoll.getChoices()) {
                    if (currentChoice.getName().equals(request.getParameter(currentPoll.getName()))) selectionId=Integer.parseInt(currentChoice.getId());
                                  }
pass=(Integer.parseInt(currentPoll.getCorrectChoice())==selectionId) ? "PASS" : "FAIL";
if(Integer.parseInt(currentPoll.getCorrectChoice())==selectionId) numberPassed++;
if(sess.getTestMode().equals("true")){ 
res+="<tr><td>"+currentPoll.getName()+"</td><td>"+request.getParameter(currentPoll.getName())+"</td><td>"+pass+"</td></tr>\n";
}else {
res+="<tr><td>"+currentPoll.getName()+"</td><td>"+request.getParameter(currentPoll.getName())+"</td></tr>\n";
}

answers.add(answerItem.setItem(Integer.parseInt(currentPoll.getId()),selectionId));
        }

}
res+="</table>";
Float ress=numberPassed/numberTotal;
if(sess.getTestMode().equals("true")){
res+="Your score"+Float.toString(ress);  
res+=((ress)>Float.parseFloat(sess.getMinScore()))?"YOU PASS</h1>":"<h1>YOU FAIL </h1>\n";
}
return(res);
}
%>
