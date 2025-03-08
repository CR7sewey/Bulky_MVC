$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblDataCompany').DataTable(
        {
            ajax: {
                url: '/admin/company/get',
                dataSrc: 'data'
            },

            columns: [

                { data: 'name', width: '20%' },
                { data: 'phoneNumber', width: '20%' },
                { data: 'streetAddress', width: '20%' },
                { data: 'city', width: '20%' },
                { data: 'state', width: '20%' },
                {
                    data: 'id',
                    render: function (data) {
                        return `<div class="w-75 btn-group" role="group">
                                    <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                                    <a onClick=Delete('/Admin/Company/DeleteCompany/${data}',dataTable) class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i></a>

                                </div>`
                    }
                }
                
            ]
        },

    );


}


function Delete(url, dataTable) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        Swal.fire(
                            "Deleted!",
                            "Your file has been deleted.",
                            "success"
                        );
                        dataTable.ajax.reload();
                    } else {
                        Swal.fire(
                            "Error!",
                            "There was an error deleting the file.",
                            "error"
                        );
                    }
                }
            });
        }
    });
}
