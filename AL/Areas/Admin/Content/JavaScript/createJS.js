    document.addEventListener('DOMContentLoaded', function () {
        document.getElementById('Dosage').focus();
        var saveModal = new bootstrap.Modal(document.getElementById('saveModal'));

        // Fade out success or error messages after a few seconds
        const serverMessage = document.getElementById('server-message');
        if (serverMessage) {
            setTimeout(() => {
                serverMessage.style.transition = 'opacity 0.5s ease-out';
                serverMessage.style.opacity = '0';
                setTimeout(() => serverMessage.remove(), 500);
            }, 5000); // 5 seconds
        }

        document.querySelectorAll('input').forEach(input => {
            input.addEventListener('input', function() {
                this.classList.remove('is-invalid');
                document.getElementById('validation-summary').classList.add('d-none');
            });
        });

        document.getElementById('Dosage').addEventListener('input', function() {
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
            validationSummary.classList.remove('fade-out'); 
        }

        function displayError(message) {
            const validationSummary = document.getElementById('validation-summary');
            validationSummary.innerHTML = message;
            validationSummary.classList.remove('d-none');
            validationSummary.classList.add('alert-danger');

            // Set timeout for fade-out
            setTimeout(() => {
                validationSummary.style.transition = 'opacity 0.5s ease-out';
                validationSummary.style.opacity = '0';
                setTimeout(() => {
                    validationSummary.classList.add('d-none');
                    validationSummary.style.opacity = '1';
                }, 500); // Wait for the transition to finish before hiding
            }, 5000); // Display for 5 seconds
        }

        function validateForm() {
            let isValid = true;
            const patient = document.getElementById('Patient');
            const drugName = document.getElementById('DrugName');
            const dosage = document.getElementById('Dosage');

            clearErrors();
            if (patient.value.trim() === "") {
                displayError("All field/s are required.");
                patient.classList.add('is-invalid');
                isValid = false;
            } else if (!/^[a-zA-Z\s'-]+$/.test(patient.value) || patient.value.length > 50) {
                displayError("Patient name must be alphanumeric with hyphens, apostrophes, spaces and max 50 characters.");
                patient.classList.add('is-invalid');
                isValid = false;
            }
            if (drugName.value.trim() === "") {
                if (isValid) displayError("All field/s are required.");
                drugName.classList.add('is-invalid');
                isValid = false;
            } else if (!/^[a-zA-Z0-9\s]+$/.test(drugName.value) || drugName.value.length > 50) {
                if (isValid) displayError("Drug name must be alphanumeric with spaces and max 50 characters.");
                drugName.classList.add('is-invalid');
                isValid = false;
            }
            const dosageValue = dosage.value.trim();
            const parsedDosage = parseFloat(dosageValue);
            if (dosageValue === "") {
                if (isValid) displayError("All field/s are required.");
                dosage.classList.add('is-invalid');
                isValid = false;
            } else if (isNaN(parsedDosage) || parsedDosage <= 0) {
                if (isValid) displayError("Dosage must be a positive number.");
                dosage.classList.add('is-invalid');
                isValid = false;
            } else if (parsedDosage > 999.9999) {
                if (isValid) displayError("Dosage cannot exceed 999.9999.");
                dosage.classList.add('is-invalid');
                isValid = false;
            } else {
                const decimalPlaces = (dosageValue.split('.')[1] || '').length;
                if (decimalPlaces > 4) {
                    if (isValid) displayError("Dosage can only have up to 4 decimal places.");
                    dosage.classList.add('is-invalid');
                    isValid = false;
                }
            }
            if (!isValid) {
                const firstInvalid = document.querySelector('.is-invalid');
                if (firstInvalid) {
                    firstInvalid.focus();
                }
            }
            return isValid;
        }

        document.getElementById('saveButton').addEventListener('click', async function (e) {
            e.preventDefault();
            if (!validateForm()) {
                return;
            }
            const patient = document.getElementById('Patient').value;
            const drugName = document.getElementById('DrugName').value;
            const dosage = document.getElementById('Dosage').value;
            try {
                const response = await fetch('@Url.Action("ValidatePatient", "Patient")', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.getElementsByName('__RequestVerificationToken')[0].value
                    },
                    body: JSON.stringify({ patient, drugName, dosage })
                });
                const data = await response.json();
                if (!data.success) {
                    displayError(data.message);
                    if (data.errorType === 'alreadyExist') {
                        document.getElementById('DrugName').classList.add('is-invalid');
                        document.getElementById('Dosage').classList.add('is-invalid');
                    } else if (data.errorType === 'uniqueDrug') {
                        document.getElementById('Patient').classList.add('is-invalid');
                        document.getElementById('DrugName').classList.add('is-invalid');
                    } else if (data.errorType === 'invalidDosage') {
                        document.getElementById('Dosage').classList.add('is-invalid');
                    }
                    return;
                }
                saveModal.show();
            } catch (error) {
                console.error('Validation failed:', error);
            }
        });

        document.getElementById('confirmSaveBtn').addEventListener('click', function () {
            document.getElementById('mainForm').submit();
        });

        document.getElementById('clearButton').addEventListener('click', function () {
            document.getElementById('Dosage').focus();
            document.getElementById('Patient').value = '';
            document.getElementById('DrugName').value = '';
            document.getElementById('Dosage').value = '';
            clearErrors();
        });
    });
