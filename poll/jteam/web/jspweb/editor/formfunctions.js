var docRef = document.frmEdit;
var gEditStatus = "";
var gCurrentPoll;
var gobjDatabaseDom;
var gobjDatabaseDomTree;
var sess;
var TestMode;
var maxid;
var contactNodeSet;
function navigateUserList(direction) {
		     switch (direction) {
        case "next":
     
    
            gCurrentPoll = gCurrentPoll.getNextSibling();
               
            break;

        case "previous":
            gCurrentPoll = gCurrentPoll.getPreviousSibling();
            break;

        case "first":
            contactNodeSet = gobjDatabaseDomTree.getChildNodes();
            gCurrentPoll = contactNodeSet.item(0);
            break;

        case "last":
            gCurrentPoll = contactNodeSet.item(contactNodeSet.getLength() -1);
            break;

    } // end switch


    if (gCurrentPoll != null ) {
        displayUserData(gCurrentPoll);
    }

    //decide what to do with the navigation forms
    setNavigationButtonStateForEOFandBOF();
}
function displayUserData(user) {
	clearform();
sess=user.getParentNode();
	 docRef.SessName.value=sess.getAttribute("name");
	 docRef.MinScore.value=sess.getAttribute("minScore");
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
alert(a+" "+b);
		       if(a==b) {
			       addRowToTable(name_ch,true,false);
	     } else { 
		     addRowToTable(name_ch,false,false);
	     }
 }   
  else { 
		     addRowToTable(name_ch,false,false);
	     } 
	     
	    }


} // end function displayUserData
function formInit() {
    //first set up the database object. In this test case, I know I have data,
    var parser = new DOMImplementation();
    gobjDatabaseDom = parser.loadXML(docRef.txtDatabase.value);
    gobjDatabaseDomTree = gobjDatabaseDom.getDocumentElement();
    
       document.getElementById("cmdAddNew").disabled = false;
    document.getElementById("cmdEdit").disabled = false;
    document.getElementById("cmdDelete").disabled = false;
    if(document.getElementById("sessiontype").value=="new"){
	    	    gCurrentPoll = gobjDatabaseDomTree;
	     setNavigationButtonsDisabledState(true);
            document.getElementById("plnm").style.display='none';
            document.getElementById("pldsc").style.display='none';
            document.getElementById("cuc").style.display='none';
            document.getElementById("coc").style.display='none';
            document.getElementById("cmdSend").disabled=true;;
             document.getElementById("cmdDelete").disabled=true;;
              document.getElementById("cmdEdit").disabled=true;;
	     contactNodeSet = gobjDatabaseDomTree.getChildNodes();
	    } else {
   navigateUserList("first");
}
    // enable the add new button
 

} 
function addRowToTable(name_ch,checked_is,state)
{
  var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
  // if there's no header row in the table, then iteration = lastRow + 1
  var iteration = lastRow-3;
  var row = tbl.insertRow(lastRow);
  
  // left cell
  row.disbaled=true;
  row.id="row"+iteration;
  var cellLeft = row.insertCell(0);
  var el = document.createElement('input');
  el.type = 'radio';
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
  //el.size = 40;
  cellRight.appendChild(el);
 
  // delete cell
  var cellDel = row.insertCell(2);
  el = document.createElement('input');
  el.type = 'button';
  el.id = 'choiceAct' + iteration;
  el.disabled=!state;
  el.onclick=delRow;
  el.value="Delete";
  el.size = 40;
  el.parentform="row"+iteration;
  el.radio='choiceRadio' + iteration;
  cellDel.appendChild(el);
 }
function clearform() {
    document.getElementById("PollName").value = "";
    document.getElementById("PollDesc").value = "";
   docRef.Custom.checked=false;
var lgh=document.getElementById('polltbl').rows.length-4;
 for(i=0;i<lgh;i++){
	 	 	 RemoveRow();
	 	 		 	 	
	 }

} 
function delRow(){
	document.getElementById(this.parentform).style.display='none';
	var delChecked=document.getElementById(this.radio).checked;
	
	var numvis=0;
	  var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
	for (i=lastRow-4;i>0;i--){
		
	if  (document.getElementById("row"+i).style.display!='none'){
		if(delChecked){
			document.getElementById('choiceRadio' + i).checked=true;
			
			}
			numvis++;
		}
		
		
	}
	
	if((1*numvis)<3){
		for (i=1;i<lastRow-3;i++){
	   document.getElementById("choiceAct"+i).disabled=true;
	  }		
		}
	
	 
	}
