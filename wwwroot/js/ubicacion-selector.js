document.addEventListener('DOMContentLoaded', function () {
    initUbicacionSelectors();
});

function initUbicacionSelectors() {
    const containers = document.querySelectorAll('.ubicacion-selector');

    containers.forEach(container => {
        const tipoParcelaSelect = container.querySelector('.js-tipo-parcela');
        const seccionSelect = container.querySelector('.js-seccion');
        const parcelaSelect = container.querySelector('.js-parcela');

        const urlSecciones = container.dataset.urlSecciones;
        const urlParcelas = container.dataset.urlParcelas;

        const estadoDifuntoSelect = document.getElementById('estadoDelDifunto');

        // Inicializar
        setupSelectors(tipoParcelaSelect, seccionSelect, parcelaSelect,
            urlSecciones, urlParcelas, estadoDifuntoSelect);
    });
}

function setupSelectors(tipoParcelaSelect, seccionSelect, parcelaSelect,
    urlSecciones, urlParcelas, estadoDifuntoSelect) {

    // Cargar secciones cuando cambia el tipo
    tipoParcelaSelect.addEventListener('change', async function () {
        const tipoParcelaId = this.value;

        // Limpiar selects
        seccionSelect.innerHTML = '<option value="">--Elija una opción--</option>';
        parcelaSelect.innerHTML = '<option value="">--Elija una opción--</option>';

        if (tipoParcelaId) {
            try {
                const response = await fetch(`${urlSecciones}?tipoParcelaId=${tipoParcelaId}`);
                const secciones = await response.json();

                secciones.forEach(seccion => {
                    const option = document.createElement('option');
                    option.value = seccion.id;
                    option.textContent = seccion.nombre.toUpperCase();
                    seccionSelect.appendChild(option);
                });

                // Habilitar select de secciones
                seccionSelect.disabled = false;
            } catch (error) {
                console.error('Error cargando secciones:', error);
                alert('Error al cargar las secciones. Por favor, intente nuevamente.');
            }
        } else {
            seccionSelect.disabled = true;
            parcelaSelect.disabled = true;
        }
    });

    // Función para actualizar parcelas
    async function actualizarParcelas() {
        const seccionId = seccionSelect.value;
        const tipoParcelaId = tipoParcelaSelect.value;
        const estadoDelDifuntoId = estadoDifuntoSelect ? estadoDifuntoSelect.value : null;

        // Limpiar parcelas
        parcelaSelect.innerHTML = '<option value="">--Elija una opción--</option>';

        // Ocultar placa de mármol siempre al principio
        const placaDiv = document.getElementById("PlacaMarmol");
        if (placaDiv) {
            placaDiv.style.display = "none";
        }

        if (!seccionId || !estadoDelDifuntoId) {
            return;
        }

        try {
            const response = await fetch(
                `${urlParcelas}?seccionId=${seccionId}&estadoDifuntoId=${estadoDelDifuntoId}`
            );
            const parcelas = await response.json();

            if (!Array.isArray(parcelas)) {
                throw new Error('Formato de respuesta inválido');
            }

            parcelas.forEach(parcela => {
                const option = document.createElement('option');

                // Guardar datos en atributos data-*
                option.dataset.tipoParcelaId = tipoParcelaId;
                option.dataset.cantidadDifuntos = parcela.cantidadDifuntos || 0;
                option.dataset.tipoPanteonId = parcela.tipoPanteonId || '';
                option.dataset.tipoNichoId = parcela.tipoNichoId || '';
                option.dataset.nombrePanteon = parcela.nombrePanteon || '';
                option.dataset.nroParcela = parcela.nroParcela || '';
                option.dataset.nroFila = parcela.nroFila || '';

                option.value = parcela.id;

                // Construir texto según tipo de parcela
                const cantidadDifuntos = parseInt(parcela.cantidadDifuntos) || 0;
                let cantidadTexto = cantidadDifuntos >= 1 ? "- (Ocupado)" : "";

                let tipoNichoTexto = "";
                if (parcela.tipoNichoId) {
                    if (parcela.tipoNichoId === 1) {
                        tipoNichoTexto = "- féretro";
                    } else if (parcela.tipoNichoId === 2) {
                        tipoNichoTexto = "- urnario";
                    } else if (parcela.tipoNichoId === 3) {
                        tipoNichoTexto = "- especial";
                    }
                }

                let tipoPanteonTexto = "";
                if (parcela.tipoPanteonId) {
                    if (parcela.tipoPanteonId === 1) {
                        tipoPanteonTexto = "- con nichos";
                    } else if (parcela.tipoPanteonId === 2) {
                        tipoPanteonTexto = "- sin nichos";
                    }
                }

                let nombrePanteon = parcela.nombrePanteon ? `- ${parcela.nombrePanteon}` : "";

                // Texto según tipo de parcela
                if (tipoParcelaId == 1) { // Nicho
                    option.textContent = `Nicho ${parcela.nroParcela || ''} Fila ${parcela.nroFila || ''} ${tipoNichoTexto} ${cantidadTexto}`;
                } else if (tipoParcelaId == 2) { // Fosa
                    option.textContent = `Fosa ${parcela.nroParcela || ''} ${cantidadTexto}`;
                } else if (tipoParcelaId == 3) { // Panteón
                    option.textContent = `Lote ${parcela.nroParcela || ''} ${cantidadTexto} ${tipoPanteonTexto} ${nombrePanteon}`;
                } else {
                    option.textContent = `Parcela ${parcela.nroParcela || ''}`;
                }

                parcelaSelect.appendChild(option);
            });

            // Habilitar select de parcelas
            parcelaSelect.disabled = false;

        } catch (error) {
            console.error('Error cargando parcelas:', error);
            alert('Error al cargar las parcelas. Por favor, intente nuevamente.');
        }
    }

    // Eventos que disparan el cambio de parcelas
    seccionSelect.addEventListener('change', actualizarParcelas);
    if (estadoDifuntoSelect) {
        estadoDifuntoSelect.addEventListener('change', actualizarParcelas);
    }

    // Cargar datos iniciales si hay valores
    cargarDatosIniciales(tipoParcelaSelect, seccionSelect, parcelaSelect,
        urlSecciones, urlParcelas, estadoDifuntoSelect, actualizarParcelas);
}


