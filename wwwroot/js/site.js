//JS FOR SEARCH
document.addEventListener("DOMContentLoaded", function () {
  const searchForm = document.getElementById("searchForm");
  const searchButton = document.getElementById("searchButton");
  const searchInput = document.getElementById("searchInput");
  const icon = searchButton.querySelector("i");

  searchButton.addEventListener("click", function (event) {
    // Si el input no está expandido, lo expandimos y movemos el botón
    if (!searchInput.classList.contains("expanded")) {
      event.preventDefault(); // Evita que se envíe el formulario
      searchInput.classList.add("expanded");
      searchButton.classList.add("move"); // Mueve el botón a la derecha
      icon.classList.remove("fa-bounce");
      searchInput.focus();
    }
  });

  // Permitir el envío del formulario cuando el usuario presione "Enter"
  searchInput.addEventListener("keydown", function (event) {
    if (event.key === "Enter" && searchInput.classList.contains("expanded")) {
      searchForm.submit();
    }
  });

  // Ocultar el input y mover el botón de vuelta si el usuario hace clic fuera
  document.addEventListener("click", function (event) {
    if (!searchForm.contains(event.target)) {
      searchInput.classList.remove("expanded");
      searchButton.classList.remove("move"); // Regresar el botón a su posición original
      icon.classList.add("fa-bounce");
    }
  });
});
