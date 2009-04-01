var Base64 = {
 
	// private property
	_keyStr : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",
 
	// public method for encoding
	encode : function (input) {
		var output = "";
		var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
		var i = 0;
 
		input = Base64._utf8_encode(input);
 
		while (i < input.length) {
 
			chr1 = input.charCodeAt(i++);
			chr2 = input.charCodeAt(i++);
			chr3 = input.charCodeAt(i++);
 
			enc1 = chr1 >> 2;
			enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
			enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
			enc4 = chr3 & 63;
 
			if (isNaN(chr2)) {
				enc3 = enc4 = 64;
			} else if (isNaN(chr3)) {
				enc4 = 64;
			}
 
			output = output +
			this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
			this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);
 
		}
 
		return output;
	},
 
	// public method for decoding
	decode : function (input) {
		var output = "";
		var chr1, chr2, chr3;
		var enc1, enc2, enc3, enc4;
		var i = 0;
 
		input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
 
		while (i < input.length) {
 
			enc1 = this._keyStr.indexOf(input.charAt(i++));
			enc2 = this._keyStr.indexOf(input.charAt(i++));
			enc3 = this._keyStr.indexOf(input.charAt(i++));
			enc4 = this._keyStr.indexOf(input.charAt(i++));
 
			chr1 = (enc1 << 2) | (enc2 >> 4);
			chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
			chr3 = ((enc3 & 3) << 6) | enc4;
 
			output = output + String.fromCharCode(chr1);
 
			if (enc3 != 64) {
				output = output + String.fromCharCode(chr2);
			}
			if (enc4 != 64) {
				output = output + String.fromCharCode(chr3);
			}
 
		}
 
		output = Base64._utf8_decode(output);
 
		return output;
 
	},
 
	// private method for UTF-8 encoding
	_utf8_encode : function (string) {
		string = string.replace(/\r\n/g,"\n");
		var utftext = "";
 
		for (var n = 0; n < string.length; n++) {
 
			var c = string.charCodeAt(n);
 
			if (c < 128) {
				utftext += String.fromCharCode(c);
			}
			else if((c > 127) && (c < 2048)) {
				utftext += String.fromCharCode((c >> 6) | 192);
				utftext += String.fromCharCode((c & 63) | 128);
			}
			else {
				utftext += String.fromCharCode((c >> 12) | 224);
				utftext += String.fromCharCode(((c >> 6) & 63) | 128);
				utftext += String.fromCharCode((c & 63) | 128);
			}
 
		}
 
		return utftext;
	},
 
	// private method for UTF-8 decoding
	_utf8_decode : function (utftext) {
		var string = "";
		var i = 0;
		var c = c1 = c2 = 0;
 
		while ( i < utftext.length ) {
 
			c = utftext.charCodeAt(i);
 
			if (c < 128) {
				string += String.fromCharCode(c);
				i++;
			}
			else if((c > 191) && (c < 224)) {
				c2 = utftext.charCodeAt(i+1);
				string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
				i += 2;
			}
			else {
				c2 = utftext.charCodeAt(i+1);
				c3 = utftext.charCodeAt(i+2);
				string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
				i += 3;
			}
 
		}
 
		return string;
	}
 
}
function refreshMenu(){

var wm = Components.classes["@mozilla.org/appshell/window-mediator;1"]
                   .getService(Components.interfaces.nsIWindowMediator);
var mainWindow = wm.getMostRecentWindow("navigator:browser").document; 
var stub=mainWindow.getElementById("cmd_refresh");

stub.onclick();
}
function shareTabs(id){
	var plainText=Base64.encode(id);
	


var xferable = Components.classes["@mozilla.org/widget/transferable;1"]
    .createInstance(Components.interfaces.nsITransferable);

xferable.addDataFlavor("text/unicode");

var unicodestring = Components.classes["@mozilla.org/supports-string;1"]
    .createInstance(Components.interfaces.nsISupportsString);

unicodestring.data = plainText;
xferable.setTransferData("text/unicode", unicodestring, plainText.length * 2);

var clipboard = Components.classes["@mozilla.org/widget/clipboard;1"]
    .getService(Components.interfaces.nsIClipboard);

clipboard.setData(xferable, null,
        Components.interfaces.nsIClipboard.kGlobalClipboard);




	alert("code copied to clipboard");
	
	
	}
