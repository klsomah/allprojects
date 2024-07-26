function rgRenderDataTable(element){
    $(element).DataTable( {
        responsive: true
    } ).columns.adjust();
    var selectElement= $(element).siblings(".dataTables_length").find("select");
    $(selectElement).addClass("form-control");
    $(element).siblings(".dataTables_length").find("label").html(selectElement);
    $(element).siblings(".dataTables_filter").html('<div class="form-group search-input">	<input type="text" class="form-control" placeholder="Search">	<button class="btn" type="button">		<span class="material-icons">search</span>	</button></div>');;       
}