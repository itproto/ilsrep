<html>
<head> <title>JSP JAVA</title></head>
<body>
<script lang="javascript">
var l=0;
function next(k){
if(k==max+1) document.polls.submit(); 
document.getElementById(k).style.display='';
if (k>0) document.getElementById(k-1).style.display='none';
if(k==max+1) document.polls.submit(); 
return k+1;
}
</script>
<form name="polls" id="polls" action="submit.jsp" method="get">
<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<% 
out.println("<input type='hidden' name='session' value='"+request.getParameter("session")+"' >");
Pollsession sess;
DBManager db;
int num=0;
db = new SQLiteDBManager("/pollserver.db");
sess=db.getPollsessionById(request.getParameter("session"));

out.println("<h1>"+sess.getName()+"</h1>");
 for (Poll cur : sess.getPolls()) {
out.println("<div id="+Integer.toString(num)+">");
out.println("<h2>"+cur.getName()+"</h2>");
out.println("<h3>"+cur.getDescription().getValue()+"</h3>");
for( Choice ch : cur.getChoices()){
out.println("<input type='radio' name='"+cur.getName()+"' value='"+ch.getName()+"'>"+ch.getName()+"<br>");
}
if(cur.getCustomEnabled().equals("true")){
out.println("<input type='radio' name='"+cur.getName()+"' value='custom_choice'>Custom<input type='textarea' name='custom"+cur.getName()+"'>");
}
out.println("</div>");
num++;
}
%>

</br><input type="button" id="but" value="GO" onClick="l=next(l);">
</form>
<script lang="javascript">
<%
out.println("var max="+Integer.toString(num-1)+";");
for (int n=0;n<num;n++) out.println("document.getElementById('"+Integer.toString(n)+"').style.display='none';");
%>
l=next(l);

</script>
</body>
<html>