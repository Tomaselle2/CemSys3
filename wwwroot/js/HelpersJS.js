//PARA ANIMACION DE PANTALLA DE CARGA
window.Loader = {
    show() {
        document.getElementById("global-loader")?.classList.remove("d-none");
    },
    hide() {
        document.getElementById("global-loader")?.classList.add("d-none");
    }
};

// Antes de navegar (links normales)
document.addEventListener("click", function (e) {
    const link = e.target.closest("a");

    if (!link) return;
    if (link.target === "_blank") return;
    if (link.hasAttribute("data-no-loader")) return;
    if (link.getAttribute("href")?.startsWith("#")) return;

    //si es HTMX
    if (link.hasAttribute("hx-get") || link.hasAttribute("hx-post")) return;

    Loader.show();
});

//PARA LOS FORMUALRIOS
document.addEventListener("submit", async function (e) {

    const form = e.target;
    const submitter = e.submitter;

    //si es HTMX
    if (form.hasAttribute("hx-post")) return;

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

        Loader.show();
        AlertService.blockButton(submitter);
        form.submit();
        return;
    }


    //Formularios normales
    Loader.show();
    AlertService.blockButton(submitter);
});

////FUNCIONES PARA TABLAS (ANIMACION)
//function showTableLoaderFromElement(element) {
//    const container = element.closest(".table-container");
//    container?.querySelector(".table-loader")?.classList.remove("d-none");
//}

//document.addEventListener("submit", function (e) {
//    const form = e.target;

//    if (!form.closest(".table-container")) return;

//    showTableLoaderFromElement(form);
//});
