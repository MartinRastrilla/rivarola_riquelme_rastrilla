document.addEventListener("DOMContentLoaded", function () {
  var calendarEl = document.getElementById("calendar");
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
      let startDate = info.startStr;
      let endDate = new Date(info.end); // Convertir a objeto Date
      endDate.setDate(endDate.getDate() - 1); // Restar 1 día para corregir problema de fechas de FullCalendar
      let endDateStr = endDate.toISOString().split("T")[0]; // Formatear fecha

      // Actualizar los inputs
      document.getElementById("Fecha_inicio").value = startDate;
      document.getElementById("Fecha_fin").value = endDateStr;

      // Limpiar eventos previos y agregar el nuevo evento corregido
      calendar.removeAllEvents();
      calendar.addEvent({
        title: "Reserva de Inmueble",
        start: startDate,
        end: info.endStr, // FullCalendar maneja internamente la exclusividad
        color: "#C0392B",
        textColor: "white",
      });
    },
    eventChange: function (info) {
      let startDate = info.event.startStr;
      let endDate = new Date(info.event.end);
      endDate.setDate(endDate.getDate() - 1);
      let endDateStr = endDate.toISOString().split("T")[0];

      // Actualizar los inputs con la fecha corregida
      document.getElementById("Fecha_inicio").value = startDate;
      document.getElementById("Fecha_fin").value = endDateStr;
    },
  });

  calendar.render();

  // Manejar cambios en los inputs manualmente
  document.getElementById("Fecha_fin").addEventListener("change", function () {
    var startDate = document.getElementById("Fecha_inicio").value;
    var endDate = new Date(document.getElementById("Fecha_fin").value);
    endDate.setDate(endDate.getDate() + 1); // Sumar 1 día para corregir el problema
    let endDateStr = endDate.toISOString().split("T")[0];

    if (startDate && endDateStr) {
      calendar.removeAllEvents();
      calendar.addEvent({
        title: "Reserva de Inmueble",
        start: startDate,
        end: endDateStr,
        color: "#C0392B",
        textColor: "white",
      });
    }
  });
});
