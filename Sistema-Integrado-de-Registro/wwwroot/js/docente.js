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
            Rol: $('input[name=Rol]').val().trim(),
            Contrasena: $('input[name=Contrasena]').val().trim(),
            Codigo: $('input[name=Codigo]').val().trim(),
            CargaHoras: parseInt($('input[name=CargaHoras]').val()),
            Asignaturas: [$('#Asignaturas').val()] || []
        };

        $.ajax({
            url: '/gestion-escolar/docentes/guardar',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (res) {
                Swal.fire({
                    icon: 'success',
                    title: 'Guardado exitoso',
                    text: res.message,
                    confirmButtonText: 'Aceptar'
                });
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
        $.get(`/gestion-escolar/docentes/obtener/${id}`, function (data) {
            for (const key in data) {
                let newKey = key.charAt(0).toUpperCase() + key.slice(1);
                const field = $(`[name="${newKey}"]`);
                if (field.length) field.val(data[key]);
            }
            $('#Asignaturas').val(data.asignaturas);
            $('#modalDocente').modal('show');
        });
    };

    window.eliminar = function (id) {
        Swal.fire({
            title: '¿Estás seguro?',
            text: "Esta acción eliminará al docente de forma permanente.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/gestion-escolar/docentes/eliminar/${id}`,
                    method: 'DELETE',
                    success: function (res) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Eliminado',
                            text: res.message,
                            confirmButtonText: 'Aceptar'
                        });
                        cargarTabla();
                    }
                });
            }
        });
    };

    function cargarTabla() {
        $.get('/gestion-escolar/docentes/obtener-todos', function (data) {
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
            // Inicializar DataTables después de cargar los datos
            $(document).ready(function () {
                $('#tablaDocentes').DataTable({
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
                    }
                    // Puedes agregar más opciones de configuración aquí
                });
            });
        });
    }
});
