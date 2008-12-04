Enhanced version. works without frameworks etc.
Install:
1) download jax-ws tools: https://jax-ws.dev.java.net/2.1.5/
2) install them via java -jar JAXWS2.1.5-20081030.jar
 to any folder
3) copy .jar file from lib directory from installed folder to lib directory of your tomcat installation.
4) restart tomcat
5) run 
ant compile; ant webcompile; ant web;
in the jteam source directory
6)deploy WebJPoll.war via tomcat manager (acessable by http://localhost:940/manager/)
7)goto http://localhost:940/WebJPoll/WebJPoll to see the summary

Binding.jxb file is used for wsdl import and is not needed during compilation