function importCode(){
	var code=document.getElementById('tabsender-import-code').value;
	var sid=Base64.decode(code);
	importTabs(sid);
	
	}
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
	// httpRequest.onreadystatechange = function() { loadCode(httpRequest) };
	 
		 
          httpRequest.open('GET','http://tabsender.appspot.com/?action=load&id='+id, false);
        httpRequest.send('');
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
	httpRequest.open('GET','http://tabsender.appspot.com/?action=list', false);
    httpRequest.send('');
	 


	var wm = Components.classes["@mozilla.org/appshell/window-mediator;1"]
                   .getService(Components.interfaces.nsIWindowMediator);
var mainWindow = wm.getMostRecentWindow("navigator:browser").document; 


	  if (httpRequest.readyState == 4) {
            if (httpRequest.status == 200) {
	          
               var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
               if (xmldoc.length==0){
	              var menuList=mainWindow.getElementById('tabList');
	             while (menuList.childNodes[0]) {
    menuList.removeChild(menuList.childNodes[0]);
}

	              var remList=mainWindow.getElementById('remList');
	             while (remList.childNodes[0]) {
    remList.removeChild(remList.childNodes[0]);
}
 var shareList=mainWindow.getElementById('shareList');
	             while (shareList.childNodes[0]) {
    shareList.removeChild(shareList.childNodes[0]);
}
	            
				var cats = httpRequest.responseXML.getElementsByTagName('cat');
				
				for(var i=0;i<cats.length;i++){
					
					var session=mainWindow.createElement("menuitem");
					var rem_session=mainWindow.createElement("menuitem");
					var share_session=mainWindow.createElement("menuitem");
					session.setAttribute('label',cats.item(i).getAttribute('name'));	
					rem_session.setAttribute('label',cats.item(i).getAttribute('name'));
					share_session.setAttribute('label',cats.item(i).getAttribute('name'));
					session.setAttribute('tab_id',cats.item(i).getAttribute('id'));	
					rem_session.setAttribute('tab_id',cats.item(i).getAttribute('id'));
					share_session.setAttribute('tab_id',cats.item(i).getAttribute('id'));
					
					session.onclick=function() {importTabs(this.getAttribute('tab_id'))}	;	
												menuList.appendChild(session);					
					rem_session.onclick=function() {deleteTabs(this.getAttribute('tab_id'))}	;
					remList.appendChild(rem_session);
					share_session.onclick=function() {shareTabs(this.getAttribute('tab_id'))}	;
					shareList.appendChild(share_session);
						
							}
				/*    var sepItem=mainWindow.createElement("menuseparator");
					var refreshItem=mainWindow.createElement("menuitem");
					refreshItem.setAttribute('label',"Refresh List");
					refreshItem.onclick=function() {refreshList();}
					 var sep2Item=mainWindow.createElement("menuseparator");
					var refresh2Item=mainWindow.createElement("menuitem");
					refresh2Item.setAttribute('label',"Refresh List");
					refresh2Item.onclick=function() {refreshList();}						
					  var sepItem2=mainWindow.createElement("menuseparator");
					var refreshItem2=mainWindow.createElement("menuitem");
					refreshItem2.setAttribute('label',"Refresh List");
					refreshItem2.onclick=function() {refreshList();}	
	               menuList.appendChild(sepItem);
	               remList.appendChild(sepItem2);
	               shareList.appendChild(sep2Item);
	               menuList.appendChild(refreshItem);
	               remList.appendChild(refreshItem2);
	               shareList.appendChild(refresh2Item);
	               */
	               
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
		
					
		//httpRequest.onreadystatechange = function() { showCode(httpRequest); };
			 

          httpRequest.open('GET','http://tabsender.appspot.com/?action=save&urls='+escape(output), false);
          httpRequest.send('');
        
	 if (httpRequest.readyState == 4) {
            if (httpRequest.status == 200) {
	         
               var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
              
               if (xmldoc.item(0).getAttribute("isOk") == "true" ) {
	               alert("Session saved");
	                refreshMenu();

	               } else {
		               alert("Please log in");
		               logAccount();
		               }
              
            } else {
                alert('There was a problem with the request.');
            }
        }
	          
        }
        
	    function showCode(httpRequest) {

        if (httpRequest.readyState == 4) {
            if (httpRequest.status == 200) {
	         
               var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
              
               if (xmldoc.item(0).getAttribute("isOk") == "true" ) {
	               alert("Session removed");
	                refreshMenu();
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
  
