$(document).ready(function () {
    let table = $('#tblNotas').DataTable({
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

        $.get(`/gestion-escolar/notas/obtener-notas/${anioEscolarId}/${asignaturaId || ''}`, function (data) {
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

    // Editar nota
    $('#tblNotas').on('click', '.btn-editar', function () {
        const estudianteId = $(this).data('id');
        const asignaturaId = $(this).data('asignatura');
        const anioEscolarId = $('#anioEscolarId').val();

        if (!anioEscolarId) {
            toastr.warning('Primero filtre por año escolar');
            return;
        }

        // Aquí deberías implementar la lógica para cargar los datos de la nota específica
        // Esto es un ejemplo simplificado
        $('#modalFormLabel').text('Editar Nota');
        $('#modalForm').modal('show');
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