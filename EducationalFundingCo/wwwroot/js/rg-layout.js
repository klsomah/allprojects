$(document).ready(function () {
    var windowOriginalWidth;
    windowOriginalWidth=window.innerWidth;
  
    if(window.innerWidth<constant.mobileWidth)
        $('body main').addClass('rg-collapse');
  
  
    $(window).resize(function() { 
      if(window.innerWidth!==windowOriginalWidth){
        reloadComponents();
      }    
    });
    
    function reloadComponents(){    
      location.reload();
    }
  
    var expandMenuClass="expand-options";
    var expandMenuClassSelector="."+expandMenuClass;
    var slideSpeed="normal";
    var sidenavCollapsibleActive='main-nav-collapsible-active';
    var expandedMenus;
  
    function hideSeparator(){
      $(".main-navigation .more_horiz").hide();
    }
    function showSeparator(){
      $(".main-navigation .more_horiz").show();
    }
    
    hideSeparator();
  
    $('.rg-toggle-button').on('click', function () {
      $('body > main').toggleClass('rg-collapse');
    })
  
    function openLastStateMenu(){
      if(!expandedMenus||expandedMenus.length==0)
        return;
  
      for(var i=0;i<expandedMenus.length;i++){
        var element=$(expandedMenus[i]);      
        openMenu(expandedMenus[i]);
      }
    }
  
    $("main aside").mouseenter(function(){
      if($('main').hasClass("rg-collapse")){
        hideSeparator();
        openLastStateMenu();
      }
    });
    
    $("main aside").mouseleave(function(){
      if($('main').hasClass("rg-collapse")){
        expandedMenus=$(expandMenuClassSelector).prev();
        closeAllOpenedMenu();
        showSeparator();
      }      
    });
  
    // var ps_theme_customiser = new PerfectScrollbar(".main-navigation ul", {
    //   suppressScrollX: true
    // });
    $('#pills-tab a').on('click', function (e) {
      e.preventDefault()
      $(this).tab('show')
    });
    
    function isDescendentOfAnyOpenedMenu(e){
      if($(e.target).parent().parent().is(expandMenuClassSelector))
        return true;
  
      return false;      
    }
  
    function closeAllOpenedMenu(){
      $(expandMenuClassSelector).parent().parent().removeClass(sidenavCollapsibleActive);
  
      $(expandMenuClassSelector).prev().children(":nth-child(3)").html("keyboard_arrow_right");
      $(expandMenuClassSelector).slideUp(slideSpeed);
      $(expandMenuClassSelector).removeClass(expandMenuClass);
      
    }
  
    function closeMenu(ulElement){
      $(ulElement).prev().children(":nth-child(3)").html("keyboard_arrow_right");
      $(ulElement).slideUp(slideSpeed);
      $(ulElement).removeClass(expandMenuClass);
      $(ulElement).parent().parent().removeClass(sidenavCollapsibleActive);
    }
  
    function closeSiblingMenu(e){
      var siblings=$(e.target).parent().siblings();
      for(var i=0;i<siblings.length;i++){
        var element=siblings[i];
        if(element.children[1])
          closeMenu(element.children[1]);
      }
    }
  
    function changeArrow(element,open){
      var arrow=$(element).children(":nth-child(3)");
      open?arrow.html("keyboard_arrow_down"):arrow.html("keyboard_arrow_right");
    }
  
    function openMenu(element){
      var sibling=$(element).siblings();
      if (sibling.is(":visible")) {
        $(element).next().removeClass(expandMenuClass);
        $(element).parent().parent().removeClass(sidenavCollapsibleActive);
        sibling.slideUp(slideSpeed);  
        changeArrow(element,false);
        return;
      }
      $(element).next().addClass(expandMenuClass);
      $(element).parent().parent().addClass(sidenavCollapsibleActive);
      sibling.slideDown(slideSpeed);
      changeArrow(element,true);
    }
  
    function expandMenu(e){    
      if(!isDescendentOfAnyOpenedMenu(e))
        closeAllOpenedMenu();    
  
      closeSiblingMenu(e);
  
      console.dir($(e.target))
      openMenu($(e.target));
    }
  
    $('.main-nav-collapsible li a').click(expandMenu);
  
    $('.notification-toggle').click(function(){
      $('.notification-bar').toggleClass('d-none')
    })

    if($('main > aside .main-navigation').length>0)
      new PerfectScrollbar('main > aside .main-navigation');

      

      function highlightCurrentLink(){
        var allULs=$("a.active").parents("ul.main-nav-collapsible > li");
        for(var i=allULs.length-1;i>0;i--){
        var ul=allULs[i];
        $(ul).children("a").click();
        }
    }

    setTimeout(function(){
      highlightCurrentLink();  
    }, 100);

  });


 