    function agregarTarea() {
            const idx = document.querySelectorAll('#tareas .task-row').length;

    document.getElementById('tareas').insertAdjacentHTML('beforeend', `
    <div class="d-flex gap-2 mb-2 task-row align-items-center">
        <input type="hidden" name="Tareas[${idx}].Id" value="0" />
        <input type="hidden" name="Tareas[${idx}].Eliminada" value="false" class="eliminada-flag" />

        <input type="checkbox"
            class="form-check-input"
            name="Tareas[${idx}].Estado"
            value="true" />

        <input type="hidden" name="Tareas[${idx}].Estado" value="false" />

        <input class="form-control"
            name="Tareas[${idx}].Descripcion" />

        <button type="button"
            class="btn btn-danger btn-sm btn-eliminar"
            onclick="marcarEliminada(this)">✖</button>

        <button type="button"
            class="btn btn-warning btn-sm btn-restaurar d-none"
            onclick="restaurar(this)">↩</button>
    </div>
    `);
        }

    function marcarEliminada(btn) {
            const row = btn.closest('.task-row');

    row.querySelector('.eliminada-flag').value = "true";
    row.classList.add('eliminada');

    row.querySelector('.btn-eliminar').classList.add('d-none');
    row.querySelector('.btn-restaurar').classList.remove('d-none');
        }

    function restaurar(btn) {
            const row = btn.closest('.task-row');

    row.querySelector('.eliminada-flag').value = "false";
    row.classList.remove('eliminada');

    row.querySelector('.btn-restaurar').classList.add('d-none');
    row.querySelector('.btn-eliminar').classList.remove('d-none');
        }

    function validarTareas() {
            const tareas = document.querySelectorAll('#tareas .task-row');

            tareas.forEach(row => {
                const descripcion = row.querySelector('input[name$=".Descripcion"]').value.trim();
    const eliminada = row.querySelector('.eliminada-flag').value;

    if (descripcion === '' && eliminada !== "true") {
        row.remove();
                }
            });

    reindexarTareas();
    return true;
        }

    function reindexarTareas() {
            const rows = document.querySelectorAll('#tareas .task-row');

            rows.forEach((row, index) => {
        row.querySelectorAll('input').forEach(input => {
            const name = input.getAttribute('name');
            if (name) {
                input.setAttribute('name', name.replace(/\[\d+\]/, `[${index}]`));
            }
        });
            });
        }
