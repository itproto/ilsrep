function exportTabs(){
	
	var wm = Components.classes["@mozilla.org/appshell/window-mediator;1"]
                   .getService(Components.interfaces.nsIWindowMediator);
var mainWindow = wm.getMostRecentWindow("navigator:browser");

var gBrowser=mainWindow.getBrowser();

	var numTabs = gBrowser.browsers.length;
var output='<?xml version="1.0" encoding="UTF-8" standalone="yes"?><tabsender><urls>';
	
	for(i=0;i<numTabs;i++){
	output+='<url link="'+gBrowser.getBrowserAtIndex(i).currentURI.spec+'" />'+"\n";
	}		
	
		output+="</urls></tabsender>"
		var httpRequest = new XMLHttpRequest();
		 httpRequest.overrideMimeType('text/xml');
		var prefManager = Components.classes["@mozilla.org/preferences-service;1"].getService(Components.interfaces.nsIPrefBranch);
		var login=prefManager.getCharPref("extensions.tabSender.user");
		
	var password=prefManager.getCharPref("extensions.tabSender.password");
		var server=prefManager.getCharPref("extensions.tabSender.server");
		output=output.replace(/&/g,'&amp;');
		
					
		httpRequest.onreadystatechange = function() { showCode(httpRequest); };
			 if (!(server === undefined)) {
          httpRequest.open('GET',server+'/senderServer/tabServer?action=export&urls='+escape(output), true);
          httpRequest.send('');
          	 }else {
window.open('chrome://tabsender/content/window.xul', 'Tabsender', 'chrome, centerscreen=yes');	
	}
		          
        }
        
	    function showCode(httpRequest) {

        if (httpRequest.readyState == 4) {
            if (httpRequest.status == 200) {
	          
                var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
                
								var result=xmldoc.item(0).getAttribute('message');
								
								document.getElementById('tabsender-code').value=result;
            } else {
                alert('There was a problem with the request.');
            }
        }

    }
    
 
    
    function importTabs(){
	    
	    var prefManager = Components.classes["@mozilla.org/preferences-service;1"].getService(Components.interfaces.nsIPrefBranch);
	var login=prefManager.getCharPref("extensions.tabSender.user");
	var password=prefManager.getCharPref("extensions.tabSender.password");
	var server=prefManager.getCharPref("extensions.tabSender.server");
	    var code=document.getElementById('tabsender-code').value;
	    var httpRequest = new XMLHttpRequest();
	 httpRequest.overrideMimeType('text/xml');
	 httpRequest.onreadystatechange = function() { loadCode(httpRequest) };
	 if (!(server === undefined)) {
		 
          httpRequest.open('GET',server+'/senderServer/tabServer?action=import&code='+code, true);
        httpRequest.send('');
	    
	    
	    }else {
window.open('chrome://tabsender/content/window.xul', 'Tabsender', 'chrome, centerscreen=yes');	
	}
	    
    }
    
    function loadCode(httpRequest){
	    
	        if (httpRequest.readyState == 4) {
			            if (httpRequest.status == 200) {
				        
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
                				alert('There was a problem with the request.');
        						}
        
        }
            
	}
  