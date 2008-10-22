var docRef = document.frmEdit;
var gEditStatus = "edit";
var gCurrentPoll;
var gobjDatabaseDom;
var gobjDatabaseDomTree;
var sess;
var TestMode;
var maxid;
var contactNodeSet;
var firstrun=false;
var toggle=true;
var choicetoggle=false;
var deleted=false;
var num=0;
var override=false;
var maximum=0;
var curr=0;
function navigateUserList(direction) {
	
	var disabled=false;
		     switch (direction) {
        case "next":
        
		if(override||(document.getElementById("cmdMoveNext").disabled==false)){
           if(!deleted) cmdSaveClicked();
   if ((checkSaveStatus() == "OK_SAVE")||(deleted)){
	override=false;
            gCurrentPoll = gCurrentPoll.getNextSibling();
            
           } 
		   } else disabled=true;
            break;

        case "previous":
		if(document.getElementById("cmdMovePrevious").disabled==false){
        if(!deleted) cmdSaveClicked();
    if ((checkSaveStatus() == "OK_SAVE")||(deleted)){
            gCurrentPoll = gCurrentPoll.getPreviousSibling();
            
        }
		}else disabled=true;
            break;

        case "first":
		if(firstrun || (document.getElementById("cmdMoveFirst").disabled==false)){
       if(!deleted) if(!firstrun) cmdSaveClicked();
       if (deleted || (checkSaveStatus() == "OK_SAVE")||firstrun){  
	      
          contactNodeSet = gobjDatabaseDomTree.getChildNodes();
            gCurrentPoll = contactNodeSet.item(0);
        }
		}else disabled=true;
            break;

        case "last":
		if(document.getElementById("cmdMoveLast").disabled==false){
       if(!deleted)  cmdSaveClicked();
        if ((checkSaveStatus() == "OK_SAVE")||(deleted)){
            gCurrentPoll = contactNodeSet.item(contactNodeSet.getLength() -1);
        }
		}else disabled=true;
            break;

    } // end switch
if(!disabled){
	
    if ((gCurrentPoll != null)&&((checkSaveStatus() == "OK_SAVE")||firstrun||deleted) ) {
	    cmdToggleChoice();
	  
        displayUserData(gCurrentPoll);
        firstrun=false;
        cmdToggleChoice();
    }
deleted=false;
    //decide what to do with the navigation forms
    setNavigationButtonStateForEOFandBOF();
}
}
function displayUserData(user) {
	 
	clearform();
sess=user.getParentNode();
	 docRef.SessName.value=sess.getAttribute("name");
	if(sess.getAttribute("testMode")=="true"){
	  for (j=5;j>2;j--){
		  
	 docRef.MinScore.value=sess.getAttribute("minScore").substring(0,j);
	 if(sess.getAttribute("minScore").substring(j,j+1)!="0") break;
		
 }
}
	  if(sess.getAttribute("testMode")=="true") {
	    docRef.TestMode.checked=true;
	      document.getElementById("minscoretr").style.display.visible="";
	    TestMode=true;
    } else {
	     document.getElementById("minscoretr").style.display.visible="none";
	    docRef.TestMode.checked=false;
	    TestMode=false;
	    }
    docRef.PollName.value = user.getAttribute("name");
     docRef.PollDesc.value = user.getElementsByTagName("description").item(0).getFirstChild().getNodeValue();
    if(user.getAttribute("customChoiceEnabled")=="true") {
	    docRef.Custom.checked=true;
    } else {
	    docRef.Custom.checked=false;
	    }
    var length_ch = user.getElementsByTagName("choice").length;
    for (i=0;i<length_ch;i++){
	     var name_ch = user.getElementsByTagName("choice").item(i).getAttribute("name");
	    if(sess.getAttribute("testMode")=="true") {
var a=user.getElementsByTagName("choice").item(i).getAttribute("id")*1;
var b=user.getAttribute("correctChoice")*1;

		       if((a*1)==(b*1)) {
			     			       addRowToTable(name_ch,true,choicetoggle);
			     			       
			     
	     } else { 
		     addRowToTable(name_ch,false,choicetoggle);
	     }
 }   
  else { 
		     addRowToTable(name_ch,false,choicetoggle);
	     } 
	     
	    }
TestMode=!TestMode;
onTestMode();
checkvis();

if(deleted) {maximum--;
curr--;
} else {
	maximum=contactNodeSet.getLength();
		curr=0;
		for (i=0;i<maximum;i++){
	if (gCurrentPoll==contactNodeSet.item(i)) curr=i+1;
	}
	}
	document.getElementById("position").innerHTML=" "+curr+" of "+maximum+" ";
} // end function displayUserData
function formInit() {
    //first set up the database object. In this test case, I know I have data,
    var parser = new DOMImplementation();
    docRef = document.getElementById("frmEdit");
    gEditStatus = "edit";
 firstrun=false;
 toggle=true;
    gobjDatabaseDom = parser.loadXML(docRef.txtDatabase.value);
    gobjDatabaseDomTree = gobjDatabaseDom.getDocumentElement();
    
       document.getElementById("cmdAddNew").disabled = false;
  //  document.getElementById("cmdEdit").disabled = false;
    document.getElementById("cmdDelete").disabled = false;
   
		  firstrun=true;
   navigateUserList("first");

    // enable the add new button
 

} 
function addRowToTable(name_ch,checked_is,statetoggle)
{
	statetoggle=statetoggle ? "" : "none";
	state=true;
  var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
  // if there's no header row in the table, then iteration = lastRow + 1
  var iteration = lastRow-9;
  var row = tbl.insertRow(lastRow);

  // left cell
  row.disbaled=true;
  row.id="row"+iteration;
  row.deleted="not";
  row.style.display=statetoggle;
  if (navigator.appName=="Microsoft Internet Explorer");
  var cellLeft = row.insertCell(0);
  var el="";
  if (navigator.appName=="Microsoft Internet Explorer"){
  el = document.createElement("<input type=radio name=pollchoices>");
} else {
	el = document.createElement("input");
	
	}

  el.type = 'radio';
  el.correct=checked_is;
  el.checked=checked_is;
  el.name = 'pollchoices';
  el.id = 'choiceRadio' + iteration;
  el.disabled=!state;
  el.value=name_ch;
  el.size = 40;
   cellLeft.appendChild(el);

  
  // right cell
  var cellRight = row.insertCell(1);
  el = document.createElement('input');
  el.type = 'text';
  el.name = 'choice' + iteration;
  el.id = 'choiceRow' + iteration;
  el.disabled=!state;
  el.value=name_ch;
  //el.size = 4;
  cellRight.appendChild(el);
 
  // delete cell
 var cellDel = row.insertCell(2);
  el = document.createElement('img');
  el.src = '../images/but2.png';
  el.id = 'choiceAct' + iteration;
  el.disabled=!state;
  el.onclick=delRow;
  el.onmouseover=hoverButMinDynamic;
  el.onmouseout=outButMinDynamic;
   el.onmousedown=pressButMinDynamic;
  el.onmouseup=releaseButMinDynamic;
  el.value="Delete";
  el.size = 40;
  el.parentform="row"+iteration;
  el.radio='choiceRadio' + iteration;
  cellDel.appendChild(el);
if(checked_is) document.getElementById('choiceRadio' + iteration).checked=true;

 }
