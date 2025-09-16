$(document).ready(function () {
    const form = $("#patientForm");

    // --- Save/Update Button ---
    $(document).on("click", "#btnSave", function (e) {
        e.preventDefault();

        // Clear previous errors
        form.find(".is-invalid").removeClass("is-invalid");

        let firstInvalid = null;
        let isValid = true;

        // Validate required inputs
        form.find("input[required]").each(function () {
            if (!$(this).val().trim()) {
                $(this).addClass("is-invalid");
                if (!firstInvalid) firstInvalid = this;
                isValid = false;
            }
        });

        // Validate required inputs
        form.find("input[required]").each(function () {
            let val = $(this).val().trim();

            // If empty OR (if numeric and equals 0)
            if (!val || ($(this).attr("id") === "Dosage" && parseFloat(val) === 0)) {
                $(this).addClass("is-invalid");
                if (!firstInvalid) firstInvalid = this;
                isValid = false;
            }
        });

        // If invalid, focus first invalid field
        if (!isValid) {
            if (firstInvalid) firstInvalid.focus();
            return;
        }

        // If valid, show confirmation
        openConfirmModal({
            title: $(this).data("modal-title"),
            message: $(this).data("modal-body"),
            confirmButtonText: $(this).data("modal-confirm-text"),
            confirmButtonClass: $(this).data("modal-confirm-class"),
            onConfirm: function () {
                form.submit();
            }
        });
    });

    // Remove error class on typing
    $(document).on("input", "input", function () {
        $(this).removeClass("is-invalid");
    });

    // --- Delete Button ---
    $(document).on("click", ".btn-delete", function () {
        let drugId = $(this).data("id");

        openConfirmModal({
            title: $(this).data("modal-title"),
            message: $(this).data("modal-body"),
            confirmButtonText: $(this).data("modal-confirm-text"),
            confirmButtonClass: $(this).data("modal-confirm-class"),
            onConfirm: function () {
                $.ajax({
                    url: "/Admin/Patient/DeletePatient",
                    type: "POST",
                    dataType: "json",
                    data: {
                        id: drugId,
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (res) {
                        if (res.success) {
                            $("#drugRow_" + drugId).remove();
                            showAlert(res.message || "Deleted successfully", "success");
                        } else {
                            showAlert(res.message || "Delete failed", "danger");
                        }
                    },
                    error: function () {
                        showAlert("Delete failed.", "danger");
                    }
                });
            }
        });
    });


    // --- Shared Confirmation Modal Function ---
    function openConfirmModal(options) {
        let modalHtml = `
            <div class="modal fade" id="confirmModal" tabindex="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">${options.title || "Confirm"}</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <p>${options.message || "Are you sure?"}</p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <button type="button" class="btn ${options.confirmButtonClass || "btn-primary"}" id="confirmBtn">
                                ${options.confirmButtonText || "Yes"}
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;

        $("#confirmModal").remove(); // remove old
        $("body").append(modalHtml);

        let modal = new bootstrap.Modal(document.getElementById("confirmModal"));
        modal.show();

        // Confirm action
        $(document).off("click", "#confirmBtn").on("click", "#confirmBtn", function () {
            modal.hide();
            if (typeof options.onConfirm === "function") {
                options.onConfirm();
            }
        });
    }

    // --- Simple Bootstrap alert helper ---
    function showAlert(message, type) {
        let alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>`;

        $(".container").prepend(alertHtml); // prepend at top of page
    }
});

// Auto-hide alerts after 3 seconds
$(function () {
    setTimeout(function () {
        $(".alert").fadeOut("slow", function () {
            $(this).remove();
        });
    }, 3000); // 3 seconds
});