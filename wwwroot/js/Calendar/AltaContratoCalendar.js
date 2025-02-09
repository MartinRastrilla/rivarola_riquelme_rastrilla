document.addEventListener("DOMContentLoaded", function () {
  var calendarEl = document.getElementById("calendar");
  var fechaInicioInput = document.getElementById("Fecha_inicio");
  var fechaFinInput = document.getElementById("Fecha_fin");
  var InmuebleDiv = document.getElementById("InmuebleDiv");
  var MontoDiv = document.getElementById("MontoDiv");
  var montoInput = document.querySelector("[asp-for='Monto']");
  var today = new Date().toISOString().split("T")[0]; // Fecha actual en formato YYYY-MM-DD
  let debounceTimeout;

  // Restringe fechas previas al día actual
  fechaInicioInput.setAttribute("min", today);
  fechaFinInput.setAttribute("min", today);

  var calendar = new FullCalendar.Calendar(calendarEl, {
    locale: "es",
    initialView: "dayGridMonth",
    height: 520,
    contentHeight: 600,
    editable: true,
    selectable: true,
    eventResizableFromStart: true,
    eventDurationEditable: true,
    buttonText: {
      today: "Hoy",
    },
    select: function (info) {
      let startDate = new Date(info.startStr);
      let endDate = new Date(info.endStr);
      endDate.setDate(endDate.getDate() - 1); // Ajustar porque FullCalendar da el día siguiente

      let startDateStr = startDate.toISOString().split("T")[0];
      let endDateStr = endDate.toISOString().split("T")[0];

      fechaInicioInput.value = startDateStr;
      fechaFinInput.value = endDateStr;

      validarFechas(startDateStr, endDateStr);
    },
    eventChange: function (info) {
      let startDate = new Date(info.event.startStr);
      let endDate = new Date(info.event.endStr);
      endDate.setDate(endDate.getDate() - 1);

      let startDateStr = startDate.toISOString().split("T")[0];
      let endDateStr = endDate.toISOString().split("T")[0];

      fechaInicioInput.value = startDateStr;
      fechaFinInput.value = endDateStr;

      validarFechas(startDateStr, endDateStr);
    },
  });

  calendar.render();

  function validarFechas(fechaInicio, fechaFin) {
    clearTimeout(debounceTimeout);
    debounceTimeout = setTimeout(() => {
      if (!fechaInicio || !fechaFin) return;

      let startDate = new Date(fechaInicio);
      let endDate = new Date(fechaFin);
      let currentDate = new Date();
      currentDate.setHours(0, 0, 0, 0); // Eliminar la parte de la hora para comparar solo fechas

      // Validar si alguna fecha es menor a la actual
      if (startDate < currentDate || endDate < currentDate) {
        alert(
          "Las fechas seleccionadas no pueden ser menores a la fecha actual."
        );
        fechaInicioInput.value = null;
        fechaFinInput.value = null;
        return;
      }

      // Validar si la fecha de inicio es mayor que la de fin
      if (startDate > endDate) {
        alert(
          "La fecha de inicio no puede ser mayor que la fecha de finalización."
        );
        fechaInicioInput.value = null;
        fechaFinInput.value = null;
        return;
      }

      InmuebleDiv.classList.remove("d-none");
      MontoDiv.classList.remove("d-none");

      calendar.removeAllEvents();
      calendar.addEvent({
        title: "Reserva de Inmueble",
        start: fechaInicio,
        end: new Date(endDate.setDate(endDate.getDate() + 1))
          .toISOString()
          .split("T")[0], // Ajustar fin para FullCalendar
        color: "#C0392B",
        textColor: "white",
      });

      //Obtener los inmuebles disponibles
      fetch(
        `/Inmueble/ObtenerInmueblesDisponiblesFechas?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`
      )
        .then((response) => response.json())
        .then((data) => {
          actualizarSelectInmuebles(data);
        })
        .catch((error) =>
          console.error("Error al obtener los inmuebles:", error)
        );
    }, 500);
  }

  function actualizarSelectInmuebles(inmuebles) {
    let selectInmuebles = document.getElementById("Inmueble_id");

    // Vaciar el select
    selectInmuebles.innerHTML = "";

    if (inmuebles.length === 0) {
      let option = document.createElement("option");
      option.value = "";
      option.textContent = "No hay inmuebles disponibles";
      selectInmuebles.appendChild(option);
      return;
    }

    let option = document.createElement("option");
    option.value = "";
    option.textContent = "Seleccione un inmueble";
    selectInmuebles.appendChild(option);
    // Llenar con los nuevos inmuebles
    inmuebles.forEach((inmueble) => {
      let option = document.createElement("option");
      option.value = inmueble.id; // Ajustar según la estructura del objeto recibido
      option.textContent = inmueble.direccion; // Ajustar según la estructura del objeto recibido
      selectInmuebles.appendChild(option);
    });
  }

  fechaInicioInput.addEventListener("change", () =>
    validarFechas(fechaInicioInput.value, fechaFinInput.value)
  );
  fechaFinInput.addEventListener("change", () =>
    validarFechas(fechaInicioInput.value, fechaFinInput.value)
  );
});
