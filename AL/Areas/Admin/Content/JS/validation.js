    $(document).ready(function () {
        $("#patientForm").submit(function (e) {
            e.preventDefault();

            let patient = $("#Patient").val().trim();
            let drug = $("#DrugName").val().trim();
            let dosage = $("#Dosage").val().trim();

            $(".form-control").removeClass("is-invalid");

            if (patient === "") {
                $("#Patient").addClass("is-invalid").focus();
                return;
            }
            if (drug === "") {
                $("#DrugName").addClass("is-invalid").focus();
                return;
            }
            if (dosage === "" || isNaN(dosage)) {
                $("#Dosage").addClass("is-invalid").focus();
                return;
            }

            if ($("h2").text().includes("Add")) {
                $("#saveModal").modal("show");
            } else {
                $("#updateModal").modal("show");
            }
        });

        $("#confirmSaveBtn").click(function () {
            $("#patientForm")[0].submit();
        });
        $("#confirmUpdateBtn").click(function () {
            $("#patientForm")[0].submit();
        });
    });
