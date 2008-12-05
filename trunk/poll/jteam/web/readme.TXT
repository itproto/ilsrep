JSP POLL APPLICATION SETUP README[061008][okl]
1. Download [http://www.eu.apache.org/dist/tomcat/tomcat-6/v6.0.18/bin/apache-tomcat-6.0.18.exe] and install tomcat. 
Keep default settings during installation, admin password keep blank, as target location{TOMCAT_DIR} choose C:\tomcat

2. Configure tomcat. Goto {TOMCAT_DIR}/conf (that's  where server configuration files placed). 
!After configuration changes always restart server to apply changes.*
a. open server.xml for edit
b. look for node  '<Connector port="8080" protocol="HTTP/1.1" ..' change port attribute to 940 to avoid further conflicts with other servers installed on default www-port.
c. look for node '<Host name="localhost"  appBase="webapps"..' (is child of <Engine name="Catalina" defaultHost="localhost"> ). Add next child node to "<Host" node
		<Context path="/jspweb" docBase="E:\.dev\ils\ilsrep\poll\jteam\web\jspweb" reloadable="true" swallowOutput="true" />
	
	path="/jspweb" - is virtual directory attribute
	docBase="..." -  is path to your directory ([where you have svn checkout directory for ilsrep]\poll\jteam\web)

3. Start server. Use GUI {TOMCAT_DIR}/bin/tomcat6w.exe. If some problems, check recent logs {TOMCAT_DIR} \logs

4. Point your browser to http://localhost/jspweb to test poll\jteam\web trunk.

*Restart tomcat: a. run {TOMCAT_DIR}/bin/tomcat6w.exe  b. press STOP button c.after stopping -> START button

/*
1) install tomcat http://www.eu.apache.org/dist/tomcat/tomcat-6/v6.0.18/bin/apache-tomcat-6.0.18.exe
2) in tomcat conf dir find server.xml and replace it with server.xml from this directory(
if you experience any further problems, use the server-940.xml, it will start tomcat on port 940 (us in case if 80 port is blocked)
)
3) start tomcat, delete everything from tomcat /work/ directory
4)go to http://localhost/jspweb or to htp://localhost:940/jspweb if you used server-940.xml


NOTE: if you ran tomcat prior to repointing the webapps path, you will have too delete cache that is stored in /work/ directory and then restart
*/


/****To have tomcat running with web service change tomcat port to 940********/