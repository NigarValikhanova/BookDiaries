var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            {
                "data": null,
                "render": function (data, type, row, meta) {
                    return meta.row + 1; // Add 1 to start numbering from 1
                },
                "width": "3%"
            },
            { "data": 'title', "width": "20%" },
            { "data": 'isbn', "width": "10%" },
            { "data": 'price', "width": "5%" },
            { "data": 'author.name', "width": "10%" },
            { "data": 'category.name', "width": "10%" },
            { "data": 'language.name', "width": "10%" },
            { "data": 'stockQuantity', "width": "10%"},
            {
                "data": 'isBestSeller',
                "render": function (data) {
                    return data ? 'Yes' : 'No';
                },
                "width": "5%"
            },
            {
                "data": 'isDealOfTheDay',
                "render": function (data) {
                    return data ? 'Yes' : 'No';
                },
                "width": "15%"
            },
            {
                "data": 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/product/upsert?id=${data}" class="btn btn-success mx-2"><i class="fa fa-edit"></i>Edit</a>
                    <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2"><i class="fa fa-trash-o"></i> Delete</a>
                    </div>`
                },
                "width": "7%"
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