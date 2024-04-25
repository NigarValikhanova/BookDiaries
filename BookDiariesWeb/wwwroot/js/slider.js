var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/slider/getall' },
        "columns": [
            {
                "data": null,
                "render": function (data, type, row, meta) {
                    return meta.row + 1; // Add 1 to start numbering from 1
                },
                "width": "10%"
            },
            { "data": 'name', "width": "30%" },
            { "data": 'startingPrice', "width": "15%" },
            { "data": 'discountPercent', "width": "15%" },
            {
                "data": 'isUsed',
                "render": function (data) {
                    return data ? 'Yes' : 'No';
                },
                "width": "10%"
            },
            {
                "data": 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/slider/upsert?id=${data}" class="btn btn-success mx-2"><i class="fa fa-edit"></i>Edit</a>
                    <a onClick=Delete('/admin/slider/delete/${data}') class="btn btn-danger mx-2"><i class="fa fa-trash-o"></i> Delete</a>
                    </div>`
                },
                "width": "15%"
            }
        ]
    });
}


function Delete(url) {
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
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
           
        }
    });
}