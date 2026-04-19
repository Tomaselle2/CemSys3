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