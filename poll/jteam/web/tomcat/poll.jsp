<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<% String PLEASE_ENTER_POLL="Please choose poll";
%>
<%!
public String getPoll(String sessi) throws Exception{

String res="<script type=\"text/javascript\" src=\"next.js\"></script>";
res+="<form name=\"polls\" id=\"polls\" action=\"index.jsp\" method=\"get\">\n"+
"<input type='hidden' name='session' value='"+sessi+"' >";
Pollsession sess;
DBManager db;
int num=0;
db = new SQLiteDBManager("/pollserver.db");
sess=db.getPollsessionById(sessi);

res+="<h1>"+sess.getName()+"</h1>";
 for (Poll cur : sess.getPolls()) {
res+="<div id="+Integer.toString(num)+" style=\"inner_poll\">";
res+="<h2>"+cur.getName()+"</h2>";
res+="<h3>"+cur.getDescription().getValue()+"</h3>";
for( Choice ch : cur.getChoices()){
res+="<input type='radio' name='"+cur.getName()+"' value='"+ch.getName()+"'>"+ch.getName()+"<br>";
}
if(cur.getCustomEnabled().equals("true")){
res+="<input type='radio' name='"+cur.getName()+"' value='custom_choice'>Custom<input type='textarea' name='custom"+cur.getName()+"'>";
}
res+="</div>";
num++;
}

res+="</br><input type=\"button\" id=\"but\" value=\"GO\" onClick=\"l=next(l);\">\n"+
"<input type='hidden' name='sub' value='true'>\n"+
"</form>\n"+
"<script lang=\"javascript\">"+
"var max="+Integer.toString(num-1)+";";
for (int n=0;n<num;n++) res+="document.getElementById('"+Integer.toString(n)+"').style.display='none';\n";
res+="l=next(l);"+
"</script>";
return res;
}

%>