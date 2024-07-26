
  $(document).ready(function(){
    $(".user-card li").click(function(){
      $(".user-card li").removeClass("active");
      $(this).addClass("active");
    });
  });