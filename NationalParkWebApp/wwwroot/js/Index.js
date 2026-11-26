$(document).ready(function () {
    $('#tblNationalParks').DataTable({
        pageLength: 5,
        lengthMenu: [5, 10, 25, 50],
        searching: true,
        ordering: true,
        paging: true,
        info: true,
        columnDefs: [
            { targets: 0, orderable: true }
        ]
    });
});