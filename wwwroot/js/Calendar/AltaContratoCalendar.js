document.addEventListener("DOMContentLoaded", function () {
  var fechaInicioInput = document.getElementById("Fecha_inicio");
  var selectInmuebles = document.getElementById("Inmueble_id");
  var cantidadMesesInput = document.getElementById("Cantidad_meses");
  var fechaFinInput = document.getElementById("Fecha_fin");
  var InmuebleDiv = document.getElementById("InmuebleDiv");
  var MontoDiv = document.getElementById("MontoDiv");
  var Monto = document.getElementById("Monto");
  var today = new Date();
  var todayString = today.toISOString().split("T")[0];

  // Restringe la fecha mínima al día actual
  fechaInicioInput.setAttribute("min", todayString);

  fechaInicioInput.addEventListener("change", calcularFechaFin);
  cantidadMesesInput.addEventListener("input", calcularFechaFin);

  // Actualiza el monto si cambia el inmueble
  if (selectInmuebles) {
    selectInmuebles.addEventListener("change", calcularMonto);
  }

  // Función universal para sumar meses a una fecha
  function addMonths(date, months) {
    const originalDay = date.getDate();
    let newDate = new Date(date.getTime());
    const desiredMonth = newDate.getMonth() + months;

    // Ajustar el mes con el día original
    newDate.setMonth(desiredMonth, originalDay);

    // Si hubo desbordamiento, setDate(0) pone el último día del mes anterior
    if (newDate.getMonth() !== desiredMonth % 12) {
      newDate.setDate(0);
    }
    return newDate;
  }

  function calcularFechaFin() {
    let fechaInicio = new Date(fechaInicioInput.value);
    let cantidadMeses = parseInt(cantidadMesesInput.value);

    if (!fechaInicioInput.value || isNaN(cantidadMeses) || cantidadMeses <= 0) {
      fechaFinInput.value = "";
      fechaFinInput.parentElement.classList.add("d-none");
      cantidadMesesInput.value = "";
      return;
    }

    let fechaFin = addMonths(fechaInicio, cantidadMeses);

    fechaFinInput.value = fechaFin.toISOString().split("T")[0];
    fechaFinInput.parentElement.classList.remove("d-none");

    if (!isNaN(fechaInicio.getTime()) && !isNaN(fechaFin.getTime())) {
      obtenerInmueblesDisponibles(fechaInicio, fechaFin);
    }
  }

  function calcularMonto() {
    let inmuebleSeleccionado = selectInmuebles.value;
    let cantidadMeses = parseInt(cantidadMesesInput.value);

    if (!inmuebleSeleccionado || isNaN(cantidadMeses) || cantidadMeses <= 0) {
      Monto.value = "Seleccione un inmueble y una cantidad de meses.";
      return;
    }

    fetch(`/Inmueble/ObtenerInmueble/${inmuebleSeleccionado}`)
      .then((response) => response.json())
      .then((data) => {
        let monto = data.precio * cantidadMeses;
        Monto.value = monto.toFixed(2);
      })
      .catch((error) => {
        console.error("Error al obtener el inmueble:", error);
        Monto.value = "Error al obtener el inmueble.";
      });
  }

  function obtenerInmueblesDisponibles(fechaInicio, fechaFin) {
    fetch(
      `/Inmueble/ObtenerInmueblesDisponiblesFechas?fechaInicio=${
        fechaInicio.toISOString().split("T")[0]
      }&fechaFin=${fechaFin.toISOString().split("T")[0]}`
    )
      .then((response) => response.json())
      .then((data) => actualizarSelectInmuebles(data))
      .catch((error) =>
        console.error("Error al obtener los inmuebles:", error)
      );
  }

  function actualizarSelectInmuebles(inmuebles) {
    selectInmuebles.innerHTML =
      "<option value=''>Seleccione un inmueble</option>";
    inmuebles.forEach((inmueble) => {
      let option = document.createElement("option");
      option.value = inmueble.id;
      option.textContent = inmueble.direccion;
      selectInmuebles.appendChild(option);
    });
    InmuebleDiv.classList.remove("d-none");
    MontoDiv.classList.remove("d-none");
  }
});
