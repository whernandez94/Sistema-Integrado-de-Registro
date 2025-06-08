$(document).ready(function () {
    let table = $('#tblInasistencias').DataTable({
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
            }
        ]
    });

    $(document).on('click', '.btn-editar', function () {
        const id = $(this).data('id');

        $.get(`/gestion-escolar/inasistencias/obtener/${id}`, function (data) {
            $('#formInasistencia input[name="Id"]').val(data.id);
            $('#formInasistencia select[name="EstudianteId"]').val(data.estudianteId);
            $('#formInasistencia select[name="AsignaturaId"]').val(data.asignaturaId);
            $('#formInasistencia select[name="AnioEscolarId"]').val(data.anioEscolarId);
            $('#formInasistencia select[name="Lapso"]').val(data.lapso);
            $('#formInasistencia input[name="Porcentaje"]').val(data.porcentaje);
            $('#formInasistencia textarea[name="Observaciones"]').val(data.observaciones);

            const porcentaje = $(`#formInasistencia select[name="AsignaturaId"] option:selected`).data('porcentaje');
            $('#limiteInasistencia').text(`Límite permitido: ${porcentaje || 0}%`);

            const modal = new bootstrap.Modal(document.getElementById('modalForm'));
            modal.show();
        }).fail(function () {
            toastr.error('No se pudo cargar la inasistencia.');
        });
    });


    function renderPorcentaje(data) {
        if (data === null || data === undefined) return '-';
        const alerta = data > $('#porcentajePermitido').val();
        return `<span class="${alerta ? 'text-danger fw-bold' : ''}">${data}%</span>`;
    }

    $.get('/gestion-escolar/inasistencias/obtener-por-filtros', function (data) {
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

    $('#btnFiltrar').click(function () {
        const anioEscolarId = $('#anioEscolarId').val();
        const asignaturaId = $('#asignaturaId').val();

        if (!anioEscolarId) {
            toastr.warning('Seleccione un año escolar');
            return;
        }

        let url = `/gestion-escolar/inasistencias/obtener-todas/${anioEscolarId}`;

        if (asignaturaId) {
            url += `/${asignaturaId}`;
        }

        $.get(url, function (data) {
            table.clear().rows.add(data).draw();
        });
    });


    $('#anioEscolarId').change(function () {
        const anioEscolarId = $(this).val();
        if (!anioEscolarId) return;

        $.get(`/gestion-escolar/inasistencias/obtener-estudiantes-por-anio/${anioEscolarId}`, function (estudiantes) {
            $('#EstudianteId').empty().append('<option value="">Seleccione un estudiante</option>');
            $.each(estudiantes, function (i, item) {
                $('#EstudianteId').append($('<option>', {
                    value: item.id,
                    text: `${item.nombre} ${item.apellido}`
                }));
            });
        });
    });

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
            url: '/gestion-escolar/inasistencias/guardar',
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