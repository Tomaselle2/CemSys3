document.addEventListener("submit", async function (e) {

    const form = e.target;
    const submitter = e.submitter;

    if (!submitter) return;

    //Formularios con confirmación
    if (form.classList.contains("js-confirm")) {

        e.preventDefault(); //frena submit automático

        const confirmado = await AlertService.confirm(
            form.dataset.confirmTitle || 'Confirmar',
            form.dataset.confirmMessage || '¿Desea continuar?',
            form.dataset.confirmIcon || 'warning'
        );

        if (!confirmado) return; // cancelar → no bloquea nada

        AlertService.blockButton(submitter);
        form.submit();
        return;
    }

    //Formularios normales
    AlertService.blockButton(submitter);
});
