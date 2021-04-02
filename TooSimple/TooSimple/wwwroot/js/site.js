// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.datepicker').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '30d'
});

//function updatePlaidAccounts() {
//    $.ajax({
//        url: $('#urlPlaidAccountUpdate').attr('href'),
//            type: 'POST',
//            success: function (response) {
//                $.each(response.transactions, function (i, item) {
//                    var $tr = $('<tr>').append(
//                        $('<td>').text(item.transactiondatedisplayvalue),
//                        $('<td>').text('<a href="~/Dashboard/LoadTransaction?id=' + item.transactionid + '"><div style="height:100%; width:100%;">' + item.name))
//                });
//            },
//            error: function () {
//                alert('This failed');
//            }
//    })
//}

//$(document).on('click', '#tableTransactions .pagerLink', function (e) {
//    e.preventDefault();

//    $.ajax({
//        url: $(this).attr('href'),
//        success: function (html) {
//            $('#tableTransactions').replaceWith(html);
//        },
//        error: function () {
//            alert('Something went wrong while performing the search.')
//        }
//    })

//    return false;
//})

