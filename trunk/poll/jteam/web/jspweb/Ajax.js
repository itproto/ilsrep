 function sendResult(widget,url){
	 var choice=0;
	 rad=document.getElementById("polls").choice;
	 for(i=0;i<rad.length;i++){
		 if(rad[i].checked==true) choice=rad[i].value;
		  }
	 makeRequest(url+"/jspweb/widgetresult.jsp?widget="+widget+"&choice="+choice);
	 
	 
	 }
 function makeRequest(url) {
        var httpRequest;

        if (window.XMLHttpRequest) { // Mozilla, Safari, ...
            httpRequest = new XMLHttpRequest();
            if (httpRequest.overrideMimeType) {
                httpRequest.overrideMimeType('text/xml');
                // See note below about this line
            }
        } 
        else if (window.ActiveXObject) { // IE
            try {
                httpRequest = new ActiveXObject("Msxml2.XMLHTTP");
            } 
            catch (e) {
                try {
                    httpRequest = new ActiveXObject("Microsoft.XMLHTTP");
                } 
                catch (e) {}
            }
        }

        if (!httpRequest) {
            alert('Giving up :( Cannot create an XMLHTTP instance');
            return false;
        }
        httpRequest.onreadystatechange = function() { alertContents(httpRequest); };
        httpRequest.open('GET', url, true);
        httpRequest.send('');

    }
    function alertContents(httpRequest) {

        if (httpRequest.readyState == 4) {
            if (httpRequest.status == 200) {
                plength=document.getElementById("polls").choice.length;
                var xmldoc = httpRequest.responseXML.getElementsByTagName('result');
                for(i=0;i<rad.length;i++){
	         document.getElementById("result"+i).childNodes[0].childNodes[0].style.width=xmldoc.item(i).getAttribute("percent")+"%";
	        document.getElementById("result"+i).childNodes[1].innerHTML=xmldoc.item(i).getAttribute("votes"); 
	            $("#widget_poll").hide("slow");
$(".empty").animate({
	width: ($(".empty").width()+160)+"px"},1500);
            }
	            
            } else {
                alert('There was a problem with the request.');
            }
        }

    }

