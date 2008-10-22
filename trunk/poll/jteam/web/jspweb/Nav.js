	
	function navOver(obj){
		if(document.getElementById(obj).disabled!=true){
	document.getElementById(obj).src='./images/'+obj+'h.png';
	
}
		}
		
			function navOut(obj){
		if(document.getElementById(obj).disabled!=true){
	document.getElementById(obj).src='./images/'+obj+'.png';
}
		}
		
			function navDown(obj){
		if(document.getElementById(obj).disabled!=true){
	document.getElementById(obj).src='./images/'+obj+'p.png';
}
		}
			function navUp(obj){
		if(document.getElementById(obj).disabled!=true){
	document.getElementById(obj).src='./images/'+obj+'.png';
}
		}