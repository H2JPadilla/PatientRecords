function filterTable() {
    var drugInput = document.getElementById("searchDrug").value.toLowerCase();
    var patientInput = document.getElementById("searchPatient").value.toLowerCase();
    var doseInput = document.getElementById("searchDose").value.toLowerCase();
    var dateInput = document.getElementById("searchDate").value;

    var table = document.getElementById("drugsTable");
    var rows = table.getElementsByTagName("tr");
    var visibleCount = 0;

    for (var i = 1; i < rows.length; i++) {
        var cols = rows[i].getElementsByTagName("td");
        if (cols.length > 0) {
            var patient = cols[1].innerText.toLowerCase();
            var drug = cols[2].innerText.toLowerCase();
            var dose = cols[3].innerText.toLowerCase();
            var dateText = cols[4].innerText;

            var parts = dateText.split("/");
            var formattedDate = parts[2] + "-" + parts[0].padStart(2, '0') + "-" + parts[1].padStart(2, '0');

            var matchDate = !dateInput || formattedDate === dateInput;

            if (patient.includes(patientInput) &&
                drug.includes(drugInput) &&
                dose.includes(doseInput) &&
                matchDate) {
                rows[i].style.display = "";
                visibleCount++;
            } else {
                rows[i].style.display = "none";
            }
        }
    }

    document.getElementById("noResults").style.display = (visibleCount === 0) ? "block" : "none";
}

function resetSearch() {
    document.getElementById("searchDrug").value = "";
    document.getElementById("searchPatient").value = "";
    document.getElementById("searchDose").value = "";
    document.getElementById("searchDate").value = "";
    filterTable();
}