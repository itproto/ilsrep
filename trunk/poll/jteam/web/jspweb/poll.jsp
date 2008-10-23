<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="ilsrep.poll.common.protocol.AnswerItem"%>
<%@page import="ilsrep.poll.common.protocol.Answers"%>
<% String PLEASE_ENTER_POLL="Please choose poll";
%>
<%!
public String getPoll(String sessi, String polli, HttpSession hsession, String choici  , String customi  ) throws Exception{

String resultingOutput="<script type=\"text/javascript\" src=\"next.js\"></script>";
resultingOutput+="<form name=\"polls\" id=\"polls\" action=\"index.jsp\" method=\"get\">\n"+
"<input type='hidden' name='session' value='"+sessi+"' >";
Pollsession sess;
DBManager db;
int numberOfPolls=0;
db = new SQLiteDBManager(null,getServletContext().getRealPath("/")+"/pollserver.db");
sess=db.getPollsessionById(sessi);

resultingOutput+="<h1>"+sess.getName()+"</h1>";
Poll currentPoll= null;
if (!(Integer.parseInt(polli)==sess.getPolls().size())) currentPoll=sess.getPolls().get(Integer.parseInt(polli));
Answers ans=null;
if (Integer.parseInt(polli)==0){
	ans= new Answers();
	ans.setPollSesionId(sess.getId());
	ans.setAnswers(new ArrayList<AnswerItem>());
	ans.setUsername((String)hsession.getAttribute("username"));
	hsession.setAttribute("answers",ans);
			} else {
		
		AnswerItem ansitem=new AnswerItem();
		if (!(choici.equals("custom_choice"))) {
			Poll previousPoll=sess.getPolls().get(Integer.parseInt(polli)-1);
			int selId=0;
for (Choice currentChoice : previousPoll.getChoices()) {
                    if (currentChoice.getName().equals(choici)) selId=Integer.parseInt(currentChoice.getId());
                                  }
			ansitem.setItem(Integer.parseInt(previousPoll.getId()),selId);
			
			} else {
				Poll previousPoll=sess.getPolls().get(Integer.parseInt(polli)-1);
				ansitem.setItem(Integer.parseInt(previousPoll.getId()),customi);
				
				}
			Answers answer = (Answers)hsession.getAttribute("answers");
			boolean duplicate=false;
			for (AnswerItem ansItm: answer.getAnswers()) {
			if (ansItm.getQuestionId().equals(ansitem.getQuestionId())) {
				duplicate=true;
				}
				}
			if (!duplicate) {			
			answer.getAnswers().add(ansitem);
		}
			hsession.setAttribute("answers",answer);
    }
  if (!(Integer.parseInt(polli)==sess.getPolls().size())) {  
resultingOutput+="<div id="+Integer.toString(numberOfPolls)+">";
resultingOutput+="<h2>"+currentPoll.getName()+"</h2>";
resultingOutput+="<h3>"+currentPoll.getDescription().getValue()+"</h3>";
resultingOutput+="\n <table>";
boolean rowtype=true;
for( Choice currentChoice : currentPoll.getChoices()){
resultingOutput+="<tr ";

resultingOutput+="><td><input type='radio'  name='choice' value='"+currentChoice.getName()+"' CHECKED>"+currentChoice.getName()+"</td></tr>";
rowtype=rowtype ? false :true;
}
if(currentPoll.getCustomEnabled().equals("true")){
resultingOutput+="<tr><td><input type='radio' name='choice' value='custom_choice'>Custom<input type='textarea' name='custom'></td></tr>";
}
if(!(Integer.parseInt(polli)<sess.getPolls().size()-1)) {
	resultingOutput+="<tr><td><input type='hidden' value='1' name='res' ></td></tr>\n";
	}
resultingOutput+="<tr><td align=center><Input type='hidden' name='poll' value='"+Integer.toString(Integer.parseInt(polli)+1)+"'><button onMouseover='navOver(\"cmdMoveNext\")' onMouseout='navOut(\"cmdMoveNext\")' onMousedown='navDown(\"cmdMoveNext\")' onMouseup='navUp(\"cmdMoveNext\")' onClick='document.getElementById(\"polls\").submit();' ><img src='./images/cmdMoveNext.png' name=\"cmdMoveNext\" id=\"cmdMoveNext\" >Next</button></td></tr></table>";
resultingOutput+="</div></form>";
}
if (Integer.parseInt(polli)==sess.getPolls().size()) resultingOutput=getRes(hsession.getAttribute("answers"));
return resultingOutput;
}

%>