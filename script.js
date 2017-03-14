$('.removeFromCart').click(function () {
    var albumId = $(this).attr("data-id");
    var albumTitle = $(this).closest('tr').find('td:first-child > a').html();
    var $cartNav = $('#navlist').find('a[href="/cart"]');
    var count = parseInt($cartNav.html().match(/\d+/));

    $.post("/cart/remove/" + albumId, function (data) {
        $('#main').html(data);
        $('#update-message').html(albumTitle + ' has been removed from your shopping cart.');
        $cartNav.html('Cart (' + (count - 1) + ')');
    });
});