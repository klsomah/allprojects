
    $(document).ready(function() {
        // rg accordion initialization
        rg_accordion('#accordion1');
        $("#price-range").slider({ from: 1000, to: 100000, step: 500, smooth: true, round: 0, dimension: "&nbsp;$", skin: "plastic" });

        //Filter toggle for small screen
        $('.toggle-filters').on('click', function(){
            $('.card-filters').toggleClass('show');
        })
    });