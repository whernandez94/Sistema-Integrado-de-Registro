$(document).ready(function () {
    cargarAnios();

    $('#formAnioEscolar').on('submit', function (e) {
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
            url: '/AnioEscolar/Guardar',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(anio),
            success: function () {
                $('#formAnioEscolar')[0].reset();
                cargarAnios();
            },
            error: function (xhr) {
                alert(xhr.responseText || "Error al guardar.");
            }
        });
    });
});

function cargarAnios() {
    $.get('/AnioEscolar/ObtenerTodos', function (data) {
        let html = '';
        data.forEach(function (a) {
            html += `
                <tr>
                    <td>${a.anio}</td>
                    <td>${a.finalizado ? 'Finalizado' : 'Activo'}</td>
                    <td>
                        <button class="btn btn-warning btn-sm" onclick="editar(${a.id})" ${a.finalizado ? 'disabled' : ''}>Editar</button>
                        <button class="btn btn-danger btn-sm" onclick="eliminar(${a.id})" ${a.finalizado ? 'disabled' : ''}>Eliminar</button>
                        <button class="btn btn-secondary btn-sm" onclick="finalizar(${a.id})" ${a.finalizado ? 'disabled' : ''}>Finalizar</button>
                    </td>
                </tr>`;
        });
        $('#tablaAnios tbody').html(html);
    });
}

function editar(id) {
    $.get(`/AnioEscolar/Obtener?id=${id}`, function (data) {
        $('input[name="id"]').val(data.id);
        $('input[name="anio"]').val(data.anio);
    });
}

function eliminar(id) {
    if (!confirm('¿Eliminar este año escolar?')) return;

    $.ajax({
        url: `/AnioEscolar/Eliminar?id=${id}`,
        type: 'DELETE',
        success: function () {
            cargarAnios();
        }
    });
}

function finalizar(id) {
    if (!confirm('¿Finalizar este año escolar?')) return;

    $.post(`/AnioEscolar/Finalizar?id=${id}`, function () {
        cargarAnios();
    });
}
