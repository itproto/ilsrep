function logAccount(){
	var wm = Components.classes["@mozilla.org/appshell/window-mediator;1"]
                   .getService(Components.interfaces.nsIWindowMediator);
var mainWindow = wm.getMostRecentWindow("navigator:browser");
var gBrowser=mainWindow.getBrowser();
	gBrowser.addTab("http://tabsender.appspot.com/?action=regtemp");
	}
function importTabs(id){
	    
	  
	    var httpRequest = new XMLHttpRequest();
	 httpRequest.overrideMimeType('text/xml');
	 httpRequest.onreadystatechange = function() { loadCode(httpRequest) };
	 
		 
          httpRequest.open('GET','http://tabsender.appspot.com/?action=load&id='+id, true);
        httpRequest.send('');
	    	    
	    
	    
    }
function deleteTabs(id){
	    
	  
	    var httpRequest = new XMLHttpRequest();
	 httpRequest.overrideMimeType('text/xml');
	 httpRequest.onreadystatechange = function() { showCode(httpRequest) };
	 
		 
          httpRequest.open('GET','http://tabsender.appspot.com/?action=remove&id='+id, true);
        httpRequest.send('');
	    	    
	    
	    
    }
function refreshList(){
	var httpRequest = new XMLHttpRequest();
	httpRequest.overrideMimeType('text/xml');
	httpRequest.onreadystatechange = function() { genList(httpRequest); };
	httpRequest.open('GET','http://tabsender.appspot.com/?action=list', true);
    httpRequest.send('');
	
	}
function genList(httpRequest){
	  if (httpRequest.readyState == 4) {
            if (httpRequest.status == 200) {
	          
               var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
               if (xmldoc.length==0){
	              var menuList=document.getElementById('tabList');
	             while (menuList.childNodes[0]) {
    menuList.removeChild(menuList.childNodes[0]);
}
	              var remList=document.getElementById('remList');
	             while (remList.childNodes[0]) {
    remList.removeChild(remList.childNodes[0]);
}

	            
				var cats = httpRequest.responseXML.getElementsByTagName('cat');
				
				for(var i=0;i<cats.length;i++){
					
					var session=document.createElement("menuitem");
					var rem_session=document.createElement("menuitem");
					session.setAttribute('label',cats.item(i).getAttribute('name'));	
					rem_session.setAttribute('label',cats.item(i).getAttribute('name'));
					session.setAttribute('tab_id',cats.item(i).getAttribute('id'));	
					rem_session.setAttribute('tab_id',cats.item(i).getAttribute('id'));
					
					session.onclick=function() {importTabs(this.getAttribute('tab_id'))}	;	
												menuList.appendChild(session);					
					rem_session.onclick=function() {deleteTabs(this.getAttribute('tab_id'))}	;
					remList.appendChild(rem_session);
					
						
							}
				    var sepItem=document.createElement("menuseparator");
					var refreshItem=document.createElement("menuitem");
					refreshItem.setAttribute('label',"Refresh List");
					refreshItem.onclick=function() {refreshList();}						
					  var sepItem2=document.createElement("menuseparator");
					var refreshItem2=document.createElement("menuitem");
					refreshItem2.setAttribute('label',"Refresh List");
					refreshItem2.onclick=function() {refreshList();}	
	               menuList.appendChild(sepItem);
	               remList.appendChild(sepItem2);
	               menuList.appendChild(refreshItem);
	               remList.appendChild(refreshItem2);
	               
	               
	               } else {
		               alert("Please log in");
		                logAccount();
		               }
               
            } else {
                alert('There was a problem with the request.');
            }
        }
	
	}
function exportTabs(){
	
	var wm = Components.classes["@mozilla.org/appshell/window-mediator;1"]
                   .getService(Components.interfaces.nsIWindowMediator);
var mainWindow = wm.getMostRecentWindow("navigator:browser");

var gBrowser=mainWindow.getBrowser();
var code=document.getElementById('tabsender-code').value;
	var numTabs = gBrowser.browsers.length;
var output='<?xml version="1.0" encoding="UTF-8" standalone="yes"?><urls name="'+code+'">';
	
	for(i=0;i<numTabs;i++){
	output+='<url link="'+gBrowser.getBrowserAtIndex(i).currentURI.spec+'" />';
	}		
	
		output+="</urls>"
		var httpRequest = new XMLHttpRequest();
		 httpRequest.overrideMimeType('text/xml');
		var prefManager = Components.classes["@mozilla.org/preferences-service;1"].getService(Components.interfaces.nsIPrefBranch);
		output=output.replace(/&/g,'&amp;');
		
					
		httpRequest.onreadystatechange = function() { showCode(httpRequest); };
			 
				 alert(output);
          httpRequest.open('GET','http://tabsender.appspot.com/?action=save&urls='+escape(output), true);
          httpRequest.send('');
        
		          
        }
        
	    function showCode(httpRequest) {

        if (httpRequest.readyState == 4) {
            if (httpRequest.status == 200) {
	         
               var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
              
               if (xmldoc.item(0).getAttribute("isOk") == "true" ) {
	               alert("Operation Succesful");
	                refreshList();
	               } else {
		               alert("Please log in");
		               logAccount();
		               }
              
            } else {
                alert('There was a problem with the request.');
            }
        }

    }
    
 
    
    
    
    function loadCode(httpRequest){
	    
	        if (httpRequest.readyState == 4) {
			            if (httpRequest.status == 200) {
				   var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
              
             if (xmldoc.length==0){   
	var wm = Components.classes["@mozilla.org/appshell/window-mediator;1"]
                   .getService(Components.interfaces.nsIWindowMediator);
var mainWindow = wm.getMostRecentWindow("navigator:browser");

var gBrowser=mainWindow.getBrowser();

								
													
											var url=new Array();
											var urls = httpRequest.responseXML.getElementsByTagName('url');
											for(var i=0;i<urls.length;i++){
												url[i]=urls.item(i).getAttribute("link");
												url[i]=url[i].replace('&amp;','&');
												
												}
											gBrowser.loadTabs(url,false,true);
								} else {
									alert("Please log in");
									logAccount();
									}
											
											
											
			
	 					} else {
                				alert('There was a problem with the request.');
        						}
        
        }
            
	}
  