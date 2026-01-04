const AlertService = {
    // Función principal que mapea el DTO de C#
    showFromDto: function (dto) {
        if (!dto || !dto.mensaje) return;

        this.show(dto.titulo || 'Aviso', dto.mensaje, dto.tipo || 'info');
    },

    // Función genérica para usar manualmente desde cualquier JS
    show: function (titulo, mensaje, tipo) {
        // type puede ser: 'success', 'error', 'warning', 'info', 'question'
        Swal.fire({
            title: titulo,
            text: mensaje,
            icon: tipo,
            confirmButtonText: 'Aceptar',
            confirmButtonColor: '#3085d6'
        });
    },

    // Función para confirmaciones (Borrar, Salir, etc)
    confirm: async function (titulo, mensaje, tipo = 'warning') {
        const result = await Swal.fire({
            title: titulo,
            text: mensaje,
            icon: tipo,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, continuar',
            cancelButtonText: 'Cancelar'
        });
        return result.isConfirmed;
    }
};