function clearform() {
    document.getElementById("PollName").value = "";
    document.getElementById("PollDesc").value = "";
   docRef.Custom.checked=false;
var lgh=document.getElementById('polltbl').rows.length-10;
 for(i=0;i<lgh;i++){
	 	 	 RemoveRow();
	 	 		 	 	
	 }

} 
function delRow(){
	if(document.getElementById(this.id).disabled!=true){
	document.getElementById(this.parentform).style.display='none';
	document.getElementById(this.parentform).deleted='none';
	var delChecked=document.getElementById(this.radio).checked;
	/*
	var numvis=0;
	  var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
	for (i=lastRow-9;i>0;i--){
		var chkd=true;
	if  (document.getElementById("row"+i).style.display!='none'){
	
		if((delChecked) && (chkd)){
			
			document.getElementById('choiceRadio' + i).checked=true;
			chkd=false;
			}
			numvis++;
		}
		
		
	}
	
	if((1*numvis)<3){
		
		for (i=1;i<lastRow-9;i++){
	   document.getElementById("choiceAct"+i).src="../images/but2d.png";
	   document.getElementById("choiceAct"+i).disabled=true;
	 
	  }		
		}*/
}
	 

	}
	function checkvis() {
	/*	
		var numvis=0;
	  var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
	for (i=lastRow-10;i>0;i--){

	if  (document.getElementById("row"+i).style.display!='none'){
					numvis++;
		}
		
		
	}
	
	if((1*numvis)<3){
		
		for (i=1;i<lastRow-9;i++){
	   document.getElementById("choiceAct"+i).src="../images/but2d.png";
	   document.getElementById("choiceAct"+i).disabled=true;
	 
	  }		
		}*/
}
		
		
		
