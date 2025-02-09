document.addEventListener("DOMContentLoaded", function () {
  const fechaInicioInput = document.getElementById("Fecha_inicio");
  const fechaFinInput = document.getElementById("Fecha_fin");
  const inmuebleSelect = document.querySelector("[asp-for='Inmueble_id']");
  const InmuebleDiv = document.getElementById("InmuebleDiv");
  const MontoDiv = document.getElementById("MontoDiv");
  const montoInput = document.querySelector("[asp-for='Monto']");
  const today = new Date().toISOString().split("T")[0];

  // Restringe la selección de fechas para que no sean anteriores al día actual
  fechaInicioInput.setAttribute("min", today);
  fechaFinInput.setAttribute("min", today);

  function validarFechas() {
    const fechaInicio = fechaInicioInput.value;
    const fechaFin = fechaFinInput.value;

    if (!fechaInicio || !fechaFin) {
      return;
    }

    if (fechaFin < fechaInicio) {
      alert(
        "La fecha de finalización no puede ser anterior a la fecha de inicio."
      );
      return;
    }

    // Si las fechas son válidas, mostrar los campos ocultos
    InmuebleDiv.classList.remove("d-none");
    MontoDiv.classList.remove("d-none");

    cargarInmueblesDisponibles(fechaInicio, fechaFin);
  }

  function cargarInmueblesDisponibles(fechaInicio, fechaFin) {
    fetch(
      `/Contratos/GetInmueblesDisponibles?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`
    )
      .then((response) => response.json())
      .then((data) => {
        inmuebleSelect.innerHTML =
          '<option value="">Seleccione un inmueble</option>';
        data.forEach((inmueble) => {
          const option = document.createElement("option");
          option.value = inmueble.id;
          option.textContent = inmueble.direccion;
          inmuebleSelect.appendChild(option);
        });
      })
      .catch((error) => console.error("Error al cargar los inmuebles:", error));
  }

  fechaInicioInput.addEventListener("change", validarFechas);
  fechaFinInput.addEventListener("change", validarFechas);
});
