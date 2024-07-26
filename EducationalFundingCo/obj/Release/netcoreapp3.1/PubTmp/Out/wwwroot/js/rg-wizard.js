function showTab(element) {
    $(element).find(".rg-tab").hide();
    $(element).find(".rg-tab.active").show();
}

$.fn.rgWizard = function() {
    var listElements = $(this);
    
    for (var i = 0; i < listElements.length; i++) {
        var element = listElements[i];
        showTab(element);
        var ul = element.children[0];

        var listLiElements = ul.children;
        for (var j = 0; j < listLiElements.length; j++) {
            var li = listLiElements[j];
            var span = li.children[1] || li.children[0];
            span.addEventListener("click", function() {

                var clickedElement = $(this);
                $(this).parent().parent().siblings().removeClass("active");
                var tab = clickedElement[0].attributes.tab.nodeValue;
                $("#" + tab).addClass("active");
                $(this).parent().parent().children('li').removeClass('active');
                $(this).parent().addClass('active');
                showTab(element);
            });
        }
    }
}

$(document).ready(function() {
    $(".rg-form-wizard").rgWizard();
});