document.getElementById("formFinalizar").addEventListener("submit", function () {

    const tareasDOM = document.querySelectorAll('#tareas .task-row');
    const form = this;

    // limpiar inputs generados antes
    form.querySelectorAll('.tarea-dinamica').forEach(e => e.remove());

    tareasDOM.forEach((row, index) => {

        const id = row.querySelector('input[name$=".Id"]')?.value || 0;
        const descripcion = row.querySelector('input[name$=".Descripcion"]').value;
        const estado = row.querySelector('input[type="checkbox"]').checked;
        const eliminada = row.querySelector('.eliminada-flag').value;

        const notaId = row.querySelector('input[name$=".NotaId"]')?.value || "";
        const plantillaId = row.querySelector('input[name$=".TareaPlantillaId"]')?.value || "";
        const tramiteId = row.querySelector('input[name$=".TramiteId"]')?.value || "";

        function add(name, value) {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = `Tareas[${index}].${name}`;
            input.value = value;
            input.classList.add('tarea-dinamica');
            form.appendChild(input);
        }

        add("Id", id);
        add("Descripcion", descripcion);
        add("Estado", estado);
        add("Eliminada", eliminada);
        add("NotaId", notaId);
        add("TareaPlantillaId", plantillaId);
        add("TramiteId", tramiteId);
    });
});

function verificarTareasCompletas() {
    const tareas = document.querySelectorAll('#tareas .task-row');

    if (tareas.length === 0) return;

    let todasCompletas = true;

    tareas.forEach(row => {
        const eliminada = row.querySelector('.eliminada-flag').value === "true";

        if (eliminada) return;

        const checkbox = row.querySelector('input[type="checkbox"]');

        if (!checkbox.checked) {
            todasCompletas = false;
        }
    });

    const btnFinalizar = document.getElementById("btnFinalizar");

    if (btnFinalizar) {
        btnFinalizar.disabled = !todasCompletas;
    }
}

// Escuchar cambios
document.addEventListener("change", function (e) {
    if (e.target.matches('#tareas input[type="checkbox"]')) {
        verificarTareasCompletas();
    }
});

// También cuando agregás o modificás tareas
document.addEventListener("DOMContentLoaded", verificarTareasCompletas);