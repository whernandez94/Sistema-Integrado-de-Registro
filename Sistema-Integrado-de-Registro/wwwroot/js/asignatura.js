﻿$(document).ready(function () {
    let tabla = $('#tablaAsignaturas').DataTable({
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
            url: '/gestion-escolar/asignaturas/obtener-todas',
            dataSrc: ''
        },
        columns: [
            { data: 'nombre' },
            { data: 'porcentajeInasistencia' },
            {
                data: 'id',
                render: function (id) {
                    return `
                        <button class="btn btn-sm btn-warning btn-editar" data-id="${id}">Editar</button>
                        <button class="btn btn-sm btn-danger btn-eliminar" data-id="${id}">Eliminar</button>`;
                }
            }
        ]
    });

    $('#btnNuevo').click(function () {
        $('#formAsignatura')[0].reset();
        $('input[name=id]').val('');
        $('#modalAsignatura').modal('show');
    });

    $('#tablaAsignaturas').on('click', '.btn-editar', function () {
        const id = $(this).data('id');
        $.get(`/gestion-escolar/asignaturas/obtener/${id}`, function (data) {
            $('input[name=id]').val(data.id);
            $('input[name=nombre]').val(data.nombre);
            $('input[name=porcentajeInasistencia]').val(data.porcentajeInasistencia);
            $('#modalAsignatura').modal('show');
        });
    });

    $('#tablaAsignaturas').on('click', '.btn-eliminar', function () {
        const id = $(this).data('id');
        if (!confirm("¿Eliminar asignatura?")) return;
        $.ajax({
            url: `/gestion-escolar/asignaturas/eliminar/${id}`,
            method: 'DELETE',
            success: function (res) {
                mostrarMensaje(res.message, true);
                tabla.ajax.reload();
            }
        });
    });

    $('#formAsignatura').submit(function (e) {
        e.preventDefault();
        const formData = {
            Id: $('input[name=id]').val() || 0,
            Nombre: $('input[name=nombre]').val().trim(),
            PorcentajeInasistencia: parseInt($('input[name=porcentajeInasistencia]').val())
        };

        $.ajax({
            url: '/gestion-escolar/asignaturas/guardar',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (res) {
                mostrarMensaje(res.message, true);
                $('#modalAsignatura').modal('hide');
                tabla.ajax.reload();
            },
            error: function (xhr) {
                let errores = [];
                const res = xhr.responseJSON;
                for (const key in res) {
                    if (res[key].errors)
                        res[key].errors.forEach(err => errores.push(err.errorMessage));
                }
                mostrarErrores(errores);
            }
        });
    });

    function mostrarMensaje(msg, ok) {
        $('#mensaje').html(`<div class="alert alert-${ok ? 'success' : 'danger'}">${msg}</div>`);
    }

    function mostrarErrores(lista) {
        let html = '<div class="alert alert-danger"><ul>';
        lista.forEach(e => html += `<li>${e}</li>`);
        html += '</ul></div>';
        $('#mensaje').html(html);
    }
});
