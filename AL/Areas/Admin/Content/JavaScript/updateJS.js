document.addEventListener('DOMContentLoaded', function () {
    const updateModal = new bootstrap.Modal(document.getElementById('updateModal'));
    const patientForm = document.getElementById('patientForm');
    const validatePatientUrl = document.getElementById('validatePatientUrl').value;

    // Set focus to the first input field
    document.getElementById('Dosage').focus();

    // Fade out server messages
    const serverMessage = document.getElementById('server-message');
    if (serverMessage) {
        setTimeout(() => {
            serverMessage.style.transition = 'opacity 0.5s ease-out';
            serverMessage.style.opacity = '0';
            setTimeout(() => serverMessage.remove(), 500);
        }, 5000);
    }

    // removes the error indication when the user starts typing.
    document.querySelectorAll('input').forEach(input => {
        input.addEventListener('input', function () {
            this.classList.remove('is-invalid');
            const validationSummary = document.getElementById('validation-summary');
            if (!validationSummary.classList.contains('d-none')) {
                validationSummary.classList.add('d-none');
            }
        });
    });

    // remove leading zeros from the Dosage field
    document.getElementById('Dosage').addEventListener('input', function () {
        let value = this.value;
        if (value.startsWith('0') && value.length > 1 && value.charAt(1) !== '.') {
            this.value = parseFloat(value);
        }
    });

    function clearErrors() {
        document.querySelectorAll('input').forEach(input => {
            input.classList.remove('is-invalid');
        });
        const validationSummary = document.getElementById('validation-summary');
        validationSummary.innerHTML = '';
        validationSummary.classList.add('d-none');
        validationSummary.classList.remove('alert-danger');
    }

    function displayError(message, errorField) {
        const validationSummary = document.getElementById('validation-summary');
        validationSummary.innerHTML = message;
        validationSummary.classList.remove('d-none');
        validationSummary.classList.add('alert-danger');
        if (errorField) {
            const field = document.getElementById(errorField);
            if (field) {
                field.classList.add('is-invalid');
            }
        }
    }

    // Client-side validation function
    function validateForm() {
        let isValid = true;
        const dosage = document.getElementById('Dosage').value.trim();
        const drugName = document.getElementById('DrugName').value.trim();
        const patient = document.getElementById('Patient').value.trim();

        clearErrors();

        if (!patient) {
            displayError("All field/s are required.", "Patient");
            isValid = false;
        }
        if (!drugName) {
            displayError("All field/s are required.", "DrugName");
            isValid = false;
        }
        if (!dosage) {
            displayError("All field/s are required.", "Dosage");
            isValid = false;
        }

        const parsedDosage = parseFloat(dosage);
        if (!isNaN(parsedDosage) && parsedDosage <= 0) {
            displayError("Dosage must be a positive number.", "Dosage");
            isValid = false;
        }
        if (!isNaN(parsedDosage) && parsedDosage > 999.9999) {
            displayError("Dosage cannot exceed 999.9999.", "Dosage");
            isValid = false;
        }
        if (!isNaN(parsedDosage) && parsedDosage > 0 && !/^\d+(\.\d{1,4})?$/.test(dosage)) {
            displayError("Dosage can have up to four decimal places.", "Dosage");
            isValid = false;
        }

        return isValid;
    }

    document.getElementById('updateButton').addEventListener('click', function () {
        if (!validateForm()) {
            return;
        }

        const patientId = document.getElementById('ID').value;
        const patient = document.getElementById('Patient').value;
        const drugName = document.getElementById('DrugName').value;
        const dosage = document.getElementById('Dosage').value;
        const requestVerificationToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

        fetch(validatePatientUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': requestVerificationToken
            },
            body: JSON.stringify({ patient: patient, drugName: drugName, dosage: dosage, id: patientId })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                const modalBody = document.getElementById('updateModal').querySelector('.modal-body');
                if (data.success) {
                    modalBody.textContent = "Are you sure you want to update this record?";
                    updateModal.show();
                } else {
                    if (data.errorType === 'noChange') {
                        modalBody.textContent = "No changes were made. Do you still want to update the record?";
                        updateModal.show();
                    } else {
                        displayError(data.message);
                    }
                }
            })
            .catch(error => {
                console.error('Fetch error:', error);
                displayError('An unexpected error occurred. Please try again.');
            });
    });

    document.getElementById('confirmUpdateBtn').addEventListener('click', function () {
        patientForm.submit();
    });

    document.getElementById('clearButton').addEventListener('click', function () {
        patientForm.reset();
        clearErrors();
    });
});