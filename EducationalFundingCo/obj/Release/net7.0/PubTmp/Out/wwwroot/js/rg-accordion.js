function rg_accordion_toggle_collapse(element) {
    $(element+" .card-header button[aria-expanded='true'] .icon-expand").show();
    $(element+" .card-header button[aria-expanded='true'] .icon-collapse").hide();

    $(element+" .card-header button[aria-expanded='false'] .icon-expand").hide();
    $(element+" .card-header button[aria-expanded='false'] .icon-collapse").show();
}

function rg_accordion(element) {
    rg_accordion_toggle_collapse(element);

    $(element).on('hidden.bs.collapse', function() {
        rg_accordion_toggle_collapse(element);
    })
    $(element).on('shown.bs.collapse', function() {
        rg_accordion_toggle_collapse(element);
    })
}