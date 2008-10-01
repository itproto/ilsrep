1) install tomcat http://www.eu.apache.org/dist/tomcat/tomcat-6/v6.0.18/bin/apache-tomcat-6.0.18.exe
2) in tomcat conf dir find a file server.xml
find this line 
<Host name="localhost"  appBase="webapps"
            unpackWARs="true" autoDeploy="true"
            xmlValidation="false" xmlNamespaceAware="false" >
            
            Change "webapps" to your folder path to /web/ directory in jteam project
3) start tomcat


NOTE: if you ran tomcat prior to repointing the webapps path, you will have too delete cache that is stored in /work/ directory and then restart