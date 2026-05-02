$(function () {
    $('.js-basic-example').DataTable({
        responsive: true,
        language: {
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        order: [[0, "desc"]]
    });

    //Exportable table
    $('.js-exportable-siparis').DataTable({
        dom: 'Bfrtip',
        responsive: true,
        language: {
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        order: [[0, "desc"]],
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
                }
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
                }
            },
        ]
    });

    $('.js-exportable-stoktan-siparis').DataTable({
        dom: 'Bfrtip',
        responsive: true,
        order: [[0, "desc"]],
        language: {
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                }
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                }
            },
        ]
    });

    $('.js-exportable-stoktan-stok-profil').DataTable({
        dom: 'Bfrtip',
        responsive: true,
        language: {
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            },
        ]
    });

    $('.js-exportable-stoktan-stok-aksesuar').DataTable({
        dom: 'Bfrtip',
        responsive: true,
        language: {
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
        ]
    });

    $('.js-exportable-stoktan-stok-fire').DataTable({
        dom: 'Bfrtip',
        responsive: true,
        order: [[0, "desc"]],
        language: {
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                }
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                }
            },
        ]
    });

    $('.js-exportable-profil-tanimlama').DataTable({
        dom: 'Bfrtip',
        responsive: true,
        order: [[2, "asc"]],
        language: {
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
        ]
    });

    $('.js-exportable-aksesuar-tanimlama').DataTable({
        dom: 'Bfrtip',
        responsive: true,
        order: [[2, "asc"]],
        language: {
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                }
            },
        ]
    });
});