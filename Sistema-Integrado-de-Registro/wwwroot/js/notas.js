$(document).ready(function () {
    let table = $('#tblNotas').DataTable({
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
            { data: 'lapso1', render: formatNota },
            { data: 'lapso2', render: formatNota },
            { data: 'lapso3', render: formatNota },
            { data: 'promedioFinal', render: formatNota },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-sm btn-primary btn-editar" data-id="${row.estudianteId}" data-asignatura="${row.asignaturaId}">
                            <i class="fas fa-edit"></i>
                        </button>
                    `;
                },
                orderable: false
            }
        ]
    });

    function formatNota(data) {
        return data ? data.toFixed(2) : '-';
    }

    // Cargar filtros
    $.get('/gestion-escolar/notas/obtener-filtros', function (data) {
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

        let url = `/gestion-escolar/notas/obtener-notas/${anioEscolarId}`;
        if (asignaturaId) {
            url += `/${asignaturaId}`;
        }

        $.get(url, function (data) {
            table.clear().rows.add(data).draw();
        });
    });


    // Abrir modal para nueva nota
    $('#btnNuevo').click(function () {
        $('#formNota')[0].reset();
        $('#Id').val(0);
        $('#modalFormLabel').text('Nueva Nota');
        $('#modalForm').modal('show');
    });

    $('#tblNotas').on('click', '.btn-editar', function () {
        const notaId = $(this).data('id');

        if (!notaId) {
            toastr.warning('No se pudo identificar la nota');
            return;
        }

        $.ajax({
            url: `/gestion-escolar/notas/obtener/${notaId}`,
            method: 'GET',
            success: function (data) {
                $('#modalFormLabel').text('Editar Nota');

                $('#formNota input[name="Id"]').val(data.id);
                $('#formNota select[name="EstudianteId"]').val(data.estudianteId);
                $('#formNota select[name="AsignaturaId"]').val(data.asignaturaId);
                $('#formNota select[name="AnioEscolarId"]').val(7);
                $('#formNota select[name="Lapso"]').val(data.lapso);
                $('#formNota input[name="Valor"]').val(data.valor);

                $('#modalForm').modal('show');
            },
            error: function () {
                toastr.error('No se pudo cargar la nota.');
            }
        });
    });


    // Guardar nota
    $('#formNota').submit(function (e) {
        e.preventDefault();

        const nota = {
            Id: $('#Id').val(),
            EstudianteId: $('#EstudianteId').val(),
            AsignaturaId: $('#AsignaturaId').val(),
            AnioEscolarId: $('#AnioEscolarId').val(),
            Lapso: $('#Lapso').val(),
            Valor: $('#Valor').val()
        };

        $.ajax({
            url: '/gestion-escolar/notas/guardar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(nota),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#modalForm').modal('hide');
                    $('#btnFiltrar').click(); // Refrescar tabla
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('Error al guardar la nota');
            }
        });
    });
});