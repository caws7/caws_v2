$(function () {
    //Tooltip
    $('[data-toggle="tooltip"]').tooltip({
        container: 'body'
    });

    //Popover
    /*$('[data-toggle="popover"]').popover();*/
    $('[data-toggle="popover"]').popover({
        html: true,
        content: function () {
            return $('#popover-content').html();
        }
    });
})