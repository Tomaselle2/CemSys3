function confirmarEliminacion(form, mensaje) {
    Swal.fire({
        title: 'Confirmar eliminación',
        text: mensaje,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            form.submit();
        }
    });

    return false; // bloquea submit automático
}