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


            ]
        },

    );

    console.log(dataTable);

}