function setNavigationButtonStateForEOFandBOF() {
    if (!gCurrentPoll.getPreviousSibling()) {
        //we're on the first record
        document.getElementById("cmdMoveFirst").disabled = true;
        document.getElementById("cmdMovePrevious").disabled = true;
    }
    else {
       document.getElementById("cmdMoveFirst").disabled = false;
        document.getElementById("cmdMovePrevious").disabled = false;
    }


    if (!gCurrentPoll.getNextSibling()) {
        //we're on the last record
       document.getElementById("cmdMoveLast").disabled = true;
       document.getElementById("cmdMoveNext").disabled = true;
    }
    else {
       document.getElementById("cmdMoveLast").disabled = false;
       document.getElementById("cmdMoveNext").disabled = false;
    }

} 
function RemoveRow(){
 obj=document.getElementById("polltbl");
obj.deleteRow(obj.rows.length-1);
}

function cmdEditClicked() {

    gSavedUserId = docRef.PollName.value;

    setNavigationButtonsDisabledState(true);

    setEditBoxDisabledState(false);

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
  for (i=1;i<lastRow-3;i++){
	  document.getElementById("choiceRow"+i).disabled=state;
	  document.getElementById("choiceRadio"+i).disabled=state;
	   document.getElementById("choiceAct"+i).disabled=state;
	  }

} 
function cmdCancelClicked() {

    setNavigationButtonsDisabledState(false);


    setEditBoxDisabledState(true);

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
	addRowToTable("",false,true);
	
	  var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
	
	
		for (i=1;i<lastRow-3;i++){
	   document.getElementById("choiceAct"+i).disabled=false;
	  }		
		}
	
		function onTestMode(){
			TestMode=!TestMode;
			var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
			document.getElementById("Custom").checked=false;;
			document.getElementById("minscoretr").style.display=(TestMode) ? '' : "none";
			document.getElementById("Custom").disabled=!TestMode;
			var need=false;
				 for (i=1;i<lastRow-3;i++){
		  document.getElementById("choiceRadio"+i).disabled=!TestMode;
		  if (document.getElementById("choiceRadio"+i).checked=true){
			  need=true;
			  }
	
	  }
	  
	  if(!need){
		  for (i=1;i<lastRow-3;i++){
			  if (document.getElementById("choiceRadio"+i).style.display="") {
				  document.getElementById("choiceRadio"+i).checked=true;
				  break;
				  }
			  
			  }
		  }		
			
			}
function cmdAddNewClicked() {
	gSavedUserId = docRef.PollName.value;

    //enable the edit boxes
    setEditBoxDisabledState(false);

    //disable the navigation
    setNavigationButtonsDisabledState(true);
    clearform();
    
    
addRowToTable("",true,true);
addRowToTable("",false,true);
addRowToTable("",false,true);

TestMode=!TestMode;
onTestMode();
    //set the buttons to be what they need to be
    document.getElementById("plnm").style.display='';
            document.getElementById("pldsc").style.display='';
            document.getElementById("cuc").style.display='';
            document.getElementById("coc").style.display='';
            document.getElementById("cmdSend").disabled=false;;
    docRef.cmdAddNew.disabled = true;
     docRef.cmdAddChoice.disabled = false;
    docRef.cmdEdit.disabled = true;
    docRef.cmdDelete.disabled = true;
    docRef.cmdSave.disabled = false;
    docRef.cmdCancel.disabled = false;

    //set the focus to the first name
    docRef.PollName.focus();

    gEditStatus = "new";

} // end function cmdAddNewClicked	
	
