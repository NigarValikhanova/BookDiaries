
$(document).ready(function () {
    GetCategory();
});

/* Read Data*/

function GetCategory() {
    $.ajax({
        url: '/admin/category/GetCategory',
        type: 'get',
        datatype: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            if (response == null || response == undefined || response.length == 0) {
                var object = '';
                object += '<tr>';
                object += '<td class="colspan=5">' + 'Category not available' + '</td>';
                $('#tblBody').html(object);
            }
            else {
                var object = '';
                $.each(response, function (index, item) {
                    object += '<tr>';
                    object += '<td>' + item.id + '</td>';
                    object += '<td>' + item.name + '</td>';
                    object += '<td> <a href="#" class="btn btn-primary btn-sm" onclick="Edit(' + item.id + ')"><i class="fa fa-edit"></i>Edit</a> <a href="#" class="btn btn-danger btn-sm" onclick="Delete(' + item.id + ')"><i class="fa fa-trash-o"></i>Delete</a></td>';
                });
                $('#tblBody').html(object);
            }
        },
        error: function () {
            alert('Unable to read the data.');
        }
    });
}


$('#btnAdd').click(function () {
    $('#CategoryModal').modal('show');
    $('#modalTitle').text('Add Category');
});

/*Insert Data*/
function Insert() {
    var formData = new Object();
    formData.id = $('#Id').val();
    formData.Name = $('#Name').val();

    $.ajax({
        url:'/admin/category/Insert',
        data:formData,
        type:'post',
        success: function (response) {
            if (response == null || response == undefined || response.length == 0) {
                alert('Unable to save the data.');
            }
            else {
                GetCategory();
                alert(response);
            }
        },
        error: function () {
            alert('Unable to save the data.');
        }
    });

}