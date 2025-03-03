$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable(
        {
            ajax: {
                url: '/admin/product/getall',
                dataSrc: 'data'
            },

            columns: [

                { data: 'title', width: '20%' },
                { data: 'isbn', width: '20%' },
                { data: 'price', width: '20%' },
                { data: 'author', width: '20%' },
                { data: 'category.categoryName', width: '20%' },
                {
                    data: 'id',
                    render: function (data) {
                        return `<div class="w-75 btn-group" role="group">
                                    <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                                    <a href="/Admin/Product/DeleteProduct/${data}" class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i></a>

                                </div>`
                    }
                }

            ]
        },

    );

    console.log(dataTable);

}
