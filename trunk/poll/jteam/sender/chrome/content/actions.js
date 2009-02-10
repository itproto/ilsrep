function register(){
	var login=document.getElementById('tabsender-login').value;
	var password=document.getElementById('tabsender-login').value;
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