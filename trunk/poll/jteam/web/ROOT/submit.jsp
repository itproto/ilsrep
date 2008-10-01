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
String res="<table>";
 List<AnswerItem> answers = new ArrayList<AnswerItem>();
Pollsession sess;
DBManager db;
db = new SQLiteDBManager(null,getServletContext().getRealPath("/")+"/pollserver.db");
sess=db.getPollsessionById(request.getParameter("session"));
if(sess.getTestMode().equals("true")){ res+="<tr><th class=\"first\" colspan=2><strong>Your selection</strong></th><th rowspan=2><strong>Result</strong></th></tr>\n";
} else {
 res+="<tr><th colspan=2><strong>Your selection</strong></th></tr>\n";
}
res+="<tr><th class=\"first\"><strong>Poll</strong></th><th><strong>Answer</strong></th></tr>\n";
float numberTotal=0;
float numberPassed=0;
boolean rowtype=true;
for (Poll currentPoll : sess.getPolls()) {
numberTotal++;
AnswerItem answerItem = new AnswerItem();
if(request.getParameter(currentPoll.getName()).equals("custom_choice")){
answers.add(answerItem.setItem(Integer.parseInt(currentPoll.getId()), request.getParameter("custom"+currentPoll.getName())));
if(sess.getTestMode().equals("true")){ 
res+="<tr ";
res+= rowtype ? "class=\"rowA\"" : "class=\"rowB\"" ;
res+="><td>"+currentPoll.getName()+"</td><td>"+request.getParameter("custom"+currentPoll.getName())+"</td><td>FAIL</td></tr>\n";
}else {
res+="<tr ";
res+= rowtype ? "class=\"rowA\"" : "class=\"rowB\"" ;
res+="><td>"+currentPoll.getName()+"</td><td>"+request.getParameter("custom"+currentPoll.getName())+"</td></tr>\n";
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
res+="<tr ";
res+= rowtype ? "class=\"rowA\"" : "class=\"rowB\"" ;
res+="><td>"+currentPoll.getName()+"</td><td>"+request.getParameter(currentPoll.getName())+"</td><td>"+pass+"</td></tr>\n";
}else {
res+="<tr ";
res+= rowtype ? "class=\"rowA\"" : "class=\"rowB\"" ;
res+="><td>"+currentPoll.getName()+"</td><td>"+request.getParameter(currentPoll.getName())+"</td></tr>\n";
}

answers.add(answerItem.setItem(Integer.parseInt(currentPoll.getId()),selectionId));
        }
rowtype=rowtype ? false :true;

}
res+="</table>";
Float ress=numberPassed/numberTotal;
if(sess.getTestMode().equals("true")){
res+="<br/><strong>Your score"+Float.toString(ress)+"</strong>";  
res+=((ress)>Float.parseFloat(sess.getMinScore()))?"You Pass</h1>":"<h1>You Fail </h1>\n";
}
return(res);
}
%>
