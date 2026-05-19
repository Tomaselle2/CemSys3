document.addEventListener('DOMContentLoaded', function () {

    const radios = document.querySelectorAll('.js-tipo-traslado');

    const bloqueExterno = document.getElementById('bloqueTrasladoExterno');
    const bloqueInterno = document.getElementById('bloqueTrasladoInterno');

    const destinoSelect = document.getElementById('destinoSelect');

    function actualizarVista() {

        const tipoSeleccionado =
            document.querySelector('.js-tipo-traslado:checked')?.value;

        // =========================================
        // TRASLADO EXTERNO
        // =========================================

        if (tipoSeleccionado === "Externo") {

            bloqueExterno.style.display = 'block';
            bloqueInterno.style.display = 'none';

            destinoSelect.required = true;

            limpiarUbicacionInterna();
        }

        // =========================================
        // TRASLADO INTERNO
        // =========================================

        else if (tipoSeleccionado === "Interno") {

            bloqueExterno.style.display = 'none';
            bloqueInterno.style.display = 'block';

            destinoSelect.required = false;

            limpiarTrasladoExterno();
        }
    }

    radios.forEach(radio => {
        radio.addEventListener('change', actualizarVista);
    });

    actualizarVista();
});


// =========================================
// LIMPIAR EXTERNO
// =========================================

function limpiarTrasladoExterno() {

    const destinoSelect = document.getElementById('destinoSelect');

    if (destinoSelect) {
        destinoSelect.value = '';
    }
}


// =========================================
// LIMPIAR INTERNO
// =========================================

function limpiarUbicacionInterna() {

    const tipo = document.getElementById('tipoParcelaSelect');
    const seccion = document.getElementById('seccionSelect');
    const parcela = document.getElementById('parcelaSelect');

    // tipo parcela
    if (tipo) {
        tipo.value = '';
    }

    // sección
    if (seccion) {

        seccion.value = '';

        seccion.innerHTML =
            '<option value="">--Elija una opción--</option>';

        seccion.disabled = true;
    }

    // parcela
    if (parcela) {

        parcela.value = '';

        parcela.innerHTML =
            '<option value="">--Elija una opción--</option>';

        parcela.disabled = true;
    }
}