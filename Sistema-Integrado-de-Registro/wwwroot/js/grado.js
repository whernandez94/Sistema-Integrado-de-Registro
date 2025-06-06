$(function () {
    let table = $('#tblGrados').DataTable({
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
            url: '/gestion-escolar/grados/obtener-todos',
            dataSrc: ''
        },
        columns: [
            { data: 'nombre' },
            { data: 'nivel' },
            {
                data: 'id',
                render: function (id) {
                    return `
                        <button class="btn btn-sm btn-primary btn-editar" data-id="${id}"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-sm btn-danger btn-eliminar" data-id="${id}"><i class="fas fa-trash"></i></button>
                    `;
                }
            }
        ]
    });

    $('#btnNuevo').click(function () {
        $('#Id').val(0);
        $('#Nombre').val('');
        $('#Nivel').val('');
        $('#modalFormLabel').text('Nuevo Grado');
        $('#modalForm').modal('show');
    });

    $('#tblGrados').on('click', '.btn-editar', function () {
        const id = $(this).data('id');
        $.get(`/gestion-escolar/grados/obtener/${id}`, function (data) {
            $('#Id').val(data.id);
            $('#Nombre').val(data.nombre);
            $('#Nivel').val(data.nivel);
            $('#modalFormLabel').text('Editar Grado');
            $('#modalForm').modal('show');
        });
    });

    $('#tblGrados').on('click', '.btn-eliminar', function () {
        const id = $(this).data('id');
        if (confirm('¿Está seguro de eliminar este grado?')) {
            $.ajax({
                url: `/gestion-escolar/grados/eliminar/${id}`,
                type: 'DELETE',
                success: function (res) {
                    if (res.success) {
                        table.ajax.reload();
                        alert(res.message);
                    } else {
                        alert('No se pudo eliminar.');
                    }
                }
            });
        }
    });

    $('#formGrado').submit(function (e) {
        e.preventDefault();
        const grado = {
            Id: $('#Id').val(),
            Nombre: $('#Nombre').val(),
            Nivel: $('#Nivel').val()
        };

        $.ajax({
            url: '/gestion-escolar/grados/guardar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(grado),
            success: function (res) {
                if (res.success) {
                    $('#modalForm').modal('hide');
                    table.ajax.reload();
                } else {
                    alert('Error al guardar');
                }
            }
        });
    });
});
