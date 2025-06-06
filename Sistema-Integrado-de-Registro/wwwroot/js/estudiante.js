$(document).ready(function () {
    cargarTabla();

    $('#formEstudiante').on('submit', function (e) {
        e.preventDefault();
        const data = {};
        $('#formEstudiante').serializeArray().forEach(x => {
            if (x.name === "Id") {
                data[x.name] = parseInt(x.value) || 0;
            } else {
                data[x.name] = x.value;
            }
        });

        $.ajax({
            url: '/gestion-escolar/estudiantes/guardar',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (res) {
                alert(res.message);
                $('#modalEstudiante').modal('hide');
                $('#formEstudiante')[0].reset();
                cargarTabla();
            },
            error: function (xhr) {
                if (xhr.responseJSON) {
                    let mensajes = [];
                    for (const key in xhr.responseJSON) {
                        mensajes.push(xhr.responseJSON[key]);
                    }
                    alert("Errores:\n" + mensajes.join('\n'));
                } else {
                    alert('Error al guardar el estudiante.');
                }
            }
        });
    });

    window.editar = function (id) {
        $.get(`/gestion-escolar/estudiantes/obtener/${id}`, function (data) {
            for (const key in data) {
                $(`[name="${key}"]`).val(data[key]);
            }
            $('#modalEstudiante').modal('show');
        });
    };

    window.eliminar = function (id) {
        if (!confirm("¿Deseas eliminar este estudiante?")) return;

        $.ajax({
            url: `/gestion-escolar/estudiantes/eliminar/${id}`,
            method: 'DELETE',
            success: function (res) {
                alert(res.message);
                cargarTabla();
            }
        });
    };

    function cargarTabla() {
        $.get('/gestion-escolar/estudiantes/obtener-todos', function (data) {
            let html = '';
            data.forEach(e => {
                html += `
                    <tr>
                        <td>${e.cedula}</td>
                        <td>${e.nombre} ${e.apellido}</td>
                        <td>${e.telefono}</td>
                        <td>${e.nombreRepresentante}</td>
                        <td>${e.telefonoRepresentante}</td>
                        <td>
                            <button class="btn btn-warning btn-sm" onclick="editar(${e.id})">Editar</button>
                            <button class="btn btn-danger btn-sm" onclick="eliminar(${e.id})">Eliminar</button>
                        </td>
                    </tr>`;
            });
            $('#tablaEstudiantes tbody').html(html);
            $(document).ready(function () {
                $('#tablaEstudiantes').DataTable({
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
                });
            });
        });
    }
});
