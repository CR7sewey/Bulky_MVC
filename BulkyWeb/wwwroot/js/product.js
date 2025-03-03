$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable(
        {
            ajax: { url: '/admin/product/getall' },

            columns: [

                { data: 'title', width: '20%' },
                { data: 'isbn', width: '20%' },
                { data: 'price', width: '20%' },
                { data: 'author', width: '20%' },
                { data: 'category.categoryName', width: '20%' },


            ]
        },

    );

}

