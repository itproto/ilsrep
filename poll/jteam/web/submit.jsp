<html>
<head> <title>JSP JAVA</title></head>
<body>
<table border="1">

<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.common.protocol.Answers"%>
<%@page import="ilsrep.poll.common.protocol.AnswerItem"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="java.util.ArrayList"%>
<%@page import="java.util.List"%>
<% 
 List<AnswerItem> answers = new ArrayList<AnswerItem>();
Pollsession sess;
DBManager db;
db = new SQLiteDBManager("/pollserver.db");
sess=db.getPollsessionById(request.getParameter("session"));
if(sess.getTestMode().equals("true")){ out.println("<tr><td colspan=2>Your selection</td><td>Result</td></tr>");
} else {
 out.println("<tr><td colspan=2>Your selection</td></tr>");
}

float n=0;
float i=0;
for (Poll cur : sess.getPolls()) {
n++;
AnswerItem itm = new AnswerItem();
if(request.getParameter(cur.getName()).equals("custom_choice")){
answers.add(itm.setItem(Integer.parseInt(cur.getId()), request.getParameter("custom"+cur.getName())));
if(sess.getTestMode().equals("true")){ 
out.println("<tr><td>"+cur.getName()+"</td><td>"+request.getParameter("custom"+cur.getName())+"</td><td>FAIL</td></tr>");
}else {
out.println("<tr><td>"+cur.getName()+"</td><td>"+request.getParameter("custom"+cur.getName())+"</td></tr>");
}
} else {
int sid=0;
String pass="";
              for (Choice ch : cur.getChoices()) {
                    if (ch.getName().equals(request.getParameter(cur.getName()))) sid=Integer.parseInt(ch.getId());
                                  }
pass=(Integer.parseInt(cur.getCorrectChoice())==sid) ? "PASS" : "FAIL";
if(Integer.parseInt(cur.getCorrectChoice())==sid) i++;
if(sess.getTestMode().equals("true")){ 
out.println("<tr><td>"+cur.getName()+"</td><td>"+request.getParameter(cur.getName())+"</td><td>"+pass+"</td></tr>");
}else {
out.println("<tr><td>"+cur.getName()+"</td><td>"+request.getParameter(cur.getName())+"</td></tr>");
}

answers.add(itm.setItem(Integer.parseInt(cur.getId()),sid));
        }

}
%>
</table>
<% 
Float res=i/n;
if(sess.getTestMode().equals("true")){
out.println("Your score"+Float.toString(res));  
out.println(((res)>Float.parseFloat(sess.getMinScore()))?"YOU PASS</h1>":"<h1>YOU FAIL </h1>");
}

%>
</body>
<html>