function setNavigationButtonStateForEOFandBOF() {
    if (!gCurrentPoll.getPreviousSibling()) {
        //we're on the first record
        document.getElementById("cmdMoveFirst").disabled = true;
        document.getElementById("cmdMovePrevious").disabled = true;
        document.getElementById("cmdMoveFirst").src = "../images/cmdMoveFirstd.png";
        document.getElementById("cmdMovePrevious").src = "../images/cmdMovePreviousd.png";
        
    }
    else {
       document.getElementById("cmdMoveFirst").disabled = false;
        document.getElementById("cmdMovePrevious").disabled = false;
          document.getElementById("cmdMoveFirst").src = "../images/cmdMoveFirst.png";
        document.getElementById("cmdMovePrevious").src = "../images/cmdMovePrevious.png";
    }


    if (!gCurrentPoll.getNextSibling()) {
        //we're on the last record
       document.getElementById("cmdMoveLast").disabled = true;
       document.getElementById("cmdMoveNext").disabled = true;
             document.getElementById("cmdMoveLast").src = "../images/cmdMoveLastd.png";
        document.getElementById("cmdMoveNext").src = "../images/cmdMoveNextd.png";
    }
    else {
       document.getElementById("cmdMoveLast").disabled = false;
       document.getElementById("cmdMoveNext").disabled = false;
            document.getElementById("cmdMoveLast").src = "../images/cmdMoveLast.png";
        document.getElementById("cmdMoveNext").src = "../images/cmdMoveNext.png";
    }

} 
function RemoveRow(){
 obj=document.getElementById("polltbl");
obj.deleteRow(obj.rows.length-1);
}

function cmdEditClicked() {

    gSavedUserId = docRef.PollName.value;

    //setNavigationButtonsDisabledState(true);

    //setEditBoxDisabledState(false);

    docRef.cmdAddNew.disabled = true;
    docRef.cmdAddChoice.disabled = false;
    docRef.cmdEdit.disabled = true;
    docRef.cmdDelete.disabled = true;
    docRef.cmdSave.disabled = false;
    docRef.cmdCancel.disabled = false;

  
    docRef.PollName.focus();

    gEditStatus = "edit";
}
function setNavigationButtonsDisabledState (state) {
    docRef.cmdMoveFirst.disabled = state;
    docRef.cmdMovePrevious.disabled = state;
    docRef.cmdMoveNext.disabled = state;
    docRef.cmdMoveLast.disabled = state;

} 
function setEditBoxDisabledState(state) {
	docRef.PollDesc.disabled = state;
	docRef.MinScore.disabled = state;
	docRef.Custom.disabled = state;
	docRef.PollName.disabled = state;
    docRef.SessName.disabled = state;
        docRef.TestMode.disabled = state;
    var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
  for (i=1;i<lastRow-9;i++){
	  document.getElementById("choiceRow"+i).disabled=state;
	  document.getElementById("choiceRadio"+i).disabled=state;
	   document.getElementById("choiceAct"+i).disabled=state;
	  }

} 
function cmdCancelClicked() {

    setNavigationButtonsDisabledState(false);


    //setEditBoxDisabledState(true);

    clearform();


    if (contactNodeSet.length != 0 ) {
        displayUserData(gCurrentPoll);
    }


    docRef.cmdAddNew.disabled = false;
    docRef.cmdCancel.disabled = true;
    docRef.cmdSave.disabled = true;
    docRef.cmdAddChoice.disabled = true;

    if (contactNodeSet.getLength() == 0 ) {
        docRef.cmdEdit.disabled = true;
        docRef.cmdDelete.disabled = true;
    }
    else {
        docRef.cmdEdit.disabled = false;
        docRef.cmdDelete.disabled = false;
    }

    setNavigationButtonStateForEOFandBOF();
    
    if (contactNodeSet.getLength()< 1 ) {
	    document.getElementById("cmdSend").disabled=true;
	    }


} 
function addUserRowToTable(){

	addRowToTable("",false,choicetoggle);
	
	
	  var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
	
	
		for (i=1;i<lastRow-9;i++){
	   document.getElementById("choiceAct"+i).disabled=false;
	   document.getElementById("choiceAct"+i).src="../images/but2.png";
	  }		
		}
	
		function onTestMode(){
			TestMode=!TestMode;
			var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
			document.getElementById("Custom").checked=false;;
			document.getElementById("minscoretr").style.display=(TestMode) ? '' : "none";
			if(document.getElementById("MinScore").value=="") document.getElementById("MinScore").value="0.1";
			document.getElementById("Custom").disabled=TestMode;
			var need=false;
				 for (i=1;i<lastRow-9;i++){
		  document.getElementById("choiceRadio"+i).disabled=!TestMode;
		  		  if (document.getElementById("choiceRadio"+i).checked==true){
			  		 
			  need=true;
			  }
	
	  }
	  
	  if(!need){
		
		  
		
		  for (i=1;i<lastRow-9;i++){
		
			  if (document.getElementById("row"+i).style.display=="") {
				   document.getElementById("choiceRadio"+i).checked=true;
				  chkd=false;
			 
				  break;
				  }
			  
			  }
		  }		
			
			}
