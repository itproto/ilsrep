var docRef = document.frmEdit;
var gEditStatus = "";
var gCurrentPoll;
var gobjDatabaseDom;
var gobjDatabaseDomTree;
var sess;
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
	  if(sess.getAttribute("testMode")=="true") {
	    docRef.TestMode.checked=true;
    } else {
	    docRef.TestMode.checked=false;
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
		       if(a==b) {
			       addRowToTable(name_ch,true);
	     } else { 
		     addRowToTable(name_ch,false);
	     }
 }   
  else { 
		     addRowToTable(name_ch,false);
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
   navigateUserList("first");
    // enable the add new button
 

} 
function addRowToTable(name_ch,checked_is)
{
  var tbl = document.getElementById('polltbl');
  var lastRow = tbl.rows.length;
  // if there's no header row in the table, then iteration = lastRow + 1
  var iteration = lastRow-3;
  var row = tbl.insertRow(lastRow);
  
  // left cell
  var cellLeft = row.insertCell(0);
  var el = document.createElement('input');
  el.type = 'radio';
  el.checked=checked_is;
  el.name = 'pollchoices';
  el.id = 'choiceRow' + iteration;
  el.disabled=true;
  el.value=name_ch;
  el.size = 40;
  cellLeft.appendChild(el);
  
  
  // right cell
  var cellRight = row.insertCell(1);
  el = document.createElement('input');
  el.type = 'text';
  el.name = 'choice' + iteration;
  el.id = 'choiceRow' + iteration;
  el.disabled=true;
  el.value=name_ch;
  el.size = 40;
  cellRight.appendChild(el);
 
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
formInit();