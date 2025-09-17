let currentPatientId = 0;

function confirmDelete(id) {
    currentPatientId = id;
    $('#deleteModal').modal('show');
}

$(document).ready(function () {
    // Handle Delete Confirmation Button Click
    $('#confirmDeleteBtn').click(function () {
        $.ajax({
            url: '@Url.Action("DeletePatient", "Patient")',
            type: 'POST',
            data: { id: currentPatientId },
            success: function (response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    $('#row_' + currentPatientId).remove();
                    alert(response.message);
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("An error occurred during deletion.");
            }
        });
    });
});