function cmdAddNewClicked() {
	cmdSaveClicked();
	if(checkSaveStatus() == "OK_SAVE"){
	gSavedUserId = docRef.PollName.value;

    //enable the edit boxes
    //setEditBoxDisabledState(false);

    //disable the navigation
    //setNavigationButtonsDisabledState(true);
    clearform();
    
    
addRowToTable("",true,choicetoggle);
addRowToTable("",false,choicetoggle);
addRowToTable("",false,choicetoggle);

TestMode=!TestMode;
onTestMode();
    //set the buttons to be what they need to be
    document.getElementById("pllname").style.display='';
            document.getElementById("polldescription").style.display='';
            document.getElementById("customch").style.display='';
            document.getElementById("correctc").style.display='';
            document.getElementById("cmdSend").disabled=false;;
    //docRef.cmdAddNew.disabled = true;
   //  docRef.cmdAddChoice.style.display = "none";
    //docRef.cmdEdit.disabled = true;
   // docRef.cmdDelete.disabled = true;
    //docRef.cmdSave.disabled = false;
    //docRef.cmdCancel.disabled = false;

    //set the focus to the first name
    //docRef.PollName.focus();

    gEditStatus = "new";
    var maximum=contactNodeSet.getLength();
var curr=0;
for (i=0;i<maximum;i++){
	if (gCurrentPoll==contactNodeSet.item(i)) curr=i+2;
	}

	maximum++;
	document.getElementById("position").innerHTML=" "+curr+" of "+maximum+" ";
	if (curr==2){
	 document.getElementById("cmdMoveFirst").disabled = false;
        document.getElementById("cmdMovePrevious").disabled = false;
          document.getElementById("cmdMoveFirst").src = "../images/cmdMoveFirst.png";
        document.getElementById("cmdMovePrevious").src = "../images/cmdMovePrevious.png";
    }
}

} // end function cmdAddNewClicked	
	
