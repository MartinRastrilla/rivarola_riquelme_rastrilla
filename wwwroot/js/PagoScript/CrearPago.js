document
  .getElementById("ContratoTxt")
  .addEventListener("blur", async function () {
    let inputField = this;
    let inputValue = inputField.value;
    let options = document.querySelectorAll("#ContratosList option");
    let match = false;
    let errorMessage = document.getElementById("errorContrato");

    options.forEach((option) => {
      if (option.value === inputValue) {
        document.getElementById("Contrato_id").value =
          option.value.split(" - ")[0]; // Extrae solo el ID
        match = true;
        errorMessage.classList.add("d-none");
      }
    });

    if (!match) {
      inputField.value = ""; // Borra el campo si la opción no es válida
      document.getElementById("Contrato_id").value = "";
      document.getElementById("Importe").value = "";
      errorMessage.classList.remove("d-none");
    }

    await obtenerContrato(document.getElementById("Contrato_id").value);
  });

async function obtenerContrato(contratoId) {
  try {
    let response = await fetch(`/Contrato/ObtenerContrato/${contratoId}`);
    if (!response.ok) {
      throw new Error("Error al obtener el contrato.");
    }
    let data = await response.json();
    document.getElementById("Importe").value = data.inmueble.precio || 0;
  } catch (error) {}
}
