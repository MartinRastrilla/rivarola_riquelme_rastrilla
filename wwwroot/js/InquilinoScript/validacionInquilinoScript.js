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

    //VALIDAR DNI
    const dniInput = document.querySelector('input[name="Dni"]');
    const dniPattern = /^[0-9]{8}$/;
    ocultarError(dniInput);
    if (!dniPattern.test(dniInput.value)) {
      mostrarError(dniInput, "Por favor, introduce un DNI válido (8 dígitos).");
      valido = false;
    }

    //VALIDAR TELEFONO
    const telefonoInput = document.querySelector('input[name="Telefono');
    const telefonoPattern = /^[0-9]{10}$/;
    ocultarError(telefonoInput);
    if (!telefonoPattern.test(telefonoInput.value)) {
      mostrarError(
        telefonoInput,
        "Por favor, introduce un número de teléfono válido (10 dígitos)."
      );
      valido = false;
    }

    //VALIDAR EMAIL
    const emailInput = document.querySelector('input[name="Email"]');
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    ocultarError(emailInput);
    if (!emailPattern.test(emailInput.value)) {
      mostrarError(
        emailInput,
        "Por favor, introduce un correo electrónico válido."
      );
      valido = false;
    }

    if (!valido) {
      event.preventDefault();
    }
  });
});
