1) install tomcat http://www.eu.apache.org/dist/tomcat/tomcat-6/v6.0.18/bin/apache-tomcat-6.0.18.exe
2) in tomcat conf dir find server.xml and replace it with server.xml from this directory(
if you experience any further problems, use the server-940.xml, it will start tomcat on port 940 (us in case if 80 port is blocked)
)
3) start tomcat, delete everything from tomcat /work/ directory
4)go to http://localhost/jspweb or to htp://localhost:940/jspweb if you used server-940.xml


NOTE: if you ran tomcat prior to repointing the webapps path, you will have too delete cache that is stored in /work/ directory and then restart