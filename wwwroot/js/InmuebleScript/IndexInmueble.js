document.addEventListener("DOMContentLoaded", function () {
  document.querySelectorAll(".btnDesactivar").forEach((btn) => {
    btn.addEventListener("click", function (event) {
      event.preventDefault();
      const inmuebleId = this.getAttribute("data-inmueble-id");

      if (!inmuebleId) {
        console.error("No se encontró el ID del inmueble.");
        return;
      }

      fetch(`/Inmueble/VerificarContratos?inmuebleId=${inmuebleId}`)
        .then((response) => {
          if (!response.ok) {
            throw new Error(
              `Error en la respuesta del servidor: ${response.statusText}`
            );
          }
          return response.json();
        })
        .then((data) => {
          if (data) {
            // Mostrar modal en lugar del alert
            document.getElementById("modalMensaje").innerText =
              "El inmueble tiene contratos activos. No se puede desactivar.";
            let modal = new bootstrap.Modal(
              document.getElementById("modalAdvertencia")
            );
            modal.show();
          } else {
            // Redirigir a la acción de desactivación
            window.location.href = `/Inmueble/Baja/${inmuebleId}`;
          }
        })
        .catch((error) => console.error("Error en la solicitud:", error));
    });
  });
});
