
  $(document).ready(function(){

    var popupmodel=$("#exampleModal");
    $("#exampleModal").remove();
    $("body").append(popupmodel);

    // Email navigation active 
    $(".email-filters li").click(function(){
      $(".email-filters li").removeClass("active");
      $(this).addClass("active");
    });

     //Filter toggle for small screen
     $('.toggle-email-filters').on('click', function(){
            $('.email-filters').toggleClass('show');
        })
  });