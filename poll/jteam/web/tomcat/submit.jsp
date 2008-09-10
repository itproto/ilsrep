

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
db = new SQLiteDBManager(getServletContext().getRealPath("/")+"/pollserver.db");
sess=db.getPollsessionById(request.getParameter("session"));
if(sess.getTestMode().equals("true")){ res+="<tr><td colspan=2>Your selection</td><td>Result</td></tr>\n";
} else {
 res+="<tr><td colspan=2>Your selection</td></tr>\n";
}

float n=0;
float i=0;
for (Poll cur : sess.getPolls()) {
n++;
AnswerItem itm = new AnswerItem();
if(request.getParameter(cur.getName()).equals("custom_choice")){
answers.add(itm.setItem(Integer.parseInt(cur.getId()), request.getParameter("custom"+cur.getName())));
if(sess.getTestMode().equals("true")){ 
res+="<tr><td>"+cur.getName()+"</td><td>"+request.getParameter("custom"+cur.getName())+"</td><td>FAIL</td></tr>\n";
}else {
res+="<tr><td>"+cur.getName()+"</td><td>"+request.getParameter("custom"+cur.getName())+"</td></tr>\n";
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
res+="<tr><td>"+cur.getName()+"</td><td>"+request.getParameter(cur.getName())+"</td><td>"+pass+"</td></tr>\n";
}else {
res+="<tr><td>"+cur.getName()+"</td><td>"+request.getParameter(cur.getName())+"</td></tr>\n";
}

answers.add(itm.setItem(Integer.parseInt(cur.getId()),sid));
        }

}
res+="</table>";
Float ress=i/n;
if(sess.getTestMode().equals("true")){
res+="Your score"+Float.toString(ress);  
res+=((ress)>Float.parseFloat(sess.getMinScore()))?"YOU PASS</h1>":"<h1>YOU FAIL </h1>\n";
}
return(res);
}
%>
