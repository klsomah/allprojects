
$(document).ready(function(){
    $('.dropdown-menu a.dropdown-toggle').on('click', function(e) {
        if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass('show');
        }
        var $subMenu = $(this).next('.dropdown-menu');
        $subMenu.toggleClass('show');
        $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function(e) {
            $('.dropdown-submenu .show').removeClass('show');
        });
        return false;
    });
    
    $('.notification-toggle').click(function(){
        $('.notification-bar').toggleClass('d-none')
    })

    function highlightCurrentLink(){
        var rootElement=$("a.dropdown-item.active").parents("li.nav-item.dropdown");
        $(".dropdown-item.active").removeClass("active");
        $(rootElement).addClass("active");

        //Handle dashboard page
        var dashboardLink=$(".nav-link.active").parent();
        $(dashboardLink).addClass("active");
    }

    setTimeout(function(){
      highlightCurrentLink();  
    }, 100);
})