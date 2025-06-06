$(document).ready(function () {
    let table = $('#tblMatriculas').DataTable({
        responsive: true,
        ajax: {
            url: '/gestion-escolar/matricula/obtener-todas',
            dataSrc: ''
        },
        columns: [
            {
                data: 'nombreEstudiante',
                render: function (data) {
                    if (!data) return '';
                    return `${data}`;
                }
            },
            {
                data: 'seccion',
                render: function (data) {
                    return data ?? '';
                }
            },
            {
                data: 'anioEscolar',
                render: function (data) {
                    return data ?? '';
                }
            },
            {
                data: 'numeroExpediente',
                render: function (data) {
                    return data ?? '';
                }
            },
            {
                data: 'fecha',
                render: function (data) {
                    if (!data) return '';
                    return data; // formato yyyy-MM-dd
                }
            },
            {
                data: 'activa',
                render: function (data) {
                    return data
                        ? '<span class="badge bg-success">Activa</span>'
                        : '<span class="badge bg-danger">Inactiva</span>';
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-sm btn-primary btn-editar mr-2" data-id="${row.id}">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-danger btn-eliminar" data-id="${row.id}">
                            <i class="fas fa-trash"></i>
                        </button>
                    `;
                },
                orderable: false
            }
        ]
    });

    // Abrir modal para nueva matrícula
    $('#btnNuevo').click(function () {
        $('#formMatricula')[0].reset();
        $('#Id').val(0);
        $('#modalFormLabel').text('Nueva Matrícula');
        $('#modalForm').modal('show');
    });

    // Editar matrícula
    $('#tblMatriculas').on('click', '.btn-editar', function () {
        const id = $(this).data('id');

        $.get(`/gestion-escolar/matricula/obtener/${id}`, function (data) {
            if (data) {
                $('#Id').val(data.id);
                $('#EstudianteId').val(data.estudianteId);
                $('#SeccionId').val(data.seccionId);
                $('#AnioEscolarId').val(data.anioEscolarId);
                $('#FechaMatricula').val(data.fechaMatricula.split('T')[0]);
                $('#NumeroExpediente').val(data.numeroExpediente);
                $('#Observaciones').val(data.observaciones);

                $('#modalFormLabel').text('Editar Matrícula');
                $('#modalForm').modal('show');
            }
        });
    });

    // Eliminar matrícula
    $('#tblMatriculas').on('click', '.btn-eliminar', function () {
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
                    url: `/gestion-escolar/matricula/eliminar/${id}`,
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
                        toastr.error('Error al eliminar la matrícula');
                    }
                });
            }
        });
    });

    // Guardar matrícula
    $('#formMatricula').submit(function (e) {
        e.preventDefault();

        const matricula = {
            Id: $('#Id').val(),
            EstudianteId: $('#EstudianteId').val(),
            SeccionId: $('#SeccionId').val(),
            AnioEscolarId: $('#AnioEscolarId').val(),
            FechaMatricula: $('#FechaMatricula').val(),
            NumeroExpediente: $('#NumeroExpediente').val(),
            Observaciones: $('#Observaciones').val()
        };

        $.ajax({
            url: '/gestion-escolar/matricula/guardar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(matricula),
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
                toastr.error('Error al guardar la matrícula');
            }
        });
    });
});