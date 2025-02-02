// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
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
document.getElementById("username").addEventListener("input", function (e) {
    this.value = this.value.replace(/[^a-zA-Z0-9-_]/g, '');
});
//walidacja hasła w modalu dla dodawania użytkownika
document.getElementById("password").addEventListener("input", function () {
    const password = this.value;
    const errorsList = document.getElementById("passwordErrors");
    const lengthError = document.getElementById("lengthError");
    const uppercaseError = document.getElementById("uppercaseError");
    const lowercaseError = document.getElementById("lowercaseError");
    const digitError = document.getElementById("digitError");
    const specialCharError = document.getElementById("specialCharError");

    let isValid = true;

    if (password.length >= 8) {
        lengthError.style.color = "green";
    } else {
        lengthError.style.color = "red";
        isValid = false;
    }

    if (/[A-Z]/.test(password)) {
        uppercaseError.style.color = "green";
    } else {
        uppercaseError.style.color = "red";
        isValid = false;
    }

    if (/[a-z]/.test(password)) {
        lowercaseError.style.color = "green";
    } else {
        lowercaseError.style.color = "red";
        isValid = false;
    }

    if (/\d/.test(password)) {
        digitError.style.color = "green";
    } else {
        digitError.style.color = "red";
        isValid = false;
    }

    if (/[^a-zA-Z0-9]/.test(password)) {
        specialCharError.style.color = "green";
    } else {
        specialCharError.style.color = "red";
        isValid = false;
    }

    errorsList.style.display = isValid ? "none" : "block";
});
//dodawanie użytkownika 
document.addEventListener("DOMContentLoaded", function () {
    const addUserForm = document.getElementById("addUserForm");

    addUserForm.addEventListener("submit", function (e) {
        e.preventDefault();

        const username = document.getElementById("username").value;
        const email = document.getElementById("email").value;
        const password = document.getElementById("password").value;
        const phone = document.getElementById("phone").value;
        const role = document.getElementById("role").value;

        fetch("/Users/AddUser", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                UserName: username, // ✅ Teraz `UserName` jest wysyłane do backendu
                Email: email,
                Password: password,
                PhoneNumber: phone,
                Role: role
            })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert("Użytkownik dodany!");
                    location.reload(); // Odświeżenie listy użytkowników
                } else {
                    alert("Błąd: " + data.message);
                }
            })
            .catch(error => console.error("Błąd:", error));
    });
});
//usuwanie użytkownika z przesyłaniem danych do modala
document.addEventListener("DOMContentLoaded", function () {
    const checkboxes = document.querySelectorAll(".selectRow");
    const deleteUserBtn = document.getElementById("deleteUserBtn");
    let selectedUser = null;

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener("change", function () {
            const checkedUsers = Array.from(checkboxes).filter(c => c.checked);
            deleteUserBtn.disabled = checkedUsers.length === 0;
        });
    });

    deleteUserBtn.addEventListener("click", function () {
        const checkedUsers = Array.from(checkboxes).filter(c => c.checked);

        if (checkedUsers.length === 1) {
            selectedUser = checkedUsers[0];

            const username = selectedUser.dataset.username;
            const userId = selectedUser.dataset.userid; // Pobieramy poprawnie ID użytkownika

            if (!userId || userId.trim() === "") {
                alert("Błąd: ID użytkownika nie zostało poprawnie pobrane.");
                return;
            }

            document.getElementById("deleteUserName").innerText = username;
            document.getElementById("deleteUserId").value = userId;

            var myModal = new bootstrap.Modal(document.getElementById("deleteUserModal"));
            myModal.show();
        } else {
            alert("Wybierz dokładnie jednego użytkownika do usunięcia.");
        }
    });

    document.getElementById("confirmDeleteBtn").addEventListener("click", function () {
        const userId = document.getElementById("deleteUserId").value;

        if (!userId || userId.trim() === "") {
            alert("Błąd: brak ID użytkownika.");
            return;
        }

        fetch(`/Users/DeleteUser/${userId}`, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert("Użytkownik usunięty!");
                    location.reload(); // Odświeżenie tabeli
                } else {
                    alert("Błąd: " + data.message);
                }
            })
            .catch(error => console.error("Błąd:", error));

    });
});
//edycja użytkownika przesyłanie danych do modala
document.addEventListener("DOMContentLoaded", function () {
    const checkboxes = document.querySelectorAll(".selectRow");
    const editUserBtn = document.getElementById("editUserBtn");
    let selectedUser = null;

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener("change", function () {
            const checkedUsers = Array.from(checkboxes).filter(c => c.checked);
            editUserBtn.disabled = checkedUsers.length !== 1; // Aktywacja tylko dla jednego zaznaczonego użytkownika
        });
    });

    editUserBtn.addEventListener("click", function () {
        const checkedUsers = Array.from(checkboxes).filter(c => c.checked);

        if (checkedUsers.length === 1) {
            selectedUser = checkedUsers[0];

            const userId = selectedUser.dataset.userid;
            console.log("ID użytkownika do edycji:", userId); // Sprawdzenie, czy ID jest poprawne

            if (!userId || userId.trim() === "") {
                alert("Błąd: ID użytkownika nie zostało poprawnie pobrane.");
                return;
            }

            fetch(`/Users/GetUser/${userId}`)
                .then(response => response.json())
                .then(data => {
                    console.log("Otrzymane dane użytkownika:", data); // Sprawdzenie, czy API zwraca poprawne dane

                    document.getElementById("editUserId").value = data.id;
                    document.getElementById("editUserName").value = data.userName;
                    document.getElementById("editEmail").value = data.email;
                    document.getElementById("editPhoneNumber").value = data.phoneNumber;
                    document.getElementById("editPassword").value = "";

                    var myModal = new bootstrap.Modal(document.getElementById("editUserModal"));
                    myModal.show();
                })
                .catch(error => console.error("Błąd:", error));
        } else {
            alert("Wybierz dokładnie jednego użytkownika do edycji.");
        }
    });

    document.getElementById("editUserForm").addEventListener("submit", function (e) {
        e.preventDefault();

        const formData = {
            id: document.getElementById("editUserId").value,
            email: document.getElementById("editEmail").value,
            phoneNumber: document.getElementById("editPhoneNumber").value,
            password: document.getElementById("editPassword").value
        };

        console.log("Dane wysyłane do EditUser:", JSON.stringify(formData));

        fetch("/Users/EditUser", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formData)
        })
            .then(response => response.json())
            .then(data => {
                console.log("Odpowiedź z serwera:", data);
                if (data.success) {
                    alert("Dane użytkownika zostały zaktualizowane!");
                    location.reload();
                } else {
                    alert("Błąd podczas edycji użytkownika: " + data.message);
                }
            })
            .catch(error => console.error("Błąd:", error));
    });
});
//walidacja hasła dla modala edycji użytkownika
document.addEventListener("DOMContentLoaded", function () {
    const passwordInput = document.getElementById("editPassword");
    const editUserForm = document.getElementById("editUserForm");
    const errorsList = document.getElementById("editPasswordErrors");
    const lengthError = document.getElementById("editLengthError");
    const uppercaseError = document.getElementById("editUppercaseError");
    const lowercaseError = document.getElementById("editLowercaseError");
    const digitError = document.getElementById("editDigitError");
    const specialCharError = document.getElementById("editSpecialCharError");

    let originalPasswordValue = passwordInput.value; // Zapamiętujemy oryginalną wartość pola hasła

    function validatePassword(password) {
        let isValid = true;

        if (password.length >= 8) {
            lengthError.style.color = "green";
        } else {
            lengthError.style.color = "red";
            isValid = false;
        }

        if (/[A-Z]/.test(password)) {
            uppercaseError.style.color = "green";
        } else {
            uppercaseError.style.color = "red";
            isValid = false;
        }

        if (/[a-z]/.test(password)) {
            lowercaseError.style.color = "green";
        } else {
            lowercaseError.style.color = "red";
            isValid = false;
        }

        if (/\d/.test(password)) {
            digitError.style.color = "green";
        } else {
            digitError.style.color = "red";
            isValid = false;
        }

        if (/[^a-zA-Z0-9]/.test(password)) {
            specialCharError.style.color = "green";
        } else {
            specialCharError.style.color = "red";
            isValid = false;
        }

        errorsList.style.display = isValid ? "none" : "block";
        return isValid;
    }

    passwordInput.addEventListener("input", function () {
        validatePassword(this.value);
    });

    editUserForm.addEventListener("submit", function (e) {
        const password = passwordInput.value;

        // Jeśli hasło zostało zmienione, ale jest niepoprawne, blokujemy zapis
        if (password !== originalPasswordValue && password.length > 0 && !validatePassword(password)) {
            e.preventDefault();
            alert("Hasło nie spełnia wymagań!");
            return;
        }

        // Tworzymy dane do wysłania
        let formData = {
            id: document.getElementById("editUserId").value,
            email: document.getElementById("editEmail").value,
            phoneNumber: document.getElementById("editPhoneNumber").value,
        };

        // Jeśli hasło zostało zmienione i jest poprawne, dodajemy je do obiektu
        if (password !== originalPasswordValue && password.length > 0) {
            formData.password = password;
        }

        fetch("/Users/EditUser", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formData)
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert("Dane użytkownika zostały zaktualizowane!");
                    location.reload();
                } else {
                    alert("Błąd podczas edycji użytkownika: " + data.message);
                }
            })
            .catch(error => console.error("Błąd:", error));

        e.preventDefault();
    });
});