function cmdDeleteClicked() {
    var id = docRef.PollName.value;
    if (confirm("Are you sure you would like to delete the Poll\n" + docRef.PollName.value + "\n" + docRef.PollDesc.value + "?") == true ) {
        // get the previous user if not at the start of the list. Otherwise, get the next

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

            docRef.cmdEdit.disabled = false;
            docRef.cmdDelete.disabled = false;
            
        }
        else {
            //the main node now will be the Poll node (which is really
            //gobjDatabaseDomTree) to allow me to
            //do a insertNodeInto when/if I do an add
         
            gCurrentPoll = gobjDatabaseDomTree;
            docRef.cmdEdit.disabled = true;
            docRef.cmdDelete.disabled = true;
            setNavigationButtonsDisabledState(true);
            lastnode=true;
            document.getElementById("plnm").style.display='none';
            document.getElementById("pldsc").style.display='none';
            document.getElementById("cuc").style.display='none';
            document.getElementById("coc").style.display='none';
            document.getElementById("cmdSend").disabled=true;;
            
        }

        docRef.cmdCancel.disabled = true;
        docRef.cmdSave.disabled = true;
    
       
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
        var correct_num;
        for (i=1;i<lastRow-3;i++){
	  choices[i]=document.getElementById("choiceRow"+i).value;
	  if(document.getElementById("choiceRadio"+i).checked==true){
		  correct_num=i;
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
          gCurrentPoll.getParentNode().setAttribute("testMode",TestMode+"");
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
          navigateUserList("current");
        }
        else {
	      
            // create new Poll Node
            var newPollNode = gobjDatabaseDom.createElement('poll');

            // create id attribute
            newPollNode.setAttribute('id', id);
     newPollNode.setAttribute("customChoiceEnabled",""+customChoice);
     newPollNode.setAttribute("correctChoice",""+correct_num);

          
            var newDescNode = gobjDatabaseDom.createElement('description');
            newDescNode.appendChild(gobjDatabaseDom.createTextNode(pollDesc));
            newPollNode.appendChild(newDescNode);
var newChoicesNode = gobjDatabaseDom.createElement('choices');
     for(i=0;i<choices.length;i++){
	     var newChoiceNode = gobjDatabaseDom.createElement('choice');
	     newChoiceNode.setAttribute("id",i);
	     newChoiceNode.setAttribute("name",choices[i]);
	         newChoicesNode.appendChild(newChoiceNode);
	     }
            newPollNode.appendChild(newChoicesNode);

        
            if (contactNodeSet.getLength() == 0) {

                //nobody in the list.
                gobjDatabaseDomTree.appendChild(newPollNode);

                navigateUserList("first");
                  gCurrentPoll.getParentNode().setAttribute("name",document.getElementById("SessName").value);
          gCurrentPoll.getParentNode().setAttribute("testMode",TestMode+"");
          gCurrentPoll.getParentNode().setAttribute("minScore",document.getElementById("MinScore").value+"");
            }
            else {
	              gCurrentPoll.getParentNode().setAttribute("name",document.getElementById("SessName").value);
          gCurrentPoll.getParentNode().setAttribute("testMode",document.getElementById("TestMode").checked+"");
          gCurrentPoll.getParentNode().setAttribute("minScore",document.getElementById("MinScore").value+"");
                var nextSib = gCurrentPoll.getNextSibling();
                if (nextSib) {
                  gobjDatabaseDomTree.insertBefore(newPollNode, nextSib);
                }
                else {
                  gobjDatabaseDomTree.appendChild(newPollNode);
                }
                navigateUserList("next");
            }
        }

        //disable the edit boxes
        setEditBoxDisabledState(true);

        //enable the command buttons as appropriate
        docRef.cmdAddNew.disabled = false;
        docRef.cmdEdit.disabled = false;
          docRef.cmdAddChoice.disabled = true;
        docRef.cmdDelete.disabled = false;
        docRef.cmdCancel.disabled = true;
        docRef.cmdSave.disabled = true;
    }
    else {
        alert("I was unable to save.\nThe error message reported was:\n" + strOKSave);
    }

} // end function cmdSaveClicked
function checkSaveStatus() {
   
    var strRet = "OK_SAVE"; // assume things are OK

   
    if (docRef.PollName.value=="") {
        strRet = "Poll Name required";
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
    return strRet;

}
function cmdSaveSessionClicked(){
	document.getElementById("result").value=gCurrentPoll.getParentNode().getXML();
	alert(gCurrentPoll.getParentNode().getXML());
	document.getElementById("submitform").submit();
	
	}
formInit();