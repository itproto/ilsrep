var l=0;
function next(k){
if(k==max+1) document.polls.submit(); 
document.getElementById(k).style.display='';
if (k>0) document.getElementById(k-1).style.display='none';
if(k==max+1) document.polls.submit(); 
return k+1;
}
