/**
*
* -----------------------------------------------------------------------------
*
* Template : Echooling - Online Education HTML Template
* Author : reacthemes
* Author URI : https://reactheme.com/

* -----------------------------------------------------------------------------
*
**/

(function($) {
    "use strict";

    //Menu Code Here
    $("#backmenu").backResponsiveMenu({
        resizeWidth: '991', 
        animationSpeed: 'medium', 
        accoridonExpAll: false 
    });

    //Menu Active Here
    var path = window.location.href; 
    $('.react-menus li a').each(function() {
        if (this.href === path) {
            $(this).addClass('react-current-page');
        }
    });
    
    // Sticky Menu
    var header = $('.react-header');
    var win = $(window);
    win.on('scroll', function() {
        var scroll = win.scrollTop();
        if (scroll < 100) {
           header.removeClass("react-sticky");
        } else {
           header.addClass("react-sticky");
        }
    });

    /*-------------------------------------
        Parallax Sidebar
    -------------------------------------*/
    var back_parallax = $('.parallax');
    if(back_parallax.length){
        $('.parallax').parallax();
    }

    // Elements Animation
    if ($('.wow').length) {
        var wow = new WOW(
            {
                boxClass: 'wow', // animated element css class (default is wow)
                animateClass: 'animated', // animation css class (default is animated)
                offset: 0, // distance to the element when triggering the animation (default is 0)
                mobile: false, // trigger animations on mobile devices (default is true)
                live: true       // act on asynchronously loaded content (default is true)
            }
        );
        wow.init();
    }

    if ($('.counter').length) { 
        $('.counter').counterUp({
            delay: 10,
            time: 2000
        });
    }
    
    // magnificPopup init
     var imagepopup = $('.image-popup');
     if(imagepopup.length) {
         $('.image-popup').magnificPopup({
             type: 'image',
             callbacks: {
                 beforeOpen: function() {
                    this.st.image.markup = this.st.image.markup.replace('mfp-figure', 'mfp-figure animated zoomInDown');
                 }
             },
             gallery: {
                 enabled: true
             }
         });
     }


    //Taggle Js
    $('#menu-btn').on('click', function(e) {
        $(this).toggleClass("back__close");
        e.preventDefault();
    });

    // Home Slider
    if ($('.home-sliders').length) {
        $('.home-sliders').owlCarousel({
            loop:true,
            items:1,
            margin:0,
            autoplay:false,
            slideSpeed : 800,
            nav:true,
            dots:false,
            responsive:{
                0:{
                    dots:false,
                    nav:false,
                },
                768:{
                    dots:false,
                },
            }
        })
    }

    // Client Slider Part
    if ($('.client-slider').length) {
        $('.client-slider').owlCarousel({
            loop:true,
            items:1,
            margin:0,
            autoplay:false,
            slideSpeed : 300,
            nav:true,
            dots:false,
            center: false,
            responsive:{
                0:{
                    items:1,
                    center: false,
                },
                575:{
                    items:1,
                    center: false,
                },
                767:{
                    items:1,
                    center: false,
                },
                1200:{
                    items:1,
                },
            }
        })
    }

    // Client Slider Part
    if ($('.event-slider').length) {
        $('.event-slider').owlCarousel({
            loop:true,
            items:4,
            margin:25,
            autoplay:false,
            slideSpeed : 300,
            nav:false,
            dots:true,
            center: false,
            responsive:{
                0:{
                    items:1,
                    center: false,
                },
                575:{
                    items:1,
                    center: false,
                },
                767:{
                    items:2,
                    center: false,
                },
                1200:{
                    items:4,
                    dots:true,
                },
            }
        })
    }

    // Client Slider Part
    if ($('.feedreact-slider').length) {
        $('.feedreact-slider').owlCarousel({
            loop:true,
            items:3,
            margin:25,
            autoplay:false,
            slideSpeed : 300,
            nav:true,
            dots:false,
            center: false,
            responsive:{
                0:{
                    items:1,
                    center: false,
                },
                575:{
                    items:1,
                    center: false,
                },
                767:{
                    items:2,
                    center: false,
                },
                1200:{
                    items:3,
                    dots:false,
                },
            }
        })
    }

    // Client Slider Part
    if ($('#react-blog-slider').length) {
        $('#react-blog-slider').owlCarousel({
            loop:true,
            items:3,
            margin:20,
            autoplay:false,
            slideSpeed : 300,
            nav:false,
            dots:false,
            center: false,
            responsive:{
                0:{
                    items:1,
                    center: false,
                },
                575:{
                    items:1,
                    center: false,
                },
                767:{
                    items:2,
                    center: false,
                },
                1200:{
                    items:3,
                },
            }
        })
    }

    // Skill bar 
    var skillbar = $('.skillbars');
    if(skillbar.length) {
        $('.skillbars').skillBars({  
            from: 0,    
            speed: 4000,    
            interval: 100,  
            decimals: 0,    
        });
    }

    //filter js
    var pifilter = $('.react-grid');
    if(pifilter.length){
        $('.react-grid').imagesLoaded(function() {
            $('.react-filter').on('click', 'button', function() {
                var filterValue = $(this).attr('data-filter');
                $grid.isotope({
                    filter: filterValue
                });
            });
            var $grid = $('.react-grid').isotope({
                itemSelector: '.grid-item',
                percentPosition: true,
                masonry: {
                    columnWidth: '.grid-item',
                }
            });
        });
    }
    
    // portfolio Filter
    var filterbutton = $('.react-filter button');
      if(filterbutton.length){
        $('.react-filter button').on('click', function(event) {
          $(this).siblings('.active').removeClass('active');
          $(this).addClass('active');
          event.preventDefault();
        });
    }

    //Videos popup jQuery 
    var popup = $('.custom-popup');
    if(popup.length) {
        $('.custom-popup').magnificPopup({
            disableOn: 10,
            type: 'iframe',
            mainClass: 'mfp-fade',
            removalDelay: 160,
            preloader: false,
            fixedContentPos: false
        }); 
    }
    

    //preloader
    $(window).on( 'load', function() {
        $("#react__preloader").delay(1000).fadeOut(400);
        $("#react__preloader").delay(1000).fadeOut(400);
    })

    // scrollTop init
    var pitotop = $('#backscrollUp'); 
    if(pitotop.length){   
        win.on('scroll', function() {
            if (win.scrollTop() > 350) {
                pitotop.fadeIn();
            } else {
                pitotop.fadeOut();
            }
        });
        pitotop.on('click', function() {
            $("html,body").animate({
                scrollTop: 0
            }, 500)
        });
    }
    var lastScrollTop = 0;
    $(window).scroll(function(event){
       var st = $(this).scrollTop();
       if (st > lastScrollTop){
           $( "#backscrollUp" ).removeClass( "react__up___scroll" );
       } else {
          $( "#backscrollUp" ).addClass( "react__up___scroll" );
       }
       lastScrollTop = st;
    });

})(jQuery);