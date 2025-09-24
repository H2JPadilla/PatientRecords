document.addEventListener('DOMContentLoaded', function () {
    let timeout = null;
    const tableBody = document.getElementById('patientTableBody');
    const paginationContainer = document.getElementById('paginationContainer');
    const searchInputs = document.querySelectorAll('#dateFilter, #drugFilter, #dosageFilter, #patientFilter');
    const resetButtonContainer = document.getElementById('resetButtonContainer');

    let patientIdToDelete = null;

    const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');

    function hasFilterValue() {
        return Array.from(searchInputs).some(input => input.value.trim() !== '');
    }

    function updateResetButtonVisibility() {
        if (hasFilterValue()) {
            resetButtonContainer.classList.remove('d-none');
        } else {
            resetButtonContainer.classList.add('d-none');
        }
    }

    updateResetButtonVisibility();

    // Event listener for the delete button using event delegation
    tableBody.addEventListener('click', function (e) {
        if (e.target.classList.contains('delete-btn')) {
            e.preventDefault();
            patientIdToDelete = e.target.getAttribute('data-id');
            deleteModal.show();
        }
    });

    // Event listener for the modal's confirm button
    confirmDeleteBtn.addEventListener('click', function () {
        if (patientIdToDelete) {
            const tempForm = document.createElement('form');
            tempForm.action = deleteUrl;
            tempForm.method = 'POST';

            const idInput = document.createElement('input');
            idInput.type = 'hidden';
            idInput.name = 'id';
            idInput.value = patientIdToDelete;
            tempForm.appendChild(idInput);

            const token = document.querySelector('input[name="__RequestVerificationToken"]');
            if (token) {
                const clonedToken = token.cloneNode(true);
                tempForm.appendChild(clonedToken);
            }

            document.body.appendChild(tempForm);
            tempForm.submit();
        }
    });

    // Re-usable search function with page number
    function searchPatients(page = 1) {
        const patient = document.getElementById('patientFilter').value.trim();
        const drug = document.getElementById('drugFilter').value.trim();
        const dosage = document.getElementById('dosageFilter').value;
        const date = document.getElementById('dateFilter').value;

        const queryString = new URLSearchParams({
            patient: patient,
            drug: drug,
            dosage: dosage,
            date: date,
            page: page
        }).toString();

        fetch(`${searchUrl}?${queryString}`, {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        })
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(data => {
                updateTable(data.patients);
                updatePagination(data.totalPages, data.page);
                currentPage = data.page;
            })
            .catch(error => console.error('Fetch error:', error));
    }

    // Update table rows
    function updateTable(patients) {
        tableBody.innerHTML = '';
        if (patients.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No records found</td></tr>';
            return;
        }

        patients.forEach(patient => {
            const row = document.createElement('tr');
            const updateLink = updateUrlTemplate.replace('__ID__', patient.ID);
            row.innerHTML = `
                <td class="col-md-2">
                    <a href="${updateLink}" class="btn btn-sm btn-outline-secondary">Edit</a> |
                    <a href="#" class="btn btn-sm btn-outline-secondary delete-btn" data-id="${patient.ID}" data-bs-toggle="modal" data-bs-target="#deleteModal">Delete</a>
                </td>
                <td class="col-md-2">${new Date(patient.ModifiedDate).toLocaleDateString()}</td>
                <td class="col-md-2">${parseFloat(patient.Dosage).toFixed(2)}</td>
                <td class="col-md-2">${patient.DrugName}</td>
                <td class="col-md-3">${patient.Patient}</td>
                <td class="col-md-1"></td>
            `;
            tableBody.appendChild(row);
        });
    }

    // Update pagination links dynamically
    function updatePagination(totalPages, currentPage) {
        const nav = document.createElement('nav');
        nav.setAttribute('aria-label', 'Page navigation');
        const paginationUl = document.createElement('ul');
        paginationUl.className = 'pagination';

        const prevLi = document.createElement('li');
        prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
        const prevLink = document.createElement('a');
        prevLink.className = 'page-link';
        prevLink.href = '#';
        prevLink.textContent = 'Previous';
        prevLink.addEventListener('click', (e) => {
            e.preventDefault();
            if (currentPage > 1) {
                searchPatients(currentPage - 1);
            }
        });
        prevLi.appendChild(prevLink);
        paginationUl.appendChild(prevLi);

        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement('li');
            li.className = `page-item ${i === currentPage ? 'active' : ''}`;
            const link = document.createElement('a');
            link.className = 'page-link';
            link.href = '#';
            link.textContent = i;
            link.addEventListener('click', (e) => {
                e.preventDefault();
                searchPatients(i);
            });
            li.appendChild(link);
            paginationUl.appendChild(li);
        }

        const nextLi = document.createElement('li');
        nextLi.className = `page-item ${currentPage === totalPages ? 'disabled' : ''}`;
        const nextLink = document.createElement('a');
        nextLink.className = 'page-link';
        nextLink.href = '#';
        nextLink.textContent = 'Next';
        nextLink.addEventListener('click', (e) => {
            e.preventDefault();
            if (currentPage < totalPages) {
                searchPatients(currentPage + 1);
            }
        });
        nextLi.appendChild(nextLink);
        paginationUl.appendChild(nextLi);

        paginationContainer.innerHTML = '';
        nav.appendChild(paginationUl);
        paginationContainer.appendChild(nav);
    }

    // Initialize event listeners for search inputs and update button visibility
    searchInputs.forEach(input => {
        input.addEventListener('input', function () {
            updateResetButtonVisibility();
            clearTimeout(timeout);
            timeout = setTimeout(function () {
                searchPatients();
            }, 500);
        });
    });

    // Reset button functionality
    document.getElementById('resetFilters').addEventListener('click', function () {
        document.getElementById('dateFilter').value = '';
        document.getElementById('dosageFilter').value = '';
        document.getElementById('drugFilter').value = '';
        document.getElementById('patientFilter').value = '';
        updateResetButtonVisibility();
        searchPatients();
    });

    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity 0.5s ease-out';
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 500);
        }, 3000);
    });
});