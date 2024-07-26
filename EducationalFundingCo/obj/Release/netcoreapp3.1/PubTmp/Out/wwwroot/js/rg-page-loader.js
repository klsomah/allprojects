 document.addEventListener('DOMContentLoaded', OnDomContentLoaded);

function OnDomContentLoaded(){
  document.querySelector("body").prepend(createElementFromHTML('<div id="page-loader"> <div class="conent">  <div class="rg-spinner-wave-circle info extra-large mr-5"> <div class="rg-wave-bounce-1"></div> <div class="rg-wave-bounce-2"></div><div class="rg-wave-bounce-4"></div> </div> </div> </div>'));
}

function createElementFromHTML(htmlString) {   
  var div = document.createElement('div');   
  div.innerHTML = htmlString.trim();    
  return div.firstChild; 
}

window.onload = function(){
  document.getElementById("page-loader").style.display = "none";
};