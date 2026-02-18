// Gestión del loader global para pantalla de carga
window.Loader = {
    show() {
        document.getElementById("global-loader")?.classList.remove("d-none");
    },
    hide() {
        document.getElementById("global-loader")?.classList.add("d-none");
    },
    isVisible() {
        const el = document.getElementById("global-loader");
        return el ? !el.classList.contains("d-none") : false;
    }
};

// Helper: detecta si el elemento solicita no mostrar el loader
function hasNoLoaderFlag(element) {
    if (!element) return false;
    // soporta data-no-loader y data-skip-loader, tanto como atributo (presencia) o con valor "true"
    if (element.hasAttribute && (element.hasAttribute("data-no-loader") || element.hasAttribute("data-skip-loader"))) return true;
    if (element.dataset && (element.dataset.noLoader === "true" || element.dataset.skipLoader === "true")) return true;
    return false;
}

// Antes de navegar (links normales)
document.addEventListener("click", function (e) {
    const link = e.target.closest("a");
    if (!link) return;
    if (link.target === "_blank") return;
    if (hasNoLoaderFlag(link)) return;
    if (link.getAttribute("href")?.startsWith("#")) return;
    // si es HTMX
    if (link.hasAttribute("hx-get") || link.hasAttribute("hx-post")) return;
    Loader.show();
});

// PARA LOS FORMULARIOS
document.addEventListener("submit", async function (e) {
    const form = e.target;
    const submitter = e.submitter;

    // si es HTMX, manejar aparte
    if (form.hasAttribute("hx-post")) return;

    // Evitar mostrar loader si el form o el botón piden no mostrarlo
    const formTarget = form.target || "";
    const submitterFormTarget = submitter ? (submitter.getAttribute("formtarget") || "") : "";
    if (formTarget === "_blank" || submitterFormTarget === "_blank") return;
    if (hasNoLoaderFlag(form) || (submitter && hasNoLoaderFlag(submitter))) return;

    if (!submitter) return;

    // Formularios con confirmación
    if (form.classList.contains("js-confirm")) {
        e.preventDefault(); // frena submit automático

        const confirmado = await AlertService.confirm(
            form.dataset.confirmTitle || 'Confirmar',
            form.dataset.confirmMessage || '¿Desea continuar?',
            form.dataset.confirmIcon || 'warning'
        );

        if (!confirmado) return; // cancelar → no bloquea nada

        Loader.show();
        AlertService.blockButton(submitter);
        form.submit();
        // fallback: ocultar si sigue visible tras 8s
        setTimeout(() => { if (Loader.isVisible()) Loader.hide(); }, 8000);
        return;
    }

    // Formularios normales
    Loader.show();
    AlertService.blockButton(submitter);

    // fallback: ocultar si sigue visible tras 8s
    setTimeout(() => { if (Loader.isVisible()) Loader.hide(); }, 8000);
});

// Fallback: ocultar spinner si el usuario vuelve al foco (por si algo quedó en estado cargando)
window.addEventListener('focus', function () {
    if (Loader.isVisible()) Loader.hide();
});
