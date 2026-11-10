document.addEventListener("DOMContentLoaded", function () {
    const formulario = document.getElementById("form-reserva");

    // 1. Validar la fecha cuando el usuario la cambia y consultar disponibilidad
    const inputFecha = document.getElementById("r-fecha");
    const grupoFecha = document.getElementById("grupo-fecha");
    const errorFecha = document.getElementById("error-fecha");

    // Creamos dinámicamente un contenedor visual para mostrar los cupos debajo del input de fecha
    const divDisponibilidad = document.createElement("div");
    divDisponibilidad.id = "info-disponibilidad";
    divDisponibilidad.style.marginTop = "8px";
    divDisponibilidad.style.fontSize = "13px";
    inputFecha.parentNode.appendChild(divDisponibilidad);

    // Revisa la fecha elegida y, si es válida, consulta los cupos al servidor
    function revisarFecha() {
        const valor = inputFecha.value;
        
        if (!valor) {
            divDisponibilidad.innerHTML = "";
            return;
        }

        const partes = valor.split("-");
        const fechaSeleccionada = new Date(partes[0], partes[1] - 1, partes[2]);

        // Obtenemos la fecha de hoy a las 00:00:00 para comparar solo el día
        const hoy = new Date();
        hoy.setHours(0, 0, 0, 0);

        // Validación A: Si la fecha es anterior a hoy (pasada/expirada)
        if (fechaSeleccionada < hoy) {
            grupoFecha.classList.add("has-error");
            errorFecha.textContent = "La fecha seleccionada expiro.";
            errorFecha.classList.remove("hidden");
            inputFecha.value = ""; // Limpia la fecha incorrecta
            divDisponibilidad.innerHTML = ""; // Limpia los cupos
            return;
        }

        // Validación B: Si es lunes (getDay() === 1)
        if (fechaSeleccionada.getDay() === 1) {
            grupoFecha.classList.add("has-error");
            errorFecha.textContent = "Los Lunes no abre el bodegón.";
            errorFecha.classList.remove("hidden");
            inputFecha.value = ""; // Limpia la fecha mal elegida
            divDisponibilidad.innerHTML = ""; // Limpia la info de cupos
        } else {
            // Si pasa todas las validaciones de fecha, ocultamos errores y consultamos
            grupoFecha.classList.remove("has-error");
            errorFecha.classList.add("hidden");

            // --- CONSULTA AJAX PARA TRAER LOS CUPOS EN TIEMPO REAL ---
            fetch(`/Reservas/GetDisponibilidad?fecha=${valor}`)
                .then(response => response.json())
                .then(data => {
                    let html = "<div style='padding: 8px; background: #f5f5f5; border-radius: 4px; border: 1px solid #ddd;'><strong>Disponibilidad para este día:</strong><br>";
                    data.forEach(item => {
                        let estado = item.completo ? "🔴 <span style='color:red; font-weight:bold;'>Completo</span>" : `🟢 <span style='color:green; font-weight:bold;'>${item.disponibles} lugares libres</span>`;
                        html += `- <strong>${item.turno}</strong>: ${estado}<br>`;
                    });
                    html += "</div>";
                    divDisponibilidad.innerHTML = html;
                })
                .catch(error => {
                    console.error("Error al consultar disponibilidad:", error);
                    divDisponibilidad.innerHTML = "";
                });
        }
    }

    // Cada vez que el usuario cambia la fecha
    inputFecha.addEventListener("change", revisarFecha);

    // Si la página vuelve con un error del servidor, la fecha ya viene cargada:
    // mostramos los cupos de nuevo sin que el usuario tenga que tocar nada
    if (inputFecha.value) {
        revisarFecha();
    }

    // 2. Validar todo cuando se intenta enviar el formulario
    formulario.addEventListener("submit", function (evento) {
        let hayErrores = false;

        // Validar Nombre
        const inputNombre = document.getElementById("r-nombre");
        const grupoNombre = document.getElementById("grupo-nombre");
        const errorNombre = document.getElementById("error-nombre");
        const nombre = inputNombre.value.trim();
        const tieneNumeros = /\d/;

        if (nombre === "" || tieneNumeros.test(nombre)) {
            grupoNombre.classList.add("has-error");
            errorNombre.classList.remove("hidden");
            hayErrores = true;
        } else {
            grupoNombre.classList.remove("has-error");
            errorNombre.classList.add("hidden");
        }

        // Validar Teléfono
        const inputTelefono = document.getElementById("r-telefono");
        const grupoTelefono = document.getElementById("grupo-telefono");
        const errorTelefono = document.getElementById("error-telefono");

        if (inputTelefono.value.trim().length < 10) {
            grupoTelefono.classList.add("has-error");
            errorTelefono.classList.remove("hidden");
            hayErrores = true;
        } else {
            grupoTelefono.classList.remove("has-error");
            errorTelefono.classList.add("hidden");
        }

        // Validar Email
        const inputEmail = document.getElementById("r-email");
        const grupoEmail = document.getElementById("grupo-email");
        const errorEmail = document.getElementById("error-email");
        const formatoEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!formatoEmail.test(inputEmail.value.trim())) {
            grupoEmail.classList.add("has-error");
            errorEmail.classList.remove("hidden");
            hayErrores = true;
        } else {
            grupoEmail.classList.remove("has-error");
            errorEmail.classList.add("hidden");
        }

        // Validar que la cantidad de personas sea al menos 1 (el máximo lo controla el cupo en el servidor)
        const inputCantidad = document.getElementById("r-personas");
        const grupoCantidad = document.getElementById("grupo-cantidad");
        const errorCantidad = document.getElementById("error-cantidad");
        
        if (inputCantidad.value < 1) {
            grupoCantidad.classList.add("has-error");
            errorCantidad.classList.remove("hidden");
            hayErrores = true;
        } else {
            grupoCantidad.classList.remove("has-error");
            errorCantidad.classList.add("hidden");
        }

        // Si hay algún error, frenamos el envío del formulario
        if (hayErrores) {
            evento.preventDefault();
        }
    });
});