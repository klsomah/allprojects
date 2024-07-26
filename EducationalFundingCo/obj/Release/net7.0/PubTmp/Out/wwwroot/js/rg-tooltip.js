$(document).ready(function(){
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })  
});

(function($) {

    if (typeof $.fn.tooltip.Constructor === 'undefined') {
        throw new Error('Bootstrap Tooltip must be included first!');
    }

    var Tooltip = $.fn.tooltip.Constructor;

    // add skin option to Bootstrap Tooltip
    $.extend( Tooltip.Default, {
        skin: ''
    });

    var _show = Tooltip.prototype.show;

    Tooltip.prototype.show = function () {

        // invoke parent method
        _show.apply(this,Array.prototype.slice.apply(arguments));

        if ( this.config.skin ) {
            var tip = this.getTipElement();
            $(tip).addClass(this.config.skin);
        }

    };

})(window.jQuery);