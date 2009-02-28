function loadData(){
document.getElementById('tabsender-login').value=document.getElementById('tabsender-login').getAttribute('savedValue');
document.getElementById('tabsender-password').value=document.getElementById('tabsender-password').getAttribute('savedValue');
if(document.getElementById('tabsender-server').hasAttribute('savedValue')){
document.getElementById('tabsender-server').value=document.getElementById('tabsender-server').getAttribute('savedValue');
}


	}
function saveData(){

document.getElementById('tabsender-login').setAttribute('savedValue',document.getElementById('tabsender-login').value);
document.persist('tabsender-login','savedValue');
document.getElementById('tabsender-password').setAttribute('savedValue',document.getElementById('tabsender-password').value);
document.persist('tabsender-password','savedValue');
document.getElementById('tabsender-server').setAttribute('savedValue',document.getElementById('tabsender-server').value);
document.persist('tabsender-server','savedValue');
		var prefManager = Components.classes["@mozilla.org/preferences-service;1"].getService(Components.interfaces.nsIPrefBranch);
prefManager.setCharPref("extensions.tabSender.user", document.getElementById('tabsender-login').value);
prefManager.setCharPref("extensions.tabSender.password", document.getElementById('tabsender-password').value);
prefManager.setCharPref("extensions.tabSender.server", document.getElementById('tabsender-server').value);
	}

function register(){
	var login=document.getElementById('tabsender-login').value;
	var password=document.getElementById('tabsender-password').value;
	var server=document.getElementById('tabsender-server').value;
	var httpRequest = new XMLHttpRequest();
	 httpRequest.overrideMimeType('text/xml');
	 httpRequest.onreadystatechange = function() { registerUser(httpRequest) };
	
        httpRequest.open('GET',server+'/senderServer/tabServer?action=register&username='+login+'&password='+password, true);
        httpRequest.send('');
	}
function registerUser(httpRequest){
	        if (httpRequest.readyState == 4) {
			            if (httpRequest.status == 200) {
								var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
								var result=xmldoc.item(0).getAttribute('isOk');
								if(result=="true"){
										alert('registered and logged in');
										window.close();
		
								} else {
										alert('Username exists, try again');
										}
			
	 					} else {
                				alert('There was a problem with the request.');
        						}
        
        }
            
	}
	function login(){
		var login=document.getElementById('tabsender-login').value;
	var password=document.getElementById('tabsender-password').value;
	var server=document.getElementById('tabsender-server').value;
	var httpRequest = new XMLHttpRequest();
	 httpRequest.overrideMimeType('text/xml');
	 httpRequest.onreadystatechange = function() { loginUser(httpRequest) };
	 
          httpRequest.open('GET',server+'/senderServer/tabServer?action=login&username='+login+'&password='+password, true);
        httpRequest.send('');
			
		
		
		}
		
		function loginUser(httpRequest){
	        if (httpRequest.readyState == 4) {
			            if (httpRequest.status == 200) {
								
								var xmldoc = httpRequest.responseXML.getElementsByTagName('response');
								var result=xmldoc.item(0).getAttribute('isOk');
								if(result=="true"){
										alert('logged in');
										window.close();
		
								} else {
										alert('Wrong login, try again');
										}
			
	 					} else {
                				alert('There was a problem with the request.');
        						}
        
        }
            
	}
	
	