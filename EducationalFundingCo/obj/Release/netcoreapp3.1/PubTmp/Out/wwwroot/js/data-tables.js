
$(document).ready(function() {
    renderAllDataTables();//For Data tables page
    userDataTable();
} );


function userDataTable(){
    rgRenderDataTable("#user-data-table");
}

function renderAllDataTables(){
    var listTableIds=["#simpleTable","#borderlessTable","#strippedTable","#highlightedTableHeader","#invertTable","#highlightedRowTable"];
    for(var i=0;i<listTableIds.length;i++){
        rgRenderDataTable(listTableIds[i]);
    }
}


