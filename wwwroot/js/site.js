﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById("darkModeToggle").addEventListener("click", () => {
    document.body.classList.toggle("dark-mode");
    localStorage.setItem("darkMode", document.body.classList.contains("dark-mode"));
});

window.onload = () => {
    if (localStorage.getItem("darkMode") === "true") {
        document.body.classList.add("dark-mode");
    }
};
// Dark Mode Toggle
document.getElementById("darkModeToggle").addEventListener("change", (event) => {
    const isDarkMode = event.target.checked;
    document.body.classList.toggle("dark-mode", isDarkMode);
    localStorage.setItem("darkMode", isDarkMode);
});

// Apply saved Dark Mode preference on page load
window.onload = () => {
    const isDarkMode = localStorage.getItem("darkMode") === "true";
    document.body.classList.toggle("dark-mode", isDarkMode);
    document.getElementById("darkModeToggle").checked = isDarkMode;
};
function toggleUserMenu() {
    const userMenu = document.querySelector(".user-menu");
    userMenu.classList.toggle("open");
}
document.addEventListener("DOMContentLoaded", function () {
    // Pobranie checkboxa "Zaznacz wszystkie"
    const selectAllCheckbox = document.getElementById("selectAll");

    // Pobranie wszystkich checkboxów dla wierszy tabeli
    const rowCheckboxes = document.querySelectorAll(".rowCheckbox");

    // Funkcja obsługująca zaznaczanie/odznaczanie wszystkich checkboxów
    selectAllCheckbox.addEventListener("change", function () {
        const isChecked = selectAllCheckbox.checked;
        rowCheckboxes.forEach((checkbox) => {
            checkbox.checked = isChecked;
        });
    });

    // Funkcja do odznaczenia checkboxa "Zaznacz wszystkie", jeśli jakikolwiek checkbox zostanie odznaczony
    rowCheckboxes.forEach((checkbox) => {
        checkbox.addEventListener("change", function () {
            if (!checkbox.checked) {
                selectAllCheckbox.checked = false;
            } else {
                // Sprawdź, czy wszystkie są zaznaczone
                const allChecked = Array.from(rowCheckboxes).every((cb) => cb.checked);
                selectAllCheckbox.checked = allChecked;
            }
        });
    });
});
document.addEventListener("DOMContentLoaded", function () {
    console.log("Skrypt załadowany!");

    const selectAllCheckbox = document.getElementById("selectAll");
    const rowCheckboxes = document.querySelectorAll(".selectRow");
    const selectedCountElement = document.getElementById("selectedCount");

    if (!selectAllCheckbox || !selectedCountElement) {
        console.error("Nie znaleziono elementów selectAllCheckbox lub selectedCountElement!");
        return;
    }

    function updateSelectedCount() {
        const selectedCount = document.querySelectorAll(".selectRow:checked").length;
        selectedCountElement.textContent = `[${selectedCount}]`;
        console.log(`Zaznaczonych: ${selectedCount}`);
    }

    selectAllCheckbox.addEventListener("change", function () {
        const isChecked = selectAllCheckbox.checked;
        document.querySelectorAll(".selectRow").forEach((checkbox) => {
            checkbox.checked = isChecked;
        });
        updateSelectedCount();
    });

    document.querySelectorAll(".selectRow").forEach((checkbox) => {
        checkbox.addEventListener("change", function () {
            const allChecked = document.querySelectorAll(".selectRow").length === document.querySelectorAll(".selectRow:checked").length;
            selectAllCheckbox.checked = allChecked;
            updateSelectedCount();
        });
    });

    updateSelectedCount(); // Inicjalizacja
});
