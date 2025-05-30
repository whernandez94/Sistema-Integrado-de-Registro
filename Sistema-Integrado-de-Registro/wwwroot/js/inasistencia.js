﻿$(document).ready(function () {
    let table = $('#tblInasistencias').DataTable({
        responsive: true,
        columns: [
            { data: 'estudianteNombre' },
            { data: 'asignaturaNombre' },
            { data: 'porcentajePermitido', render: function (data) { return data + '%'; } },
            { data: 'lapso1', render: renderPorcentaje },
            { data: 'lapso2', render: renderPorcentaje },
            { data: 'lapso3', render: renderPorcentaje },
            {
                data: 'alerta',
                render: function (data) {
                    return data
                        ? '<span class="badge bg-danger">ALERTA</span>'
                        : '<span class="badge bg-success">Normal</span>';
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-sm btn-primary btn-editar" data-id="${row.inasistenciaId}" data-asignatura="${row.inasistenciaId}">
                            <i class="fas fa-edit"></i>
                        </button>
                    `;
                },
                orderable: false
            }
        ]
    });

    function renderPorcentaje(data) {
        if (data === null || data === undefined) return '-';
        const alerta = data > $('#porcentajePermitido').val();
        return `<span class="${alerta ? 'text-danger fw-bold' : ''}">${data}%</span>`;
    }

    // Cargar filtros
    $.get('/inasistencias/ObtenerFiltros', function (data) {
        $('#anioEscolarId').empty().append('<option value="">Seleccione un año</option>');
        $('#asignaturaId').empty().append('<option value="">Todas</option>');

        $.each(data.anios, function (i, item) {
            $('#anioEscolarId').append($('<option>', {
                value: item.id,
                text: item.anio
            }));
        });

        $.each(data.asignaturas, function (i, item) {
            $('#asignaturaId').append($('<option>', {
                value: item.id,
                text: item.nombre
            }));
        });
    });

    // Filtrar
    $('#btnFiltrar').click(function () {
        const anioEscolarId = $('#anioEscolarId').val();
        const asignaturaId = $('#asignaturaId').val();

        if (!anioEscolarId) {
            toastr.warning('Seleccione un año escolar');
            return;
        }

        $.get(`/inasistencias/ObtenerInasistencias?anioEscolarId=${anioEscolarId}&asignaturaId=${asignaturaId || ''}`, function (data) {
            table.clear().rows.add(data).draw();
        });
    });

    // Cambio de año escolar - cargar estudiantes
    $('#anioEscolarId').change(function () {
        const anioEscolarId = $(this).val();
        if (!anioEscolarId) return;

        $.get(`/inasistencias/ObtenerEstudiantes?anioEscolarId=${anioEscolarId}`, function (estudiantes) {
            $('#EstudianteId').empty().append('<option value="">Seleccione un estudiante</option>');
            $.each(estudiantes, function (i, item) {
                $('#EstudianteId').append($('<option>', {
                    value: item.id,
                    text: `${item.nombre} ${item.apellido}`
                }));
            });
        });
    });

    // Cambio de asignatura - actualizar límite
    $('#AsignaturaId').change(function () {
        const selectedOption = $(this).find('option:selected');
        const porcentajePermitido = selectedOption.data('porcentaje') || 0;
        $('#limiteInasistencia').text(`Límite permitido: ${porcentajePermitido}%`);
        $('#Porcentaje').attr('max', porcentajePermitido);
    });

    // Abrir modal para nuevo registro
    $('#btnNuevo').click(function () {
        $('#formInasistencia')[0].reset();
        $('#Id').val(0);
        $('#modalFormLabel').text('Nuevo Registro');
        $('#modalForm').modal('show');
    });

    // Guardar inasistencia
    $('#formInasistencia').submit(function (e) {
        e.preventDefault();

        const inasistencia = {
            Id: $('#Id').val(),
            EstudianteId: $('#EstudianteId').val(),
            AsignaturaId: $('#AsignaturaId').val(),
            AnioEscolarId: $('#AnioEscolarId').val(),
            Lapso: $('#Lapso').val(),
            Porcentaje: $('#Porcentaje').val(),
            Observaciones: $('#Observaciones').val()
        };

        $.ajax({
            url: '/inasistencias/Guardar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(inasistencia),
            success: function (response) {
                if (response.success) {
                    if (response.data) {
                        toastr.warning(response.message); // Alerta
                    } else {
                        toastr.success(response.message); // Éxito normal
                    }
                    $('#modalForm').modal('hide');
                    $('#btnFiltrar').click(); // Refrescar tabla
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('Error al guardar el registro');
            }
        });
    });
});