function cmdDeleteClicked() {
    var id = docRef.PollName.value;
    if (confirm("Are you sure you would like to delete the Poll\n" + docRef.PollName.value + "\n" + docRef.PollDesc.value + "?") == true ) {
        // get the previous user if not at the start of the list. Otherwise, get the next
deleted=true;
        clearform();
        var gDelPoll=gCurrentPoll;
        
var ctNodeSet = gobjDatabaseDomTree.getChildNodes();
var lastnode=false;
        //now set the buttons appropriately

        //add new will always be enabled
        docRef.cmdAddNew.disabled = false;

        //if there are users left, enable the edit or delete
        //and navigate to the correct user
        if (ctNodeSet.getLength()> 1 ) {
	        
            if (gCurrentPoll.getPreviousSibling() ) {
	            
                navigateUserList("previous");
            }
            else {
	            
	                           navigateUserList("next");
            }

            //docRef.cmdEdit.disabled = false;
            docRef.cmdDelete.disabled = false;
            
        }
        else {
            //the main node now will be the Poll node (which is really
            //gobjDatabaseDomTree) to allow me to
            //do a insertNodeInto when/if I do an add
         
            gCurrentPoll = gobjDatabaseDomTree;
            //docRef.cmdEdit.disabled = true;
            docRef.cmdDelete.disabled = true;
            //setNavigationButtonsDisabledState(true);
            lastnode=true;
         /*   document.getElementById("plnm").style.display='none';
            document.getElementById("pldsc").style.display='none';
            document.getElementById("cuc").style.display='none';
            document.getElementById("coc").style.display='none';
            document.getElementById("cmdSend").disabled=true;;
            */
            deleted=false;
        }

       // docRef.cmdCancel.disabled = true;
        //docRef.cmdSave.disabled = true;
    
       
gobjDatabaseDomTree.removeChild(gDelPoll);
if(!lastnode) setNavigationButtonStateForEOFandBOF();


     
    } // end confirming
} // end function cmdDeleteClicked
function cmdSaveClicked() {
	    var strOKSave = checkSaveStatus();
    if (strOKSave == "OK_SAVE") {
var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
        var pollName = docRef.PollName.value;
        var pollDesc = docRef.PollDesc.value;
        var customChoice=docRef.Custom.checked.toString();
        var choices= new Array();
        var correct_num=1;
        var n=0;
        for (i=1;i<lastRow-9;i++){
	        if((document.getElementById("row"+i).deleted!='none')){
		         n++;
	  choices[n]=document.getElementById("choiceRow"+i).value;
	    }
	  if(document.getElementById("choiceRadio"+i).checked==true){
		  correct_num=n	;
		  } 
		 
		  }

        if (gEditStatus == "edit" ) {
            //we're editing. Set the id for the user object
            id = gCurrentPoll.getAttribute("id");
        }
        else {
            //I need a new ID. The number of seconds since 1970 will do
            id = contactNodeSet.getLength()+1;
        }

        //now try to save it by constructing the node as necessary
        //the edit (replaceNodeContents doesn't need the open and close CONTACT tags,
        //in fact, if you put them there bad things happen. replaceNodeContents
        //maintains the CONTACT tag's attributes, so all we need to do
        //is replace the data inside.


        if (gEditStatus == "edit") {
          gCurrentPoll.getParentNode().setAttribute("name",document.getElementById("SessName").value);
gCurrentPoll.getParentNode().setAttribute("testMode",document.getElementById("TestMode").checked+"");
          gCurrentPoll.getParentNode().setAttribute("minScore",document.getElementById("MinScore").value+"");
	          gCurrentPoll.setAttribute("name",pollName);
     gCurrentPoll.getElementsByTagName("description").item(0).getFirstChild().setNodeValue(pollDesc);
     gCurrentPoll.setAttribute("customChoiceEnabled",""+customChoice);
     gCurrentPoll.removeChild(gCurrentPoll.getElementsByTagName("choices").item(0));
     gCurrentPoll.setAttribute("correctChoice",""+correct_num);
     var newChoicesNode = gobjDatabaseDom.createElement('choices');
     for(i=1;i<choices.length;i++){
	     var newChoiceNode = gobjDatabaseDom.createElement('choice');
	     newChoiceNode.setAttribute("id",i);
	     newChoiceNode.setAttribute("name",choices[i]);
	         newChoicesNode.appendChild(newChoiceNode);
	     }
	     gCurrentPoll.appendChild(newChoicesNode);
	     
       // navigateUserList("current");
        }
        else {
	        
	      gEditStatus = "edit";
	      deleted=true;
            // create new Poll Node
            var newPollNode = gobjDatabaseDom.createElement('poll');

            // create id attribute
            newPollNode.setAttribute('id', id);
             gCurrentPoll.getParentNode().setAttribute("name",document.getElementById("SessName").value);
gCurrentPoll.getParentNode().setAttribute("testMode",document.getElementById("TestMode").checked+"");
          gCurrentPoll.getParentNode().setAttribute("minScore",document.getElementById("MinScore").value+"");
          
	          newPollNode.setAttribute("name",pollName);
     newPollNode.setAttribute("customChoiceEnabled",""+customChoice);
     newPollNode.setAttribute("correctChoice",""+correct_num);

          
            var newDescNode = gobjDatabaseDom.createElement('description');
            newDescNode.appendChild(gobjDatabaseDom.createTextNode(pollDesc));
            newPollNode.appendChild(newDescNode);
var newChoicesNode = gobjDatabaseDom.createElement('choices');
     for(i=1;i<choices.length;i++){
	     var newChoiceNode = gobjDatabaseDom.createElement('choice');
	     newChoiceNode.setAttribute("id",i);
	     newChoiceNode.setAttribute("name",choices[i]);
	         newChoicesNode.appendChild(newChoiceNode);
	     }
            newPollNode.appendChild(newChoicesNode);

     
            if (contactNodeSet.getLength() == 0) {
   
                //nobody in the list.
                gobjDatabaseDomTree.appendChild(newPollNode);

                
                  gCurrentPoll.getParentNode().setAttribute("name",document.getElementById("SessName").value);
          gCurrentPoll.getParentNode().setAttribute("testMode",document.getElementById("TestMode").checked+"");
          gCurrentPoll.getParentNode().setAttribute("minScore",document.getElementById("MinScore").value+"");
            }
            else {
	            override=true;
	              gCurrentPoll.getParentNode().setAttribute("name",document.getElementById("SessName").value);
          gCurrentPoll.getParentNode().setAttribute("testMode",document.getElementById("TestMode").checked+"");
          gCurrentPoll.getParentNode().setAttribute("minScore",document.getElementById("MinScore").value+"");
                var nextSib = gCurrentPoll.getNextSibling();
                if (nextSib) {
	                
                  gobjDatabaseDomTree.insertBefore(newPollNode, nextSib);
                  
                  navigateUserList('next');
                  
                }
                else {
	                 
	                                 gobjDatabaseDomTree.appendChild(newPollNode);
	                                
	                                 navigateUserList('next');
	                                  
                }
                 
                 
            }
        }

        //disable the edit boxes
        //setEditBoxDisabledState(true);

        //enable the command buttons as appropriate
        docRef.cmdAddNew.disabled = false;
       // docRef.cmdEdit.disabled = false;
          //docRef.cmdAddChoice.disabled = true;
        docRef.cmdDelete.disabled = false;
        //docRef.cmdCancel.disabled = true;
        //docRef.cmdSave.disabled = true;
    }
    else {
        alert("I was unable to save.\nThe error message reported was:\n" + strOKSave);
       // this.src="../images/but.png";
    }

} // end function cmdSaveClicked
function checkSaveStatus() {
   
    var strRet = "OK_SAVE"; // assume things are OK

   
    if (docRef.PollName.value=="") {
        strRet = "Poll Name required";
        return strRet;
    }
     if (docRef.SessName.value=="") {
        strRet = "Session Name required";
        return strRet;
    }

    if((docRef.MinScore.value < 0)||(docRef.MinScore.value > 1)) {
	    strRet="Invalid minimal score (must be between 0 and 1)";
	    return strRet;
	    }
    if (docRef.PollDesc.value == "") {
        strRet = "Poll Description required";
        return strRet;
    }
    if (docRef.SessName.value == "") {
        strRet = "Session Name required";
        return strRet;
    }
    var num=false;
    var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
    for (i=1;i<lastRow-9;i++){
	        if((document.getElementById("choiceRow"+i).value=="")&&(document.getElementById("row"+i).deleted!='none')){
	        num=true;
		        }
	  	    }
	  	    if (num) strRet="Choices may not be empty"
	  	    num=0;
	  	     for (i=1;i<lastRow-9;i++) {
		  	     if(document.getElementById("row"+i).deleted!='none') {num++;
	  	     }
  	     }
	  	     if (num<2) strRet="unleast 2 choices needed";
		  	     num=false;
	  	     for (i=1;i<lastRow-9;i++){
		  	      if((document.getElementById("choiceRadio"+i).checked==true)&&(document.getElementById("row"+i).deleted!='none')) {
			  	      num=true;
		  	     }			  	     
	  	      }
	  	    
	  	      	  	     if((!num)&&(TestMode)) strRet="you have to select correct choice in test mode";
	  	    
    return strRet;

}
function cmdSaveSessionClicked(){
	cmdSaveClicked();
	if (checkSaveStatus() == "OK_SAVE"){
	document.getElementById("result").value=gCurrentPoll.getParentNode().getXML();
	document.getElementById("submitform").submit();
	
	

}
	}
