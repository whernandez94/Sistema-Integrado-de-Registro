$(document).ready(function () {
    let tabla = $('#tablaInstituciones').DataTable({
        ajax: {
            url: '/Institution/ObtenerTodas',
            dataSrc: ''
        },
        columns: [
            { data: 'nombre' },
            { data: 'nombreDirector' },
            { data: 'telefono' },
            { data: 'email' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <button class="btn btn-sm btn-warning btn-editar" data-id="${data}">Editar</button>
                        <button class="btn btn-sm btn-danger btn-eliminar" data-id="${data}">Eliminar</button>`;
                }
            }
        ]
    });

    $('#formInstitucion').on('submit', function (e) {
        e.preventDefault();
        $('#mensaje').html('');

        const formData = {
            Id: $('input[name=id]').val() || 0,
            Nombre: $('input[name=nombre]').val().trim(),
            NombreDirector: $('input[name=nombreDirector]').val().trim(),
            IdentificacionDirector: $('input[name=identificacionDirector]').val().trim(),
            Direccion: $('input[name=direccion]').val().trim(),
            Telefono: $('input[name=telefono]').val().trim(),
            Email: $('input[name=email]').val().trim()
        };

        const errores = [];
        if (!formData.Nombre) errores.push("Nombre requerido.");
        if (!formData.NombreDirector) errores.push("Nombre del director requerido.");
        if (!formData.IdentificacionDirector) errores.push("Identificación requerida.");
        if (!formData.Telefono || !/^\d+$/.test(formData.Telefono)) errores.push("Teléfono inválido.");
        if (!formData.Email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.Email)) errores.push("Correo inválido.");

        if (errores.length > 0) {
            mostrarErrores(errores);
            return;
        }

        $.ajax({
            url: '/Institution/Guardar',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (res) {
                mostrarMensaje(res.message, true);
                $('#formInstitucion')[0].reset();
                $('#modalInstitucion').modal('hide');
                tabla.ajax.reload(null, false);
            },
            error: function (xhr) {
                mostrarErroresDesdeServidor(xhr);
            }
        });
    });

    $('#tablaInstituciones').on('click', '.btn-editar', function () {
        const id = $(this).data('id');
        $.get(`/Institution/Obtener?id=${id}`, function (data) {
            for (const key in data) {
                const field = $(`[name="${key}"]`);
                if (field.length) {
                    if (field.is(':checkbox')) {
                        field.prop('checked', data[key]);
                    } else {
                        field.val(data[key]);
                    }
                }
            }
            $('#modalInstitucion').modal('show');
        });
    });

    $('#tablaInstituciones').on('click', '.btn-eliminar', function () {
        const id = $(this).data('id');
        if (!confirm("¿Deseas eliminar este registro?")) return;
        $.ajax({
            url: `/Institution/Eliminar?id=${id}`,
            method: 'DELETE',
            success: function (res) {
                mostrarMensaje(res.message, true);
                tabla.ajax.reload(null, false);
            }
        });
    });

    function mostrarMensaje(msg, success) {
        $('#mensaje').html(`<div class="alert alert-${success ? 'success' : 'danger'}">${msg}</div>`);
    }

    function mostrarErrores(lista) {
        if (lista.length > 0) {
            let html = '<div class="alert alert-danger"><ul>';
            lista.forEach(e => html += `<li>${e}</li>`);
            html += '</ul></div>';
            $('#mensaje').html(html);
        }
    }

    function mostrarErroresDesdeServidor(xhr) {
        let errors = xhr.responseJSON;
        let lista = [];
        for (const key in errors) {
            if (errors[key].errors) {
                errors[key].errors.forEach(err => lista.push(err.errorMessage));
            }
        }
        mostrarErrores(lista);
    }

    // Evento para abrir modal como nuevo registro
    $('#btnNuevo').on('click', function () {
        $('#formInstitucion')[0].reset();
        $('input[name=Id]').val('');
        $('#modalInstitucion').modal('show');
        mostrarErroresDesdeServidor([]);
        mostrarErrores([]);
    });
});
