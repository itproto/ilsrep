JSP POLL APPLICATION SETUP README[061008][okl]
1. Download [http://www.eu.apache.org/dist/tomcat/tomcat-6/v6.0.18/bin/apache-tomcat-6.0.18.exe] and install tomcat. 
Keep default settings during installation, admin password keep blank, as target location{TOMCAT_DIR} choose C:\tomcat

2. Configure tomcat. Goto {TOMCAT_DIR}/conf (that's  where server configuration files placed). 
!After configuration changes always restart server to apply changes.*
a. open server.xml for edit
b. look for node  '<Connector port="8080" protocol="HTTP/1.1" ..' change port attribute to 940 to avoid further conflicts with other servers installed on default www-port.

3. Start server. Use GUI {TOMCAT_DIR}/bin/tomcat6w.exe. If some problems, check recent logs {TOMCAT_DIR} \logs

4. Refer to README.txt in /webxml folder for instructions on site deployment