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
	
	}

function register(){
	var login=document.getElementById('tabsender-login').value;
	var password=document.getElementById('tabsender-password').value;
	var httpRequest = new XMLHttpRequest();
	 httpRequest.overrideMimeType('text/xml');
	 httpRequest.onreadystatechange = function() { registerUser(httpRequest) };
        httpRequest.open('GET','http://emoforum.org/tabreg.php?user='+login+'&pass='+password, true);
        httpRequest.send('');
	}
function registerUser(httpRequest){
	        if (httpRequest.readyState == 4) {
			            if (httpRequest.status == 200) {
								var result=httpRequest.responseText;
								alert(result);
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
	var httpRequest = new XMLHttpRequest();
	 httpRequest.overrideMimeType('text/xml');
	 httpRequest.onreadystatechange = function() { loginUser(httpRequest) };
        httpRequest.open('GET','http://emoforum.org/tablog.php?user='+login+'&pass='+password, true);
        httpRequest.send('');
			
		
		
		}
		
		function loginUser(httpRequest){
	        if (httpRequest.readyState == 4) {
			            if (httpRequest.status == 200) {
								var result=httpRequest.responseText;
								alert(result);
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