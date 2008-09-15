<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<% String PLEASE_ENTER_POLL="Please choose poll";
%>
<%!
public String getPoll(String sessi) throws Exception{

String resultingOutput="<script type=\"text/javascript\" src=\"next.js\"></script>";
resultingOutput+="<form name=\"polls\" id=\"polls\" action=\"index.jsp\" method=\"get\">\n"+
"<input type='hidden' name='session' value='"+sessi+"' >";
Pollsession sess;
DBManager db;
int numberOfPolls=0;
db = new SQLiteDBManager(null,getServletContext().getRealPath("/")+"/pollserver.db");
sess=db.getPollsessionById(sessi);

resultingOutput+="<h1>"+sess.getName()+"</h1>";
 for (Poll currentPoll : sess.getPolls()) {
resultingOutput+="<div id="+Integer.toString(numberOfPolls)+" style=\"inner_poll\">";
resultingOutput+="<h2>"+currentPoll.getName()+"</h2>";
resultingOutput+="<h3>"+currentPoll.getDescription().getValue()+"</h3>";
for( Choice currentChoice : currentPoll.getChoices()){
resultingOutput+="<input type='radio' name='"+currentPoll.getName()+"' value='"+currentChoice.getName()+"'>"+currentChoice.getName()+"<br>";
}
if(currentPoll.getCustomEnabled().equals("true")){
resultingOutput+="<input type='radio' name='"+currentPoll.getName()+"' value='custom_choice'>Custom<input type='textarea' name='custom"+currentPoll.getName()+"'>";
}
resultingOutput+="</div>";
numberOfPolls++;
}

resultingOutput+="</br><input type=\"button\" id=\"but\" value=\"GO\" onClick=\"l=next(l);\">\n"+
"<input type='hidden' name='sub' value='true'>\n"+
"</form>\n"+
"<script lang=\"javascript\">"+
"var max="+Integer.toString(numberOfPolls-1)+";";
for (int n=0;n<numberOfPolls;n++) resultingOutput+="document.getElementById('"+Integer.toString(n)+"').style.display='none';\n";
resultingOutput+="l=next(l);"+
"</script>";
return resultingOutput;
}

%>