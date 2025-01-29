$(document).ready(function () {
    $(".delete-link").click(function (e) {
        e.preventDefault();

        var id = $(this).attr("data-id");

        $.ajax({
            url: deleteUrl,
            type: 'POST',
            data: { Id: id },
            success: function (response) {
                if (response.success) {
                    $('#row-' + id).remove();
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                alert('An error occurred: ' + error + ' (code: ' + status + ')');
            }
        });
    });

    $(".edit-link").click(function (e) {
        e.preventDefault();

        var id = $(this).attr("data-id");

        $.ajax({
            url: editUserUrl,
            type: 'GET',
            data: { id: id },
            success: function (data) {
                $('#editModal .modal-content').html(data);
                $('#editModal').modal('show');
            },
            error: function (xhr, status, error) {
                alert('An error occurred: ' + error + ' (code: ' + status + ')');
            }
        });
    });

    $(document).on("submit", "#editUserForm", function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: editUserUrl,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    $('#editModal').modal('hide');
                    location.reload(); 
                } else {
                    alert('Error saving data: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                alert('An error occurred: ' + error + ' (code: ' + status + ')');
            }
        });
    });
});