//formInit();

function hoverButPlus(obj){
	document.getElementById(obj).src='../images/butp.png';
	
	}
	function outButPlus(obj){
	document.getElementById(obj).src='../images/but.png';
	
	}
	
	function hoverButMin(obj){
	document.getElementById(obj).src='../images/but2p.png';
	this.src='../images/but2p.png';
	}
	function outButMin(obj){
	document.getElementById(obj).src='../images/but2.png';
	this.src='../images/but2.png';
	}
	
		function hoverButMinDynamic(){
	if(document.getElementById(this.id).disabled!=true){
	this.src='../images/but2p.png';
	
}
	}
	function outButMinDynamic(){
if(document.getElementById(this.id).disabled!=true){
	this.src='../images/but2.png';
}
	}

	
function pressButPlus(obj){
	document.getElementById(obj).src='../images/butpr.png';
	
	}
	function releaseButPlus(obj){
	document.getElementById(obj).src='../images/butp.png';
	
	}
	
	function pressButMin(obj){
	document.getElementById(obj).src='../images/but2pr.png';
	
	}
	function releaseButMin(obj){
	document.getElementById(obj).src='../images/but2.png';
	
	}
	function pressButMinDynamic(obj){
		if(document.getElementById(this.id).disabled!=true){
this.src='../images/but2pr.png';

}
	
	}
	function releaseButMinDynamic(obj){
		if(document.getElementById(this.id).disabled!=true){
	this.src='../images/but2.png';
}
	
	}
	
	
	function navOver(obj){
		if(document.getElementById(obj).disabled!=true){
	document.getElementById(obj).src='../images/'+obj+'h.png';
}
		}
		
			function navOut(obj){
		if(document.getElementById(obj).disabled!=true){
	document.getElementById(obj).src='../images/'+obj+'.png';
}
		}
		
			function navDown(obj){
		if(document.getElementById(obj).disabled!=true){
	document.getElementById(obj).src='../images/'+obj+'p.png';
}
		}
			function navUp(obj){
		if(document.getElementById(obj).disabled!=true){
	document.getElementById(obj).src='../images/'+obj+'.png';
}
		}
		function cmdTogglePoll(){
			toggle=!toggle;
			state= toggle ? "" : "none";
		var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
       document.getElementById('pllname').style.display=state;
        document.getElementById('polldescription').style.display=state;
               document.getElementById('customch').style.display=state;
       document.getElementById('correctc').style.display="none";
         document.getElementById('navi').style.display=state;
           document.getElementById('adpolltosession').style.display=state;
            document.getElementById('cmdDelete').style.display=state;
             document.getElementById('cmdAddNew').style.display=state;
             document.getElementById('cmdAddChoice').style.display='none';
              for (i=1;i<lastRow-9;i++){
	        if((document.getElementById("row"+i).deleted!='none')){
		      
	  document.getElementById("row"+i).style.display='none';
	    }
			
			}
			choicetoggle=false;
		}
		function startedit(){
	formInit();
				cmdTogglePoll();
				
	}
	function cmdToggleChoice() {
		 var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
		choicetoggle=!choicetoggle;
			state= choicetoggle ? "" : "none";
			     for (i=1;i<lastRow-9;i++){
		 if((document.getElementById("row"+i).deleted!='none')){
		      
	  document.getElementById("row"+i).style.display=state;
	 
	    }
			
			}
		document.getElementById('cmdAddChoice').style.display=state;
		document.getElementById('correctc').style.display=state;
		}