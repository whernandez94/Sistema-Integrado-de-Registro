$(document).ready(function () {
    cargarTabla();

    $('#formDocente').on('submit', function (e) {
        e.preventDefault();
        const form = $(this);
        const data = {
            Id: $('input[name=Id]').val() || 0,
            Cedula: $('input[name=Cedula]').val().trim(),
            Nombre: $('input[name=Nombre]').val().trim(),
            Apellido: $('input[name=Apellido]').val().trim(),
            Telefono: $('input[name=Telefono]').val().trim(),
            Correo: $('input[name=Correo]').val().trim(),
            CargaHoras: parseInt($('input[name=CargaHoras]').val()),
            Asignaturas: $('#Asignaturas').val() || []
        };

        $.ajax({
            url: '/docentes/Guardar',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (res) {
                $('#mensajeDocente').html(`<div class="alert alert-success">${res.message}</div>`);
                $('#formDocente')[0].reset();
                $('#modalDocente').modal('hide');
                cargarTabla();
            },
            error: function (xhr) {
                const err = xhr.responseJSON?.message || 'Error inesperado.';
                $('#mensajeDocente').html(`<div class="alert alert-danger">${err}</div>`);
            }
        });
    });

    window.editar = function (id) {
        $.get(`/docentes/Obtener?id=${id}`, function (data) {
            for (const key in data) {
                const field = $(`[name="${key}"]`);
                if (field.length) field.val(data[key]);
            }
            $('#Asignaturas').val(data.asignaturas);
            $('#modalDocente').modal('show');
        });
    };

    window.eliminar = function (id) {
        if (!confirm("¿Deseas eliminar este docente?")) return;
        $.ajax({
            url: `/docentes/Eliminar?id=${id}`,
            method: 'DELETE',
            success: function (res) {
                alert(res.message);
                cargarTabla();
            }
        });
    };

    function cargarTabla() {
        $.get('/docentes/ObtenerTodos', function (data) {
            let html = '';
            data.forEach(d => {
                html += `
                    <tr>
                        <td>${d.cedula}</td>
                        <td>${d.nombreCompleto}</td>
                        <td>${d.telefono}</td>
                        <td>${d.correo}</td>
                        <td>${d.cargaHoras}</td>
                        <td>${d.asignaturas}</td>
                        <td>
                            <button class="btn btn-warning btn-sm" onclick="editar(${d.id})">Editar</button>
                            <button class="btn btn-danger btn-sm" onclick="eliminar(${d.id})">Eliminar</button>
                        </td>
                    </tr>`;
            });
            $('#tablaDocentes tbody').html(html);
        });
    }
});
