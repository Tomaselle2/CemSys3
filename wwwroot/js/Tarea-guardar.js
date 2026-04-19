document.getElementById("btnGuardarTareas").addEventListener("click", async function () {

    if (!validarTareas()) return;

    const form = document.getElementById("formTareas");
    const formData = new FormData(form);

    try {
        const response = await fetch('/TramiteConcesion/GuardarTareas', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();

        if (result.success) {
            AlertService.show('Éxito', result.message, 'success');
        } else {
            AlertService.show('Error', result.message, 'error');
        }

    } catch (error) {
        console.error(error);
        alert("Error inesperado");
    }
});