$(document).ready(function () {
    var url = window.location.search;
    var url2 = new URLSearchParams(url);
    var status = url2.get('status');
    console.log(status);
    if (status == null) {
        loadDataTable(`?status=all`);
    }
    else {
        loadDataTable(`?status=${status}`);
    }
});

function loadDataTable(url) {
    dataTable = $('#tblOrder').DataTable(
        {
            ajax: {
                url: `/admin/order/getall${url}`,
                dataSrc: 'data'
            },

            columns: [

                { data: 'id', width: '20%' },
                { data: 'name', width: '20%' },
                { data: 'phoneNumber', width: '20%' },
                { data: 'applicationUser.email', width: '20%' },
                { data: 'orderStatus', width: '20%' },
                { data: 'orderTotal', width: '20%' },
                {
                    data: 'id',
                    render: function (data) {
                        return `<div class="w-75 btn-group" role="group">
                                    <a href="/Admin/Order/Details?orderId=${id}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                                </div>`
                    }
                }

            ],
        },

    );


}
