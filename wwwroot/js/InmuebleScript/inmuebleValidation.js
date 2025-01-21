document.addEventListener("DOMContentLoaded", function () {
  const formulario = document.querySelector("form");

  function mostrarError(input, mensaje) {
    let errorMsg = input.parentNode.querySelector(".error-message");
    if (!errorMsg) {
      errorMsg = document.createElement("span");
      errorMsg.classList.add("error-message", "text-danger");
      input.parentNode.appendChild(errorMsg);
    }
    errorMsg.textContent = mensaje;
    input.classList.add("is-invalid");
  }

  function ocultarError(input) {
    let errorMsg = input.parentNode.querySelector(".error-message");
    if (errorMsg) {
      errorMsg.remove();
    }
    input.classList.remove("is-invalid");
  }

  formulario.addEventListener("submit", function (event) {
    let valido = true;

    //VALIDAR PRECIO
    const precioInput = document.querySelector('input[name="Precio"]');
    const precioPattern = /^[0-9]+(,[0-9]+)?$/;
    ocultarError(precioInput);
    if (!precioPattern.test(precioInput.value)) {
      mostrarError(precioInput, "Por favor, introduce un precio válido.");
      valido = false;
    }

    //VALIDAR DNI
    const dniInput = document.querySelector('input[name="Propietario_dni"]');
    const dniPattern = /^[0-9]{8}$/;
    ocultarError(dniInput);
    if (!dniPattern.test(dniInput.value)) {
      mostrarError(dniInput, "Por favor, introduce un DNI válido (8 dígitos).");
      valido = false;
    }

    if (!valido) {
      event.preventDefault();
    }
  });
});
