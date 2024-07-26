function allowDrop(ev){    
    ev.preventDefault();
  }

  function drag(ev){
    if(ev.target.tagName==="IMG"){
      ev.dataTransfer.setData("text",ev.target.parentNode.id);
    }
    else
      ev.dataTransfer.setData("text",ev.target.id);
  }

  function drop(ev){
    ev.preventDefault();
    var id=ev.dataTransfer.getData("text");
    
    var card=$(ev.target).closest(".rg-card-action.card")[0];
    if(card){
      $("#"+id).insertAfter("#"+card.id);
    }
    else{
      var closest=$(ev.target).closest(".bucket .kanban-container")[0];
      if(closest)
          closest.appendChild(document.getElementById(id));
    }

  }

  $(document).ready(function () {
    // const ps = new PerfectScrollbar('.kanban');
  });    