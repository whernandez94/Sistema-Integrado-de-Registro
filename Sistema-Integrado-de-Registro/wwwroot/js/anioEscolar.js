$(document).ready(function () {
    cargarAnios();
});

function cargarAnios() {
    $.get('/gestion-escolar/anios/obtener-todos', function (data) {
        let html = '';
        data.forEach(function (a) {
            html += `
                <tr>
                    <td>${a.anio}</td>
                    <td>${a.finalizado ? 'Finalizado' : 'Activo'}</td>
                    <td>
                        <button class="btn btn-warning btn-sm" onclick="abrirFormulario(${a.id})" ${a.finalizado ? 'disabled' : ''}>Editar</button>
                        <button class="btn btn-danger btn-sm" onclick="eliminar(${a.id})" ${a.finalizado ? 'disabled' : ''}>Eliminar</button>
                        <button class="btn btn-secondary btn-sm" onclick="finalizar(${a.id})" ${a.finalizado ? 'disabled' : ''}>Finalizar</button>
                    </td>
                </tr>`;
        });
        $('#tablaAnios tbody').html(html);

        // Inicializar DataTables después de cargar los datos
        $(document).ready(function () {
            $('#tablaAnios').DataTable({
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

function abrirFormulario(id = 0) {
    const url = id ? `/gestion-escolar/anios/form?id=${id}` : `/gestion-escolar/anios/form`;

    $.get(url, function (html) {
        $('#modalAnioEscolarBody').html(html);
        const modal = new bootstrap.Modal(document.getElementById('modalAnioEscolar'));
        modal.show();

        // Asociar el evento submit después de cargar el contenido
        $('#formAnioEscolar').off('submit').on('submit', function (e) {
            e.preventDefault();

            const form = this;
            $(form).removeClass('was-validated');

            const anioStr = $('input[name="anio"]').val().trim();
            const anioRegex = /^\d{4}-\d{4}$/;

            if (!anioRegex.test(anioStr)) {
                alert("El formato del año debe ser 'YYYY-YYYY'.");
                return;
            }

            const [inicio, fin] = anioStr.split('-').map(Number);
            if (fin !== inicio + 1) {
                alert("El segundo año debe ser uno más que el primero.");
                return;
            }

            const anio = {
                id: $('input[name="id"]').val() || 0,
                anio: anioStr,
                finalizado: false
            };

            $.ajax({
                url: '/gestion-escolar/anios/guardar',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(anio),
                success: function () {
                    $('#modalAnioEscolar').modal('hide'); // Oculta el modal
                    cargarAnios(); // Refresca la tabla
                },
                error: function (xhr) {
                    alert(xhr.responseText || "Error al guardar.");
                }
            });
        });
    });
}

function eliminar(id) {
    if (!confirm('¿Eliminar este año escolar?')) return;

    $.ajax({
        url: `/gestion-escolar/anios/eliminar/${id}`,
        type: 'DELETE',
        success: function () {
            cargarAnios();
        }
    });
}

function finalizar(id) {
    if (!confirm('¿Finalizar este año escolar?')) return;

    $.post(`/gestion-escolar/anios/finalizar/${id}`, function () {
        cargarAnios();
    });
}