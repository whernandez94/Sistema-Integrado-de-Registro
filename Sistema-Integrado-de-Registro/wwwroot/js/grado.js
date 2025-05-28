$(function () {
    let table = $('#tblGrados').DataTable({
        ajax: {
            url: '/grados/ObtenerTodos',
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
        $.get(`/grados/Obtener?id=${id}`, function (data) {
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
                url: `/grados/Eliminar?id=${id}`,
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
            url: '/grados/Guardar',
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
