document.addEventListener("submit", function (e) {
    const btn = e.target.querySelector(".js-submit-once");

    if (btn) {
        btn.disabled = true;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Procesando...';
    }
});
