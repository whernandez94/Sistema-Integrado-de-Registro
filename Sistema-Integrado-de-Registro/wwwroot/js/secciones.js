$(document).ready(function () {
    let table = $('#tblSecciones').DataTable({
        responsive: true,
        ajax: {
            url: '/secciones/ObtenerTodas',
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
        window.open(`/secciones/ObtenerDetalle?id=${id}`, '_blank');
    });

    // Imprimir listado de sección
    $('#tblSecciones').on('click', '.btn-imprimir', function () {
        const id = $(this).data('id');
        window.open(`/secciones/ImprimirListado?id=${id}`, '_blank');
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
                    url: `/secciones/Eliminar?id=${id}`,
                    type: 'DELETE',
                    success: function (response) {
                        if (response.success) {
                            toastr.success(response.message);
                            table.ajax.reload();
                        } else {
                            toastr.error(response.message);
                        }
                    },
                    error: function () {
                        toastr.error('Error al eliminar la sección');
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
            url: '/secciones/Guardar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(seccion),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#modalForm').modal('hide');
                    table.ajax.reload();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('Error al guardar la sección');
            }
        });
    });
});