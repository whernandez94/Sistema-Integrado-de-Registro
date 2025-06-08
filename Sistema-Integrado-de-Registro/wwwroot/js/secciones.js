$(document).ready(function () {
    let table = $('#tblSecciones').DataTable({
        responsive: true,
        language: {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            },
            "oAria": {
                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
            },
            "buttons": {
                "copy": "Copiar",
                "colvis": "Visibilidad",
                "print": "Imprimir"
            }
        },
        ajax: {
            url: '/gestion-escolar/secciones/obtener-todas',
            dataSrc: ''
        },
        columns: [
            { data: 'nombre' },
            { data: 'grado' },
            { data: 'anioEscolar' },
            { data: 'docenteGuia' },
            {
                data: 'cantidadEstudiantes',
                render: function (data, type, row) {
                    return `<span class="badge bg-primary">${data}</span>`;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-sm btn-info btn-detalle mr-2" data-id="${row.id}" title="Ver detalle">
                            <i class="fas fa-list"></i>
                        </button>
                        <button class="btn btn-sm btn-warning btn-imprimir mr-2" data-id="${row.id}" title="Imprimir listado">
                            <i class="fas fa-print"></i>
                        </button>
                        <button class="btn btn-sm btn-danger btn-eliminar" data-id="${row.id}" title="Eliminar">
                            <i class="fas fa-trash"></i>
                        </button>
                    `;
                },
                orderable: false
            }
        ]
    });

    // Abrir modal para nueva sección
    $('#btnNuevo').click(function () {
        $('#formSeccion')[0].reset();
        $('#Id').val(0);
        $('#modalFormLabel').text('Nueva Sección');
        $('#modalForm').modal('show');
    });

    // Ver detalle de sección
    $('#tblSecciones').on('click', '.btn-detalle', function () {
        const id = $(this).data('id');
        window.location.href = `/gestion-escolar/secciones/obtener-detalle/${id}`;
    });

    // Imprimir listado de sección
    $('#tblSecciones').on('click', '.btn-imprimir', function () {
        const id = $(this).data('id');
        window.open(`/gestion-escolar/secciones/imprimir-listado/${id}`, '_blank');
    });

    // Eliminar sección
    $('#tblSecciones').on('click', '.btn-eliminar', function () {
        const id = $(this).data('id');

        Swal.fire({
            title: '¿Está seguro?',
            text: "Esta acción no se puede deshacer",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/gestion-escolar/secciones/eliminar/${id}`,
                    type: 'DELETE',
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Eliminado',
                                text: response.message,
                                confirmButtonText: 'Aceptar'
                            });
                            table.ajax.reload();
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: response.message,
                                confirmButtonText: 'Aceptar'
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Error al eliminar la sección',
                            confirmButtonText: 'Aceptar'
                        });
                    }
                });
            }
        });
    });

    // Guardar sección
    $('#formSeccion').submit(function (e) {
        e.preventDefault();

        const seccion = {
            Id: $('#Id').val(),
            Nombre: $('#Nombre').val(),
            Grado: $('#Grado').val(),
            AnioEscolarId: $('#AnioEscolarId').val(),
            DocenteId: $('#DocenteId').val()
        };

        $.ajax({
            url: '/gestion-escolar/secciones/guardar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(seccion),
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Guardado exitoso',
                        text: response.message,
                        confirmButtonText: 'Aceptar'
                    });
                    $('#modalForm').modal('hide');
                    table.ajax.reload();
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message,
                        confirmButtonText: 'Aceptar'
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error al guardar la sección',
                    confirmButtonText: 'Aceptar'
                });
            }
        });
    });
});
