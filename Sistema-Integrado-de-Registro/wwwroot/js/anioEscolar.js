$(document).ready(function () {
    let table;

    function inicializarDataTable() {
        table = $('#tablaAnios').DataTable({
            ajax: {
                url: '/gestion-escolar/anios/obtener-todos',
                dataSrc: ''
            },
            columns: [
                { data: 'anio' },
                {
                    data: 'finalizado',
                    render: function (data) {
                        return data ? 'Finalizado' : 'Activo';
                    }
                },
                {
                    data: 'id',
                    render: function (data, type, row) {
                        return `
                            <button class="btn btn-warning btn-sm btn-editar" data-id="${data}" ${row.finalizado ? 'disabled' : ''}>
                                <i class="fas fa-edit"></i> Editar
                            </button>
                            <button class="btn btn-danger btn-sm btn-eliminar" data-id="${data}" ${row.finalizado ? 'disabled' : ''}>
                                <i class="fas fa-trash"></i> Eliminar
                            </button>
                            <button class="btn btn-secondary btn-sm btn-finalizar" data-id="${data}" ${row.finalizado ? 'disabled' : ''}>
                                <i class="fas fa-check-circle"></i> Finalizar
                            </button>`;
                    }
                }
            ],
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
    }

    function refrescarTabla() {
        table.ajax.reload(null, false); 
    }

    $('#tablaAnios').on('click', '.btn-editar', function () {
        abrirFormulario($(this).data('id'));
    });

    $('#tablaAnios').on('click', '.btn-eliminar', function () {
        eliminar($(this).data('id'));
    });

    $('#tablaAnios').on('click', '.btn-finalizar', function () {
        finalizar($(this).data('id'));
    });

    window.abrirFormulario = function (id = 0) {
        const url = id ? `/gestion-escolar/anios/form/${id}` : `/gestion-escolar/anios/form`;

        $.get(url)
            .done(function (html) {
                $('#modalAnioEscolarBody').html(html);
                const modal = new bootstrap.Modal(document.getElementById('modalAnioEscolar'));
                modal.show();

                $('#formAnioEscolar').off('submit').on('submit', function (e) {
                    e.preventDefault();
                    $(this).removeClass('was-validated');

                    const anioStr = $('input[name="anio"]').val().trim();
                    const anioRegex = /^\d{4}-\d{4}$/;

                    if (!anioRegex.test(anioStr)) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Formato incorrecto',
                            text: "El formato del año debe ser 'YYYY-YYYY'.",
                            confirmButtonText: 'Aceptar'
                        });
                        return;
                    }

                    const [inicio, fin] = anioStr.split('-').map(Number);
                    if (fin !== inicio + 1) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Año incorrecto',
                            text: "El segundo año debe ser uno más que el primero.",
                            confirmButtonText: 'Aceptar'
                        });
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
                        data: JSON.stringify(anio)
                    })
                        .done(function () {
                            Swal.fire({
                                icon: 'success',
                                title: 'Éxito',
                                text: 'Año escolar guardado correctamente',
                                confirmButtonText: 'Aceptar',
                                timer: 2000,
                                timerProgressBar: true
                            }).then(() => {
                                $('#modalAnioEscolar').modal('hide');
                                refrescarTabla();
                            });
                        })
                        .fail(function (xhr) {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: xhr.responseText || "Error al guardar el año escolar.",
                                confirmButtonText: 'Aceptar'
                            });
                        });
                });
            })
            .fail(function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo cargar el formulario',
                    confirmButtonText: 'Aceptar'
                });
            });
    }

    function eliminar(id) {
        Swal.fire({
            title: '¿Eliminar este año escolar?',
            text: "Esta acción no se puede deshacer",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/gestion-escolar/anios/eliminar/${id}`,
                    type: 'DELETE'
                })
                    .done(function () {
                        Swal.fire({
                            icon: 'success',
                            title: 'Eliminado',
                            text: 'El año escolar ha sido eliminado',
                            confirmButtonText: 'Aceptar',
                            timer: 2000,
                            timerProgressBar: true
                        }).then(() => {
                            refrescarTabla();
                        });
                    })
                    .fail(function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo eliminar el año escolar',
                            confirmButtonText: 'Aceptar'
                        });
                    });
            }
        });
    }

    function finalizar(id) {
        Swal.fire({
            title: '¿Finalizar este año escolar?',
            text: "Esta acción no se puede deshacer y deshabilitará todas las operaciones para este año",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sí, finalizar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.post(`/gestion-escolar/anios/finalizar/${id}`)
                    .done(function () {
                        Swal.fire({
                            icon: 'success',
                            title: 'Finalizado',
                            text: 'El año escolar ha sido marcado como finalizado',
                            confirmButtonText: 'Aceptar',
                            timer: 2000,
                            timerProgressBar: true
                        }).then(() => {
                            refrescarTabla();
                        });
                    })
                    .fail(function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo finalizar el año escolar',
                            confirmButtonText: 'Aceptar'
                        });
                    });
            }
        });
    }

    inicializarDataTable();
});