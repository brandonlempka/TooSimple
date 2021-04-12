$(document).on('click', '#tableTransactions .pagerLink', function (e) {
    e.preventDefault();
    $.ajax({
        url: $(this).attr('href'),
        success: function (html) {
            $('#tableTransactions').replaceWith(html);
        },
        error: function () {
            alert('Something went wrong while performing the search.');
        }
    });
    return false;
});
//Call to update plaid data in background, enabling user to use site normally while data loads
function updatePlaidAccounts() {
    $.ajax({
        url: $('#urlPlaidAccountUpdate').attr('href'),
        type: 'POST',
        success: function (response) {
            if (response.success) {
                $('#alertRefresh').removeClass('d-none');
                $('#alertUpdating').addClass('d-none');
            }
            else {
                $('#alertUpdating').addClass('d-none');
                $('#alertRefresh').removeClass('d-none');
                $('#alertRefresh').removeClass('alert-secondary');
                $('#alertRefresh').addClass('alert-danger');
                $('#alertRefreshHeader').html('Something went wrong while refreshing accounts');
                $('#lblRefresh').html(response.errorMessage);
            }
        },
        error: function (response) {
            $('#alertUpdating').addClass('d-none');
            $('#alertRefresh').removeClass('d-none');
            $('#alertRefresh').removeClass('alert-secondary');
            $('#alertRefresh').addClass('alert-danger');
            $('#alertRefreshHeader').html('Something went wrong while refreshing accounts');
            $('#lblRefresh').html(response.responseText);
        }
    });
}
//# sourceMappingURL=app.js.map