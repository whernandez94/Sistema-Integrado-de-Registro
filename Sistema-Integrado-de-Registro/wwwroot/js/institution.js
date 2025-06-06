$(document).ready(function () {
    let tabla = $('#tablaInstituciones').DataTable({
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
            url: '/gestion-escolar/institucion/obtener-todas',
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
            url: '/gestion-escolar/institucion/guardar',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (res) {
                Swal.fire({
                    icon: 'success',
                    title: 'Guardado exitoso',
                    text: res.message,
                    confirmButtonText: 'Aceptar'
                });
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
        $.get(`/gestion-escolar/institucion/obtener/${id}`, function (data) {
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
        Swal.fire({
            title: '¿Estás seguro?',
            text: "Esta acción eliminará la institución de forma permanente.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/gestion-escolar/institucion/eliminar/${id}`,
                    method: 'DELETE',
                    success: function (res) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Eliminado',
                            text: res.message,
                            confirmButtonText: 'Aceptar'
                        });
                        tabla.ajax.reload(null, false);
                    }
                });
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

    $('#btnNuevo').on('click', function () {
        $('#formInstitucion')[0].reset();
        $('input[name=Id]').val('');
        $('#modalInstitucion').modal('show');
        mostrarErroresDesdeServidor([]);
        mostrarErrores([]);
    });
});
