let deleteId = 0;

function confirmDelete(id) {
    deleteId = id;
    $("#deleteModal").modal("show");
}

$(document).ready(function () {
    $("#confirmDeleteBtn").click(function () {
        $.ajax({
            url: "/Admin/Patient/DeletePatient",
            type: "POST",
            data: { id: deleteId },
            success: function (res) {
                if (res.success) {
                    $("#row_" + deleteId).remove();
                    $("#deleteModal").modal("hide");
                }
            }
        });
    });
});