async function cargarDatosIniciales(tipoParcelaSelect, seccionSelect, parcelaSelect,
    urlSecciones, urlParcelas, estadoDifuntoSelect, actualizarParcelas) {
    // Si hay tipo seleccionado, cargar secciones
    if (tipoParcelaSelect.value) {
        // Disparar evento para cargar secciones
        tipoParcelaSelect.dispatchEvent(new Event('change'));

        // Esperar un momento para que carguen las secciones
        setTimeout(async () => {
            // Si hay sección seleccionada, cargar parcelas
            if (seccionSelect.value && estadoDifuntoSelect && estadoDifuntoSelect.value) {
                await actualizarParcelas();
            }
        }, 100);
    }

    // Restaurar valores seleccionados si existen en los data-attributes
    const seccionSelected = seccionSelect.getAttribute('data-selected');
    if (seccionSelected) {
        seccionSelect.value = seccionSelected;
        seccionSelect.removeAttribute('data-selected');
    }

    const parcelaSelected = parcelaSelect.getAttribute('data-selected');
    if (parcelaSelected) {
        parcelaSelect.value = parcelaSelected;
        parcelaSelect.removeAttribute('data-selected');
    }
}

// Función auxiliar para limpiar y deshabilitar selects
function clearAndDisable(selectElement) {
    selectElement.innerHTML = '<option value="">--Elija una opción--</option>';
    selectElement.value = '';
    selectElement.disabled = true;
}