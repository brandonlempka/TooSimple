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
//Checks device theme for dark mode
if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
    updateDarkMode(true);
}
//watches for changes in device theme
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
    const newColorScheme = e.matches ? true : false;
    updateDarkMode(newColorScheme);
});
//Updates dark mode nav header & links
function updateDarkMode(darkMode) {
    const nav = $('nav');
    const headerLinks = $('.nav-link');
    const footer = $('footer');
    const footerLinks = footer.find('a');
    if (darkMode) {
        nav.removeClass('bg-white');
        nav.removeClass('box-shadow');
        nav.removeClass('border-bottom');
        headerLinks.removeClass('text-dark');
        headerLinks.addClass('text-light');
        footer.removeClass('text-muted');
        footer.removeClass('border-top');
        footerLinks.removeClass('text-dark');
        footerLinks.addClass('text-light');
    }
    else {
        nav.addClass('bg-white');
        nav.addClass('box-shadow');
        nav.addClass('border-bottom');
        headerLinks.addClass('text-dark');
        headerLinks.removeClass('text-light');
        footer.addClass('text-muted');
        footer.addClass('border-top');
        footerLinks.addClass('text-dark');
        footerLinks.removeClass('text-light');
    }
}
//# sourceMappingURL=app.js.map